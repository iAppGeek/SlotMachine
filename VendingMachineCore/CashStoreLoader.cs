using System.Collections.Generic;

namespace VendingMachineCore
{
    interface ICashStoreLoader
    {
        Dictionary<ICashDenomination, int> LoadData();
    }
    class CashStoreLoader : ICashStoreLoader
    {
        public Dictionary<ICashDenomination, int> LoadData()
        {
            //load up the "Vending Machine" from here for now using static data - later on there would be a store like a DB
            return new Dictionary<ICashDenomination, int>(){
                { new CashDenomination("5 pence", 5), 20 },
                { new CashDenomination("10 pence", 10), 20 },
                { new CashDenomination("20 pence", 20), 20 },
                { new CashDenomination("50 pence", 50), 20 },
                { new CashDenomination("1 pound", 100), 20 },
                { new CashDenomination("2 pounds", 200), 20 }
            };
        }
    }
}
