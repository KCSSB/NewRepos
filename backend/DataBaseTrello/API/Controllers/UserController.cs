using DataBaseInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace API.Controllers
{
    using DataBaseInfo.models;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        

        public UserController(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        [HttpGet("user/{Id}")]
        async public Task<IActionResult> GetUserById([FromRoute]int Id)
        {
            if (Id < 0)
            {
                return BadRequest("Invalid Id");
            }
            using (var context = _contextFactory.CreateDbContext())
            {

            var user = await context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetUserById), new { Id = user.Id }, user);
            }

        }
        [HttpPost("create")]
        async public Task<IActionResult> CreateUser(User nUser)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                
                var user = new User();
                user.UserName = nUser.UserName;
                user.UserPassword = nUser.UserPassword;
                user.ProjectUsers = nUser.ProjectUsers;

                context.Users.Add(user);
                await context.SaveChangesAsync();


                return CreatedAtAction(nameof(GetUserById), new { Id = user.Id }, user);
            }
        }
        
    }
}
