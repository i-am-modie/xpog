using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Xpog.Services;
using Xpog.Models;
using System.Linq;

namespace Xpog.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginInputModel model)
        {
            try
            {
                var user = _userService.Authenticate(model.Username, model.Password);

                return Ok(user);
            }
            catch (System.Exception err)
            {
                return BadRequest(new { message = err.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]LoginInputModel model)
        {
            try
            {
                var response = _userService.Register(model.Username, model.Password);

                return Ok(response.Result);
            }
            catch(System.Exception err)
            {
                return BadRequest(new { message = err.Message } );
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users.Result);
        }
    }
}