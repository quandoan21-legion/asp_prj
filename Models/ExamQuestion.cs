// Models/ExamQuestion.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class ExamQuestion
{
    [Key] public int QuestionId { get; set; }

    [Required] [Column("ExamID")] public int ExamId { get; set; }

    [Required] [MinLength(10)] public string QuestionText { get; set; } = string.Empty;

    [Required] public string OptionA { get; set; } = string.Empty;

    [Required] public string OptionB { get; set; } = string.Empty;

    [Required] public string OptionC { get; set; } = string.Empty;

    [Required] public string OptionD { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^[A-D]$", ErrorMessage = "CorrectAnswer must be A, B, C, or D")]
    public string CorrectAnswer { get; set; } = string.Empty;

    [Range(1, 10)] public float Points { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    [ForeignKey("ExamID")] public Exam? Exam { get; set; }
}