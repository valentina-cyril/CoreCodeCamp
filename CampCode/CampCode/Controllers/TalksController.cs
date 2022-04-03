using AutoMapper;
using CampCode.Data;
using CampCode.Data.Entities;
using CampCode.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampCode.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker, true);
                return _mapper.Map <TalkModel[]>(talks);

            }
            catch 
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "failed to get talks");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {

                var talk = await _repository.GetTalkByMonikerAsync(moniker, id,true);
                if (talk == null) return BadRequest("Talk not found");
                return _mapper.Map<TalkModel>(talk);
            }
            catch 
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }
        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("camp not found");

                Talk talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

               if (model.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
               if (speaker == null) return BadRequest("Speaker ID could not found");
                talk.Speaker = speaker;

                _repository.Add(talk);

                if (await _repository.SaveChangesAsync())
                {
                    return Created("", _mapper.Map<TalkModel>(model));
                }
                else
                {
                    return BadRequest("Failed to create Talk for this camp");
                }

           }
            catch 
            {
               

                  return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
           

        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return BadRequest("Couldnt find talk");
                _mapper.Map(model, talk);

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        talk.Speaker = speaker;
                    }
                    
                }

                    if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(model);
                }
                else
                {
                    return BadRequest("Could not update talk");
                }



            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return BadRequest("Could not find talk to delete");
                _repository.Delete(talk);

                if(await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("Could not delete talk");
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to get talk");
            }
        }
    }
}
