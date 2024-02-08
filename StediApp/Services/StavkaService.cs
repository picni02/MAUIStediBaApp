using MauiBudgetApp.Services;
using StediApp.Data;
using StediApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StediApp.Services
{
    public class StavkaService : IStavkaService
    {
        List<Stavka> Stavkas = new();
        private int itemCount = 0;
        public int ItemCount
        {
            get => this.itemCount;
            set
            {
                if (this.itemCount == value)
                {
                    return;
                }

                var count = Task.Run(async () =>
                {
                    var items = await this.GetStavkasAsync();
                    return items.Count;
                });

                this.itemCount = count.Result;
            }
        }

        public decimal GetTotalFor(List<Stavka> Stavkas)
        {
            if (Stavkas.Count > 0)
            {
                var total = Stavkas.Sum(i => i.Amount);
                return total;
            }
            else
            {
                return 0;
            }
        }

        public async Task<bool> SaveItemAsync(Stavka item)
        {
            if (item == null)
            {
                return false;
            }

            try
            {
                StavkaItemDatabase database = await StavkaItemDatabase.Instance;
                var res = await database.SaveItemAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Task.CompletedTask;
                return false;
            }
        }

        

        public decimal GetLeftToPay(List<Stavka> Stavkas)
        {
            if (!Stavkas.All(i => i.IsExpense))
            {
                return 0;
            }

            var toPayAmount = this.GetTotalFor(Stavkas);
            var paidAmount = Stavkas.Where(i => i.IsPaid).ToList().Sum(a => a.Amount);

            return toPayAmount - paidAmount;
        }

        public async Task<List<Stavka>> GetExpenseItemsAsync()
        {
            var items = await this.GetStavkasAsync();
            return items.Where<Stavka>(i => i.IsExpense).ToList();
        }

        public async Task<List<Stavka>> GetIncomeItemsAsync()
        {
            var items = await this.GetStavkasAsync();
            return items.Where<Stavka>(i => i.IsIncome).ToList();
        }

        public async Task<List<Stavka>> GetStavkasAsync()
        {
            StavkaItemDatabase database = await StavkaItemDatabase.Instance;
            this.Stavkas = await database.GetItemsAsync();
            return this.Stavkas;
        }
    }
}
