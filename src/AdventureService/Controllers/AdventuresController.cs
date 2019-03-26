using System;
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
    public class AdventuresController : ControllerBase
    {

        private readonly AdventureDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<AdventuresController> _localizer;
        private readonly ILogger<AdventuresController> _logger;

        public AdventuresController(AdventureDbContext dbContext, IMapper mapper,
            IStringLocalizer<AdventuresController> localizer, ILogger<AdventuresController> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _localizer = localizer;
            _logger = logger;
        }

        #region CREATE
        // POST api/adventures
        [HttpPost]
        [ProducesResponseType(typeof(ItemsViewModel<AdventureResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAdventureAsync([FromBody] AdventureCreateRequest adventureCreateRequest)
        {
            var newAdventure = _mapper.Map<Adventure>(adventureCreateRequest);
            var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == adventureCreateRequest.Category.Id);
            if (category == null)
            {
                return BadRequest(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), adventureCreateRequest.Category.Id)
                });
            }
            foreach (var adventureTag in newAdventure.AdventureTags)
            {
                var existingTag = await _dbContext.Tags.SingleOrDefaultAsync(x => x.Name
                    .Equals(adventureTag.Tag.Name, StringComparison.InvariantCultureIgnoreCase));

                if (existingTag != null)
                {
                    adventureTag.Tag = existingTag;
                }
            }
            newAdventure.Category = category;
            await _dbContext.Adventures.AddAsync(newAdventure);
            await _dbContext.SaveChangesAsync();
            var newAdventureViewModel = new[] { _mapper.Map<AdventureResponse>(newAdventure) };
            var response = new ItemsViewModel<AdventureResponse>(newAdventureViewModel.LongCount(), newAdventureViewModel);
            return CreatedAtAction(nameof(GetAdventureByIdAsync), new { id = newAdventure.Id }, response);
        }

        // POST api/adventures/bulk
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(ItemsViewModel<AdventureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkCreateAdventuresAsync([FromBody] params AdventureCreateRequest[] adventuresCreateRequest)
        {
            var errors = new StringBuilder();
            var adventuresToInsert = new List<Adventure>();
            foreach (var adventureCreateRequest in adventuresCreateRequest)
            {
                var newAdventure = _mapper.Map<Adventure>(adventureCreateRequest);
                var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == adventureCreateRequest.Category.Id);
                if (category == null)
                {
                    errors.AppendLine(string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), adventureCreateRequest.Category.Id));
                }
                else
                {
                    newAdventure.Category = category;
                    foreach (var adventureTag in newAdventure.AdventureTags)
                    {
                        var existingTag = await _dbContext.Tags.SingleOrDefaultAsync(x =>
                            x.Name.Equals(adventureTag.Tag.Name, StringComparison.InvariantCultureIgnoreCase));

                        if (existingTag != null)
                        {
                            adventureTag.Tag = existingTag;
                        }
                    }
                    adventuresToInsert.Add(newAdventure);
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
            await _dbContext.Adventures.AddRangeAsync(adventuresToInsert);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion

        #region READ
        // GET api/adventures[?pageSize=10&pageIndex=0]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<AdventureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdventuresAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var totalAdventures = await _dbContext.Adventures.LongCountAsync();
            if (totalAdventures == 0)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = _localizer.GetString("AdventuresNotFound")
                });
            }
            var adventuresOnPage = await _dbContext.Adventures
                .OrderBy(x => x.Title)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Include(x => x.Category)
                .Include(x => x.AdventureTags).ThenInclude(y => y.Tag)
                .ToListAsync();

            var adventureViewModelsOnPage = _mapper.Map<ICollection<AdventureResponse>>(adventuresOnPage);
            var response = new PaginatedItemsViewModel<AdventureResponse>(pageIndex, pageSize, totalAdventures, adventureViewModelsOnPage);

            return Ok(response);
        }

        // GET api/adventures/b6561e5e-a9a1-46f3-9160-023c65a66184
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemsViewModel<AdventureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdventureByIdAsync(string id)
        {
            var adventure = await _dbContext.Adventures
                                        .Include(x => x.Category)
                                        .Include(x => x.AdventureTags).ThenInclude(y => y.Tag)
                                        .SingleOrDefaultAsync(x => x.Id == id);
            if (adventure == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = _localizer.GetString("AdventuresNotFound")
                });
            }
            var adventureViewModel = _mapper.Map<AdventureResponse>(adventure);
            var response = new ItemsViewModel<AdventureResponse>(1, new[] { adventureViewModel });
            return Ok(response);
        }
        #endregion

        #region UPDATE
        // PUT api/adventures
        [HttpPut]
        [ProducesResponseType(typeof(ItemsViewModel<AdventureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAdventureByIdAsync([FromBody] AdventureUpdateRequest adventureUpdateRequest)
        {
            var adventure = await _dbContext.Adventures
                                    .Include(x => x.Category)
                                    .Include(x => x.AdventureTags).ThenInclude(y => y.Tag)
                                    .SingleOrDefaultAsync(x => x.Id == adventureUpdateRequest.Id);
            if (adventure == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = string.Format(_localizer["AdventureNotFoundIdParameter"], adventureUpdateRequest.Id)
                });
            }
            _mapper.Map<AdventureUpdateRequest, Adventure>(adventureUpdateRequest, adventure);
            var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == adventureUpdateRequest.Category.Id);
            if (category == null)
            {
                return BadRequest(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), adventureUpdateRequest.Category.Id)
                });
            }
            adventure.Category = category;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/adventures/bulk
        [HttpPut("bulk")]
        [ProducesResponseType(typeof(ItemsViewModel<AdventureResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkUpdateAdventuresByIdAsync([FromBody] params AdventureUpdateRequest[] adventuresUpdateRequest)
        {
            var errors = new StringBuilder();
            foreach (var adventureUpdateRequest in adventuresUpdateRequest)
            {
                var adventure = await _dbContext.Adventures
                                    .Include(x => x.Category)
                                    .Include(x => x.AdventureTags).ThenInclude(y => y.Tag)
                                    .SingleOrDefaultAsync(x => x.Id == adventureUpdateRequest.Id);
                if (adventure == null)
                {
                    errors.AppendLine(string.Format(_localizer.GetString("AdventureNotFoundIdParameter"), adventureUpdateRequest.Id));
                }
                else
                {
                    _mapper.Map<AdventureUpdateRequest, Adventure>(adventureUpdateRequest, adventure);
                    var category = await _dbContext.Categories.SingleOrDefaultAsync(x => x.Id == adventureUpdateRequest.Category.Id);
                    if (category == null)
                    {
                        errors.AppendLine(string.Format(_localizer.GetString("CategoryNotFoundIdParameter"), adventureUpdateRequest.Category.Id));
                    }
                    adventure.Category = category;
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
        // DELETE api/adventures/ecf40cb1-423f-466b-8d2b-788b2f87d09b
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdventureByIdAsync(string id)
        {
            var adventure = await _dbContext.Adventures
                                        .Include(x => x.AdventureTags)
                                        .SingleOrDefaultAsync(x => x.Id == id);
            if (adventure == null)
            {
                return NotFound(new ErrorDetail()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = string.Format(_localizer["AdventureNotFoundIdParameter"], id)
                });
            }
            _dbContext.Adventures.Remove(adventure);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/adventures/bulk
        [HttpDelete("bulk")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDetail), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkDeleteAdventuresByIdAsync([FromBody] params string[] ids)
        {
            var adventuresToDelete = new List<Adventure>();
            var errors = new StringBuilder();
            foreach (var id in ids)
            {
                var adventure = await _dbContext.Adventures
                                        .Include(x => x.AdventureTags)
                                        .SingleOrDefaultAsync(x => x.Id == id);
                if (adventure != null)
                {
                    adventuresToDelete.Add(adventure);
                }
                else
                {
                    errors.AppendLine(string.Format(_localizer.GetString("AdventureNotFoundIdParameter"), id));
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
            _dbContext.Adventures.RemoveRange(adventuresToDelete);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
        #endregion
    }
}
