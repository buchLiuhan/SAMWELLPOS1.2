using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMWELLPOS.MVVM.Models
{
    public class CartItem
    {
        public ProductModel Product { get; set; } = null!;
        public int Quantity { get; set; } = 1;
        public decimal LineTotal => Product.Price * Quantity;
    }
}
