using MechanicShop.Domain.Customers;
using MechanicShop.Domain.Customers.Vehicles;
using MechanicShop.Domain.RepairTasks;
using MechanicShop.Domain.RepairTasks.Enums;
using MechanicShop.Domain.RepairTasks.Parts;
using MechanicShop.Domain.Workorders;
using MechanicShop.Domain.WorkOrders.Enum;
using MechanicShop.Tests.Common.Security;

namespace MechanicShop.Api.IntegrationTests.Common;

public interface ITestDataBuilder<T>
{
    T Build();
}

public class PartTestDataBuilder : ITestDataBuilder<Part>
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Oil Filter";
    private decimal _cost = 25m;
    private int _quantity = 1;

    public static PartTestDataBuilder Create() => new();

    public PartTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public PartTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public PartTestDataBuilder WithCost(decimal cost)
    {
        _cost = cost;
        return this;
    }

    public PartTestDataBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public Part Build()
    {
        return Part.Create(
            id: _id,
            name: _name,
            cost: _cost,
            quantity: _quantity).Value;
    }
}

public class RepairTaskTestDataBuilder : ITestDataBuilder<RepairTask>
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Oil Change";
    private decimal _laborCost = 75m;
    private RepairDurationInMinutes _estimatedDurationInMins = RepairDurationInMinutes.Min30;
    private List<Part> _parts = [PartTestDataBuilder.Create().Build()];

    public static RepairTaskTestDataBuilder Create() => new();

    public RepairTaskTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public RepairTaskTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public RepairTaskTestDataBuilder WithLaborCost(decimal laborCost)
    {
        _laborCost = laborCost;
        return this;
    }

    public RepairTaskTestDataBuilder WithEstimatedDuration(RepairDurationInMinutes estimatedDurationInMins)
    {
        _estimatedDurationInMins = estimatedDurationInMins;
        return this;
    }

    public RepairTaskTestDataBuilder WithParts(params Part[] parts)
    {
        _parts = [.. parts];
        return this;
    }

    public RepairTaskTestDataBuilder WithParts(List<Part> parts)
    {
        _parts = parts;
        return this;
    }

    public RepairTask Build()
    {
        return RepairTask.Create(
            id: _id,
            name: _name,
            laborCost: _laborCost,
            estimatedDurationInMins: _estimatedDurationInMins,
            parts: _parts).Value;
    }
}

public class VehicleTestDataBuilder : ITestDataBuilder<Vehicle>
{
    private Guid _id = Guid.NewGuid();
    private string _make = "Toyota";
    private string _model = "Camry";
    private int _year = 2020;
    private string _licensePlate = $"TST-{Guid.NewGuid():N}"[..10];

    public static VehicleTestDataBuilder Create() => new();

    public VehicleTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public VehicleTestDataBuilder WithMake(string make)
    {
        _make = make;
        return this;
    }

    public VehicleTestDataBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public VehicleTestDataBuilder WithYear(int year)
    {
        _year = year;
        return this;
    }

    public VehicleTestDataBuilder WithLicensePlate(string licensePlate)
    {
        _licensePlate = licensePlate;
        return this;
    }

    public Vehicle Build()
    {
        return Vehicle.Create(
            id: _id,
            make: _make,
            model: _model,
            year: _year,
            licensePlate: _licensePlate).Value;
    }
}

public class CustomerTestDataBuilder : ITestDataBuilder<Customer>
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Customer";
    private string _phoneNumber = "1234567890";
    private string _email = $"customer-{Guid.NewGuid():N}@example.com";
    private List<Vehicle> _vehicles = [VehicleTestDataBuilder.Create().Build()];

    public static CustomerTestDataBuilder Create() => new();

    public CustomerTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CustomerTestDataBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CustomerTestDataBuilder WithPhoneNumber(string phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public CustomerTestDataBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CustomerTestDataBuilder WithVehicles(params Vehicle[] vehicles)
    {
        _vehicles = [.. vehicles];
        return this;
    }

    public CustomerTestDataBuilder WithVehicles(List<Vehicle> vehicles)
    {
        _vehicles = vehicles;
        return this;
    }

    public Customer Build()
    {
        return Customer.Create(
            id: _id,
            name: _name,
            phoneNumber: _phoneNumber,
            email: _email,
            vehicles: _vehicles).Value;
    }
}

public class WorkOrderTestDataBuilder : ITestDataBuilder<WorkOrder>
{
    private Guid _id = Guid.NewGuid();
    private Guid _vehicleId = Guid.NewGuid();
    private DateTimeOffset _startAt = DateTimeOffset.UtcNow;
    private DateTimeOffset _endAt = DateTimeOffset.UtcNow.AddHours(2);
    private Guid _laborId = Guid.Parse(TestUsers.Labor01.Id);
    private Spot _spot = Spot.A;
    private List<RepairTask> _repairTasks = [];
    private WorkOrderState _state = WorkOrderState.Scheduled;

    public static WorkOrderTestDataBuilder Create() => new();

    public WorkOrderTestDataBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public WorkOrderTestDataBuilder WithVehicle(Guid vehicleId)
    {
        _vehicleId = vehicleId;
        return this;
    }

    public WorkOrderTestDataBuilder WithTimeSlot(DateTimeOffset startAt, DateTimeOffset endAt)
    {
        _startAt = startAt;
        _endAt = endAt;
        return this;
    }

    public WorkOrderTestDataBuilder WithLabor(string laborId)
    {
        _laborId = Guid.Parse(laborId);
        return this;
    }

    public WorkOrderTestDataBuilder WithLabor(Guid laborId)
    {
        _laborId = laborId;
        return this;
    }

    public WorkOrderTestDataBuilder AtSpot(Spot spot)
    {
        _spot = spot;
        return this;
    }

    public WorkOrderTestDataBuilder WithRepairTasks(params RepairTask[] repairTasks)
    {
        _repairTasks = [.. repairTasks];
        return this;
    }

    public WorkOrderTestDataBuilder WithRepairTasks(List<RepairTask> repairTasks)
    {
        _repairTasks = repairTasks;
        return this;
    }

    public WorkOrderTestDataBuilder WithState(WorkOrderState state)
    {
        _state = state;
        return this;
    }

    public WorkOrderTestDataBuilder ForToday(TimeOnly? from = null, TimeOnly? to = null)
    {
        var today = DateTimeOffset.UtcNow.Date;

        var fromTime = from ?? new TimeOnly(9, 0);
        var toTime = to ?? new TimeOnly(11, 0);

        _startAt = today.Add(fromTime.ToTimeSpan());
        _endAt = today.Add(toTime.ToTimeSpan());

        return this;
    }

    public WorkOrderTestDataBuilder InProgress()
    {
        _state = WorkOrderState.InProgress;
        _startAt = DateTimeOffset.UtcNow.AddMinutes(-30);
        return this;
    }

    public WorkOrderTestDataBuilder Completed()
    {
        _state = WorkOrderState.Completed;
        _startAt = DateTimeOffset.UtcNow.AddHours(-3);
        _endAt = DateTimeOffset.UtcNow.AddHours(-1);
        return this;
    }

    public WorkOrder Build()
    {
        var workOrder = WorkOrder.Create(
            id: _id,
            vehicleId: _vehicleId,
            startAt: _startAt,
            endAt: _endAt,
            laborId: _laborId,
            spot: _spot,
            repairTasks: _repairTasks).Value;

        if (_state != WorkOrderState.Scheduled)
        {
            workOrder.UpdateState(_state);
        }

        return workOrder;
    }
}
