using System;
using Microsoft.AspNetCore.Mvc;
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
                var talks = await _repository.GetTalksByMonikerAsync(moniker, true).ConfigureAwait(false);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerMessage(e.Message));
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
                return StatusCode(StatusCodes.Status500InternalServerError, new InternalServerMessage(e.Message));
            }
        }
    
        // POST: api/Talks
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker).ConfigureAwait(false);
                if (camp == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Camp does not exist");
                }

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

                if (model.Speaker == null)
                {
                    return BadRequest("Speaker Id is required");
                }
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId).ConfigureAwait(false);
                if (speaker == null) return BadRequest("Speaker not found");
                talk.Speaker = speaker;
                
                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext,
                        "GetTalk",
                        values: new {moniker, id = talk.TalkId});
                    
                    return StatusCode(StatusCodes.Status201Created, new {
                        url, talk = _mapper.Map<TalkModel>(talk)
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Camp does not exist");
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new InternalServerMessage(e.Message));
            }
        }
    }

    internal class InternalServerMessage
    {
        public string ErrorMessage { get; }
        public string HumanMessage { get; }

        public InternalServerMessage(string humanMessage)
        {
            ErrorMessage = "failed to get talk";
            HumanMessage = humanMessage;
        }
    }
}