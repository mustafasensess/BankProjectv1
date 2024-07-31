namespace BankProject.API.DTOs;

public class AccountDto
{
    public int Id { get; set; }
    
    public decimal Balance { get; set; }

    public int UserId { get; set; }
}