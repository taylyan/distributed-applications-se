using System.ComponentModel.DataAnnotations;

namespace HuntingPermitTripManagement.Api.Models;

public class Permit
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(30)]
    public string PermitNumber { get; set; } = string.Empty;

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    [MaxLength(50)]
    public string PermitType { get; set; } = string.Empty;

    public User? User { get; set; }
}
