using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace WebApiForum.Models
{
    public class ForumDbContext : IdentityDbContext<User, IdentityRole<int>,int>
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<Like> Likes { get; set; }
      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            // Configuration Message-Reponses
            modelBuilder.Entity<Message>()
                .HasMany(m => m.Reponses)
                .WithOne(r => r.Message)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration Reponse-Likes
            modelBuilder.Entity<Reponse>()
                .HasMany(r => r.Likes)
                .WithOne(l => l.Reponse)
                .HasForeignKey(l => l.ReponseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuration Like-Users
            modelBuilder.Entity<User>()
                .HasMany(u => u.Likes)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configuration User-Messages
            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Configuration User-Reponses
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reponses)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            // Index pour améliorer les performances
            modelBuilder.Entity<Reponse>()
                .HasIndex(r => r.MessageId);

            modelBuilder.Entity<Like>()
                .HasIndex(l => l.ReponseId);

            // Contrainte d'unicité pour éviter les likes en double
            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.UserId, l.ReponseId })
                .IsUnique();

            // Configuration des propriétés par défaut
            modelBuilder.Entity<Message>()
                .Property(m => m.DatePublication)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Reponse>()
                .Property(r => r.DatePublication)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

        }
    }
}