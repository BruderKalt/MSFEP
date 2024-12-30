using System.ComponentModel.DataAnnotations;

namespace MSFEP.Components.Table;

public class ColumnDefinition
{
    [Required]
    public string PropertyName { get; set; } = string.Empty;

    [Required]
    public Type PropertyType { get; set; } = null!;

    [Required]
    public string Header { get; set; } = string.Empty;

    [Required]
    public string Filter { get; set; } = string.Empty;

    [Required]
    public bool Hidden { get; set; } = false;

    public void ToggleHidden() => Hidden = !Hidden;

    public void ChangeFilter(string filter) => Filter = filter;
}


