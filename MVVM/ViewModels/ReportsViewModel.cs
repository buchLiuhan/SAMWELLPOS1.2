using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SAMWELLPOS.MVVM.Models;
using SAMWELLPOS.Services;
using System.Collections.ObjectModel;

namespace SAMWELLPOS.MVVM.ViewModels
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        // Today
        [ObservableProperty]
        private string _todayRevenue = "₱0.00";

        [ObservableProperty]
        private string _todayTransactionCount = "0";

        [ObservableProperty]
        private string _todayItemsSold = "0";

        // This Month
        [ObservableProperty]
        private string _monthRevenue = "₱0.00";

        [ObservableProperty]
        private string _monthTransactionCount = "0";

        [ObservableProperty]
        private string _monthItemsSold = "0";

        // Top Products
        [ObservableProperty]
        private ObservableCollection<TopProductItem> _topProducts = new();

        // Recent Transactions
        [ObservableProperty]
        private ObservableCollection<TransactionModel> _recentTransactions = new();

        [RelayCommand]
        private async Task GoToHistory()
        {
            await Shell.Current.GoToAsync("AdminHistory");
        }

        public ReportsViewModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task LoadReportsAsync()
        {
            var allTransactions = await _db.GetTransactions();
            var today = DateTime.Today;
            var firstOfMonth = new DateTime(today.Year, today.Month, 1);

            // Today's transactions
            var todayTxns = allTransactions
                .Where(t => t.TransactionDate.Date == today)
                .ToList();

            TodayRevenue = $"₱{todayTxns.Sum(t => t.TotalAmount):N2}";
            TodayTransactionCount = todayTxns.Count.ToString();

            // Month's transactions
            var monthTxns = allTransactions
                .Where(t => t.TransactionDate >= firstOfMonth)
                .ToList();

            MonthRevenue = $"₱{monthTxns.Sum(t => t.TotalAmount):N2}";
            MonthTransactionCount = monthTxns.Count.ToString();

            // Items sold today and this month
            int todayItems = 0;
            int monthItems = 0;

            // Top products tracker
            var productSales = new Dictionary<string, int>();

            foreach (var txn in allTransactions)
            {
                var items = await _db.GetTransactionItems(txn.Id);

                foreach (var item in items)
                {
                    // Track top products across all time
                    if (!productSales.ContainsKey(item.ProductName))
                        productSales[item.ProductName] = 0;
                    productSales[item.ProductName] += item.Quantity;

                    // Today items
                    if (txn.TransactionDate.Date == today)
                        todayItems += item.Quantity;

                    // Month items
                    if (txn.TransactionDate >= firstOfMonth)
                        monthItems += item.Quantity;
                }
            }

            TodayItemsSold = todayItems.ToString();
            MonthItemsSold = monthItems.ToString();

            // Top 3 products
            var top3 = productSales
                .OrderByDescending(p => p.Value)
                .Take(3)
                .Select((p, index) => new TopProductItem
                {
                    Rank = index + 1,
                    ProductName = p.Key,
                    TotalSold = p.Value
                })
                .ToList();

            TopProducts = new ObservableCollection<TopProductItem>(top3);

            // Recent 5 transactions
            RecentTransactions = new ObservableCollection<TransactionModel>(
                allTransactions.Take(5));
        }
    }

    public class TopProductItem
    {
        public int Rank { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public string RankDisplay => Rank switch { 1 => "🥇", 2 => "🥈", 3 => "🥉", _ => $"#{Rank}" };
        public string SoldDisplay => $"{TotalSold} sold";
    }


}