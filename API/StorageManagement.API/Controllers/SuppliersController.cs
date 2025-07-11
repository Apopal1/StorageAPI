using Microsoft.AspNetCore.Mvc;
using StorageManagement.API.Models;
using StorageManagement.API.Repositories;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;

        public SuppliersController(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Supplier supplier)
        {
            try
            {
                var createdSupplier = await _supplierRepository.CreateAsync(supplier);
                return CreatedAtAction(nameof(GetById), new { id = createdSupplier.Id }, createdSupplier);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating supplier: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Supplier supplier)
        {
            try
            {
                var existingSupplier = await _supplierRepository.GetByIdAsync(id);
                if (existingSupplier == null)
                    return NotFound($"Supplier with ID {id} not found");
                
                supplier.Id = id;
                var updatedSupplier = await _supplierRepository.UpdateAsync(supplier);
                return Ok(updatedSupplier);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating supplier: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _supplierRepository.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
