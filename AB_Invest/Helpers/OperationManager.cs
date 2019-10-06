using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB_invest.Models
{
    public static class OperationManager
    {
        public static async Task<bool> ShareOperation(this Context context, UserInfo customerInfo, UserInfo sellerInfo, ShareBalance newShareBalance)
        {
            var share = context.Shares
                            .Include(q => q.Company)
                            .Where(q => q.Id == newShareBalance.ShareId)
                            .FirstOrDefault();
            newShareBalance.Share = share;

            var customerCurrencyBalance = context.CurrencyBalances
                .Where(q => q.AccountId == customerInfo.Id && q.Currency.Id == share.CurrencyId && q.Type == customerInfo.Type)
                .FirstOrDefault();

            var customerShareBalance = context.ShareBalances
                .Where(q => q.AccountId == customerInfo.Id && q.ShareId == newShareBalance.ShareId && q.Type == customerInfo.Type)
                .FirstOrDefault();

            var sellerCurrencyBalance = context.CurrencyBalances
               .Where(q => q.AccountId == sellerInfo.Id && q.Currency.Id == share.CurrencyId && q.Type == sellerInfo.Type)
               .FirstOrDefault();

            var sellerShareBalance = context.ShareBalances
                .Where(q => q.AccountId == sellerInfo.Id && q.Share.Id == share.Id && q.Type == sellerInfo.Type)
                .FirstOrDefault();

            var cost = share.FinalCost * newShareBalance.Amount;

            if (customerCurrencyBalance != null && 
                cost <= customerCurrencyBalance.Amount &&
                sellerShareBalance.Amount >= newShareBalance.Amount)
            {
                customerCurrencyBalance.Amount -= cost;
                CreateOrUpdateSellerCurrencyBalance(context, sellerInfo, newShareBalance, sellerCurrencyBalance, cost);
                sellerShareBalance.Amount -= newShareBalance.Amount;
                CreateOrUpdateCustomerShareBalance(context, customerInfo, newShareBalance, customerShareBalance);

                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        private static void CreateOrUpdateCustomerShareBalance(Context context, UserInfo customerInfo, ShareBalance newShareBalance, ShareBalance customerShareBalance)
        {
            if (customerShareBalance == null)
            {
                customerShareBalance = new ShareBalance
                {
                    AccountId = customerInfo.Id,
                    Amount = newShareBalance.Amount,
                    ShareId = newShareBalance.ShareId,
                    Type = customerInfo.Type
                };
                context.ShareBalances.Add(customerShareBalance);
            }
            else
            {
                customerShareBalance.Amount += newShareBalance.Amount;
                context.ShareBalances.Update(customerShareBalance);
            }
        }

        private static void CreateOrUpdateSellerCurrencyBalance(
            Context context, UserInfo sellerInfo, ShareBalance newShareBalance, CurrencyBalance sellerCurrencyBalance, decimal cost)
        {
            if (sellerCurrencyBalance == null)
            {
                sellerCurrencyBalance = new CurrencyBalance
                {
                    AccountId = sellerInfo.Id,
                    Amount = cost,
                    CurrencyId = newShareBalance.Share.CurrencyId,
                    Type = sellerInfo.Type
                };
                context.CurrencyBalances.Add(sellerCurrencyBalance);
            }
            else
            {
                sellerCurrencyBalance.Amount += cost;
                context.CurrencyBalances.Update(sellerCurrencyBalance);
            }
        }
    }
}
