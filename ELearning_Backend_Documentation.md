# 🎓 ELearning Platform - Backend Documentation

## فكرة المشروع

منصة تعليمية متكاملة تتيح للمدرسين إنشاء الكورسات وللطلاب الاشتراك فيها ومتابعة تقدمهم.

### الأدوار في النظام

| الدور | الصلاحيات |
|---|---|
| **Student** | التسجيل، الاشتراك في الكورسات، متابعة التقدم، حل الكويزات |
| **Teacher** | إنشاء الكورسات والمحتوى، إدارة الجلسات المباشرة |
| **Admin** | إدارة التصنيفات، الإشراف على المنصة |
| **Parent** | متابعة أداء الطالب (معرّف لكن غير مفعّل بعد) |

---

## Architecture المشروع

المشروع مبني على **Clean Architecture** بـ 4 طبقات:

```
ELearning.Domain          ← قلب النظام (Entities & Enums)
ELearning.Application     ← منطق الأعمال (Interfaces & DTOs)
ELearning.Infrastructure  ← التنفيذ (Database, Services, JWT)
ELearning.API             ← الواجهة الخارجية (Controllers)
```

---

## ✅ ما تم إنجازه

### 1. Domain Layer - طبقة النماذج

كل الـ Entities الأساسية مكتملة:

**User System**
- `User` — الحساب الأساسي مع JWT Refresh Token
- `StudentProfile` — بيانات الطالب (عمر، مدرسة، ولي أمر)
- `TeacherProfile` — بيانات المدرس (تخصص، توثيق، بيانات بنكية)

**Course System**
- `Category` — تصنيفات شجرية (parent/children)
- `Course` — الكورس مع Status و Level
- `Section` — أقسام الكورس مرتبة بـ OrderIndex، يحتوي على `CourseId` كـ FK مباشر
- `Lesson` — الدروس (Video / PDF / Quiz) مع `SectionId` كـ FK مباشر
- `VideoContent` / `PdfContent` — محتوى الدروس
- `LiveSession` — الجلسات المباشرة

**Learning System**
- `Quiz` / `Question` / `AnswerOption` — نظام الكويزات
- `QuizAttempt` — تتبع محاولات الطالب
- `Enrollment` — اشتراك الطالب في الكورس:
  - `UserId` + `CourseId` كـ FKs
  - `EnrolledAt`, `PaymentStatus`, `PaidAmount`
  - `CompletionPercentage` (float) — يُحسب تلقائياً
- `LessonProgress` — تتبع تقدم الطالب في كل درس:
  - `UserId` + `LessonId` كـ FKs
  - `IsCompleted`, `WatchedSeconds`, `CompletedAt`, `LastWatchedAt`
- `Certificate` — شهادات الإتمام
- `Review` — التقييمات
- `Notification` — الإشعارات

**BaseEntity** — أساس كل الـ Entities يحتوي على:
- `Id` (Guid تلقائي)
- `CreatedAt` / `UpdatedAt`
- `IsDeleted` (Soft Delete)

---

### 2. Application Layer - طبقة المنطق

**Interfaces - Repositories:**
- `IGenericRepository<T>` — CRUD الأساسي (Update/Delete بـ void مش async)
- `ICourseRepository` / `IUserRepository` / `ICategoryRepository`
- `IEnrollmentRepository`:
  ```csharp
  Task<Enrollment?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
  Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId);
  Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
  ```
- `ILessonProgressRepository`:
  ```csharp
  Task<LessonProgress?> GetByUserAndLessonAsync(Guid userId, Guid lessonId);
  Task<IEnumerable<LessonProgress>> GetUserProgressInCourseAsync(Guid userId, Guid courseId);
  Task<int> CountCompletedLessonsAsync(Guid userId, Guid courseId);
  ```
- `ILessonRepository`:
  ```csharp
  Task<Lesson?> GetWithSectionAsync(Guid lessonId); // Eager Loading للـ Section
  ```
- `IQuizRepository`:
  ```csharp
  Task<Quiz?> GetByLessonIdAsync(Guid lessonId);
  Task<Quiz?> GetWithQuestionsAsync(Guid quizId);         // Include Questions → AnswerOptions
  Task<int> CountAttemptsAsync(Guid userId, Guid quizId);
  Task<IEnumerable<QuizAttempt>> GetUserAttemptsAsync(Guid userId, Guid quizId);
  Task AddAttemptAsync(QuizAttempt attempt);
  ```
