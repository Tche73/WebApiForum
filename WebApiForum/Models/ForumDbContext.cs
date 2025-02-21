using Microsoft.EntityFrameworkCore;
using System;

namespace WebApiForum.Models
{
    public class ForumDbContext : DbContext
    {
        public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}