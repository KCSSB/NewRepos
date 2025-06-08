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
        public DbSet<Group> Groups { get; set; }
        public DbSet<MemberOfGroup> MembersOfGroups { get; set; }
        public DbSet<Board> Boards { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<_Task> Tasks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

       // public DbSet<HashedPassword> Passwords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

         //   modelBuilder.Entity<HashedPassword>(entity =>
           // {
             //   entity.HasKey(x => x.userId);
               // entity.Property(x => x.storedHash).IsRequired();
                //entity.Property(x => x.storedSalt).IsRequired();
                //entity.Property(x => x.Password).IsRequired();
           
            //});
            modelBuilder.Entity<ProjectUser>(entity =>
            {

                //первичный ключ
                entity.HasKey(u => u.Id);
                entity.Property(u => u.UserId).IsRequired();
                entity.Property(u => u.ProjectId).IsRequired();
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

            modelBuilder.Entity<User>(entity =>
            {
                //Первичный ключ
                entity.HasKey(u => u.Id);
                //Настройка полей
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(35);
                entity.Property(u => u.UserEmail).IsRequired().HasMaxLength(50);
                entity.HasIndex(u => u.UserEmail).IsUnique();
                //Настройка связи между полями(Нэту)
                // entity.HasOne(u => u.UserPassword)
                // ..WithOne(p => p.user)
                //.HasForeignKey<HashedPassword>(u => u.userId)
                //.OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(u => u.RefreshToken)
                .WithOne(r => r.User)
                .HasForeignKey<RefreshToken>(r => r.UserId);
                
            });

            modelBuilder.Entity<Project>(entity =>
            {
                //Первичный ключ
                entity.HasKey(p => p.Id);
                //Настройка полей
                entity.Property(p => p.ProjectName).IsRequired().HasMaxLength(20);
                //Настройка связи между полями(Нэту)
                entity.HasMany(p => p.Boards)
                .WithOne(b => b.Project)
                .HasForeignKey(b => b.ProjectId)
                .IsRequired();
            });
            modelBuilder.Entity<MemberOfGroup>(entity =>
            {
                
                entity.HasKey(mg => mg.Id);
                entity.Property(mg => mg.ProjectUserId).IsRequired();
                entity.Property(mg => mg.GroupId).IsRequired();

                entity.HasOne(mg => mg.User)
                    .WithMany(pu => pu.Groups)
                    .HasForeignKey(mg => mg.ProjectUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(m => m.Group)
                    .WithMany(g => g.Members)
                    .HasForeignKey(m => m.GroupId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name).IsRequired().HasMaxLength(20);
                entity.Property(g => g.ProjectId).IsRequired();
                entity.Property(g => g.LeadId).IsRequired();
                entity.Property(g => g.BoardId).IsRequired();
                entity.HasOne(g => g.Project)
                    .WithMany(p => p.Groups)
                    .HasForeignKey(g => g.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(g => g.Lead)
                    .WithMany(pu => pu.LedGroups)
                    .HasForeignKey(g => g.LeadId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Board>(entity =>
            {
                //Первичный ключ
                entity.HasKey(b => b.Id);
                //Настройка полей
                entity.Property(b => b.Name).IsRequired().HasMaxLength(20);
                entity.Property(b => b.GroupId).IsRequired();
                //Настройка связи между полями(Нэту)
                entity.HasOne(b => b.Group)
                .WithMany(g => g.Boards)
                .HasForeignKey(b => b.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(b => b.Project)
                .WithMany(p => p.Boards).HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
                
            });

            modelBuilder.Entity<Card>(entity =>
            {
                //Первичный ключ
                entity.HasKey(c => c.Id);
                //Настройка полей
                entity.Property(c => c.Name).IsRequired().HasMaxLength(20);
                entity.Property(c => c.BoardId).IsRequired();
                entity.Property(c => c.Priority).IsRequired();
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
                entity.HasKey(t => t.Id);
                //Настройка полей
                entity.Property(t => t.Name).IsRequired().HasMaxLength(20);
                entity.Property(t => t.CardId).IsRequired();
                entity.Property(t => t.Complete).IsRequired();
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
            });


        }
    }
}

