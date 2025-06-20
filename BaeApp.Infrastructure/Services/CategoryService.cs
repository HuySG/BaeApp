using BaeApp.Core.DTOs;
using BaeApp.Core.Entities;
using BaeApp.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }
        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var entity = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
            };
            await _repo.AddAsync(entity);
            return new CategoryDto
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
            };

        }

        public async Task<bool> DeleteAsync(Guid categoryId)
        {
            var entity = await _repo.GetByIdAsync(categoryId);
            if (categoryId == null)
            {
                return false;
            }
            await _repo.DeleteAsync(entity);
            return true;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var entity = await _repo.GetAllAsync();
            return entity.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
            }).ToList();
        }

        public async Task<CategoryDto> GetByIdAsync(Guid categoryId)
        {
            var entity = await _repo.GetByIdAsync(categoryId);
            return entity == null ? null : new CategoryDto
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Description = entity.Description,
            };
        }

        public async Task<bool> UpdateAsync(Guid categoryId, UpdateCategoryDto dto)
        {
            var entity = await _repo.GetByIdAsync(categoryId);
            if (entity == null)
            {
                return false;
            }
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            await _repo.UpdateAsync(entity);
            return true;

        }
    }
}
