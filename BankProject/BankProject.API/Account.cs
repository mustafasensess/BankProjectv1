using System.Text.Json.Serialization;

namespace BankProject.API;

public class Account
{
    public int Id { get; set; }
    
    public decimal Balance { get; set; }

    public string UserId { get; set; }

    //[JsonIgnore]
    public User User { get; set; }
    
    
}