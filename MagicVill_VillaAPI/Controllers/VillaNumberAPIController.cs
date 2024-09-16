using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
   
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogging _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaNumberAPIController(ApplicationDbContext db, ILogging logger,IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            this._response = new APIResponse();
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {

                IEnumerable<VillaNumber> villaNumberList = await _db.VillaNumbers.Include(u => u.Villa).ToListAsync();
                _logger.Log("Getting all villas", LogType.INFO);

                // return Ok(await _db.Villas.ToListAsync());
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
                _response.StatusCode = HttpStatusCode.OK;


                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
            

        }



        [HttpGet("{villaNo:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    _logger.Log("Get Villa Error with Id " + villaNo, LogType.ERROR);
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villaNumber = await _db.VillaNumbers.FirstOrDefaultAsync(x => x.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    _logger.Log("Villa not found", LogType.WARNING);
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }





        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null) return BadRequest(createDTO);

                if (createDTO.VillaNo < 1)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number must be greater than 0");
                    return BadRequest(ModelState);
                }

                if (await _db.VillaNumbers.FirstOrDefaultAsync(u => u.VillaNo == createDTO.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already exists!");
                    return BadRequest(ModelState);
                }

                if (await _db.Villas.FirstOrDefaultAsync(u => u.Id == createDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
                    return BadRequest(ModelState);
                }

                

               

                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
                villaNumber.CreatedDate = DateTime.Now;

                


                await _db.VillaNumbers.AddAsync(villaNumber);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);

                
                return CreatedAtRoute("GetVillaNumber", new { villaNo = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;

        }




        [HttpDelete("{villaNo:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int villaNo)
        {
            try
            {
                if (villaNo == 0)
                {
                    return BadRequest();
                }

                var villaNumber = await _db.VillaNumbers.FirstOrDefaultAsync(x => x.VillaNo == villaNo);

                if (villaNumber == null)
                {
                    return NotFound();
                }

                _db.VillaNumbers.Remove(villaNumber);
                await _db.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;


        }





        [HttpPut("{villaNo:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.VillaNo != villaNo)
                {
                    return BadRequest();
                }

                if (await _db.Villas.FirstOrDefaultAsync(u => u.Id == updateDTO.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is invalid!");
                    return BadRequest(ModelState);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);
                model.UpdatedDate = DateTime.Now;

               

                _db.VillaNumbers.Update(model);
                await _db.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;
        }




        [HttpPatch("{villaNo:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVillaNumber(int villaNo, JsonPatchDocument<VillaNumberDTO> patchDTO)
        {
            try
            {
                if (villaNo == 0 || patchDTO == null) return BadRequest();

                var villaNumberdb = await _db.VillaNumbers.AsNoTracking().FirstOrDefaultAsync(u => u.VillaNo == villaNo);

                if (villaNumberdb == null) return BadRequest();

                VillaNumberDTO villaNumberDTO = _mapper.Map<VillaNumberDTO>(villaNumberdb);

                

                if (villaNumberdb == null) return NotFound();

                patchDTO.ApplyTo(villaNumberDTO, ModelState);

                VillaNumber villaNumberMapped = _mapper.Map<VillaNumber>(villaNumberDTO);
                villaNumberMapped.UpdatedDate = DateTime.Now;
                               

                _db.VillaNumbers.Update(villaNumberMapped);
                await _db.SaveChangesAsync();

                if (!ModelState.IsValid) return BadRequest(ModelState);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;

        }

    }


    
}
