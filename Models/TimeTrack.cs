namespace MSFEP.Models;

public class TimeTrackEntry
{
    public int Id { get; set; }
    public string UserPrincipalName { get; set; } = string.Empty;
    public double Hours { get; set; }
    public DateTime Workday { get; set; }
    public int ProjectId { get; set; }
}
