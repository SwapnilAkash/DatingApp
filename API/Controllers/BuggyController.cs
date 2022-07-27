using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController:BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret(){
            return "secret text";
        }
        
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound(){
            var result = _context.Users.Find(-1);

            if(result == null)
                return NotFound();
            
            return Ok(result);

        }

        [HttpGet("server-error")]
        public ActionResult<string> GetSereverError(){
            var result = _context.Users.Find(-1);

            var response = result.ToString();

            return response;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest(){
            return BadRequest("Not a good Request!!");
        }
   }
}