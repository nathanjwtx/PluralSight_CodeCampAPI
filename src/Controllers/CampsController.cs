using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _repository;

        public CampsController(ICampRepository repository)
        {
            _repository = repository;
        }
        
        // GET: api/CampsController
        [HttpGet]
        public async Task<IActionResult> GetCamps()
        {
            try
            {
                var results = await _repository.GetAllCampsAsync();

                return Ok(results);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Database error: {e}");
            }
            
        }

        // // GET: api/CampsController/5
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetOBJECT([FromRoute] int id)
        // {
        // }
        //
        // // POST: api/CampsController
        // [HttpPost]
        // public async Task<IActionResult> PostOBJECT([FromBody] OBJECT o)
        // {
        // }
    }
}