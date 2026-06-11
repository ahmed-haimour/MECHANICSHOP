using MechanicShop.Application.Features.Labors.Mappers;
using MechanicShop.Domain.Employees;
using MechanicShop.Tests.Common.Employees;
using Xunit;

namespace MechanicShop.Application.UnitTests.Mappers;

public class LaborMapperTest
{
    [Fact]
    public void ToDto_ShouldMapCorrectly()
    {
        var labor = EmployeeFactory.CreateLabor(
            firstName: "John",
            lastName: "Labor").Value;

        var dto = labor.ToDto();

        Assert.Equal(labor.Id, dto.LaborId);
        Assert.Equal(labor.FullName, dto.Name);
    }

    [Fact]
    public void ToDtos_ShouldMapListCorrectly()
    {
        var labor = EmployeeFactory.CreateLabor(
            firstName: "John",
            lastName: "Labor").Value;

        var labors = new List<Employee> { labor };

        var dtos = labors.ToDtos();

        Assert.Single(dtos);
        var dto = dtos[0];

        Assert.Equal(labor.Id, dto.LaborId);
        Assert.Equal(labor.FullName, dto.Name);
    }
}
