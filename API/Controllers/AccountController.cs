using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email,
                Member = new Member
                {
                    DisplayName = registerDto.DisplayName,
                    Gender = registerDto.Gender,
                    City = registerDto.City,
                    Country = registerDto.Country,
                    DateOfBirth = registerDto.DateOfBirth
                },
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Identity", error.Description);
                }

                return ValidationProblem();
            }

            await userManager.AddToRoleAsync(user, "Member");

            await SetRefreshTokenCookie(user);

            return await user.ToDto(tokenService);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid email address");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result) return Unauthorized("Invalid password");

            await SetRefreshTokenCookie(user);

            return await user.ToDto(tokenService);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null) return NoContent();

            var user = await userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.RefreshTokenExpiry > DateTime.UtcNow);

            if (user == null) return Unauthorized();

            await SetRefreshTokenCookie(user);

            return await user.ToDto(tokenService);
        }

        private async Task SetRefreshTokenCookie(AppUser user)
        {
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
