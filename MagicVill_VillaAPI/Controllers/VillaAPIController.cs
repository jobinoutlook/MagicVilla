using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")] test
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ILogging _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db, ILogging logger, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            this._response = new APIResponse();
        }

        
        [HttpGet]
        
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {

                IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
                _logger.Log("Getting all villas", LogType.INFO);

                // return Ok(await _db.Villas.ToListAsync());
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
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


        
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Villa Error with Id " + id, LogType.ERROR);
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

                if (villa == null)
                {
                    _logger.Log("Villa not found", LogType.WARNING);
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaDTO>(villa);
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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> Create([FromBody] VillaCreateDTO villaCreateDTO)
        {
            try
            {
                if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already exists!");
                    return BadRequest(ModelState);
                }

                if (villaCreateDTO == null) return BadRequest(villaCreateDTO);

                //if (villaDTO.Id > 0)
                //{
                //    return StatusCode(StatusCodes.Status500InternalServerError);
                //}

                Villa villa = _mapper.Map<Villa>(villaCreateDTO);
                villa.CreatedDate = DateTime.Now;

                //Villa model = new()
                //{
                //    Amenity = villaDTO.Amenity,
                //    Details = villaDTO.Details,
                //    ImageUrl = villaDTO.ImageUrl,
                //    Name = villaDTO.Name,
                //    Occupancy = villaDTO.Occupancy,
                //    Rate = villaDTO.Rate,
                //    Sqft = villaDTO.Sqft,
                //    CreatedDate = DateTime.Now
                //};


                await _db.Villas.AddAsync(villa);
                await _db.SaveChangesAsync();

                _response.Result = _mapper.Map<VillaDTO>(villa);

                //return Ok(villaDTO);

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;

        }




        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

                if (villa == null)
                {
                    return NotFound();
                }

                _db.Villas.Remove(villa);
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




        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaDTO)
        {
            try
            {
                if (villaDTO == null || villaDTO.Id != id)
                {
                    return BadRequest();
                }

                Villa model = _mapper.Map<Villa>(villaDTO);
                model.UpdatedDate = DateTime.Now;

                //Villa model = new()
                //{
                //    Id=villaDTO.Id,
                //    Amenity = villaDTO.Amenity,
                //    Details = villaDTO.Details,
                //    ImageUrl = villaDTO.ImageUrl,
                //    Name = villaDTO.Name,
                //    Occupancy = villaDTO.Occupancy,
                //    Rate = villaDTO.Rate,
                //    Sqft = villaDTO.Sqft,
                //    UpdatedDate=DateTime.Now
                //};

                _db.Villas.Update(model);
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



        [Authorize(Roles = "admin")]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            try
            {
                if (id == 0 || patchDTO == null) return BadRequest();

                var villadb = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

                if (villadb == null) return BadRequest();

                VillaDTO villaDTO = _mapper.Map<VillaDTO>(villadb);

                //VillaDTO villaDTO = new()
                //{
                //    Id = villa.Id,
                //    Amenity = villa.Amenity,
                //    Details = villa.Details,
                //    ImageUrl = villa.ImageUrl,
                //    Name = villa.Name,
                //    Occupancy = villa.Occupancy,
                //    Rate = villa.Rate,
                //    Sqft = villa.Sqft
                //};

                if (villadb == null) return NotFound();

                patchDTO.ApplyTo(villaDTO, ModelState);

                Villa villaMapped = _mapper.Map<Villa>(villaDTO);
                villaMapped.UpdatedDate = DateTime.Now;

                //Villa model = new()
                //{
                //    Id = villaDTO.Id,
                //    Amenity = villaDTO.Amenity,
                //    Details = villaDTO.Details,
                //    ImageUrl = villaDTO.ImageUrl,
                //    Name = villaDTO.Name,
                //    Occupancy = villaDTO.Occupancy,
                //    Rate = villaDTO.Rate,
                //    Sqft = villaDTO.Sqft,
                //    UpdatedDate = DateTime.Now
                //};

                _db.Villas.Update(villaMapped);
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
