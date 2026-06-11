using MechanicShop.Application.Features.WorkOrders.Dtos;
using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.WorkOrders.Enum;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.CreateWorkOrder;

public sealed record CreateWorkOrderCommand(
    Spot Spot,
    Guid VehicleId,
    DateTimeOffset StartAt,
    List<Guid> RepairTaskIds,
    Guid? LaborId)
: IRequest<Result<WorkOrderDto>>;