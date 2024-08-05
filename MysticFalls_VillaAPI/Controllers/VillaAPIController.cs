﻿using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MysticFalls_VillaAPI.Data;
using MysticFalls_VillaAPI.Models;
using MysticFalls_VillaAPI.Models.Dto;



namespace MysticFalls_VillaAPI.Controllers
{

   /* [Route("api/[controller]")]*/
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
       
        private readonly ApplicationDbContext _db;
        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
                    
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public  ActionResult <IEnumerable<VillaDTO>> GetVillas() 
        {
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

/*        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
*/
        public ActionResult<VillaDTO> GetVillas(int id)
        {
            if (id == 0)
            {
                return BadRequest();      
            }
           var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if (villa == null) 
            {
                return NotFound();
            }


            return Ok(villa);

        }

        [HttpPost]
       
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {
            /* if (!ModelState.IsValid) 
             {
                 return BadRequest(ModelState);
             }*/

            if (_db.Villas.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError","Villa already Exists!");
                return BadRequest(ModelState);
            }
            if(villaDTO == null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Villa model = new ()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };

            _db.Villas.Add(model);
            _db.SaveChanges();


            return CreatedAtRoute("GetVilla",new {id=villaDTO.Id},villaDTO);
        }

        [HttpDelete("{id:int}", Name = "DelelteVilla")]
        public IActionResult DeleteVilla(int id) 
        {
            if(id == 0) 
            {
                return BadRequest();
            }
            var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
            if(villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            _db.SaveChanges();
            return Ok();
        }

        [HttpPut("")]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            /* var villa = _db.Villas.FirstOrDefault(u => u.Id == id);
             villa.Name = villaDTO.Name;
             villa.Sqft = villaDTO.Sqft;
             villa.Occupancy = villaDTO.Occupancy;*/
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
                
            _db.Villas.Update(model);
            _db.SaveChanges();

            return Ok();
        }


        [HttpPatch("{id:int}",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = _db.Villas.AsNoTracking().FirstOrDefault(u=> u.Id == id);

           

            VillaDTO villaDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft
            };

            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villaDTO, ModelState);
            Villa model = new()
            {
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Id = villaDTO.Id,
                ImageUrl = villaDTO.ImageUrl,
                Name = villaDTO.Name,
                Occupancy = villaDTO.Occupancy,
                Rate = villaDTO.Rate,
                Sqft = villaDTO.Sqft
            };
              _db.Villas.Update(model);
               _db.SaveChanges();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent(); 
        }


    }
}
