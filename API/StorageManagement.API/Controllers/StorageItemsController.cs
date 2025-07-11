using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageItemsController : ControllerBase
    {
        private readonly IStorageItemRepository _storageItemRepository;

        public StorageItemsController(IStorageItemRepository storageItemRepository)
        {
            _storageItemRepository = storageItemRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _storageItemRepository.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _storageItemRepository.GetByIdAsync(id);
            if (item == null)
                return NotFound();
            return Ok(item);
        }

        [HttpGet("by-supplier/{supplierId}")]
        public async Task<IActionResult> GetBySupplier(int supplierId)
        {
            var items = await _storageItemRepository.GetBySupplierIdAsync(supplierId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StorageItem item)
        {
            try
            {
                var createdItem = await _storageItemRepository.CreateAsync(item);
                return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating item: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StorageItem item)
        {
            try
            {
                var existingItem = await _storageItemRepository.GetByIdAsync(id);
                if (existingItem == null)
                    return NotFound($"Item with ID {id} not found");
                
                item.Id = id;
                var updatedItem = await _storageItemRepository.UpdateAsync(item);
                return Ok(updatedItem);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating item: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _storageItemRepository.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
