﻿using DreamyShop.Api.Authorization;
using DreamyShop.Domain.Shared.Dtos;
using DreamyShop.Logic.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizeAttribute = DreamyShop.Api.Authorization.AuthorizeAttribute;

namespace DreamyShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthLogic _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IAuthLogic authenService, 
            ILogger<AuthController> logger)
        {
            _logger = logger;
            _authService = authenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto userRegisterDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.Register(userRegisterDto);
            if (result.Result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto userLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.Login(userLoginDto);
            if (result.Result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        [Member]
        public async Task<IActionResult> ChangePassword([FromForm] UserChangePassword userLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = (AuthEntity)HttpContext.Items["Auth"];
            if(user == null)
            {
                return NotFound(user);
            }
            var result = await _authService.ChangePassword(user.Email, userLoginDto);
            if (result.Result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
