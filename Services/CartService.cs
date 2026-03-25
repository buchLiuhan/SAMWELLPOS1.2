using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMWELLPOS.MVVM.Models;

namespace SAMWELLPOS.Services
{
    public class CartService
    {
        private readonly List<CartItem> _items = new();

        public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

        public int TotalItemCount => _items.Sum(i => i.Quantity);

        public decimal TotalAmount => _items.Sum(i => i.LineTotal);

        public event Action? CartChanged;

        public void AddOrIncrement(ProductModel product)
        {
            var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing is not null)
            {
                existing.Quantity++;
            }
            else
            {
                _items.Add(new CartItem { Product = product, Quantity = 1 });
            }
            CartChanged?.Invoke();
        }

        public void Decrement(ProductModel product)
        {
            var existing = _items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing is null) return;

            existing.Quantity--;
            if (existing.Quantity <= 0)
                _items.Remove(existing);

            CartChanged?.Invoke();
        }

        public int GetQuantity(ProductModel product)
        {
            return _items.FirstOrDefault(i => i.Product.Id == product.Id)?.Quantity ?? 0;
        }

        public void Clear()
        {
            _items.Clear();
            CartChanged?.Invoke();
        }
    }
}