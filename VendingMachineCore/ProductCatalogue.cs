using System;
using System.Collections.Generic;

namespace VendingMachineCore
{
    public interface IProductCatalogue
    {
        Dictionary<IProduct, int> GetProductAvailability();
        bool ContainsProduct(IProduct product);

        bool WithdrawProduct(IProduct product, out string failureReason);
    }

    internal class ProductCatalogue : IProductCatalogue
    {
        private readonly IProductCatalogueLoader catalogueLoader;

        internal ProductCatalogue(IProductCatalogueLoader catalogueLoader)
        {
            this.catalogueLoader = catalogueLoader;
            productAvailability = catalogueLoader.GetCataloge();
        }

        private Dictionary<IProduct, int> productAvailability;

        public Dictionary<IProduct, int> GetProductAvailability()
        {
            //return copy to stop others from altering the interal collection
            return CopyDictionary(productAvailability);   
        }

        private Dictionary<IProduct, int> CopyDictionary(Dictionary<IProduct, int> original)
        {
            Dictionary<IProduct, int> copy = new Dictionary<IProduct, int>(original.Count, original.Comparer);
            foreach (KeyValuePair<IProduct, int> entry in original)
            {
                copy.Add(entry.Key, entry.Value);
            }
            return copy;
        }

        public bool ContainsProduct(IProduct product)
        {
            return productAvailability.ContainsKey(product);
        }

        public bool WithdrawProduct(IProduct product, out string failureReason)
        {
            if (!productAvailability.TryGetValue(product, out int stockCount))
            {
                failureReason = "Product not known";
                return false;
            }
            if (stockCount < 1)
            {
                failureReason = "Product is not in Stock";
                return false;
            }

            productAvailability[product]--;
            failureReason = string.Empty;
            return true;
        }
    }
}
