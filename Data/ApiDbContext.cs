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
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.FirstName)
                    .IsRequired();

                entity.Property(x => x.LastName)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.CreatedAt);
                entity.Property(x => x.LastLogin);

                
            });

            // Setting the properties for the Tasks table
            modelBuilder.Entity<ApiTask>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(x => x.Priority)
                    .IsRequired();

                entity.Property(x => x.IsCollapsed)
                    .IsRequired();

                entity.Property(x => x.DueDate);
                entity.Property(x => x.CreatedAt);
                entity.Property(x => x.UpdatedAt);
                entity.Property(x => x.ProjectId);
                entity.Property(x => x.UserId);
            });
            // Set the relationship that ApiTask has
            modelBuilder.Entity<ApiTask>()
                .HasOne<ApiProject>()
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
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
            // Set the relationship that ApiProject has
            modelBuilder.Entity<ApiProject>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        // This uses the constructor from the base class 
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }
    }

}
