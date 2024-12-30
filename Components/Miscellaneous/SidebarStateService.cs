namespace MSFEP.Components.Miscellaneous;

public class SidebarStateService
{
    private bool isCollapsed = true;
    public event Action OnSidebarStateChanged;

    public bool IsCollapsed
    {
        get => isCollapsed;
        set
        {
            if (isCollapsed != value)
            {
                isCollapsed = value;
                NotifySidebarStateChanged();
            }
        }
    }

    private void NotifySidebarStateChanged() => OnSidebarStateChanged?.Invoke();
}
