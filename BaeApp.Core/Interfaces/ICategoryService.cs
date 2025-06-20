using BaeApp.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaeApp.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(Guid categoryId);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

        Task<bool> UpdateAsync(Guid categoryId, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(Guid categoryId);
    }
}
