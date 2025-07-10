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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] StorageItem item)
        {
            var createdItem = await _storageItemRepository.CreateAsync(item);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] StorageItem item)
        {
            item.Id = id;
            var updatedItem = await _storageItemRepository.UpdateAsync(item);
            return Ok(updatedItem);
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
