using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace BankProject.API.Controllers;

public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _dbContext;

    public AuthController(IConfiguration config, UserManager<User> userManager, IMapper mapper,ApplicationDbContext dbContext)
    {
        _config = config;
        _userManager = userManager;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        var user = await _userManager.FindByNameAsync(loginRequestDto.Username);

        if (user == null)
            return BadRequest("Username or password incorrect!");
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

        if (!checkPasswordResult)
            return BadRequest("Username or password incorrect!");

        var jwtToken = CreateJwtToken(user);

        return Ok(jwtToken);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IdentityResult> CreateUserAsync(CreateUserRequestDto requestDto)
    {
        var user = _mapper.Map<User>(requestDto);
        user.UserName = requestDto.AccountNo.ToString();
        var result = await _userManager.CreateAsync(user, requestDto.Password);
        if (!result.Succeeded)
            return result;
        var registeredUser = await _userManager.FindByNameAsync(user.UserName);
        var account = new Account
        {
            Balance = requestDto.Balance,
            UserId = registeredUser.Id
        };
        await _dbContext.Accounts.AddAsync(account);
        await _dbContext.SaveChangesAsync();
        return result;
    }

    public string CreateJwtToken(IdentityUser user)
    {
        //Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}