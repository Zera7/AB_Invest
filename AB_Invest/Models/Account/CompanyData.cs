using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public class CompanyData
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public List<Share> Shares { get; set; }
    }
}
