
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffDb.Data;
using StaffDb.DTOs;
using StaffDb.Filters;
using StaffDb.Mappers;
using StaffDb.Models;

namespace StaffDb.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController(
    AppDbContext context) : ControllerBase
{
    // GET: api/departments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponse>>> GetDepartments(
        CancellationToken cancellationToken)
    {
        var departments = await context.Departments
            .AsNoTracking()
            .OrderBy(department => department.Id)
            .ToListAsync(cancellationToken);

        var response = departments
            .Select(DepartmentMapper.ToResponse)
            .ToList();

        return Ok(response);
    }

    // GET: api/departments/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentResponse>> GetDepartment(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);

        if (department is null)
            return NotFound("Department not found.");

        return Ok(DepartmentMapper.ToResponse(department));
    }

    // POST: api/departments
    [HttpPost]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<ActionResult<DepartmentResponse>> CreateDepartment(
        DepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();

        var departmentExists = await context.Departments
            .AnyAsync(
                department => department.Name == name,
                cancellationToken);

        if (departmentExists)
            return Conflict(
                "A department with this name already exists.");

        var department = new Department
        {
            Name = name
        };

        context.Departments.Add(department);

        await context.SaveChangesAsync(cancellationToken);

        var response = DepartmentMapper.ToResponse(department);

        return CreatedAtAction(
            nameof(GetDepartment),
            new { id = department.Id },
            response);
    }

    // PUT: api/departments/1
    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ValidationFilter))]
    public async Task<ActionResult<DepartmentResponse>> UpdateDepartment(
        int id,
        DepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);

        if (department is null)
            return NotFound("Department not found.");

        var name = request.Name.Trim();

        var nameExists = await context.Departments
            .AnyAsync(
                department =>
                    department.Name == name &&
                    department.Id != id,
                cancellationToken);

        if (nameExists)
            return Conflict(
                "Another department already uses this name.");

        department.Name = name;

        await context.SaveChangesAsync(cancellationToken);

        return Ok(DepartmentMapper.ToResponse(department));
    }

    // DELETE: api/departments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteDepartment(
        int id,
        CancellationToken cancellationToken)
    {
        var department = await context.Departments
            .FirstOrDefaultAsync(
                department => department.Id == id,
                cancellationToken);

        if (department is null)
            return NotFound("Department not found.");

        var hasEmployees = await context.Employees
            .AnyAsync(
                employee => employee.DepartmentId == id,
                cancellationToken);

        if (hasEmployees)
            return Conflict(
                "This department cannot be deleted because employees are assigned to it.");

        context.Departments.Remove(department);

        await context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}
