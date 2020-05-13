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

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker).ConfigureAwait(false);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }

        // GET: api/talks/{moniker}/talks/5
        [HttpGet("{id}")]
        /* leaving the variable as just {id} causes a 400 Bad Request error
         whereas {id:int} returns a 404 Not Found error*/
        public async Task<ActionResult<TalkModel>> GetTalk([FromRoute] int id, string moniker)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id).ConfigureAwait(false);
                if (talk != null)
                {
                    return _mapper.Map<TalkModel>(talk);
                }

                return StatusCode(StatusCodes.Status404NotFound, "No talk matching that Id");
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }
    //
    //     // POST: api/Talks
    //     [HttpPost]
    //     public async Task<IActionResult> PostOBJECT([FromBody] OBJECT o)
    //     {
    //     }
    }
}