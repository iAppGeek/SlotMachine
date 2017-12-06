using System;
using System.Collections.Generic;
using System.Linq;

namespace VendingMachineCore
{
    public interface ICashStore
    {
        /// <summary>
        /// Add cash to the Vending Machines insternal store
        /// </summary>
        /// <param name="denimination">can only support deposit of single item at the moment</param>
        /// <param name="depositFailedReason"></param>
        /// <returns>Returns true if store is able to accept the cash</returns>
        IEnumerable<ICashDenomination> DepositToStore(int depositAmount, IEnumerable<ICashDenomination> denominations,
            out string depositFailedReason);

        IEnumerable<ICashDenomination> GetAcceptedDenominations();
    }

    internal class CashStore : ICashStore
    {
        private Dictionary<ICashDenomination, int> deposits;
        private readonly ICashStoreLoader cashStoreLoader;

        internal CashStore(ICashStoreLoader cashStoreLoader)
        {
            deposits = cashStoreLoader.LoadData();
        }
        
        public IEnumerable<ICashDenomination> DepositToStore(int depositAmount, IEnumerable<ICashDenomination> denominations,
            out string depositFailedReason)
        {
            if (!denominations.Any())
            {
                depositFailedReason = "No denominations depositied";
                return denominations;
            }

            //in the future may implelment storage capacities for cash, so mutiple reasons for failure
            if (!denominations.Any(d => deposits.ContainsKey(d)))
            {
                depositFailedReason = "One of the denominations is not known to this Cash Store";
                return denominations;
            }

            //ensure enough coins deposited for the amount intended
            int denominationSum = denominations.Sum(d => d.Value);
            if (denominationSum < depositAmount)
            {
                depositFailedReason = "Fewer denominations depositied than required for amount";
                return denominations;
            }

            if (CalculateChange(denominationSum - depositAmount, out List<ICashDenomination> change))
            {
                //assume everything we know about exists in the deposits
                foreach(var d in denominations)
                {
                    deposits[d]++;
                }
            }
            else
            {
                //failed to calculate change?
            }

            depositFailedReason = string.Empty;
            return change;
        }

        public IEnumerable<ICashDenomination> GetAcceptedDenominations()
        {
            return deposits.Keys;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v">amount of change needed</param>
        /// <param name="change"></param>
        /// <returns></returns>
        private bool CalculateChange(int changeNeeded, out List<ICashDenomination> change)
        {
            change = new List<ICashDenomination>();

            int changeSumCounter = 0;
            foreach(var item in deposits.OrderByDescending(d => d.Key.Value))
            { //this will loop through all the denominations in any case...
                while (true)
                {
                    int changeRemaining = changeNeeded - changeSumCounter;
                    if ((changeRemaining >= item.Key.Value))
                    {
                        change.Add(item.Key);
                        changeSumCounter += item.Key.Value;
                    }
                    else
                    {
                        break;
                    }
                }
                
            }
            

            if(changeSumCounter == changeNeeded)
            { //success
                foreach(var c in change)
                {
                    if(deposits[c] <= 0)
                    { //fail, we dont have the change
                        change = new List<ICashDenomination>();
                        return false;
                    }
                    deposits[c]--;
                }
            }
            else
            {
                change = new List<ICashDenomination>();
                return false;
            }
            return true;
        }

        /*
         * not required for now
        public IEnumerable<ICashDenomination> WithdrawFromStore(int amount, out string withdrawFailedReason)
        {
            if (!deposits.ContainsKey(denomination))
            {
                withdrawFailedReason = "The denomination is not known to this Cash Store";
                return false;
            }

            if(!(deposits[denomination] > 0))
            {
                withdrawFailedReason = "There is no denomination avalable of this type in this Cash Store";
                return false;
            }

            withdrawFailedReason = string.Empty;
            deposits[denomination]--;
            return true;
        }

        public int TotalInStore()
        {
            return deposits.Sum(i => i.Key.Value * i.Value);
        }
        */
    }
}
