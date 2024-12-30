using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MSFEP.Components.Miscellaneous;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Models;

namespace MSFEP.Components.Table;
public partial class ProjectTable : ComponentBase, IDisposable
{
    private List<Project> projects = [];
    private bool isCreateModalVisible;
    private bool isDetailModalVisible;

    private string filterInput = string.Empty;
    private bool OrderAscending = true;
    private string sortColumn = nameof(Project.Id);

    private Virtualize<Project> virtualizeRef = null!;
    private string selectedHeaderDropdown = string.Empty;

    [Inject]
    public IProjectDataAccess dataAccess { get; set; } = null!;

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
            new() { PropertyName = nameof(Project.Id), PropertyType = typeof(int), Header = "Id", },
            new() { PropertyName = nameof(Project.Name), PropertyType = typeof(string), Header = "Name", },
            new() { PropertyName = nameof(Project.Manager), PropertyType = typeof(string), Header = "Manager", },
        ];

    private async ValueTask<ItemsProviderResult<Project>> LoadItems(ItemsProviderRequest request)
    {
        Dictionary<string, string> appliedFilters = [];
        foreach (var column in columns.Where(e => !string.IsNullOrEmpty(e.Filter)))
        {
            appliedFilters.Add(column.PropertyName.ToLower(), column.Filter);
        }

        var projectsCount = await dataAccess.GetCountAsync();
        var result = await dataAccess.GetAsync(request.StartIndex, request.Count, sortColumn, OrderAscending, appliedFilters);
        result ??= [];

        return new ItemsProviderResult<Project>(result, projectsCount);
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
            var propertyInfo = typeof(Project).GetProperty(sortColumn);
            if (propertyInfo != null)
            {
                projects = (OrderAscending)
                    ? projects.OrderBy(a => propertyInfo.GetValue(a, null)).ToList()
                    : projects.OrderByDescending(a => propertyInfo.GetValue(a, null)).ToList();
            }
        }
    }

    private async Task CloseModals()
    {
        isCreateModalVisible = false;
        isDetailModalVisible = false;
        await virtualizeRef.RefreshDataAsync();
    }

    private async Task ShowCreateModal() => isCreateModalVisible = true;
}
