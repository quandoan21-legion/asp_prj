using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApi.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserID { get; set; }   // PK, Identity

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;   // Login username

    [Required]
    [MaxLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;      // Login email

    [MaxLength(30)]
    public string? Phone { get; set; }                     // Optional phone

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; set; } = string.Empty; // Store hashed password

    public bool IsAdmin { get; set; } = false;             // 👑 Admin flag

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
}