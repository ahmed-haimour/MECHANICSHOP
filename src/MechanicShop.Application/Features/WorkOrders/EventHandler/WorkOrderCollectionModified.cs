using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common;
using MechanicShop.Domain.WorkOrders.Events;
using MediatR;

namespace MechanicShop.Domain.Workorders.Events;


public sealed class WorkOrderCollectionModifiedEventHandler(IWorkOrderNotifier notifier)
        : INotificationHandler<WorkOrderCollectionModified>
{
    private readonly IWorkOrderNotifier _notifier = notifier;

    public Task Handle(WorkOrderCollectionModified notification, CancellationToken ct) =>
        _notifier.NotifyWorkOrdersChangedAsync(ct);
}