using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticFalls_VillaAPI.Data;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;
using System.Collections.Generic;



namespace MysticFalls_VillaAPI.Controllers
{

   /* [Route("api/[controller]")]*/
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
       
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaAPIController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
                    
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task <ActionResult <IEnumerable<VillaDTO>>> GetVillas() 
        {

            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

/*        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
*/
        public async Task<ActionResult<VillaDTO>> GetVillas(int id)
        {
            if (id == 0)
            {
                return BadRequest();      
            }
           var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null) 
            {
                return NotFound();
            }


            return Ok(_mapper.Map<VillaDTO>(villa));

        }

        [HttpPost]
       
        public async Task <ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO craeteDTO)
        {
            /* if (!ModelState.IsValid) 
             {
                 return BadRequest(ModelState);
             }*/

            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == craeteDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError","Villa already Exists!");
                return BadRequest(ModelState);
            }
            if(craeteDTO == null)
            {
                return BadRequest(craeteDTO);
            }
            /* if (villaDTO.Id > 0)
             {
                 return StatusCode(StatusCodes.Status500InternalServerError);
             }*/

            Villa model = _mapper.Map<Villa>(craeteDTO);
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

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();


            return CreatedAtRoute("GetVilla",new {id=model.Id},model);
        }

        [HttpDelete("{id:int}", Name = "DelelteVilla")]
        public  async Task<IActionResult> DeleteVilla(int id) 
        {
            if(id == 0) 
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}",Name="UpdateVilla")]
        public async Task< IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            

            Villa model = _mapper.Map<Villa>(updateDTO);
         

            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return Ok();
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
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);


            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            
            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = _mapper.Map<Villa>(villaDTO);
           


              _db.Villas.Update(model);
               await _db.SaveChangesAsync();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent(); 
        }


    }
}
