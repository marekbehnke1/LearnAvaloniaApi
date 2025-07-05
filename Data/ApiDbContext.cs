using LearnAvaloniaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAvaloniaApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<ApiTask> Tasks { get; set; }
        public DbSet<ApiProject> Projects { get; set; }
        public DbSet <User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This is setting the table columns
            // Essentially saying our table made from the user entity will have these columns..

            // Setting the properties for the Tasks table
            modelBuilder.Entity<ApiTask>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Priority)
                    .IsRequired();

                entity.Property(x => x.IsCollapsed)
                    .IsRequired();

                entity.Property(x => x.DueDate);
                entity.Property(x => x.CreatedAt);
                entity.Property(x => x.UpdatedAt);
                entity.Property(x => x.ProjectId);
                entity.Property(x => x.UserId)
                    .IsRequired();
            });
            // Task -> Project
            modelBuilder.Entity<ApiTask>()
                .HasOne(x => x.Project)

                // Configure the navigation property for tasks
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task -> User
            modelBuilder.Entity<ApiTask>()
                .HasOne(x => x.User)
                // This is what connects the navigation properties to the data
                .WithMany(x => x.Tasks)

                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            

            //Setting the properties for the Projects table
            modelBuilder.Entity<ApiProject>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.DateCreated);
                entity.Property(x => x.UpdatedAt);

                entity.Property(x => x.UserId)
                    .IsRequired();
            });
            // Project -> User 
            modelBuilder.Entity<ApiProject>()
                .HasOne(x => x.User)
                // This is setting the navigations property for projects
                .WithMany(x => x.Projects)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Set properties for the user table
            modelBuilder.Entity<User>(entity =>
            {  
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(255);
                // Ensures that we don't get duplicate accounts
                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                // Security & Auditing
                entity.Property(x => x.CreatedAt);
                entity.Property(x => x.UpdatedAt);
                entity.Property(x => x.LastLogin);

                //Account Management
                entity.Property(x => x.EmailConfirmed);
                entity.Property(x => x.IsActive);

                // Security Tracking
                entity.Property(x => x.FailedLoginAttempts);
                entity.Property(x => x.LockoutEnd);

                // Navigation Properties
            });
        }

        // This uses the constructor from the base class 
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
    }

}
