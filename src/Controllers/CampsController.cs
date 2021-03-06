using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;

namespace CoreCodeCamp.Controllers
{
    // for including version in the URL
    // [Route("api/v{version:apiVersion}/[controller]")]
    
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        
        // GET: api/Camps
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks).ConfigureAwait(false);

                // CampModel[] models = _mapper.Map<CampModel[]>(results);
                // return StatusCode(StatusCodes.Status200OK, models);
                
                // by changing to ActionResult and adding the return type, the above code can be simplified to
                // additionally it will generate automatically the return Ok status code

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
            
        }

        // // GET: api/Camps/moniker
        [HttpGet("{moniker}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<CampModel>> GetCamp(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker).ConfigureAwait(false);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
        }
        
        // // GET: api/Camps/moniker
        [HttpGet("{moniker}")]
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel>> GetCamp11(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker, true).ConfigureAwait(false);

                if (result == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);

                if (!results.Any()) return StatusCode(StatusCodes.Status404NotFound);

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
        }
        
        // POST: api/Camps
        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var existingCamp = await _repository.GetCampAsync(model.Moniker).ConfigureAwait(false);
                if (existingCamp != null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"Camp Moniker {model.Moniker} is already in use");
                }
                
                // the values in the anonymous object are the route parameters
                var location = _linkGenerator.GetPathByAction("GetCamp", "Camps",
                    new {moniker = model.Moniker});

                if (string.IsNullOrWhiteSpace(location))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Could not use current moniker");
                }

                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);

                if (await _repository.SaveChangesAsync())
                {
                    return StatusCode(StatusCodes.Status201Created, new
                    {
                        location = location,
                        newCamp = _mapper.Map<CampModel>(camp)
                    });
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }

            return StatusCode(StatusCodes.Status400BadRequest);
        }
        
        // PUT: api/camps
        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(model.Moniker).ConfigureAwait(false);
                if (oldCamp == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                        $"Camp with moniker {model.Moniker} could not be found");
                }

                _mapper.Map(model, oldCamp);

                if (await _repository.SaveChangesAsync().ConfigureAwait(false))
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
            
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        
        // DELETE: api/camps
        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker).ConfigureAwait(false);
                if (oldCamp == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                
                _repository.Delete(oldCamp);

                if (await _repository.SaveChangesAsync().ConfigureAwait(false))
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
            
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}