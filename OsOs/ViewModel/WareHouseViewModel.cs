using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using OsOs.Annotations;
using OsOs.Handler;
using OsOs.Model;
using OsOs.Utilities;

namespace OsOs.ViewModel
{
    class WareHouseViewModel : INotifyPropertyChanged
    {
        #region ViewVisibilityProps-For-General
        private ICommand _oneDeleteButtonToRuleThemAllCommand;
        public ICommand OneDeleteButtonToRuleThemAllCommand
        {
            get { return _oneDeleteButtonToRuleThemAllCommand; }
            set { _oneDeleteButtonToRuleThemAllCommand = value; OnPropertyChanged(); }
        }
        private Visibility _isVisibleOverlaySearch;
        private string _overlaySearchHeader;
        private string _overlaySearchTerm;
        public ICommand CloseOverlaySearchCommand { get; set; }
        public Visibility IsVisibleOverlaySearch
        {
            get { return _isVisibleOverlaySearch; }
            set { _isVisibleOverlaySearch = value; OnPropertyChanged(); }
        }
        public string OverlaySearchHeader
        {
            get { return _overlaySearchHeader; }
            set { _overlaySearchHeader = value; OnPropertyChanged(); }
        }
        public string OverlaySearchTerm
        {
            get { return _overlaySearchTerm; }
            set
            {
                _overlaySearchTerm = value;
                OnPropertyChanged();
                RefreshFilteredProducts();
                if (Product != null)
                {
                    RefreshFilteredFromLocation();
                    RefreshFilteredToLocation();
                }
            }
        }
        private void CloseOverlaySearch()
        {
            IsVisibleOverlaySearch = Visibility.Collapsed;
            OverlaySearchTerm = "";

            IsVisibleProductList = Visibility.Collapsed;
            ToggleVisibilityProductList = false;

            IsVisibleLocationList = Visibility.Collapsed;
            ToggleVisibilityLocationList = false;

            IsVisibleSecondLocationList = Visibility.Collapsed;
            ToggleVisibilitySecondLocationList = false;
        }
        #endregion

        #region ViewVisibilityProps-For-Products
        private Visibility _isVisibleProductList;

