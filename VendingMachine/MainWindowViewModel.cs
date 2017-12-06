using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using VendingMachineCore;

namespace SlotMachine
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private IEnumerable<ICashDenomination> acceptedCash;
        private readonly IVendingService service;

        public object SelectedProductView { get; set; }

        public IProduct SelectedProduct
        {
            get
            {
                return (SelectedProductView as ProductViewModel).SourceObj;
            }
        }

        public MainWindowViewModel()
        {
            PurchaseOutput = "...";

            service = new VendingService();
            acceptedCash = service.GetAcceptedDenominations();

            PurchaseCommand = new MyCommand(Purchase);
            InitCollections();
        }

        private void Purchase()
        {
            string failureString = string.Empty;

            var inputItems = CashInputDenominations.SourceCollection as List<CoinViewModel>;
            List<ICashDenomination> cash = new List<ICashDenomination>();
            foreach(var item in inputItems)
            {
                if(item.Count > 0)
                {
                    for(int i = 0; i < item.Count; i++)
                    {
                        cash.Add(item.SourceObj);
                    }
                }
            }
            IEnumerable<ICashDenomination> change = new List<ICashDenomination>();
            if (!service.PurchaseProduct(SelectedProduct, cash, out change, out failureString))
            {
                PurchaseOutput = failureString;
            }
            else
            {
                PurchaseOutput = "Purchase Made!";
            }
            InitCollections(); //poor way of handling the changing model values for now
            ShowChange(change);
        }

        private void ShowChange(IEnumerable<ICashDenomination> change)
        {
            var l = new List<CoinViewModel>();

            foreach (var i in acceptedCash)
            {
                l.Add(new CoinViewModel(i) {Count = change.Count(c => c == i) });
            }

            CashOutputDenominations = new ListCollectionView(l);
            OnPropertyChanged(nameof(CashOutputDenominations));
        }

        private void InitCollections()
        {
            var l1 = new List<CoinViewModel>();
            var l2 = new List<CoinViewModel>();

            foreach(var i in acceptedCash)
            {
                l1.Add(new CoinViewModel(i));
                l2.Add(new CoinViewModel(i));
            }

            CashInputDenominations = new ListCollectionView(l1);
            CashOutputDenominations = new ListCollectionView(l2);

            List<ProductViewModel> prods = new List<ProductViewModel>();
            List<ProductViewModel> prods2 = new List<ProductViewModel>();
            foreach (var p in service.GetProductsAvailable())
            {
                prods.Add(new ProductViewModel(p.Key, p.Value));
                prods2.Add(new ProductViewModel(p.Key, p.Value));
            }

            ProductStoreDetails = new ListCollectionView(prods);
            Products = new ListCollectionView(prods2);

            CollectionsChanges();
        }

        public string WindowTitle => "Vending Machine";

        private string purchaseOutput;
        public string PurchaseOutput
        {
            get { return purchaseOutput; }
            set { purchaseOutput = value; OnPropertyChanged(nameof(PurchaseOutput)); }
        }
        
        public MyCommand PurchaseCommand { get; private set; }
        public ListCollectionView CashInputDenominations { get; private set; }
        public ListCollectionView CashOutputDenominations { get; private set; }
        public ListCollectionView ProductStoreDetails { get; private set; }
        public ListCollectionView Products { get; private set; }

        //TODO should ensure that event are managed better for each item change rather than bulk changes
        private void CollectionsChanges()
        {
            OnPropertyChanged(nameof(CashInputDenominations));
            OnPropertyChanged(nameof(CashOutputDenominations));
            OnPropertyChanged(nameof(ProductStoreDetails));
        }

        private class CoinViewModel : INotifyPropertyChanged
        {
            private string coinLabel;
            private int count;
            private ICashDenomination i;

            public CoinViewModel(ICashDenomination i)
            {
                SourceObj = i;
                CoinLabel = i.FriendlyName;
            }

            public string CoinLabel {
                get => coinLabel;
                private set { coinLabel = value; OnPropertyChanged(nameof(CoinLabel)); }
            }
            public int Count
            {
                get => count;
                set { count = value; OnPropertyChanged(nameof(Count)); }
            }
            
            public ICashDenomination SourceObj { get; private set; }

            protected void OnPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class MyCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private readonly Action action;

            public MyCommand(Action a)
            {
                this.action = a;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                action.Invoke();
            }
        }

        private class ProductViewModel : INotifyPropertyChanged
        {
            private string productName;
            private int count;
            private int cost;
            private KeyValuePair<IProduct, int> p;

            public ProductViewModel(IProduct p, int count)
            {
                SourceObj = p;
                ProductName = p.Name;
                Cost = p.Cost;
                Count = count;
            }

            public IProduct SourceObj { get; private set; }

            public string ProductName
            {
                get => productName;
                set { productName = value; OnPropertyChanged(nameof(ProductName)); }
            }
            public int Count
            {
                get => count;
                set { count = value; OnPropertyChanged(nameof(Count)); }
            }
            public int Cost
            {
                get => cost;
                set { cost = value; OnPropertyChanged(nameof(Cost)); }
            }
            
            protected void OnPropertyChanged(string propName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
