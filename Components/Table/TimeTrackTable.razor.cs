using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MSFEP.Components.Miscellaneous;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Models;

namespace MSFEP.Components.Table;

public partial class TimeTrackTable : ComponentBase, IDisposable
{
    private List<TimeTrackEntry> timeTrackEntires = [];
    private List<Project> projects = [];
    private bool isCreateModalVisible;
    private bool isDetailModalVisible;

    private string filterInput = string.Empty;
    private bool OrderAscending = true;
    private string sortColumn = nameof(TimeTrackEntry.Id);

    private Virtualize<TimeTrackEntry> virtualizeRef = null!;
    private string selectedHeaderDropdown = string.Empty;

    [Inject]
    public ITimeTrackDataAccess dataAccess { get; set; } = null!;

    [Inject]
    public IProjectDataAccess projectDataAccess { get; set; } = null!;

    [Inject]
    public SidebarStateService SidebarState { get; set; } = null!;

    protected override void OnInitialized()
    {
        SidebarState.OnSidebarStateChanged += HandleSidebarStateChanged;
    }

    private void HandleSidebarStateChanged()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        SidebarState.OnSidebarStateChanged -= HandleSidebarStateChanged;
    }

    private async Task AddFilter()
    {
        if (!string.IsNullOrEmpty(selectedHeaderDropdown))
        {
            foreach (var column in columns.Where(e => string.Equals(e.Header, selectedHeaderDropdown)))
            {
                column.Filter = filterInput;
                selectedHeaderDropdown = string.Empty;
                await virtualizeRef.RefreshDataAsync();
                StateHasChanged();
            }
        }
    }

    private readonly List<ColumnDefinition> columns =
        [
            new() { PropertyName = nameof(TimeTrackEntry.Id), PropertyType = typeof(int), Header = "Id", },
            new() { PropertyName = nameof(TimeTrackEntry.UserPrincipalName), PropertyType = typeof(string), Header = "MitarbeiterId", },
            new() { PropertyName = nameof(TimeTrackEntry.Workday), PropertyType = typeof(DateTime), Header = "Tag", },
            new() { PropertyName = nameof(TimeTrackEntry.Hours), PropertyType = typeof(double), Header = "Arbeitsstunden", },
            new() { PropertyName = nameof(TimeTrackEntry.ProjectId), PropertyType = typeof(int), Header = "ProjektId", },
        ];

    private async ValueTask<ItemsProviderResult<TimeTrackEntry>> LoadItems(ItemsProviderRequest request)
    {
        Dictionary<string, string> appliedFilters = [];
        foreach (var column in columns.Where(e => !string.IsNullOrEmpty(e.Filter)))
        {
            appliedFilters.Add(column.PropertyName.ToLower(), column.Filter);
        }

        var timeTrackEntriesCount = await dataAccess.GetCountAsync();
        var result = await dataAccess.GetAsync(request.StartIndex, request.Count, sortColumn, OrderAscending, appliedFilters);
        result ??= [];

        return new ItemsProviderResult<TimeTrackEntry>(result, timeTrackEntriesCount);
    }

    private async Task SortColumn(string columnName)
    {
        if (sortColumn == columnName)
        {
            OrderAscending = !OrderAscending;
        }
        else
        {
            sortColumn = columnName;
            OrderAscending = true;
        }

        await virtualizeRef.RefreshDataAsync();
        SortArticles();
        StateHasChanged();
    }

    private void SortArticles()
    {
        if (!string.IsNullOrEmpty(sortColumn))
        {
            var propertyInfo = typeof(TimeTrackEntry).GetProperty(sortColumn);
            if (propertyInfo != null)
            {
                timeTrackEntires = (OrderAscending)
                    ? timeTrackEntires.OrderBy(a => propertyInfo.GetValue(a, null)).ToList()
                    : timeTrackEntires.OrderByDescending(a => propertyInfo.GetValue(a, null)).ToList();
            }
        }
    }

    private async Task CloseModals()
    {
        isCreateModalVisible = false;
        isDetailModalVisible = false;
        await virtualizeRef.RefreshDataAsync();
    }

    private async Task ShowCreateModal()
    {
        projects = (await projectDataAccess.GetAll()).ToList();
        isCreateModalVisible = true;
    }
}