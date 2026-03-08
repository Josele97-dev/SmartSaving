using System.Reflection;

public enum TransactionType
{
    Income,
    Expense
}

public class Transaction
{
    public int Id { get; set; }
    public TransactionType Type { get; set; }
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }

}