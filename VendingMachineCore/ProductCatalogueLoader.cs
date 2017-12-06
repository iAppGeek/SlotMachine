using System.Collections.Generic;

namespace VendingMachineCore
{
    public interface IProductCatalogueLoader
    {
        Dictionary<IProduct, int> GetCataloge();
    }

    internal class ProductCatalogueLoader : IProductCatalogueLoader
    {
        public Dictionary<IProduct, int> GetCataloge()
        {
            //load up the "Vending Machine" from here for now using static data - later on there would be a store like a DB
            return new Dictionary<IProduct, int>(){
                { new Product("KitKat", 110), 3 },
                { new Product("Dairy Milk", 140), 5 },
                { new Product("Milky Way", 80), 1 },
                { new Product("Galaxy", 70), 10 },
                { new Product("Kinder Surprise", 200), 0 }
            };
        }
    }
}
