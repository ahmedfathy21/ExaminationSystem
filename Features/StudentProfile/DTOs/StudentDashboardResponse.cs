namespace ExaminationSystem.Features.StudentProfile.DTOs;

public class StudentDashboardResponse
{
    public DashboardProfileDto Profile { get; set; } = new();
    public List<EnrolledDiplomaDto> EnrolledDiplomas { get; set; } = new();
    public List<RecentAttemptDto> RecentAttempts { get; set; } = new();
    public DashboardStatisticsDto Statistics { get; set; } = new();
}

public class DashboardProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class EnrolledDiplomaDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public int TotalQuizzes { get; set; }
    public int CompletedQuizzes { get; set; }
    public int InProgressQuizzes { get; set; }
    public int NotStartedQuizzes { get; set; }
    public double ProgressPercentage { get; set; }
    public List<DiplomaQuizDto> Quizzes { get; set; } = new();
}

public class DiplomaQuizDto
{
    public Guid QuizId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int BestScore { get; set; }
    public int TotalQuestions { get; set; }
    public int PassScore { get; set; }
    public DateTime? LastAttemptAt { get; set; }
}

public class RecentAttemptDto
{
    public Guid AttemptId { get; set; }
    public Guid QuizId { get; set; }
    public string QuizTitle { get; set; } = string.Empty;
    public string DiplomaTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class DashboardStatisticsDto
{
    public int TotalDiplomas { get; set; }
    public int TotalQuizzesAttempted { get; set; }
    public int TotalQuizzesPassed { get; set; }
    public int TotalQuizzesFailed { get; set; }
    public int InProgressQuizzes { get; set; }
    public double AverageScore { get; set; }
    public double PassRate { get; set; }
}
