using AB_invest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest
{
    public static class WebClassHelper
    {
        public static string GetButtonStyle(AccountType typeA, AccountType typeB)
        {
            return typeA == typeB ? "btn-primary" : "btn-default";
        }

        public static string GetSharesOwnerTitle(AccountType type)
        {
            switch (type)
            {
                case AccountType.user:
                    return "Ваши акции";
                case AccountType.company:
                    return "Акции компании";
                default:
                    throw new Exception("Неопределенный тип аккаунта");
            }
        }

        public static string GetBtnStyleByCompare(string strA, string strB)
        {
            return strA == strB ? "btn btn-primary" : "btn btn-default";
        }
    }
}


