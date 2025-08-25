namespace MyApi.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Exam
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExamId { get; set; } // PK, Identity

    [Required] [MaxLength(150)] public string Name { get; set; } = string.Empty;

    [Required] public DateTime ExamDate { get; set; }

    [Required] public DateTime ApplicationOpen { get; set; }

    [Required] public DateTime ApplicationClose { get; set; }

    public DateTime? ResultPublishDate { get; set; } // optional

    [Column(TypeName = "decimal(10,2)")] public decimal FeeAmount { get; set; }

    public string? Description { get; set; } // NVARCHAR(MAX)

    public bool IsActive { get; set; } = true;

    public int? CourseId { get; set; }

    [ForeignKey("CourseId")] public Course? Course { get; set; }
}