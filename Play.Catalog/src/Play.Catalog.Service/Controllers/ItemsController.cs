using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private static readonly List<ItemDto> items = new()
    {
        new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Arrow", "+5 Damage to Knees", 2, DateTimeOffset.UtcNow),
        new ItemDto(Guid.NewGuid(), "Staff of Borb", "Effective against frog-type enemies", 20, DateTimeOffset.UtcNow),
    };

    [HttpGet]
    public IEnumerable<ItemDto> Get()
    {
        return items;
    }

    //TODO: make this return an IActionResult
    [HttpGet("{id}")]
    public ItemDto Get(Guid? id)
    {
        return items.FirstOrDefault(item => item.Id == id);
    }

    [HttpPost]
    public IActionResult Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);

        items.Add(item);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }
}