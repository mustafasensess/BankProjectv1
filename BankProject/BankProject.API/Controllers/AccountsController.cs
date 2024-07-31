using AutoMapper;
using BankProject.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankProject.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;

    public AccountsController(ApplicationDbContext context, IMapper mapper,UserManager<User> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("{userId}")]
    public async Task<IActionResult> GetAll(string userId)
    {
        var user = await _userManager.FindByNameAsync(userId.ToString());
        if (user == null)
        {
            return NotFound();
        }

        var accounts = _context.Accounts.Where(a => a.UserId == userId).AsNoTracking().ToList();

        var accountsDtoList = _mapper.Map<List<AccountDto>>(accounts);
        
        return Ok(accountsDtoList);
    }

    [HttpGet]
    [Route("{id}/balance")]
    [Authorize]
    public async Task<IActionResult> GetBalanceById(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        return Ok(account.Balance);
    }
    
}