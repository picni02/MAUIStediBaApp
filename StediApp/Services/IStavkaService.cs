using StediApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBudgetApp.Services
{
    public interface IStavkaService
    {
        public decimal GetTotalFor(List<Stavka> Stavkas);
        public Task<bool> SaveItemAsync(Stavka item);
        public decimal GetLeftToPay(List<Stavka> Stavkas);
        public Task<List<Stavka>> GetStavkasAsync();
        public Task<List<Stavka>> GetExpenseItemsAsync();
        public Task<List<Stavka>> GetIncomeItemsAsync();

    }
}
