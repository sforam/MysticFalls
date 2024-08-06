using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticFalls_VillaAPI.Data;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;
using MysticFalls_VillaAPI.Repository.IRepository;
using System.Collections.Generic;
using System.Net;



namespace MysticFalls_VillaAPI.Controllers
{

   /* [Route("api/[controller]")]*/
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
        protected APIResponse _response;
       
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;
        public VillaAPIController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new ();
                    
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task <ActionResult <APIResponse>> GetVillas() 
        {

            try
            {
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex) 
            {
                _response.IsSuccess  =false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() }; 
            }
            return _response;   
          
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public async Task<ActionResult<APIResponse>> GetVillas(int id)
        {
            try

            {

                if (id == 0)
                {
                    _response.StatusCode =HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
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
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;

        }

        [HttpPost]
       
        public async Task <ActionResult<APIResponse>> CreateVilla([FromBody]VillaCreateDTO craeteDTO)
        {
            try
            {


                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == craeteDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                if (craeteDTO == null)
                {
                    return BadRequest(craeteDTO);
                }
                /* if (villaDTO.Id > 0)
                 {
                     return StatusCode(StatusCodes.Status500InternalServerError);
                 }*/

                Villa villa = _mapper.Map<Villa>(craeteDTO);
                /* Villa model = new ()
                 {
                     Amenity = craeteDTO.Amenity,
                     Details = craeteDTO.Details,
                   *//*  Id = villaDTO.Id,*//*
                     ImageUrl = craeteDTO.ImageUrl,
                     Name = craeteDTO.Name,
                     Occupancy = craeteDTO.Occupancy,
                     Rate = craeteDTO.Rate,
                     Sqft = craeteDTO.Sqft
                 };*/



                await _dbVilla.CreateAsync(villa);
                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;


                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpDelete("{id:int}", Name = "DelelteVilla")]
        public  async Task<ActionResult<APIResponse>> DeleteVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                await _dbVilla.RemoveAsync(villa);

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

        [HttpPut("{id:int}",Name="UpdateVilla")]
        public async Task<ActionResult< APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    return BadRequest();
                }


                Villa model = _mapper.Map<Villa>(updateDTO);


                await _dbVilla.UpdateAsync(model);
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


        [HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id,tracked:false);


            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            
            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);



            await _dbVilla.UpdateAsync(model);
              
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent(); 
        }


    }
}
