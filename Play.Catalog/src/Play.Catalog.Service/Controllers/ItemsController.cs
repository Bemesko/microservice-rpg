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
    public IActionResult Get(Guid? id)
    {
        var item = items.FirstOrDefault(item => item.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public IActionResult Post(CreateItemDto createItemDto)
    {
        var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);

        items.Add(item);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = items.Where(item => item.Id == id).SingleOrDefault();

        var updatedItem = existingItem with
        {
            Name = updateItemDto.Name,
            Description = updateItemDto.Description,
            Price = updateItemDto.Price
        };

        if (updatedItem == null)
        {
            return NotFound();
        }

        var index = items.FindIndex(existingItem => existingItem.Id == id);
        items[index] = updatedItem;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var index = items.FindIndex(existingItem => existingItem.Id == id);

        if (index < 0)
        {
            return NotFound();
        }

        items.RemoveAt(index);

        return NoContent();
    }
}