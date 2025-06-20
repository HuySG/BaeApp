using BaeApp.Core.DTOs;
using BaeApp.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BaeApp.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var category = await _categoryService.GetAllAsync();
            if (category == null)
            {
                return NotFound(new { error = "Chưa có Categories nào" });
            }
            return Ok(category);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { error = "Không tìm thấy Categories này" });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var create = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = create.CategoryId }, create);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var update = await _categoryService.UpdateAsync(id, dto);
            return update ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var delete = await _categoryService.DeleteAsync(id);
            return delete ? NoContent() : NotFound();
        }

    }
}
