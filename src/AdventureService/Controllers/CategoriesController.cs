using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdventureService.Data;
using AdventureService.Infraestructure;
using AdventureService.Models;
using AdventureService.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace AdventureService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AdventureDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AdventuresController> _localizer;
        private readonly ILogger<AdventuresController> _logger;

        public CategoriesController(AdventureDbContext dbContext, IMapper mapper,
            IStringLocalizer<AdventuresController> localizer, ILogger<AdventuresController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _localizer = localizer;
            _logger = logger;
        }

        #region CREATE
        // POST api/categories
        [HttpPost]
        [ProducesResponseType(typeof(ItemsViewModel<CategoryResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryCreateRequest categoryCreateRequest)
        {
            var newCategory = _mapper.Map<Category>(categoryCreateRequest);
            await _dbContext.Categories.AddAsync(newCategory);
            await _dbContext.SaveChangesAsync();
            var newCategoriesResponse = new[] { _mapper.Map<CategoryResponse>(newCategory) };
            var response = new ItemsViewModel<CategoryResponse>(newCategoriesResponse.LongCount(), newCategoriesResponse);
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = newCategory.Id }, response);
        }

        // POST api/categories/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(ItemsViewModel<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkCreateCategoriesAsync([FromBody] params CategoryCreateRequest[] categoriesCreateRequest)
        {
            var newCategories = _mapper.Map<ICollection<Category>>(categoriesCreateRequest);
            await _dbContext.Categories.AddRangeAsync(newCategories);
            await _dbContext.SaveChangesAsync();
            var newCategoriesResponse = _mapper.Map<ICollection<CategoryResponse>>(newCategories);
            var response = new ItemsViewModel<CategoryResponse>(newCategoriesResponse.LongCount(), newCategoriesResponse);
            return Ok(response);
        }
        #endregion

        #region READ
        // GET api/categories
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] int pageSize = 10, int pageIndex = 0)
        {
            var totalCategories = await _dbContext.Categories.LongCountAsync();
            if (totalCategories == 0)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = _localizer.GetString("CategoriesNotFound")
                });
            }
            var categoriesOnPage = await _dbContext.Categories
                .OrderBy(x => x.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            var categoryViewModelsOnPage = _mapper.Map<ICollection<CategoryResponse>>(categoriesOnPage);
            var response = new PaginatedItemsViewModel<CategoryResponse>(pageIndex, pageSize, totalCategories, categoryViewModelsOnPage);

            return Ok(response);
        }

        // GET api/categories/b6561e5e-a9a1-46f3-9160-023c65a66184
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemsViewModel<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryByIdAsync(string id)
        {
            var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = _localizer.GetString("CategoriesNotFound")
                });
            }
            var categoriesViewModel = new[] { _mapper.Map<CategoryResponse>(category) };
            var response = new ItemsViewModel<CategoryResponse>(categoriesViewModel.LongCount(), categoriesViewModel);
            return Ok(response);
        }
        #endregion

        #region UPDATE
        // PUT api/categories
        [HttpPut]
        [ProducesResponseType(typeof(ItemsViewModel<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategoryByIdAsync([FromBody] CategoryUpdateRequest categoryUpdateRequest)
        {
            var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == categoryUpdateRequest.Id);
            if (category == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = string.Format(_localizer["CategoryNotFoundIdParameter"], categoryUpdateRequest.Id)
                });
            }
            _mapper.Map<CategoryUpdateRequest, Category>(categoryUpdateRequest, category);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/categories/bulk
        [HttpPut("bulk")]
        [ProducesResponseType(typeof(ItemsViewModel<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkUpdateCategoriesByIdAsync([FromBody] params CategoryUpdateRequest[] categoriesUpdateRequest)
        {
            var errors = new StringBuilder();
            foreach (var categoryUpdateRequest in categoriesUpdateRequest)
            {
                var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == categoryUpdateRequest.Id);
                if (category != null)
                {
                    _mapper.Map<CategoryUpdateRequest, Category>(categoryUpdateRequest, category);
                }
                else
                {
                    errors.AppendLine(string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), categoryUpdateRequest.Id));
                }
            }
            if (errors.Length > 0)
            {
                return BadRequest(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = errors.ToString()
                });
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region DELETE
        // DELETE api/categories/ecf40cb1-423f-466b-8d2b-788b2f87d09b
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategoryByIdAsync(string id)
        {
            var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = string.Format(_localizer["CategoryNotFoundIdParameter"], id)
                });
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        // DELETE api/categories/bulk
        [HttpDelete("bulk")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkDeleteCategoriesByIdAsync([FromBody] params string[] ids)
        {
            var categoriesToDelete = new List<Category>();
            var errors = new StringBuilder();
            foreach (var id in ids)
            {
                var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == id);
                if (category != null)
                {
                    categoriesToDelete.Add(category);
                }
                else
                {
                    errors.AppendLine(string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), id));
                }
            }
            if (errors.Length > 0)
            {
                return BadRequest(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = errors.ToString()
                });
            }
            _dbContext.Categories.RemoveRange(categoriesToDelete);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion
    }
}