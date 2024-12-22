using Microsoft.AspNetCore.Mvc;
using V1.DTOs;
using V1.Services;

namespace V1.Controllers
{
    /// <summary>
    /// Handles user-related operations.
    /// </summary>
    /// <param name="userService">The service to handle user operations.</param>
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">The data required for creating a new user, including username and password.</param>
        /// <returns>A response indicating the success or failure of the user registration.</returns>
        /// <response code="201">Successfully created the user.</response>
        /// <response code="400">Bad request, such as validation errors or missing data.</response>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]CreateUserDto request)
        {
            try
            {
                await _userService.RegisterUser(request);
                return CreatedAtAction(nameof(Register), null);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}