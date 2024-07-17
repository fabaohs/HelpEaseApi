using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpEaseApi.Contexts;
using HelpEaseApi.Dtos.Account;
using HelpEaseApi.Dtos.Register;
using HelpEaseApi.Mappers;
using HelpEaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpEaseApi.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<AppUser> userManager, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register([FromBody] RegisterDto user)
        {
            var response = new Response<ReadAccountDto>();

            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Message = "Invalid data" + ModelState.Values;
                    return BadRequest(response);
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
                    response.Success = false;
                    response.Message = "Error creating user" + createUser.Errors;
                    return BadRequest(response);
                }
                response.Success = true;
                response.Message = "User created successfully";
                response.Data = newUser.ToReadAccount();

                return CreatedAtAction("Register", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating user" + ex.Message);
                response.Success = false;
                response.Message = "Internal server error";
                return BadRequest(response);
            }
        }

        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] LoginDto user)
        {
            var response = new Response<ReadAccountDto>();

            try
            {
                if (!ModelState.IsValid)
                {
                    response.Success = false;
                    response.Message = "Invalid data" + ModelState.Values;
                    return BadRequest(response);
                }

                var userExists = await _userManager.FindByEmailAsync(user.Email);

                if (userExists == null)
                {
                    response.Success = false;
                    response.Message = "User not found";
                    return BadRequest(response);
                }

                var userValid = await _userManager.CheckPasswordAsync(userExists, user.Password);

                if (!userValid)
                {
                    response.Success = false;
                    response.Message = "Invalid password";
                    return BadRequest(response);
                }

                response.Success = true;
                response.Data = userExists.ToReadAccount();
                response.Message = "User authenticated successfully";
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError("Error authenticating user" + e.Message);
                response.Success = false;
                response.Message = "Internal server error";
                return StatusCode(500, response);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            // var response = new Response<List<ReadAccountDto>>();
            var response = new Response<List<AppUser>>();

            try
            {
                var users = await _context.Users.ToListAsync();
                response.Success = true;
                response.Message = "Users retrieved successfully";
                response.Data = users;
                // response.Data = users.Select(u => u.ToReadAccount()).ToList();
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting all users" + e.Message);
                response.Success = false;
                response.Message = "Internal server error";
                return StatusCode(500, response);
            }
        }

    }
}