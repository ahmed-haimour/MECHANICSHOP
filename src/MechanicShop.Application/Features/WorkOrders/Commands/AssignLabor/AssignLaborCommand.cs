using MechanicShop.Domain.Common.Results;
using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Workorders;
using MediatR;

namespace MechanicShop.Application.Features.WorkOrders.Commands.AssignLabor;

public sealed record AssignLaborCommand(Guid WorkOrderId, Guid LaborId) : IRequest<Result<Updated>>;