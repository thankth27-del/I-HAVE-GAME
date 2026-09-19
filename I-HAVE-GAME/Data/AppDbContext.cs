using Microsoft.EntityFrameworkCore;
using I_HAVE_GAME.Models;

namespace I_HAVE_GAME.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<GameLibraryItem> GameLibraryItems { get; set; } = null!;
        public DbSet<SearchHistory> SearchHistories { get; set; } = null!;
        public DbSet<QuizQuestion> QuizQuestions { get; set; } = null!;
        public DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        // Games collection for internal search/index
        public DbSet<Models.Game> Games { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Email).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.Nickname).IsRequired(false);
                entity.Property(u => u.MainDevice).IsRequired(false);
            });

            // GameLibraryItem configuration
            modelBuilder.Entity<GameLibraryItem>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.UserId).IsRequired();
                entity.Property(g => g.GameId).IsRequired();
                entity.Property(g => g.GameName).IsRequired();
                entity.Property(g => g.Status).IsRequired();
                entity.Property(g => g.GameSlug).IsRequired(false);
                entity.Property(g => g.GameImageUrl).IsRequired(false);
                entity.Property(g => g.Rating).IsRequired(false);
                entity.Property(g => g.Review).IsRequired(false);
                entity.Property(g => g.Genre).IsRequired(false);
                entity.Property(g => g.AddedAt).IsRequired();
                entity.HasOne(g => g.User)
                    .WithMany(u => u.GameLibraryItems)
                    .HasForeignKey(g => g.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // SearchHistory configuration
            modelBuilder.Entity<SearchHistory>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.UserId).IsRequired();
                entity.Property(s => s.ResultCount).IsRequired();
                entity.Property(s => s.Genre).IsRequired(false);
                entity.Property(s => s.Device).IsRequired(false);
                entity.Property(s => s.PlayMode).IsRequired(false);
                entity.Property(s => s.Budget).IsRequired(false);
                entity.Property(s => s.Era).IsRequired(false);
                entity.HasOne(s => s.User)
                    .WithMany(u => u.SearchHistories)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // QuizQuestion configuration
            modelBuilder.Entity<QuizQuestion>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.QuestionText).IsRequired();
                entity.Property(q => q.CorrectAnswer).IsRequired();
                entity.Property(q => q.Choice1).IsRequired();
                entity.Property(q => q.Choice2).IsRequired();
                entity.Property(q => q.Choice3).IsRequired();
                entity.Property(q => q.Choice4).IsRequired();
                entity.Property(q => q.CreatedByAdminId).IsRequired(false);
            });

            // QuizAttempt configuration
            modelBuilder.Entity<QuizAttempt>(entity =>
            {
                entity.HasKey(qa => qa.Id);
                entity.Property(qa => qa.UserId).IsRequired();
                entity.Property(qa => qa.Score).IsRequired();
                entity.Property(qa => qa.PlayedAt).IsRequired();
                entity.HasOne(qa => qa.User)
                    .WithMany(u => u.QuizAttempts)
                    .HasForeignKey(qa => qa.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
