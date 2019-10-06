using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public class MinimalAccountData
    {
        public MinimalAccountData(Account account)
        {
            this.Type = account.Type;
            this.Phone = account.Phone;
            this.RegistrationDate = account.RegistrationDate;
        }

        public byte Type { get; set; }
        public DateTime RegistrationDate { get; set; }

        private string phone;
        public string Phone {
            get => phone;
            set
            {
                var newValue = $"x(xxx)-xxx-xx-{value.TakeLast(2)}";
                phone = newValue;
            }
        }

        public static bool HasSelectedType(MinimalAccountData minData, byte typeShift)
        {
            typeShift++;
            return (byte)((minData.Type >> typeShift - 1) << 8 - typeShift) != 0;
        }

        public bool HasSelectedType(byte typeIndex)
        {
            typeIndex++;
            return (byte)((Type >> typeIndex - 1) << 8 - typeIndex) != 0;
        }

        public bool HasSelectedType(AccountType typeIndex)
        {
            return HasSelectedType((byte)typeIndex);
        }
    }
}
