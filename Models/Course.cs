using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class Course
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CourseID { get; set; }   // PK, Identity

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;   // Unique course code

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;  // Course name

    public string? Description { get; set; }           // NVARCHAR(MAX)

    public bool IsNew { get; set; }                    // Marks “New course”
    public bool IsActive { get; set; }                 // Soft on/off

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
