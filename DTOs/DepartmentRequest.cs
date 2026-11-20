
using System.ComponentModel.DataAnnotations;

namespace StaffDb.DTOs;

public class DepartmentRequest
{
   
    [Required(ErrorMessage = "Department name is required.")]

    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "Department name must contain 2 to 100 characters.")]
    public string Name { get; set; } = string.Empty;
}
