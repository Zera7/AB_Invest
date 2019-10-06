using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public class CurrencyDeal
    {
        public int Id { get; set; }

        public int CustomerID { get; set; }
        public Account CustomerAccount { get; set; }

        public int SellerID { get; set; }
        public Account SellerAccount { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
    }
}
