using System;
using System.Collections.Generic;
using System.Linq;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public decimal InitialBalance { get; set; }

    public List<Transaction> Transactions { get; set; } = new List<Transaction>();

}
