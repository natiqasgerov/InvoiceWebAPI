using InvoiceApiFinal.DTOs.User;
using InvoiceApiFinal.Models;
using InvoiceApiFinal.Providers;
using InvoiceApiFinal.Services.Hash;
using InvoiceApiFinal.Services.TokenServices;
using InvoiceApiFinal.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.RegularExpressions;

namespace InvoiceApiFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        private IUserService _userService;

        private IUserProvider _userProvider;
        public UserController(IUserService userService, IJwtService jwtService, IUserProvider userProvider)
        {
            _userService = userService;
            _jwtService = jwtService;
            _userProvider = userProvider;
        }


        /// <summary>
        /// Registers a new user with the provided registration form data.
        /// </summary>
        /// <param name="userRegister">The user registration form data.</param>
        /// <returns>An HTTP status code indicating the success or failure of the registration process.</returns>

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterForm userRegister)
        {
            Log.Information("Register endpoint called");

            var checking = await _userService.CheckUser(userRegister);

            if (checking is not null)
            {
                Log.Warning("User registration failed due to duplicate user");
                return Conflict();
            }

            User user = new User()
            {
                Address = userRegister.Address,
                Email = userRegister.Email,
                Name = userRegister.Name,
                Password = PasswordService.Hashing(userRegister.Password),
                PhoneNumber = userRegister.PhoneNumber,
                CreatedAt = new DateTimeOffset(DateTime.Now),
                UpdatedAt = new DateTimeOffset(DateTime.Now)
            };

            user.Token = _jwtService.GenerateSecurityToken(user.Id, user.Email);

            if (await _userService.CreateUser(user))
            {
                Log.Information("User registration successful");
                return Ok();
            }
            else
            {
                Log.Error("User registration failed");
                return BadRequest();
            }
        }


        /// <summary>
        /// Authenticates a user and generates an access token.
        /// </summary>
        /// <param name="userLogin">User's login information</param>
        /// <returns>UserTokenDTO containing the access token and token expiration date</returns>

        [HttpPost("Login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] UserLoginForm userLogin)
        {
            Log.Information("Login endpoint called");

            var user = await _userService.GetUserByName(userLogin.Name);
            if (user is null)
            {
                Log.Warning("User not found");
                return NotFound();
            }
            
            
            var loginUser =  await _userService.GetUser(userLogin);
            if (loginUser != null)
            {
                await _userService.UpdateToken(loginUser, _jwtService.GenerateSecurityToken(loginUser.Id, loginUser.Email));
                Log.Information("User login successful");
                return new UserTokenDTO { AccessToken = loginUser.Token };
            }
            else
            {
                Log.Error("User login failed");
                return NotFound();
            }

        }


        /// <summary>
        /// Edits the current user's profile information.
        /// </summary>
        /// <param name="editRequest">An object containing the updated user information.</param>
        /// <returns>The updated user object.</returns>

        [Authorize]
        [HttpPut("EditProfile")]
        public async Task<ActionResult<UserDto>> EditProfile([FromBody] UserEditRequest editRequest)
        {
            var userCookie = _userProvider.GetUserInfo();
            if (userCookie is null)
            {
                Log.Warning("Unauthorized access - User cookie is null");
                return Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(editRequest.Name))
            {
                var find = await _userService.GetUserByName(editRequest.Name);
                if (find is not null)
                {
                    Log.Warning("Conflict - User with the same name already exists");
                    return Conflict();
                }
            }

            var findUser = await _userService.GetUserById(userCookie.Id);
    
            if (findUser != null)
            {
                var updated = await _userService.UpdateUser(editRequest, findUser);
                Log.Information("User profile updated successfully");
                return new UserDto
                {
                    Name = updated.Name,
                    Email = updated.Email,
                    Address = updated.Address,
                    PhoneNumber = updated.PhoneNumber,
                    CreatedAt = updated.CreatedAt,
                    UpdatedAt = updated.UpdatedAt,
                };
            }

            Log.Warning("User not found");
            return NotFound();
        }


        /// <summary>
        /// Changes the password of the currently logged in user.
        /// </summary>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>An ActionResult representing the outcome of the password change operation.</returns>

        [Authorize]
        [HttpPatch("{newPassword}/ChangePassword")]
        public async Task<ActionResult> ChangePassword(string newPassword)
        {
            if (!new Regex("^(?=.*[A-Za-z])(?=.*\\d)(?=.*[@$!%*#?&])[A-Za-z\\d@$!%*#?&]{8,}$").IsMatch(newPassword))
            {
                Log.Warning("Invalid password format");
                return ValidationProblem(title: "Password", detail: " 'Password' is not in the correct format.");
            }

            var userCookie = _userProvider.GetUserInfo();
            if (userCookie is null)
            {
                Log.Warning("Unauthorized access - User cookie is null");
                return Unauthorized();
            }

            var findUser = await _userService.GetUserById(userCookie.Id);
            if (findUser != null)
            {
                await _userService.UpdateOnlyPass(findUser, newPassword);
                Log.Information("User password changed successfully");
                return Ok();
            }

            Log.Warning("User not found");
            return NotFound();
        }


        /// <summary>
        /// Deletes the user profile and associated data.
        /// </summary>
        /// <returns>An action result representing the outcome of the operation.</returns>

        [Authorize]
        [HttpDelete("DeleteProfile")]
        public async Task<ActionResult> DeleteProfile()
        {
            var userCookie = _userProvider.GetUserInfo();
            if (userCookie is null)
            {
                Log.Warning("Unauthorized access - User cookie is null");
                return Unauthorized();
            }

            
            var findUser = await _userService.GetUserById(userCookie.Id);
            if (findUser != null)
            {
                await _userService.DeleteUser(findUser);
                Log.Information("User profile deleted successfully");
                return Ok();
            }

            Log.Warning("User not found");
            return NotFound();
        }


        /// <summary>
        /// Gets the authenticated user's information.
        /// </summary>
        /// <returns>The authenticated user's information.</returns>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="200">Returns the authenticated user's information.</response>
        
        [Authorize]
        [HttpGet("GetInfo")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            var userCookie = _userProvider.GetUserInfo();
            if (userCookie is null)
            {
                Log.Warning("Unauthorized access - User cookie is null");
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userCookie.Id);

            if (user is null)
            {
                Log.Warning("User not found");
                return NotFound();
            }

            Log.Information("User information retrieved successfully");
            return new UserDto
            {
                Address = user.Address,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
            
        }
    }
}