- `IUnitOfWork` — يجمع: `Courses`, `Users`, `Enrollments`, `Categories`, `LessonProgresses`, `Lessons`, `Quizzes`

**Interfaces - Services:**
- `IAuthService` / `ICourseService` / `ICategoryService` / `IProfileService`
- `IEnrollmentService`:
  ```csharp
  Task<Result<EnrollmentDto>> EnrollAsync(Guid userId, Guid courseId);
  Task<Result<IEnumerable<EnrollmentDto>>> GetMyEnrollmentsAsync(Guid userId);
  Task<Result<bool>> UnenrollAsync(Guid userId, Guid courseId);
  ```
- `ILessonProgressService`:
  ```csharp
  Task<Result<LessonProgressDto>> MarkLessonCompleteAsync(Guid studentId, Guid lessonId);
  Task<Result<CourseProgressDto>> GetCourseProgressAsync(Guid studentId, Guid courseId);
  ```
- `IQuizService`:
  ```csharp
  Task<Result<QuizDto>> GetQuizByLessonAsync(Guid userId, Guid lessonId);
  Task<Result<QuizResultDto>> SubmitAttemptAsync(Guid userId, Guid quizId, QuizAttemptRequest request);
  Task<Result<IEnumerable<QuizAttemptSummaryDto>>> GetMyAttemptsAsync(Guid userId, Guid quizId);
  ```
- `IJwtService`

**DTOs:**
- Auth: `RegisterRequest`, `LoginRequest`, `AuthResponse`, `UserDto`
- Courses: `CourseDto`, `CourseDetailsDto`, `SectionDto`, `LessonDto`
- Categories: `CategoryDto` (recursive للشجرة), `CreateCategoryRequest`
- Profile: `StudentProfileDto`, `TeacherProfileDto`
- Enrollments:
  ```csharp
  EnrollmentDto { Id, CourseId, CourseTitle, CourseThumbnailUrl,
                  TeacherName, EnrolledAt, CompletionPercentage, IsCompleted }
  ```
- Progress:
  ```csharp
  LessonProgressDto  { LessonId, LessonTitle, IsCompleted, CompletedAt }
  CourseProgressDto  { CourseId, CourseTitle, CompletionPercentage,
                       TotalLessons, CompletedLessons, IsCourseCompleted,
                       LessonsProgress: IEnumerable<LessonProgressDto> }
  ```
- Quizzes:
  ```csharp
  // QuizDto.cs
  AnswerOptionDto    { Id, OptionText }                        // IsCorrect مش بيترجع للـ client
  QuestionDto        { Id, QuestionText, QuestionType, Points, OrderIndex, AnswerOptions }
  QuizDto            { Id, Title, PassingScore, TimeLimitMinutes, ShuffleQuestions,
                       MaxAttempts, LessonId, Questions }

  // QuizAttemptRequest.cs
  QuestionAnswerRequest  { QuestionId, SelectedOptionId }
  QuizAttemptRequest     { TimeTakenSeconds, Answers: IEnumerable<QuestionAnswerRequest> }

  // QuizResultDto.cs
  QuestionResultDto      { QuestionId, QuestionText, SelectedOptionId, CorrectOptionId,
                           IsCorrect, Points, Explanation }
  QuizResultDto          { AttemptId, Score, TotalPoints, IsPassed, TimeTakenSeconds,
                           AttemptedAt, AttemptsUsed, MaxAttempts, Results }
  QuizAttemptSummaryDto  { AttemptId, Score, IsPassed, TimeTakenSeconds, AttemptedAt }
  ```

**Result Pattern:**
```csharp
Result<T>.Success(value)   // ✅ نجاح
Result<T>.Failure("error") // ❌ فشل بسبب معروف
```

---

### 3. Infrastructure Layer - طبقة التنفيذ

**Database (EF Core + SQL Server):**
- `AppDbContext` مع كل الـ DbSets
- Global Soft Delete Filters على كل الـ tables
- Auto `UpdatedAt` في `SaveChangesAsync`
- Configurations: `CourseConfiguration`, `EnrollmentConfiguration`, `LessonConfiguration`, `ReviewConfiguration`, `StudentProfileConfiguration`, `UserConfiguration`

