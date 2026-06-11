using MechanicShop.Application.Features.RepairTasks.Mappers;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Tests.Common.RepaireTasks;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;


public class RepairTaskMapperTest
{
    [Fact]
    public void ToDto_ShouldMapCorrectly()
    {
        var part = PartFactory.CreatePart(
            name: "Oil Filter",
            cost: 25m,
            quantity: 2).Value;

        var repairTask = RepairTaskFactory.CreateRepairTask(
            name: "Oil Change",
            laborCost: 100m,
            repairDurationInMinutes: RepairDurationInMinutes.Min45,
            parts: [part]).Value;

        var expectedTotalCost = repairTask.LaborCost + (part.Cost * part.Quantity);

        var dto = repairTask.ToDto();

        Assert.Equal(repairTask.Id, dto.RepairTaskId);
        Assert.Equal(repairTask.Name, dto.Name);
        Assert.Equal(repairTask.LaborCost, dto.LaborCost);
        Assert.Equal(repairTask.EstimatedDurationInMins, dto.EstimatedDurationInMins);
        Assert.Equal(expectedTotalCost, dto.TotalCost);

        Assert.Single(dto.Parts);
        Assert.Equal(part.Id, dto.Parts[0].PartId);
        Assert.Equal(part.Name, dto.Parts[0].Name);
        Assert.Equal(part.Cost, dto.Parts[0].Cost);
        Assert.Equal(part.Quantity, dto.Parts[0].Quantity);
    }

    [Fact]
    public void ToDtos_ShouldMapListCorrectly()
    {
        var repairTask = RepairTaskFactory.CreateRepairTask(
            name: "Brake Inspection",
            laborCost: 150m,
            repairDurationInMinutes: RepairDurationInMinutes.Min60,
            parts:
            [
                PartFactory.CreatePart(name: "Brake Pads", cost: 80m, quantity: 1).Value
            ]).Value;

        var repairTasks = new List<RepairTask> { repairTask };

        var dtos = repairTasks.ToDtos();

        Assert.Single(dtos);
        var dto = dtos[0];

        Assert.Equal(repairTask.Id, dto.RepairTaskId);
        Assert.Equal(repairTask.Name, dto.Name);
        Assert.Equal(repairTask.LaborCost, dto.LaborCost);
        Assert.Equal(repairTask.TotalCost, dto.TotalCost);
        Assert.Equal(repairTask.EstimatedDurationInMins, dto.EstimatedDurationInMins);
        Assert.Single(dto.Parts);
    }
}
