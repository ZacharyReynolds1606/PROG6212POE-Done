using RaceDay.Contracts.Categories;
using RaceDay.Contracts.Events;

namespace RaceDay.Mvc.ViewModels;

public class EventDetailsVm
{
    public EventDto Event { get; set; } = new();
    public List<CategoryDto> Categories { get; set; } = new();
}