**Repositories:**
- `GenericRepository<T>` — تنفيذ كامل مع FindAsync بـ Lambda
- `CourseRepository` — مع Eager Loading (Teacher, Category, Sections, Lessons)
- `UserRepository` — مع GetByEmail و GetByRefreshToken
- `CategoryRepository` — مع Children Loading
- `EnrollmentRepository`:
  - `GetByUserAndCourseAsync` — FirstOrDefault بـ UserId + CourseId
  - `GetUserEnrollmentsAsync` — Include Course → Teacher، مرتب بـ EnrolledAt تنازلياً
  - `IsEnrolledAsync` — AnyAsync سريع
- `LessonProgressRepository`:
  - `GetByUserAndLessonAsync` — FirstOrDefault بـ UserId + LessonId
  - `GetUserProgressInCourseAsync` — Include Lesson، فلتر بـ `Lesson.Section.CourseId`
  - `CountCompletedLessonsAsync` — CountAsync بـ IsCompleted + CourseId عبر Lesson.Section
- `LessonRepository`:
  - `GetWithSectionAsync` — Include Section لتجنب NullReferenceException
- `QuizRepository`:
  - `GetByLessonIdAsync` — FirstOrDefault بـ LessonId
  - `GetWithQuestionsAsync` — Include Questions (مرتبة بـ OrderIndex) → ThenInclude AnswerOptions
  - `CountAttemptsAsync` — CountAsync بـ UserId + QuizId
  - `GetUserAttemptsAsync` — Where بـ UserId + QuizId، مرتب بـ AttemptedAt تنازلياً
  - `AddAttemptAsync` — AddAsync مباشرة على `QuizAttempts` DbSet

**Unit of Work (`UnitOfWork.cs`):**
- Lazy initialization لكل الـ Repositories
- Transaction support (Begin / Commit / Rollback)
- Repositories المسجلة: `Courses`, `Users`, `Enrollments`, `Categories`, `LessonProgresses`, `Lessons`, `Quizzes`

**Services:**
- `JwtService` — Access Token (HMAC-SHA256) + Refresh Token (Random 64 bytes)
- `AuthService` — Register / Login / RefreshToken مع BCrypt
- `CourseService` — CRUD كامل + Sections + Lessons + Publish
- `CategoryService` — CRUD مع منع حذف Category بها أبناء
- `ProfileService` — Complete/Get لـ Student و Teacher profiles
- `EnrollmentService`:
  - `EnrollAsync` — التحقق من نشر الكورس + عدم الاشتراك المسبق
  - `GetMyEnrollmentsAsync` — جلب كل اشتراكات المستخدم مع بيانات الكورس
  - `UnenrollAsync` — حذف الاشتراك (void Delete)
  - `MapToDto` — يستخدم `course.Teacher?.FullName`
- `LessonProgressService`:
  - `MarkLessonCompleteAsync` — يجلب الدرس بـ `GetWithSectionAsync` للوصول لـ `Section.CourseId`، يتحقق من الاشتراك، ينشئ أو يحدث Progress، ثم يستدعي `UpdateEnrollmentProgressAsync`
  - `GetCourseProgressAsync` — يبني خريطة Progress بـ Dictionary للأداء، يرجع تقدم كل درس
  - `UpdateEnrollmentProgressAsync` (private) — يحسب `CompletionPercentage` كـ `float` بـ `Math.Round`
- `QuizService`:
  - `GetQuizByLessonAsync` — يجلب الدرس بـ `GetWithSectionAsync`، يتحقق من الاشتراك أو `IsFree`، يرجع الكويز مع الأسئلة بدون `IsCorrect`
  - `SubmitAttemptAsync` — يتحقق من `MaxAttempts`، يبني `answersMap` بـ Dictionary، يحسب النقاط لكل سؤال، يحسب `scorePercentage` كنسبة مئوية، يحفظ `QuizAttempt` بـ `AddAttemptAsync`، يرجع `QuizResultDto` مع تفاصيل كل سؤال
  - `GetMyAttemptsAsync` — يرجع ملخص كل محاولات المستخدم
  - `MapToDto` (private) — يحول Quiz → QuizDto مع إخفاء `IsCorrect` من الـ AnswerOptions

**DependencyInjection (`DependencyInjection.cs`):**
```csharp
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IJwtService, JwtService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IEnrollmentService, EnrollmentService>();
services.AddScoped<ILessonProgressService, LessonProgressService>();
services.AddScoped<IQuizService, QuizService>();
services.AddScoped<IProfileService, ProfileService>();
services.AddScoped<ICategoryService, CategoryService>();
services.AddScoped<ICourseService, CourseService>();
```

---

### 4. API Layer - طبقة الـ Controllers

