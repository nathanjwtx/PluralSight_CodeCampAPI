using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        // GET: api/Camps
        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCamps()
        {
            try
            {
                var results = await _repository.GetAllCampsAsync().ConfigureAwait(false);

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
        
        // // POST: api/CampsController
        // [HttpPost]
        // public async Task<IActionResult> PostOBJECT([FromBody] OBJECT o)
        // {
        // }
    }
}