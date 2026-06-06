namespace ELearning.Domain.Enums;

public enum UserRole { Student, Teacher, Admin, Parent }
public enum CourseStatus { Draft, UnderReview, Published, Archived }
public enum CourseLevel { Beginner, Intermediate, Advanced }
public enum ContentType { Video, PDF, Quiz, Text }
public enum QuestionType { MultipleChoice, TrueFalse, ShortAnswer }
public enum LiveSessionStatus { Scheduled, Live, Ended, Cancelled }
public enum PaymentStatus { Free, Pending, Completed, Refunded }