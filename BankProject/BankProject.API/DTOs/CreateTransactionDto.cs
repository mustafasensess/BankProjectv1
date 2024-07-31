namespace BankProject.API.DTOs;

public class CreateTransactionDto
{
    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public decimal Amount { get; set; }
}