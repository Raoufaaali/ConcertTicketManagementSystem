using System.ComponentModel.DataAnnotations;

namespace Concert_Ticket_Management_System.Shared;

public class ManageCapacityRequest
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Capacity must be a non-negative integer.")]
    public int NewCapacity { get; set; }
}