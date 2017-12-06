using System.Collections.Generic;

namespace VendingMachineCore
{
    public interface IVendingService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="product">Product to purchase</param>
        /// <param name="deposits">cash offered to pay for product</param>
        /// <param name="change">change to return to buyer. In case of failure, all deposits will be returned</param>
        /// <param name="failureReason">contains text only if purchase failes</param>
        /// <returns>Returns true if purchase successs, false otherwise</returns>
        bool PurchaseProduct(IProduct product, IEnumerable<ICashDenomination> deposits, out IEnumerable<ICashDenomination> change, out string failureReason);

        Dictionary<IProduct, int> GetProductsAvailable();

        IEnumerable<ICashDenomination> GetAcceptedDenominations();
        
    }

    public class VendingService : IVendingService
    {
        private readonly ICashStore cashStore;
        private readonly IProductCatalogue productCatalogue;

        public VendingService()
        {
            this.cashStore = new CashStore(new CashStoreLoader());
            this.productCatalogue = new ProductCatalogue(new ProductCatalogueLoader());
        }

        public IEnumerable<ICashDenomination> GetAcceptedDenominations()
        {
            return cashStore.GetAcceptedDenominations();
        }

        public Dictionary<IProduct, int> GetProductsAvailable()
        {
            return productCatalogue.GetProductAvailability();
        }
        
        public bool PurchaseProduct(IProduct product, IEnumerable<ICashDenomination> deposits, 
            out IEnumerable<ICashDenomination> change, out string failureReason)
        {
            change = cashStore.DepositToStore(product.Cost, deposits, out string depositRejctReason);
            if (!string.IsNullOrEmpty(depositRejctReason))
            {
                failureReason = $"Unable to deposit cash: {depositRejctReason}";
                return false;
            }
            
            //try to withdraw the product
            if (!productCatalogue.WithdrawProduct(product, out string withdrawFailReason))
            {
                failureReason = $"Unable to withdraw item: {withdrawFailReason}";
                change = deposits;
                return false;
            }
            
            failureReason = string.Empty;
            return true;
        }
    }
}