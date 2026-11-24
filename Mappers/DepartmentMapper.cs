using StaffDb.DTOs;
using StaffDb.Models;

namespace StaffDb.Mappers;

public static class DepartmentMapper
{
    public static DepartmentResponse ToResponse(
        Department department)
    {
        return new DepartmentResponse
        {
            Id = department.Id,
            Name = department.Name
        };
    }
}