
using Microsoft.EntityFrameworkCore;
using StaffDb.Models;

namespace StaffDb.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(department => department.Id);

            entity.Property(department => department.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(department => department.Name)
                .IsUnique();
        });

        // Employee configuration
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(employee => employee.Id);

            entity.Property(employee => employee.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(employee => employee.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(employee => employee.Email)
                .IsUnique();

            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(user => user.Username)
                .IsUnique();

            entity.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
        });

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(token => token.Id);

            entity.Property(token => token.TokenHash)
                .IsRequired()
                .HasMaxLength(64);

            entity.HasIndex(token => token.TokenHash)
                .IsUnique();

            entity.HasOne(token => token.User)
                .WithMany()
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
