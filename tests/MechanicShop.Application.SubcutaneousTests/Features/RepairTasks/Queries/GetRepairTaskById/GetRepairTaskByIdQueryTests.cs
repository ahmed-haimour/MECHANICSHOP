using MechanicShop.Application.Features.RepairTasks.Queries.GetRepairTaskById;
using Xunit;

namespace MechanicShop.Application.SubcutaneousTests.Features.RepairTasks.Queries.GetRepairTaskById;

public class GetRepairTaskByIdQueryTests
{
    [Fact]
    public void Constructor_ShouldSetProperties()
    {
        var repairTaskId = Guid.NewGuid();

        var query = new GetRepairTaskByIdQuery(repairTaskId);

        Assert.Equal(repairTaskId, query.RepairTaskId);
        Assert.Equal($"repair-task_{repairTaskId}", query.CacheKey);
        Assert.Equal(["repair-task"], query.Tags);
    }
}
