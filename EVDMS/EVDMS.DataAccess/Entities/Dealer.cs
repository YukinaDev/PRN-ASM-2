using System.Collections.Generic;

namespace EVDMS.DataAccess.Entities;

public class Dealer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<DealerAllocation> Allocations { get; set; } = new List<DealerAllocation>();
}
