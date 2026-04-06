using SQLite;

namespace SAMWELLPOS.MVVM.Models
{
    [Table("TransactionItems")]
    public class TransactionItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TransactionId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

        [Ignore]
        public string PriceDisplay => $"₱{Price:N2}";

        [Ignore]
        public string LineTotalDisplay => $"₱{LineTotal:N2}";
    }
}