using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public class ShareBalance
    {
        public int Id { get; set; }
        public uint Amount { get; set; }

        public byte Type { get; set; }

        public int ShareId { get; set; }
        public Share Share { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
