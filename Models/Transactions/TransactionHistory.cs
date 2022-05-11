namespace WebApi.Models.Transactions
{
    public class TransactionEntry
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
