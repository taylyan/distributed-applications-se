namespace HuntingPermitTripManagement.Web.Models;

public class PermitViewModel
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string PermitNumber { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }

    public DateTime ExpirationDate { get; set; }

    public bool IsActive { get; set; }

    public string PermitType { get; set; } = string.Empty;
}