using BankProject.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankProject.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionsController(ApplicationDbContext _dbContext)
    {
        this._dbContext = _dbContext;
    }
    [HttpPost]
    public async Task<IActionResult> InternalTransfer(CreateTransactionDto transactionDto)
    {
        if (transactionDto.Amount <= 0)
        {
            return BadRequest();
        }

        if (transactionDto.ReceiverId == transactionDto.SenderId)
        {
            return BadRequest();
        }

        var sender = await _dbContext.Accounts.FindAsync(transactionDto.SenderId);
        if (sender == null)
        {
            return BadRequest();
        }

        if (sender.Balance < transactionDto.Amount)
        {
            return BadRequest();
        }
        var receiver = await _dbContext.Accounts.FindAsync(transactionDto.ReceiverId);
        if (receiver == null)
        {
            return BadRequest();
        }

        receiver.Balance += transactionDto.Amount;
        sender.Balance -= transactionDto.Amount;

        var transaction = new Transaction
        {
            Amount = transactionDto.Amount,
            ReceiverId = transactionDto.ReceiverId,
            SenderId = transactionDto.SenderId
        };
        
        await _dbContext.Transactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        return Ok(transactionDto);
    }

    [HttpGet]
    [Route("{accountId}")]
    public async Task<IActionResult> GetByAccountId(int accountId)
    {
        var account = await _dbContext.Accounts.FindAsync(accountId);
        if (account == null)
        {
            return BadRequest();
        }

        var transactions = await _dbContext.Transactions
            .Where(t => t.SenderId == accountId || t.ReceiverId == accountId).AsNoTracking().ToListAsync();
        return Ok(transactions);
    }
}