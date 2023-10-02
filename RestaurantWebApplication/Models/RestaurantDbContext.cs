using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RestaurantWebApplication.Models;

public partial class RestaurantDbContext : DbContext
{
    public RestaurantDbContext()
    {
    }

    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.Property(e => e.CloseTime).HasMaxLength(8);
            entity.Property(e => e.OpenTime).HasMaxLength(8);
            entity.Property(e => e.Location).HasMaxLength(100);

            entity.HasOne(d => d.Type).WithMany(p => p.Places)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Places_Types");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasOne(d => d.Client).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Clients");

            entity.HasOne(d => d.Place).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.PlaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Places");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Type");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
