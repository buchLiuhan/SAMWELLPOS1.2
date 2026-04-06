using SQLite;

namespace SAMWELLPOS.MVVM.Models
{
    [Table("Transactions")]
    public class TransactionModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string TransactionNumber { get; set; } = string.Empty;
        public string CashierFullName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal CashTendered { get; set; }
        public decimal ChangeAmount { get; set; }
        public DateTime TransactionDate { get; set; }

        [Ignore]
        public string TotalDisplay => $"₱{TotalAmount:N2}";

        [Ignore]
        public string DateDisplay => TransactionDate.ToString("MMM dd, yyyy hh:mm tt");
    }
}