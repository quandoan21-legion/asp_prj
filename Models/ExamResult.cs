using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class ExamResult
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ResultID { get; set; }   // PK, Identity

    [Required]
    public int RegistrationID { get; set; }  // FK → StudentCourseRegistrations.RegistrationID

    [Column(TypeName = "decimal(5,2)")]
    public decimal MarksObtained { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? MaxMarks { get; set; }   // Optional

    [MaxLength(50)]
    public string? AssignedClass { get; set; }  // e.g., "With Basics"

    [Column(TypeName = "decimal(10,2)")]
    public decimal CourseFee { get; set; }

    public DateTime PaymentDeadline { get; set; }

    [MaxLength(20)]
    public string PaymentStatus { get; set; } = "Pending";  // e.g., Pending/Paid

    public bool ExtraLabOpted { get; set; } = false;

    [Column(TypeName = "decimal(10,2)")]
    public decimal ExtraLabFee { get; set; } = 0;

    public string? Notes { get; set; } // Faculty feedback (optional)

    // 🔗 Navigation property (FK relationship)
    [ForeignKey("RegistrationID")]
    public StudentCourseRegistration? Registration { get; set; }
}
