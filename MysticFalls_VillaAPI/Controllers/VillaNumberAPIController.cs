using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;
using MysticFalls_VillaAPI.Repository.IRepository;
using System.Net;

namespace MysticFalls_VillaAPI.Controllers
{

    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;
        private readonly IVillaNumberRepository _dbVillaNumber;

        private readonly IVillaRepository _dbVilla; 
        private readonly IMapper _mapper;


        public VillaNumberAPIController(IVillaNumberRepository dbVillaNumber, IMapper mapper,IVillaRepository dbVilla)
        {
            _dbVillaNumber = dbVillaNumber;
            _mapper = mapper;
            this._response = new();
            _dbVilla = dbVilla;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<APIResponse>> GetVillasNumber()
        {
            try
            {
                IEnumerable<VillaNumber> VillaNumberList = await _dbVillaNumber.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(VillaNumberList);
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

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<APIResponse>> GetVillasNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
                if (villaNumber == null)
                {
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
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaNumberCreateDTO createDTO)
        {
            try
            {
                if(await _dbVillaNumber.GetAsync(u=>u.VillaNo == createDTO.VillaNo)!=null)
                {
                    ModelState.AddModelError("CustomError", "Villa Number already Exists!");
                    return BadRequest(_response);
                }

                if(await _dbVillaNumber.GetAsync(u=>u.VillaID == createDTO.VillaID)!=null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is Invalid!");
                    return BadRequest(_response);
                }

                if (createDTO == null)
                {
                    return BadRequest(_response);
                }
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);

                await _dbVillaNumber.CreateAsync(villaNumber);
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.Message };
            }
            return _response;

        }

        [HttpDelete("{id:int}",Name ="DeleteVillaNumber")]

        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if(id==0)
                {
                    return BadRequest();
                }
                var VillaNumber = await _dbVillaNumber.GetAsync(u=>u.VillaNo == id);
                if (VillaNumber == null)
                {
                    return NotFound();
                }
                await _dbVillaNumber.RemoveAsync(VillaNumber);

                _response.StatusCode=HttpStatusCode.OK;
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


        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.VillaNo)
                {
                    return BadRequest();
                }
                if (await _dbVillaNumber.GetAsync(u => u.VillaID == updateDTO.VillaID) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa Id is Invalid!");
                    return BadRequest(_response);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(updateDTO);


                await _dbVillaNumber.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVillaNumber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id, tracked: false);


            VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);


            if (villaNumber == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaNumberDTO, ModelState);
            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDTO);



            await _dbVillaNumber.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }



    }


}