| Controller | Endpoints | Auth |
|---|---|---|
| `AuthController` | POST /register, /login, /refresh-token | Public |
| `CategoriesController` | GET (public), POST/PUT/DELETE | Admin |
| `ProfileController` | Complete Student/Teacher, GET /me | Authorized |
| `CoursesController` | GET (public), إدارة كاملة | Teacher/Admin |
| `EnrollmentsController` | POST /{courseId}, GET /my, DELETE /{courseId} | Student / Authorized |
| `ProgressController` | POST /{lessonId}, GET /{courseId} | Student |
| `QuizController` | GET /lesson/{lessonId}, POST /{quizId}/attempt, GET /{quizId}/attempts/my | Authorized / Student |

**EnrollmentsController تفاصيل:**
```
POST   /api/enrollments/{courseId}   ← الاشتراك في كورس          [Student]
GET    /api/enrollments/my           ← كورساتي المشترك فيها      [Authorized]
DELETE /api/enrollments/{courseId}   ← إلغاء الاشتراك            [Student]
```

**ProgressController تفاصيل:**
```
POST   /api/progress/{lessonId}      ← تحديد درس كمكتمل         [Student]
GET    /api/progress/{courseId}      ← تقدمي في كورس معين        [Student]
```

**QuizController تفاصيل:**
```
GET    /api/quiz/lesson/{lessonId}        ← جلب كويز الدرس           [Authorized]
POST   /api/quiz/{quizId}/attempt         ← تقديم إجابات الكويز       [Student]
GET    /api/quiz/{quizId}/attempts/my     ← محاولاتي السابقة          [Student]
```

**Features:**
- Swagger UI مع JWT Bearer support
- `GetUserId()` helper من `ClaimTypes.NameIdentifier`
- Role-based authorization على كل endpoint

---

## ❌ ما لم يتم إنجازه بعد

### 🔴 أولوية عالية (Core Features)

#### 1. File Upload (Video & PDF)
```
POST   /api/upload/video                  ← رفع فيديو
POST   /api/upload/pdf                    ← رفع PDF
POST   /api/upload/thumbnail              ← رفع صورة
```
**ما يحتاجه:**
- `IFileStorageService` interface
- تنفيذ (Local Storage أو Azure Blob أو AWS S3)
- ربط `VideoContent` و `PdfContent` بالـ Lesson بعد الرفع

---

### 🟡 أولوية متوسطة

#### 3. Review System
```
POST   /api/reviews/{courseId}            ← إضافة تقييم
GET    /api/reviews/{courseId}            ← تقييمات الكورس
DELETE /api/reviews/{reviewId}            ← حذف تقييم
```

#### 4. Certificate Generation
```
GET    /api/certificates/my               ← شهاداتي
GET    /api/certificates/{id}/download    ← تحميل شهادة
```
- إصدار تلقائي عند `CompletionPercentage >= 100`
- PDF generation (مكتبة مثل QuestPDF)

#### 5. Live Sessions
```
GET    /api/livesessions/{courseId}        ← جلسات الكورس
POST   /api/livesessions                   ← إنشاء جلسة (Teacher)
PUT    /api/livesessions/{id}/status       ← تغيير الحالة
```

#### 6. Notification System
```
GET    /api/notifications                  ← إشعاراتي
PUT    /api/notifications/{id}/read        ← تحديد كمقروء
PUT    /api/notifications/read-all         ← تحديد الكل كمقروء
```

---

### 🟢 أولوية منخفضة (Improvements)

#### 7. Input Validation (FluentValidation)
#### 8. Pagination (`PagedResult<T>`)
#### 9. Search & Filter للكورسات
#### 10. Parent Role
#### 11. Admin Dashboard Endpoints
#### 12. Soft Delete Fix في CourseService

---

## خريطة الطريق المقترحة

```
المرحلة الأولى  ✅ Auth + Courses + Categories + Profile
                ✅ Enrollment + LessonProgress
                ✅ Quiz System
المرحلة الثانية → File Upload + Review + Certificate
المرحلة الثالثة → Live Sessions + Notifications
المرحلة الرابعة → Validation + Pagination + Search + Admin
```

---

## Tech Stack

| الأداة | الاستخدام |
|---|---|
| ASP.NET Core | Web API Framework |
| Entity Framework Core | ORM |
| SQL Server | Database |
| JWT Bearer | Authentication |
| BCrypt.Net | Password Hashing |
| Clean Architecture | Project Structure |
| Repository + UoW Pattern | Data Access |
| Result Pattern | Error Handling |
