// Provides EF Core classes such as DbContext, DbSet and ModelBuilder.
using Microsoft.EntityFrameworkCore;

// Provides access to Department, Employee and User entity classes.
using StaffDb.Models;

// Defines the namespace containing the database context.
namespace StaffDb.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    // Represents the Departments table in the database.
    public DbSet<Department> Departments => Set<Department>();

    // Represents the Employees table in the database.
    public DbSet<Employee> Employees => Set<Employee>();

    // Represents the Users table in the database.
    public DbSet<User> Users => Set<User>();

    // Configures database tables, columns, keys, indexes and relationships.
    // EF Core uses these configurations when generating migrations.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configures the Department entity.
        modelBuilder.Entity<Department>(entity =>
        {
            // Defines Id as the primary key.
            entity.HasKey(department => department.Id);

            // Department Name is required and limited to 100 characters.
            entity.Property(department => department.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Prevents duplicate Department names.
            entity.HasIndex(department => department.Name)
                .IsUnique();
        });

        // Configures the Employee entity.
        modelBuilder.Entity<Employee>(entity =>
        {
            // Defines Id as the primary key.
            entity.HasKey(employee => employee.Id);

            // Employee Name is required and limited to 100 characters.
            entity.Property(employee => employee.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Employee Email is required and limited to 150 characters.
            entity.Property(employee => employee.Email)
                .IsRequired()
                .HasMaxLength(150);

            // Prevents duplicate employee emails.
            entity.HasIndex(employee => employee.Email)
                .IsUnique();

            // Defines the relationship between Employee and Department.
            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configures the User entity for authentication.
        modelBuilder.Entity<User>(entity =>
        {
            // Defines Id as the primary key.
            entity.HasKey(user => user.Id);

            // Username is required and limited to 50 characters.
            entity.Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(50);

            // Prevents duplicate usernames in the database.
            entity.HasIndex(user => user.Username)
                .IsUnique();

            // PasswordHash is required and limited to 500 characters.
            entity.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);
        });

        // Applies any remaining EF Core configurations.
        base.OnModelCreating(modelBuilder);
    }
}