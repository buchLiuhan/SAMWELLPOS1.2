using SQLite;

namespace SAMWELLPOS.MVVM.Models
{
    [Table("Products")]
    public class ProductModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }

        // --- [Ignore] computed properties ---

        [Ignore]
        public bool HasImage => !string.IsNullOrEmpty(ImagePath);

        [Ignore]
        public string Initials
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name)) return "?";
                var parts = Name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return parts.Length >= 2
                    ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                    : parts[0][0].ToString().ToUpper();
            }
        }

        [Ignore]
        public string PriceDisplay => $"₱{Price:N2}";

        [Ignore]
        public string StockDisplay => $"Stock: {Quantity}";

        [Ignore]
        public Color StockColor => Quantity <= 5
            ? Color.FromArgb("#EF5350")   // ErrorColor — low stock
            : Color.FromArgb("#66BB6A");  // SuccessColor — healthy stock

        [Ignore]
        public int CartQuantity { get; set; } = 0;

        [Ignore]
        public string CartQuantityDisplay => CartQuantity.ToString();
    }
}