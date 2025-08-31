using DataBaseInfo.models;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;


namespace DataBaseInfo
{

    using models;
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
  
        public DbSet<User> Users { get; set; } // Таблица пользователей
        public DbSet<Project> Projects { get; set; } // Таблица проектов
        public DbSet<ProjectUser> ProjectUsers { get; set; }
        public DbSet<MemberOfBoard> MembersOfBoards{ get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<_Task> Tasks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         
            modelBuilder.Entity<User>(entity =>
            {
                //Первичный ключ
                entity.Property(u => u.Id).IsRequired();
                //Настройка полей
                entity.Property(u => u.FirstName).HasMaxLength(35);
                entity.Property(u => u.SecondName).HasMaxLength(35);
                entity.Property(u => u.InviteId).IsRequired();
                entity.Property(u => u.UserEmail).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.UserEmail).IsUnique();
               
            });

            modelBuilder.Entity<Project>(entity =>
            {
                //Первичный ключ
                entity.Property(p => p.Id).IsRequired();
                //Настройка полей
                entity.Property(p => p.ProjectName).IsRequired().HasMaxLength(50);
                //Настройка связи между полями(Нэту)
            });
            modelBuilder.Entity<ProjectUser>(entity =>
            {

                //первичный ключ
                entity.Property(up => up.Id).IsRequired();
                entity.Property(up => up.UserId).IsRequired();
                entity.Property(up=> up.ProjectId).IsRequired();
                //Связь между моделями User и ProjectUser
                entity.HasOne(up => up.User)
                .WithMany(us => us.ProjectUsers)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(up => up.Project)
                .WithMany(us => us.ProjectUsers)
                .HasForeignKey(up => up.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(up => new { up.UserId, up.ProjectId })
                      .IsUnique();
            });

            modelBuilder.Entity<MemberOfBoard>(entity =>
            {

                entity.Property(mg => mg.Id).IsRequired();
                entity.Property(mg => mg.ProjectUserId).IsRequired();

                entity.HasOne(mb => mb.Board)
                .WithMany(b => b.MemberOfBoards)
                .HasForeignKey(mb => mb.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mb => mb.ProjectUser)
                .WithMany(pu => pu.MembersOfBoards)
                .HasForeignKey(mb => mb.ProjectUserId)
                .OnDelete(DeleteBehavior.Cascade);


            });

           

            modelBuilder.Entity<Board>(entity =>
            {
                //Первичный ключ
                entity.Property(b => b.Id).IsRequired();
                //Настройка полей
                entity.Property(b => b.Name).IsRequired().HasMaxLength(20);
                
                //Настройка связи между полями(Нэту)
                
               
                
            });

            modelBuilder.Entity<Card>(entity =>
            {
                //Первичный ключ
                entity.Property(c => c.Id).IsRequired();
                //Настройка полей
                entity.Property(c => c.Name).IsRequired().HasMaxLength(20);
                entity.Property(c => c.BoardId).IsRequired();
             
                entity.Property(c => c.Progress).IsRequired();
                //Настройка связи между полями(Нэту)
                entity.HasOne(c => c.Board)
                .WithMany(b => b.Cards)
                .HasForeignKey(b => b.BoardId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<_Task>(entity =>
            {
                //Первичный ключ
                entity.Property(t => t.Id).IsRequired();
                //Настройка полей
                entity.Property(t => t.Name).IsRequired().HasMaxLength(20);
                entity.Property(t => t.CardId).IsRequired();
                entity.Property(t => t.Complete).IsRequired();
                entity.Property(t => t.Priority).IsRequired();
                //Настройка связи между полями(Нэту)
                entity.HasOne(t => t.Card)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CardId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.CreatedAt).IsRequired();
               
             
                entity.Property(r => r.UserId).IsRequired();
                entity.HasOne(rt => rt.User)
               .WithMany(u => u.RefreshToken)
               .HasForeignKey(r => r.UserId);

            });


        }
    }
}