        public Visibility IsVisibleProductList
        {
            get { return _isVisibleProductList; }
            set { _isVisibleProductList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityProductList;

        private Product _selectedItemProduct;

        public bool ToggleVisibilityProductList
        {
            get { return _toggleVisibilityProductList; }
            set
            {
                _toggleVisibilityProductList = value;
                if (_toggleVisibilityProductList)
                {
                    OverlaySearchHeader = "Søg produkt";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleProductList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SetSelectedProductCommand { get; set; }

        private void SetSelectedProduct()
        {
            if (SelectedItemProduct != null)
            {
                Product = SelectedItemProduct;
                ProductName = SelectedItemProduct.Name;
                FilteredFromLocation = FindFromLocations();
                FilteredToLocation = FindToLocations();
            }
            
            
            ClearProductListView();
            CloseOverlaySearch();
        }
        public ICommand ClearProductListViewCommand { get; set; }

        public Product SelectedItemProduct
        {
            get { return _selectedItemProduct; }
            set { _selectedItemProduct = value; OnPropertyChanged(); }
        }
        public void ClearProductListView()
        {
            CloseOverlaySearch();
            SelectedItemProduct = null;
        }
        #endregion

        #region ViewVisibilityProps-For-Location
        private Visibility _isVisibleLocationList;

        public Visibility IsVisibleLocationList
        {
            get { return _isVisibleLocationList; }
            set { _isVisibleLocationList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityLocationList;

        private Product_Location _selectedItemLocation;

        public bool ToggleVisibilityLocationList
        {
            get { return _toggleVisibilityLocationList; }
            set
            {
                _toggleVisibilityLocationList = value;
                if (_toggleVisibilityLocationList)
                {
                    OverlaySearchHeader = "Søg lokation";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleLocationList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SetSelectedLocationCommand { get; set; }

        private void SetSelectedLocation()
        {
            if (SelectedItemLocation != null)
            {
                LocationId = SelectedItemLocation.Location;
                Location = SelectedItemLocation;
            }
            CloseOverlaySearch();
            ClearLocationListView();
        }

        public ICommand ClearLocationListViewCommand { get; set; }

        public Product_Location SelectedItemLocation
        {
            get { return _selectedItemLocation; }
            set { _selectedItemLocation = value; OnPropertyChanged(); }
        }
        public void ClearLocationListView()
        {
            CloseOverlaySearch();
            SelectedItemLocation = null;
        }

        private Visibility _isVisibleSearchProduct;
        public Visibility IsVisibleSearchProduct
        {
            get { return _isVisibleSearchProduct; }
            set { _isVisibleSearchProduct = value; OnPropertyChanged(); }
        }

        #endregion

        #region ViewVisibilityProps-For-Second-Location
        private Visibility _isVisibleSecondLocationList;

        public Visibility IsVisibleSecondLocationList
        {
            get { return _isVisibleSecondLocationList; }
            set { _isVisibleSecondLocationList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilitySecondLocationList;

        private Product_Location _selectedItemSecondLocation;

        public bool ToggleVisibilitySecondLocationList
        {
            get { return _toggleVisibilitySecondLocationList; }
            set
            {
                _toggleVisibilitySecondLocationList = value;
                if (_toggleVisibilitySecondLocationList)
                {
                    OverlaySearchHeader = "Søg lokation";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleSecondLocationList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SetSelectedSecondLocationCommand { get; set; }

        private void SetSelectedSecondLocation()
        {
            if (SelectedItemSecondLocation != null)
            {
                LocationIdSecond = SelectedItemSecondLocation.Location;
                SecondLocation = SelectedItemSecondLocation;
            }
            CloseOverlaySearch();
            ClearSecondLocationListView();
        }

        public ICommand ClearSecondLocationListViewCommand { get; set; }

        public Product_Location SelectedItemSecondLocation
        {
            get { return _selectedItemSecondLocation; }
            set { _selectedItemSecondLocation = value; OnPropertyChanged(); }
        }
        public void ClearSecondLocationListView()
        {
            CloseOverlaySearch();
            SelectedItemSecondLocation = null;
        }
        #endregion
        public ICommand SubmitWithdrawButtonCommand { get; set; }
        public Product_Location SecondLocation { get; set; }
        public ICommand SubmitMoveButtonCommand { get; set; }
        #region ViewVisibilityProps-For-OrderProcess
        private Visibility _isVisibleOrderProcessOverlay;
        private bool _toggleVisibilityProcessOrder;
        public bool ToggleVisibilityProcessOrder
        {
            get { return _toggleVisibilityProcessOrder; }
            set
            {
                _toggleVisibilityProcessOrder = value;
                if (_toggleVisibilityProcessOrder)
                {
                    if (SelectedItemOrder != null)
                    {
                        IsVisibleOrderProcessOverlay = Visibility.Visible;
                        SetSelectedOrder();
                    }
                    else
                    {
                        ToggleVisibilityProcessOrder = false;
                    }
                }
                OnPropertyChanged();
            }
        }

        public ICommand CancleOrderProcessCommand { get; set; }

        private void CancleOrderProcess()
        {
            ClearSelectedOrder();
            ToggleVisibilityProcessOrder = false;
            IsVisibleOrderProcessOverlay = Visibility.Collapsed;
        }
        public Visibility IsVisibleOrderProcessOverlay
        {
            get { return _isVisibleOrderProcessOverlay; }
            set { _isVisibleOrderProcessOverlay = value; OnPropertyChanged();}
        }
        private Order _selectedItemOrder;

        public Order SelectedItemOrder
        {
            get { return _selectedItemOrder; }
            set { _selectedItemOrder = value; OnPropertyChanged();}
        }
        private void SetSelectedOrder()
        {
            if (SelectedItemOrder != null)
            {
                OrderNumber = SelectedItemOrder.Id;
                OrderAddress = SelectedItemOrder.Address;
                GetOrderLines();
            }
        }
        private void ClearSelectedOrder()
        {
            SelectedItemOrder = null;
            OrderNumber = 0;
            OrderAddress = null;
            OrderLines = null;
        }
        #endregion

        #region PropsProduct
        private Product _product;
        private string _productName;
        private string _locationId;
        private string _locationIdSecond;
        public ObservableCollection<Product> Products { get; set; }
        private ObservableCollection<Product> _filteredProducts;
        public ObservableCollection<Product> FilteredProducts
        {
            get { return _filteredProducts; }
            set { _filteredProducts = value; OnPropertyChanged(); }
        }

        private void RefreshFilteredProducts()
        {
            var result = Products.Where(p => p.Name.ToLower().Contains(OverlaySearchTerm));
            if (FilteredProducts == null || FilteredProducts.Count != result.Count())
            {
                FilteredProducts = new ObservableCollection<Product>(result);
            }
        }
        public Product Product
        {
            get { return _product; }
            set { _product = value; OnPropertyChanged(); }
        }
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; OnPropertyChanged(); }
        }

        #endregion

        #region PropsLocation
        private int _amount;
        private string _batchNo;
        public string LocationId
        {
            get { return _locationId; }
            set { _locationId = value; OnPropertyChanged(); }
        }

        public string LocationIdSecond
        {
            get { return _locationIdSecond; }
            set { _locationIdSecond = value; OnPropertyChanged(); }
        }

        public string BatchNo
        {
            get { return _batchNo; }
            set { _batchNo = value; OnPropertyChanged(); }
        }

        public int Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged(); }
        }

        public bool Reserved { get; set; }
        public int AmountToMove { get; set; }
        public DateTime DateOfArrival { get; set; }

        public bool LocationReserved { get; set; }


        public Product_Location Location { get; set; }
        #endregion

        #region PropsOrder
        private int _orderNumber;
        private Address _orderAddress;

        private ObservableCollection<Order_Line> _orderLines;
        public ObservableCollection<Order_Line> OrderLines
        {
            get { return _orderLines; }
            set { _orderLines = value; OnPropertyChanged();}
        }

        public int OrderNumber
        {
            get { return _orderNumber; }
            set { _orderNumber = value; OnPropertyChanged(); }
        }

        public Address OrderAddress
        {
            get { return _orderAddress; }
            set { _orderAddress = value; OnPropertyChanged();}
        }
        private void GetOrderLines()
        {
            var result = Singleton.GetInstance().OrderLines.Where(ol => ol.FK_Order_Id == OrderNumber);
            if(OrderLines == null)
                OrderLines = new ObservableCollection<Order_Line>(result);
        }
        #endregion


        public ObservableCollection<Order> Orders { get; set; }
       
        public ICommand SubmitDepositButtonCommand { get; set; }

        public WareHouseHandler wareHouseHandler { get; set; }
        public DateTime DateToday { get; set; }
        private ObservableCollection<Product_Location> _filteredFromLocation;
        private ObservableCollection<Product_Location> _filteredToLocation;
        private ObservableCollection<Product_Location> _backUpToLocations;
        private ObservableCollection<Product_Location> _backUpFromLocations;


        public ObservableCollection<Product_Location> FilteredFromLocation
        {
            get { return _filteredFromLocation; }
            set { _filteredFromLocation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product_Location> BackUpFromLocations
        {
            get { return _backUpFromLocations; }
            set { _backUpFromLocations = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Product_Location> FindFromLocations()
        {
            var result = Singleton.GetInstance().Locations.Where(l => l.Product == SelectedItemProduct);
            //            var result = Singleton.GetInstance().Locations.Where(l => l.Product.Id == SelectedItemProduct.Id && l.Quantity > 0);
            BackUpFromLocations = new ObservableCollection<Product_Location>(result);
            return new ObservableCollection<Product_Location>(result);

        }
        private void RefreshFilteredFromLocation()
        {

            var result = BackUpFromLocations.Where(l => l.Location.ToLower().Contains(OverlaySearchTerm));
            if (FilteredFromLocation == null || FilteredFromLocation.Count != result.Count())
            {
                FilteredFromLocation = new ObservableCollection<Product_Location>(result);
            }
        }

        public ObservableCollection<Product_Location> FilteredToLocation
        {
            get { return _filteredToLocation; }
            set { _filteredToLocation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Product_Location> BackUpToLocations
        {
            get { return _backUpToLocations; }
            set { _backUpToLocations = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Product_Location> FindToLocations()
        {
            var result = Singleton.GetInstance().Locations.Where(l =>
                                (l.Product == SelectedItemProduct) ||
                                (l.Reserved && l.Product == SelectedItemProduct) ||
                                l.Reserved == false && l.Product != SelectedItemProduct);
            BackUpToLocations = new ObservableCollection<Product_Location>(result);
            return new ObservableCollection<Product_Location>(result);
        }
        private void RefreshFilteredToLocation()
        {
            var result = BackUpToLocations.Where(l => l.Location.ToLower().Contains(OverlaySearchTerm));
            if (FilteredToLocation == null || FilteredToLocation.Count != result.Count())
            {
                FilteredToLocation = new ObservableCollection<Product_Location>(result);
            }
        }

        public WareHouseViewModel()
        {
            wareHouseHandler = new WareHouseHandler(this);
            CloseOverlaySearchCommand = new RelayCommand(CloseOverlaySearch);
            DateToday = DateTime.Today;
            Orders = Singleton.GetInstance().Orders;
            Products = Singleton.GetInstance().Products;
            FilteredProducts = Products;
            SetSelectedLocationCommand = new RelayCommand(SetSelectedLocation);
            ClearLocationListViewCommand = new RelayCommand(ClearLocationListView);
            SetSelectedProductCommand = new RelayCommand(SetSelectedProduct);
            ClearProductListViewCommand = new RelayCommand(ClearProductListView);
            SetSelectedSecondLocationCommand = new RelayCommand(SetSelectedSecondLocation);
            ClearSecondLocationListViewCommand = new RelayCommand(ClearSecondLocationListView);
            SubmitDepositButtonCommand = new RelayCommand(() => wareHouseHandler.AddToLocation(Location));
            SubmitWithdrawButtonCommand = new RelayCommand(() => wareHouseHandler.Withdraw(Location));
            SubmitMoveButtonCommand = new RelayCommand(wareHouseHandler.MoveToLocation);
            CancleOrderProcessCommand = new RelayCommand(CancleOrderProcess);
            CloseOverlaySearch();
            IsVisibleOrderProcessOverlay = Visibility.Collapsed;
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}