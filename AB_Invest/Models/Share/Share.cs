using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace AB_invest.Models
{
    public enum ShareModerateStatus
    {
        WaitingVerification,
        Verification,
        Accepted,
        Rejected
    }

    public class Share
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public decimal Dividends { get; set; }
        public float MarkupPercent { get; set; }
        public ShareModerateStatus Status { get; set; }

        public DateTime? NextPayDay { get; set; }
        public byte Frequency { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public int CompanyDataId { get; set; }
        public CompanyData Company { get; set; }

        public List<ShareBalance> Balances { get; set; }

        public decimal FinalCost
        {
            get => GetFinalCost();
        }

        public string GetStatusStyle()
        {
            switch (Status)
            {
                case ShareModerateStatus.WaitingVerification:
                    return "label label-default";
                case ShareModerateStatus.Verification:
                    return "label label-info";
                case ShareModerateStatus.Accepted:
                    return "label label-success";
                case ShareModerateStatus.Rejected:
                    return "label label-danger";
                default:
                    return "";
            }
        }

        public decimal GetFinalCost()
        {
            return Cost * ((decimal)MarkupPercent / 100 + 1);
        }
    }
}