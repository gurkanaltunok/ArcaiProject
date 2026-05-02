using Microsoft.EntityFrameworkCore;
using ArcaiProject.Entities.Entities;
using ArcaiProject.Entities.Enums;

namespace ArcaiProject.DataAccess.Context
{
    /// <summary>
    /// Arcai Arşiv Yönetim Sistemi için Entity Framework Core DbContext
    /// </summary>
    public class ArcaiDbContext : DbContext
    {
        /// <summary>
        /// Dependency Injection için constructor
        /// </summary>
        /// <param name="options">DbContext yapılandırma seçenekleri</param>
        public ArcaiDbContext(DbContextOptions<ArcaiDbContext> options) : base(options)
        {
        }

        // DbSet Özellikleri - Veritabanı Tabloları
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<BorrowingRecord> BorrowingRecords { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<AcademicPeriod> AcademicPeriods { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<DocumentTag> DocumentTags { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Fluent API kullanarak model yapılandırması
        /// </summary>
        /// <param name="modelBuilder">Model builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- SEED DATA START ---
            // 1. Locations
            modelBuilder.Entity<Location>().HasData(
                new Location { Id = 1, FriendlyName = "Room 205 - Cabinet A - Shelf 1", Room = "Room 205", Cabinet = "Cabinet A", Shelf = "Shelf 1" },
                new Location { Id = 2, FriendlyName = "Room 205 - Cabinet A - Shelf 2", Room = "Room 205", Cabinet = "Cabinet A", Shelf = "Shelf 2" },
                new Location { Id = 3, FriendlyName = "Room 205 - Cabinet B - Shelf 1", Room = "Room 205", Cabinet = "Cabinet B", Shelf = "Shelf 1" }
            );

            // 2. DocumentTypes
            modelBuilder.Entity<DocumentType>().HasData(
                new DocumentType { Id = 1, Name = "Midterm Exam Paper", Description = "Midterm (ara sınav) exam papers." },
                new DocumentType { Id = 2, Name = "Final Exam Paper", Description = "End-of-term (final) exam papers." },
                new DocumentType { Id = 3, Name = "Internship Report", Description = "Student internship reports and logbooks." },
                new DocumentType { Id = 4, Name = "Administrative Document", Description = "Official faculty administrative documents." }
            );

            // 3. AcademicPeriods
            modelBuilder.Entity<AcademicPeriod>().HasData(
                new AcademicPeriod { Id = 1, PeriodName = "2023-2024 Fall" },
                new AcademicPeriod { Id = 2, PeriodName = "2023-2024 Spring" },
                new AcademicPeriod { Id = 3, PeriodName = "2024-2025 Fall" }
            );

            // 4. Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, CourseCode = "CEN403", CourseName = "Software Design", Department = "Computer Engineering" },
                new Course { Id = 2, CourseCode = "MT101", CourseName = "Calculus I", Department = "Basic Sciences" },
                new Course { Id = 3, CourseCode = "CEN305", CourseName = "Database Systems", Department = "Computer Engineering" }
            );

            // 5. Tags
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Theoretical" },
                new Tag { Id = 2, Name = "Practical" },
                new Tag { Id = 3, Name = "Project" },
                new Tag { Id = 4, Name = "MultipleChoice" }
            );

            // --- USER SEED DATA ---
            string adminHash = "AQAAAAIAAYagAAAAENn+B2BvDgeN3A3S/jAfXqfA+kQ6/bA8qTLcOhmfI4YqKE1XlK0/j8H+c0D+A9p+QQ==";
            string profHash = "AQAAAAIAAYagAAAAEMp/yC0m8Cg1Q2bH8V/4l1EK1fB/iYqSKc2iBCcBOy1F0Pq+Yqf/QYQ3aN6xXqGvIQ==";

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "Admin", LastName = "Secretary", Email = "secretary@arcai.com", Role = "Admin", PasswordHash = adminHash, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new User { Id = 2, FirstName = "Prof. Ibrahim", LastName = "Ersan", Email = "ibrahim.ersan@arcai.com", Role = "Professor", PasswordHash = profHash, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // --- SEED DATA END ---

            // 1. DocumentTag için Composite Primary Key (Birleşik Birincil Anahtar)
            modelBuilder.Entity<DocumentTag>().HasKey(dt => new { dt.DocumentId, dt.TagId });

            // 2. Benzersiz (Unique) Kısıtlamaları
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<DocumentType>().HasIndex(dt => dt.Name).IsUnique();
            modelBuilder.Entity<Location>().HasIndex(l => l.FriendlyName).IsUnique();
            modelBuilder.Entity<Course>().HasIndex(c => c.CourseCode).IsUnique();
            modelBuilder.Entity<AcademicPeriod>().HasIndex(ap => ap.PeriodName).IsUnique();
            modelBuilder.Entity<Tag>().HasIndex(t => t.Name).IsUnique();

            // 3. Enum'ların String Olarak Kaydedilmesi
            modelBuilder.Entity<Document>().Property(d => d.Status).HasConversion<string>();
            modelBuilder.Entity<BorrowingRecord>().Property(b => b.Status).HasConversion<string>();

            // 4. İlişkiler
            modelBuilder.Entity<BorrowingRecord>()
                .HasOne(b => b.RequesterUser)
                .WithMany(u => u.RequestedRecords)
                .HasForeignKey(b => b.RequesterUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BorrowingRecord>()
                .HasOne(b => b.ApproverUser)
                .WithMany(u => u.ApprovedRecords)
                .HasForeignKey(b => b.ApproverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.AddedByUser)
                .WithMany(u => u.DocumentsAdded)
                .HasForeignKey(d => d.AddedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification relation
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. Genel Silme Davranışının Kısıtlanması
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                if (foreignKey.DeleteBehavior == DeleteBehavior.Cascade && !foreignKey.IsOwnership)
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }
    }
}

