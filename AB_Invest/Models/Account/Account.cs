using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    // Type
    // +1 - user
    // +2 - company
    // +4 - ...

    // +3 - user + company ...

    public enum AccountType
    {
        user = 0,
        company,

        count
    }

    public class Account
    {
        public int Id { get; set; }
        public byte Type { get; set; }  

        public string Phone { get; set; }
        public string Password { get; set; }
        public DateTime RegistrationDate { get; set; }

        public UserData UserData { get; set; }
        public CompanyData CompanyData { get; set; }

        public List<CurrencyBalance> Balances { get; set; }
        public List<ShareBalance> Shares { get; set; }

        public bool HasSelectedType(byte typeIndex)
        {
            typeIndex++;
            return (byte)((Type >> typeIndex - 1) << 8 - typeIndex) != 0;
        }

        public bool HasSelectedType(AccountType typeIndex)
        {
            return HasSelectedType((byte)typeIndex);
        }

        public string GetDefaultTypeString()
        {
            for (byte i = 0; i < 8; i++)
            {
                if (HasSelectedType(i))
                    return ((AccountType)i).ToString();
            }
            return "";
        }

        public AccountType GetDefaultType()
        {
            for (byte i = 0; i < 8; i++)
            {
                if (HasSelectedType(i))
                    return ((AccountType)i);
            }
            return 0;
        }

        public static byte GetByteByType(AccountType type)
        {
            return (byte)(1 << (int)type);
        }
    }
}