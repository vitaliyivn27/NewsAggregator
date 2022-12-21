
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAggregator.Core.Abstractions;
using NewsAggregator.WebAPI.Models.Requests;
using NewsAggregator.WebAPI.Models.Responses;
using NewsAggregator.WebAPI.Utils;
using Serilog;

namespace NewsAggregator.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IJwtUtil _jwtUtil;

        public TokenController(IUserService userService,
            IRoleService roleService, IMapper mapper, IJwtUtil jwtUtil)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
        }

        /// <summary>
        /// Login user
        /// </summary>
        /// <param name="request">Create access token</param>
        /// <returns></returns>
        [Route("Login/CreateAccessToken")]
        [HttpPost]
        public async Task<IActionResult> CreateJwtToken([FromBody]LoginUserRequestModel request)
        {
            try
            {
                var user =  await _userService.GetUserByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(new ErrorModel()
                    {
                        Message = "Email is incorrect"
                    });
                }

                var isPassCorrect = await _userService.CheckUserPassword(request.Email, request.Password);

                if (!isPassCorrect)
                {
                    return BadRequest(new ErrorModel()
                    {
                        Message = "Password is incorrect"
                    });
                }

                var response = await _jwtUtil.GenerateTokenAsync(user);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Refresh access token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns></returns>
        [Route("RefreshAccessTokenByRefreshToken")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            try
            {
                var user = await _userService.GetUserByRefreshTokenAsync(request.RefreshToken);

                var response = await _jwtUtil.GenerateTokenAsync(user);

                await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);
                
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }

        /*/// <summary>
        /// Revoke refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestModel request)
        {
            try
            {
                await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);

                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }*/
    }
}