using ExaminationSystem.Common.Models;
using Microsoft.EntityFrameworkCore;
namespace ExaminationSystem.Common.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
 
    public DbSet<User> Users => Set<User>();
    public DbSet<Diploma> Diplomas => Set<Diploma>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Attempt> Attempts => Set<Attempt>();
    public DbSet<Answer> Answers => Set<Answer>();
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
 
        // ── Users ────────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.FullName).IsRequired().HasMaxLength(150);
            e.Property(u => u.Email).IsRequired().HasMaxLength(255);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<string>();
            e.Property(u => u.VerificationCode).HasMaxLength(10);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
            e.Property(u => u.UpdatedAt).HasDefaultValueSql("NOW()");
        });
 
        // ── Diplomas ─────────────────────────────────────────────
        modelBuilder.Entity<Diploma>(e =>
        {
            e.ToTable("diplomas");
            e.HasKey(d => d.Id);
            e.Property(d => d.Title).IsRequired().HasMaxLength(200);
            e.Property(d => d.IsActive).HasDefaultValue(true);
            e.Property(d => d.CreatedAt).HasDefaultValueSql("NOW()");
            e.Property(d => d.UpdatedAt).HasDefaultValueSql("NOW()");
        });
 
        // ── Quizzes ──────────────────────────────────────────────
        modelBuilder.Entity<Quiz>(e =>
        {
            e.ToTable("quizzes");
            e.HasKey(q => q.Id);
            e.Property(q => q.Title).IsRequired().HasMaxLength(200);
            e.Property(q => q.Status).HasConversion<string>();
            e.Property(q => q.DurationMinutes).IsRequired();
            e.Property(q => q.PassScore).IsRequired();
            e.Property(q => q.CreatedAt).HasDefaultValueSql("NOW()");
            e.Property(q => q.UpdatedAt).HasDefaultValueSql("NOW()");
 
            e.HasOne(q => q.Diploma)
             .WithMany(d => d.Quizzes)
             .HasForeignKey(q => q.DiplomaId)
             .OnDelete(DeleteBehavior.Cascade);
        });
 
        // ── Questions ────────────────────────────────────────────
        modelBuilder.Entity<Question>(e =>
        {
            e.ToTable("questions");
            e.HasKey(q => q.Id);
            e.Property(q => q.Body).IsRequired();
            e.Property(q => q.Type).HasConversion<string>();
            e.Property(q => q.OrderIndex).IsRequired();
            e.Property(q => q.CreatedAt).HasDefaultValueSql("NOW()");
 
            e.HasOne(q => q.Quiz)
             .WithMany(qz => qz.Questions)
             .HasForeignKey(q => q.QuizId)
             .OnDelete(DeleteBehavior.Cascade);
        });
 
        // ── Options ──────────────────────────────────────────────
        modelBuilder.Entity<Option>(e =>
        {
            e.ToTable("options");
            e.HasKey(o => o.Id);
            e.Property(o => o.Body).IsRequired();
            e.Property(o => o.IsCorrect).HasDefaultValue(false);
            e.Property(o => o.OrderIndex).IsRequired();
 
            e.HasOne(o => o.Question)
             .WithMany(q => q.Options)
             .HasForeignKey(o => o.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);
        });
 
        // ── Attempts ─────────────────────────────────────────────
        modelBuilder.Entity<Attempt>(e =>
        {
            e.ToTable("attempts");
            e.HasKey(a => a.Id);
            e.Property(a => a.Status).HasConversion<string>();
            e.Property(a => a.StartedAt).HasDefaultValueSql("NOW()");
            e.Property(a => a.CreatedAt).HasDefaultValueSql("NOW()");
            e.Property(a => a.UpdatedAt).HasDefaultValueSql("NOW()");
 
            e.HasOne(a => a.User)
             .WithMany(u => u.Attempts)
             .HasForeignKey(a => a.UserId)
             .OnDelete(DeleteBehavior.Restrict);
 
            e.HasOne(a => a.Quiz)
             .WithMany(q => q.Attempts)
             .HasForeignKey(a => a.QuizId)
             .OnDelete(DeleteBehavior.Restrict);
        });
 
        // ── Answers ──────────────────────────────────────────────
        modelBuilder.Entity<Answer>(e =>
        {
            e.ToTable("answers");
            e.HasKey(a => a.Id);
            e.Property(a => a.IsCorrect).HasDefaultValue(false);
            e.Property(a => a.AnsweredAt).HasDefaultValueSql("NOW()");
 
            e.HasOne(a => a.Attempt)
             .WithMany(at => at.Answers)
             .HasForeignKey(a => a.AttemptId)
             .OnDelete(DeleteBehavior.Cascade);
 
            e.HasOne(a => a.Question)
             .WithMany(q => q.Answers)
             .HasForeignKey(a => a.QuestionId)
             .OnDelete(DeleteBehavior.Restrict);
 
            e.HasOne(a => a.SelectedOption)
             .WithMany(o => o.Answers)
             .HasForeignKey(a => a.SelectedOptionId)
             .OnDelete(DeleteBehavior.Restrict);
 
            // One answer per question per attempt
            e.HasIndex(a => new { a.AttemptId, a.QuestionId }).IsUnique();
        });
    }
 
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }
 
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }
 
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);
 
        foreach (var entry in entries)
            entry.Entity.UpdatedAt = DateTime.UtcNow;
    }
}