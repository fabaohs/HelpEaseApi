using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpEaseApi.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HelpEaseApi.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {

        private readonly ILogger<TaskController> _logger;
        private readonly ApplicationDbContext _context;

        public TaskController(ILogger<TaskController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _context.Tasks.ToListAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all tasks");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}