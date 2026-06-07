# 🎓 ELearning Platform — Backend API

<div align="center">

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=for-the-badge&logo=jsonwebtokens)
![Cloudinary](https://img.shields.io/badge/Cloudinary-File%20Upload-3448C5?style=for-the-badge&logo=cloudinary)

منصة تعليمية متكاملة تتيح للمدرسين إنشاء الكورسات وللطلاب الاشتراك فيها ومتابعة تقدمهم

</div>

---

## 📋 جدول المحتويات

- [نظرة عامة](#-نظرة-عامة)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [الميزات](#-الميزات)
- [API Endpoints](#-api-endpoints)
- [تشغيل المشروع](#-تشغيل-المشروع)
- [متغيرات البيئة](#-متغيرات-البيئة)
- [هيكل المشروع](#-هيكل-المشروع)

---

## 🌟 نظرة عامة

منصة تعليمية متكاملة (LMS) مبنية على **Clean Architecture** تدعم 4 أدوار مختلفة:

| الدور | الصلاحيات |
|---|---|
| **Student** | التسجيل، الاشتراك في الكورسات، متابعة التقدم، حل الكويزات |
| **Teacher** | إنشاء الكورسات والمحتوى، رفع الفيديوهات والـ PDF، إدارة الكويزات |
| **Admin** | إدارة التصنيفات، الإشراف على المنصة |
| **Parent** | متابعة أداء الطالب *(قريباً)* |

---

## 🛠 Tech Stack

| الأداة | الاستخدام |
|---|---|
| ASP.NET Core 8 | Web API Framework |
| Entity Framework Core 8 | ORM |
| SQL Server | Database |
| JWT Bearer | Authentication |
| BCrypt.Net | Password Hashing |
| Cloudinary | File Storage (Video, PDF, Images) |
| Clean Architecture | Project Structure |
| Repository + Unit of Work | Data Access Pattern |
| Result Pattern | Error Handling |

---

## 🏗 Architecture

المشروع مبني على **Clean Architecture** بـ 4 طبقات:

```
ELearning.Domain          ← Entities & Enums
ELearning.Application     ← Interfaces, DTOs, Business Logic
ELearning.Infrastructure  ← Database, Services, JWT, Cloudinary
ELearning.API             ← Controllers, Middleware
```

---

## ✨ الميزات

- ✅ **Authentication** — Register, Login, Refresh Token (JWT + BCrypt)
- ✅ **Course Management** — CRUD كامل + Sections + Lessons + Publish
- ✅ **Enrollment System** — اشتراك وإلغاء اشتراك مع تتبع الحالة
- ✅ **Progress Tracking** — تتبع تقدم الطالب وحساب نسبة الإتمام تلقائياً
- ✅ **Quiz System** — كويزات متعددة الأسئلة مع تتبع المحاولات والنتائج
- ✅ **File Upload** — رفع فيديوهات وـ PDF وصور عبر Cloudinary
- ✅ **Category System** — تصنيفات شجرية للكورسات
- ✅ **Profile System** — ملفات شخصية للطلاب والمدرسين
- ✅ **Soft Delete** — حذف منطقي على كل الجداول
- ✅ **CORS** — مُهيَّأ للـ Angular frontend

---

## 📡 API Endpoints

### 🔐 Auth
```
POST   /api/auth/register          ← تسجيل حساب جديد
POST   /api/auth/login             ← تسجيل الدخول
POST   /api/auth/refresh-token     ← تجديد الـ Token
```

### 📚 Courses
```
GET    /api/courses                ← كل الكورسات المنشورة  [Public]
GET    /api/courses/{id}           ← تفاصيل كورس           [Public]
POST   /api/courses                ← إنشاء كورس            [Teacher]
PUT    /api/courses/{id}           ← تعديل كورس            [Teacher]
DELETE /api/courses/{id}           ← حذف كورس              [Teacher]
POST   /api/courses/{id}/publish   ← نشر كورس              [Teacher]
POST   /api/courses/{id}/sections              ← إضافة قسم
POST   /api/courses/{id}/sections/{sid}/lessons ← إضافة درس
```

### 🎓 Enrollments
```
POST   /api/enrollments/{courseId} ← الاشتراك في كورس      [Student]
GET    /api/enrollments/my         ← كورساتي               [Authorized]
DELETE /api/enrollments/{courseId} ← إلغاء الاشتراك        [Student]
```

### 📈 Progress
```
POST   /api/progress/{lessonId}    ← تحديد درس كمكتمل      [Student]
GET    /api/progress/{courseId}    ← تقدمي في كورس          [Student]
```

### 📝 Quiz
```
GET    /api/quiz/lesson/{lessonId}      ← كويز الدرس        [Authorized]
POST   /api/quiz/{quizId}/attempt       ← تقديم إجابات      [Student]
GET    /api/quiz/{quizId}/attempts/my   ← محاولاتي          [Student]
```

### 📁 Upload
```
POST   /api/upload/video                    ← رفع فيديو      [Teacher]
POST   /api/upload/pdf                      ← رفع PDF        [Teacher]
POST   /api/upload/thumbnail                ← رفع صورة       [Teacher]
POST   /api/upload/lesson/{lessonId}/video  ← رفع وربط فيديو [Teacher]
POST   /api/upload/lesson/{lessonId}/pdf    ← رفع وربط PDF   [Teacher]
```

### 👤 Profile
```
GET    /api/profile/me                      ← بياناتي        [Authorized]
POST   /api/profile/complete/student        ← إكمال ملف طالب [Student]
POST   /api/profile/complete/teacher        ← إكمال ملف مدرس [Teacher]
```

### 🗂 Categories
```
GET    /api/categories             ← كل التصنيفات           [Public]
POST   /api/categories             ← إضافة تصنيف            [Admin]
PUT    /api/categories/{id}        ← تعديل تصنيف            [Admin]
DELETE /api/categories/{id}        ← حذف تصنيف              [Admin]
```

---

## 🚀 تشغيل المشروع

### المتطلبات
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Cloudinary Account](https://cloudinary.com) (مجاني)

### خطوات التشغيل

**1. Clone المشروع**
```bash
git clone https://github.com/your-username/elearning-backend.git
cd elearning-backend
```

**2. إعداد متغيرات البيئة**
```bash
# انسخ ملف الإعدادات
cp ELearning.API/appsettings.example.json ELearning.API/appsettings.json
# ثم عدّل القيم (راجع قسم متغيرات البيئة)
```

**3. إنشاء قاعدة البيانات**
```bash
dotnet ef database update -p ELearning.Infrastructure -s ELearning.API
```

**4. تشغيل المشروع**
```bash
dotnet run --project ELearning.API
```

**5. فتح Swagger**
```
https://localhost:7211/swagger
```

---

## ⚙️ متغيرات البيئة

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ELearningDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong",
    "Issuer": "ELearningAPI",
    "Audience": "ELearningClient",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  }
}
```

> ⚠️ **تحذير:** لا تحفظ الـ `ApiSecret` أو `JwtSecret` في الـ repository. استخدم [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) محلياً و Environment Variables على السيرفر.

```bash
# User Secrets (محلياً)
dotnet user-secrets set "Cloudinary:ApiSecret" "your-secret" --project ELearning.API
dotnet user-secrets set "JwtSettings:Secret" "your-secret" --project ELearning.API
```

---

## 📁 هيكل المشروع

```
ELearning/
├── ELearning.Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Course.cs
│   │   ├── Section.cs
│   │   ├── Lesson.cs
│   │   ├── VideoContent.cs
│   │   ├── PdfContent.cs
│   │   ├── Enrollment.cs
│   │   ├── LessonProgress.cs
│   │   ├── Quiz.cs
│   │   ├── Question.cs
│   │   ├── AnswerOption.cs
│   │   ├── QuizAttempt.cs
│   │   ├── Certificate.cs
│   │   ├── Review.cs
│   │   └── Notification.cs
│   ├── Enums/
│   │   ├── UserRole.cs
│   │   ├── CourseStatus.cs
│   │   └── LessonType.cs
│   └── Common/
│       └── BaseEntity.cs
│
├── ELearning.Application/
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   ├── ICourseRepository.cs
│   │   ├── IEnrollmentRepository.cs
│   │   ├── ILessonProgressRepository.cs
│   │   ├── IQuizRepository.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IAuthService.cs
│   │   ├── ICourseService.cs
│   │   ├── IEnrollmentService.cs
│   │   ├── ILessonProgressService.cs
│   │   ├── IQuizService.cs
│   │   ├── IFileStorageService.cs
│   │   └── IJwtService.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── Courses/
│   │   ├── Enrollments/
│   │   ├── Progress/
│   │   ├── Quizzes/
│   │   └── Upload/
│   └── Common/
│       └── Result.cs
│
├── ELearning.Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── CourseRepository.cs
│   │   ├── EnrollmentRepository.cs
│   │   ├── LessonProgressRepository.cs
│   │   └── QuizRepository.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── CourseService.cs
│   │   ├── EnrollmentService.cs
│   │   ├── LessonProgressService.cs
│   │   ├── QuizService.cs
│   │   ├── CloudinaryService.cs
│   │   └── JwtService.cs
│   ├── Settings/
│   │   ├── JwtSettings.cs
│   │   └── CloudinarySettings.cs
│   └── DependencyInjection.cs
│
└── ELearning.API/
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── CoursesController.cs
    │   ├── EnrollmentsController.cs
    │   ├── ProgressController.cs
    │   ├── QuizController.cs
    │   ├── UploadController.cs
    │   ├── ProfileController.cs
    │   └── CategoriesController.cs
    ├── appsettings.json
    └── Program.cs
```

---

## 🔗 Frontend

الـ Frontend مبني بـ Angular 17+ — [رابط المشروع](https://github.com/your-username/elearning-frontend)

---

<div align="center">
Made with ❤️ using ASP.NET Core 8
</div>
