namespace WebApi.Entities
{
    public enum TransactionStatus
    {
        Pending,
        Completed,
        Failed
    }

    public abstract class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Created { get; set; }
        public TransactionStatus Status { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }

    public class InternalTransaction : Transaction
    {
        public int FromUserId { get; set; }
        public virtual User FromUser { get; set; }
    }

    public class ManualTransaction : Transaction
    {
    }

}
