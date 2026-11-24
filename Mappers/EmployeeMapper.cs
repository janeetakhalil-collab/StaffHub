using StaffDb.DTOs;
using StaffDb.Models;

namespace StaffDb.Mappers;

public static class EmployeeMapper
{
    public static EmployeeResponse ToResponse(
        Employee employee)
    {
        return new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.Name,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name
        };
    }
}