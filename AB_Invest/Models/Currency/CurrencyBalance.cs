using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public class CurrencyBalance
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public byte Type { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
