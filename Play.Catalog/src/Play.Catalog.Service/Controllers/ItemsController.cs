using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IRepository _itemsRepository;

    public ItemsController(IRepository itemsRepository)
    {
        _itemsRepository = itemsRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
        var items = (await _itemsRepository.GetAllAsync())
                    .Select(item => item.AsDto());
        return items;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item.AsDto());
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(CreateItemDto createItemDto)
    {
        var item = new Item
        {
            Name = createItemDto.Name,
            Description = createItemDto.Description,
            Price = createItemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _itemsRepository.CreateAsync(item);

        return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
        var existingItem = await _itemsRepository.GetAsync(id);

        if (existingItem == null)
        {
            return NotFound();
        }

        existingItem.Name = updateItemDto.Name;
        existingItem.Description = updateItemDto.Description;
        existingItem.Price = updateItemDto.Price;

        await _itemsRepository.UpdateAsync(existingItem);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var item = await _itemsRepository.GetAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        await _itemsRepository.RemoveAsync(item.Id);

        return NoContent();
    }
}