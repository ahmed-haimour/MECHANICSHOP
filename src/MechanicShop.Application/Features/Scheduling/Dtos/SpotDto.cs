using MechanicShop.Domain.WorkOrders.Enum;

namespace MechanicShop.Application.Features.Scheduling.Dtos;

public class SpotDto
{
    public Spot Spot { get; set; }
    public List<AvailabilitySlotDto> Slots { get; set; } = [];
}