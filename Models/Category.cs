public class Category
{
    public int Id { get; set; }
    public string Title { get; set; }
    public TransactionType Type { get; set; }
    public List<Transaction> Transactions { get; set; }
}