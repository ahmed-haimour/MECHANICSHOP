using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTasks;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Queries.GetRepairTasks;

public class GetRepairTasksQueryTests
{
    [Fact]
    public void Constructor_ShouldSetCacheProperties()
    {
        var query = new GetRepairTasksQuery();

        Assert.Equal("repair-tasks", query.CacheKey);
        Assert.Equal(["repair-tasks"], query.Tags);
    }
}
