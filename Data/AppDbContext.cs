
// Provides EF Core classes such as DbContext, DbSet and ModelBuilder.
using Microsoft.EntityFrameworkCore;

// Provides access to Department and Employee entity classes.
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


    // Configures database tables, columns, keys, indexes and relationships.
    // EF Core uses these configurations when generating migrations.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
        modelBuilder.Entity<Department>(entity =>
        {
       
            entity.HasKey(department => department.Id);
            entity.Property(department => department.Name)
                .IsRequired()
                .HasMaxLength(100);

        
            entity.HasIndex(department => department.Name)
                .IsUnique();
        });

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
        base.OnModelCreating(modelBuilder);
    }
}
