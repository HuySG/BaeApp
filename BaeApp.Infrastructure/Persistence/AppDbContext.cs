using BaeApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Khai báo Dbset cho mỗi entity

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TaskCategory> TaskCategories { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình User

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
                entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
                entity.Property(u => u.FullName)
                .HasMaxLength(255);

                // Map enum RoleType sang chuỗi Member/Admin

                entity.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);
            });

            // 2. Cấu hình TaskItem
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.TaskItemId);
                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.HasOne(t => t.Creator)
                      .WithMany(u => u.TasksCreated)
                      .HasForeignKey(t => t.CreatorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // 3. Cấu hình Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            // 4. Cấu hình TaskCategory (bảng trung gian cho N–N)
            modelBuilder.Entity<TaskCategory>(entity =>
            {
                entity.HasKey(tc => new { tc.TaskItemId, tc.CategoryId });

                entity.HasOne(tc => tc.TaskItem)
                      .WithMany(t => t.TaskCategories)
                      .HasForeignKey(tc => tc.TaskItemId);

                entity.HasOne(tc => tc.Category)
                      .WithMany(c => c.TaskCategories)
                      .HasForeignKey(tc => tc.CategoryId);
            });
            // 5. Cấu hình Reminder
            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.HasKey(r => r.ReminderId);

                entity.HasOne(r => r.TaskItem)
                      .WithMany(t => t.Reminders)
                      .HasForeignKey(r => r.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // 6. Cấu hình Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.NotificationId);
                entity.Property(n => n.Content)
                      .IsRequired()
                      .HasMaxLength(500);

                // Chuyển sang Restrict hoặc NoAction thay vì Cascade
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                // ← Trước đây bạn dùng DeleteBehavior.Cascade

                entity.HasOne(n => n.TaskItem)
                      .WithMany(t => t.Notifications)
                      .HasForeignKey(n => n.TaskItemId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}
