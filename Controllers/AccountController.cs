using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpEaseApi.Dtos.Account;
using HelpEaseApi.Dtos.Register;
using HelpEaseApi.Mappers;
using HelpEaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HelpEaseApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.Phone,
                };

                var createUser = await _userManager.CreateAsync(newUser, user.Password);

                if (!createUser.Succeeded)
                {
                    return BadRequest(createUser.Errors);
                }

                return CreatedAtAction("Register", newUser.ToReadAccount());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExists = await _userManager.FindByEmailAsync(user.Email);

                if (userExists == null)
                {
                    return BadRequest("User not found");
                }

                var userValid = await _userManager.CheckPasswordAsync(userExists, user.Password);

                if (!userValid)
                {
                    return BadRequest("Invalid password");
                }


                return Ok(userExists.ToReadAccount());
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}