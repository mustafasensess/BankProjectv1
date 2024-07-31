using Microsoft.AspNetCore.Identity;

namespace BankProject.API;

public class User : IdentityUser
{
    public int AccountNo { get; set; }
}