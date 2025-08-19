using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class StudentCourseRegistration
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RegistrationID { get; set; }   // PK, Identity

    // 🔗 Foreign keys
    [Required]
    public int StudentID { get; set; }

    [Required]
    public int CourseID { get; set; }

    [Required]
    public int ExamID { get; set; }   // The exam cycle used for entry

    [Required]
    [MaxLength(50)]
    public string RollNumber { get; set; } = string.Empty;  // Unique per ExamID

    [MaxLength(20)]
    public string ApplicationStatus { get; set; } = "Submitted";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 🔗 Navigation properties
    [ForeignKey("StudentID")]
    public Student? Student { get; set; }

    [ForeignKey("CourseID")]
    public Course? Course { get; set; }

    [ForeignKey("ExamID")]
    public Exam? Exam { get; set; }

    public ICollection<ExamResult>? ExamResults { get; set; }
}