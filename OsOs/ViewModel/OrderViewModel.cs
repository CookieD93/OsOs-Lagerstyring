using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
    class OrderViewModel : INotifyPropertyChanged
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
                RefreshFilteredOrders();
                RefreshFilteredCustomers();
                RefreshFilteredProducts();
                RefreshFilteredAddresses();
            }
        }
        private void CloseOverlaySearch()
        {
            IsVisibleOverlaySearch = Visibility.Collapsed;
            OverlaySearchTerm = "";
            IsVisibleOrderList = Visibility.Collapsed;
            ToggleVisibilityOrderList = false;
            IsVisibleCustomerList = Visibility.Collapsed;
            ToggleVisibilityCustomerList = false;
            IsVisibleAddressList = Visibility.Collapsed;
            ToggleVisibilityAddressList = false;
            IsVisibleProductList = Visibility.Collapsed;
            ToggleVisibilityProductList = false;
        }
        #endregion


        #region ViewVisibilityProps-For-Customer
        private Visibility _isVisibleCustomerList;
        public Visibility IsVisibleCustomerList
        {
            get { return _isVisibleCustomerList; }
            set { _isVisibleCustomerList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityCustomerList;

        private Customer _selectedItemCustomer;

        public bool ToggleVisibilityCustomerList
        {
            get { return _toggleVisibilityCustomerList; }
            set
            {
                _toggleVisibilityCustomerList = value;
                if (_toggleVisibilityCustomerList)
                {
                    OverlaySearchHeader = "Søg kunde";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleCustomerList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SetSelectedCustomerCommand { get; set; }

        private void SetSelectedCustomer()
        {
            if (SelectedItemCustomer != null)
            {
                OrderCustomer = SelectedItemCustomer;
                OrderAddress = SelectedItemCustomer.Address;
                CustomerName = SelectedItemCustomer.Name;
                AddressName = SelectedItemCustomer.Name;
                Address1 = SelectedItemCustomer.Address.Address1;
                Address2 = SelectedItemCustomer.Address.Address2;
                AddressPostal = SelectedItemCustomer.Address.Postal;
                AddressTown = SelectedItemCustomer.Address.Town;
                AddressCountryIso = SelectedItemCustomer.Address.CountryCode.Iso;
                AddressCountryName = SelectedItemCustomer.Address.CountryCode.Country;
                AddressPhone = SelectedItemCustomer.Address.Phone;
                AddressEmail = SelectedItemCustomer.Address.Email;
            }
            ClearCustomerListView();
            CloseOverlaySearch();

        }

        public ICommand ClearCustomerListViewCommand { get; set; }

        public Customer SelectedItemCustomer
        {
            get { return _selectedItemCustomer; }
            set { _selectedItemCustomer = value; OnPropertyChanged(); }
        }
        public void ClearCustomerListView()
        {
            CloseOverlaySearch();
            SelectedItemCustomer = null;
        }
        #endregion

        #region ViewVisibilityProps-For-Order
        private Visibility _isVisibleOrderList;
        public Visibility IsVisibleOrderList
        {
            get { return _isVisibleOrderList; }
            set { _isVisibleOrderList = value; OnPropertyChanged(); }
        }
        private bool _toggleVisibilityOrderList;
        public bool ToggleVisibilityOrderList
        {
            get { return _toggleVisibilityOrderList; }
            set
            {
                _toggleVisibilityOrderList = value;
                if (_toggleVisibilityOrderList)
                {
                    OverlaySearchHeader = "Søg ordre";

                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleOrderList = Visibility.Visible;

                }
                OnPropertyChanged();
            }
        }
        private Order _selectedItemOrder;
        public ICommand SetSelectedOrderCommand { get; set; }

        public void SetSelectedOrder()
        {
            if (SelectedItemOrder != null)
            {
                OrderDate = SelectedItemOrder.Date;
                OrderId = SelectedItemOrder.Id;
                OrderCustomer = SelectedItemOrder.Customer;
                OrderAddress = OrderCustomer.Address;
                CustomerName = SelectedItemOrder.Customer.Name;
                AddressName = OrderAddress.Name;
                Address1 = OrderAddress.Address1;
                Address2 = OrderAddress.Address2;
                AddressPostal = OrderAddress.Postal;
                AddressTown = OrderAddress.Town;
                AddressCountryIso = OrderAddress.CountryCode.Iso;
                AddressCountryName = OrderAddress.CountryCode.Country;
                AddressPhone = OrderAddress.Phone;
                AddressEmail = OrderAddress.Email;
                OrderHandler.LoadOrder(SelectedItemOrder);
            }
            ClearOrderListView();
            CloseOverlaySearch();
        }
        public Order SelectedItemOrder
        {
            get { return _selectedItemOrder; }
            set { _selectedItemOrder = value; OnPropertyChanged(); }
        }
        public ICommand ClearOrderListViewCommand { get; set; }
        public void ClearOrderListView()
        {
            CloseOverlaySearch();
            SelectedItemOrder = null;
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
                SelectedProduct = SelectedItemProduct;
                ProductName = SelectedItemProduct.Name;
                Batch = "";
            }

            CloseOverlaySearch();
            ClearProductListView();
        }

        public void UnsetSelectedProduct()
        {
           
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

        #region ViewVisibilityProps-For-Address
        private Visibility _isVisibleSearchAddress;
        private Visibility _isVisibleAddressList;
        public Visibility IsVisibleSearchAddress
        {
            get { return _isVisibleSearchAddress; }
            set { _isVisibleSearchAddress = value; OnPropertyChanged(); }
        }
        public Visibility IsVisibleAddressList
        {
            get { return _isVisibleAddressList; }
            set { _isVisibleAddressList = value; OnPropertyChanged(); }
        }
        private bool _toggleVisibilitySearchAddress;
        private bool _toggleVisibilityAddressList;
        private Address _selectedItemAddress;

        public bool ToggleVisibilityAddressList
        {
            get { return _toggleVisibilityAddressList; }
            set
            {
                _toggleVisibilityAddressList = value;
                if (_toggleVisibilityAddressList)
                {
                    OverlaySearchHeader = "Søg eksisterende adresse";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleAddressList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        public bool ToggleVisibilitySearchAddress
        {
            get { return _toggleVisibilitySearchAddress; }
            set
            {
                _toggleVisibilitySearchAddress = value;
                if (_toggleVisibilitySearchAddress)
                {
                    IsVisibleSearchAddress = Visibility.Visible;
                }
                else
                {
                    IsVisibleSearchAddress = Visibility.Collapsed;
                }
                OnPropertyChanged();
            }
        }

        
        public ICommand ClearAddressListViewCommand { get; set; }
        public ICommand SetSelectedAddressCommand { get; set; }

        private void SetSelectedAddress()
        {
            if (SelectedItemAddress != null)
            {
                OrderAddress = SelectedItemAddress;
                AddressName = SelectedItemAddress.Name;
                Address1 = SelectedItemAddress.Address1;
                Address2 = SelectedItemAddress.Address2;
                AddressPostal = SelectedItemAddress.Postal;
                AddressTown = SelectedItemAddress.Town;
                AddressCountryIso = SelectedItemAddress.CountryCode.Iso;
                AddressCountryName = SelectedItemAddress.CountryCode.Country;
                AddressPhone = SelectedItemAddress.Phone;
                AddressEmail = SelectedItemAddress.Email;
            }
            CloseOverlaySearch();
            ClearAddressListView();
        }
        
        public Address SelectedItemAddress
        {
            get { return _selectedItemAddress; }
            set { _selectedItemAddress = value; OnPropertyChanged(); }
        }

        public void ClearAddressListView()
        {
            CloseOverlaySearch();
            SelectedItemAddress = null;
        }

        #endregion


        #region OrderProperties
        private ObservableCollection<Order> _filteredOrders;
        private int _orderId;

        public int OrderId
        {
            get { return _orderId; }
            set { _orderId = value; OnPropertyChanged(); }
        }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public byte OrderStatus { get; set; }
        public Customer OrderCustomer { get; set; }
        public ObservableCollection<Order_Line> OrderLines { get; set; } = new ObservableCollection<Order_Line>();
        public Address OrderAddress { get; set; }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }

        public ObservableCollection<Order> FilteredOrders
        {
            get { return _filteredOrders; }
            set { _filteredOrders = value; OnPropertyChanged();}
        }

        private void RefreshFilteredOrders()
        {
            var result = Singleton.GetInstance().Orders.Where(o => o.Id.ToString().Contains(OverlaySearchTerm) || o.Customer.Name.Contains(OverlaySearchTerm));
            if (FilteredOrders == null || FilteredOrders.Count != result.Count())
            {
                FilteredOrders = new ObservableCollection<Order>(result);
            }
        }

        #endregion

        #region UnitProperties

        public int UnitId { get; set; }
        public string UnitName { get; set; }

        #endregion

        #region CustomerProperties
        private string _customerName;
        private ObservableCollection<Customer> _filteredCustomers;


        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; OnPropertyChanged(); }
        }

        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Customer> FilteredCustomers
        {
            get { return _filteredCustomers; }
            set { _filteredCustomers = value; OnPropertyChanged(); }
        }

        private void RefreshFilteredCustomers()
        {
            var result = Singleton.GetInstance().Customers.Where(c => c.Name.ToLower().Contains(OverlaySearchTerm));
            if (FilteredCustomers == null || FilteredCustomers.Count != result.Count())
            {
                FilteredCustomers = new ObservableCollection<Customer>(result);
            }
        }

        public Address Address { get; set; }
        private ObservableCollection<Address> _filteredAddresses;

        #region AddressProperties
        private string _addressName;
        private string _address1;
        private string _address2;
        private int _addressPostal;
        private string _addressTown;
        private string _addressCountryIso;
        private string _addressCountryName;
        private string _addressPhone;
        private string _addressEmail;
        public string AddressName
        {
            get { return _addressName; }
            set { _addressName = value; OnPropertyChanged(); }
        }

        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; OnPropertyChanged(); }
        }

        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; OnPropertyChanged(); }
        }

        public int AddressPostal
        {
            get { return _addressPostal; }
            set { _addressPostal = value; OnPropertyChanged(); }
        }

        public string AddressTown
        {
            get { return _addressTown; }
            set { _addressTown = value; OnPropertyChanged(); }
        }

        public string AddressCountryIso
        {
            get { return _addressCountryIso; }
            set { _addressCountryIso = value; OnPropertyChanged(); }
        }

        public string AddressCountryName
        {
            get { return _addressCountryName; }
            set { _addressCountryName = value; OnPropertyChanged(); }
        }

        public string AddressPhone
        {
            get { return _addressPhone; }
            set { _addressPhone = value; OnPropertyChanged(); }
        }

        public string AddressEmail
        {
            get { return _addressEmail; }
            set { _addressEmail = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Address> FilteredAddresses
        {
            get { return _filteredAddresses; }
            set { _filteredAddresses = value; OnPropertyChanged(); }
        }

        private void RefreshFilteredAddresses()
        {
            var result = Singleton.GetInstance().Addresses.Where(a => a.Name.ToLower().Contains(OverlaySearchTerm));
            if (FilteredAddresses == null || FilteredAddresses.Count != result.Count())
            {
                FilteredAddresses = new ObservableCollection<Address>(result);
            }
        }

        #endregion

        #endregion

        #region LocationProperties

        public string LocationId { get; }
        public string LocationBatch { get; set; }
        public Product LocationProduct { get; set; }
        public int LocationAmount { get; set; }
        public Boolean LocationReserved { get; set; }
        public DateTime LocationDate { get; set; }

        #endregion

        #region ProductProperties

        public ICommand AddProductToOrderCommand { get; set; }
        private ObservableCollection<Product> _filteredProducts;
        private int _amount;

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; OnPropertyChanged();}
        }

        public int Amount
        {
            get { return _amount; }
            set { _amount = value; OnPropertyChanged(); }
        }

        public string Batch { get; set; }
        public ObservableCollection<Product> FilteredProducts
        {
            get { return _filteredProducts; }
            set { _filteredProducts = value; OnPropertyChanged();}
        }

        private void RefreshFilteredProducts()
        {
            var result = Singleton.GetInstance().Products.Where(p => p.Name.ToLower().Contains(OverlaySearchTerm));
            if (FilteredProducts == null || FilteredProducts.Count != result.Count())
            {
                FilteredProducts = new ObservableCollection<Product>(result);
            }
        }

        
        #endregion
        public OrderHandler OrderHandler;
        private string _productName;
        private Order_Line _selectedItemOrderLine;


        public ICommand SubmitSaveButtonCommand { get; set; }
        public ICommand SubmitDeleteButtonCommand { get; set; }
        public ICommand SubmitEdit { get; set; }
        public Product SelectedProduct { get; set; }
        public Singleton Singleton { get; set; } = Singleton.GetInstance();
        //Making a reference to the singleton so the Order property can be changed directly in the view.


        public Order_Line SelectedItemOrderLine
        {
            get { return _selectedItemOrderLine; }
            set { _selectedItemOrderLine = value;OnPropertyChanged(); }
        }

        public ICommand RemoveSelectedOrderLineCommand { get; set; }


        public OrderViewModel()
        {
            OrderHandler = new OrderHandler(this);
            Orders = Singleton.GetInstance().Orders;
            CloseOverlaySearchCommand = new RelayCommand(CloseOverlaySearch);
            FilteredOrders = Singleton.GetInstance().Orders;
            FilteredCustomers = Singleton.GetInstance().Customers;
            SetSelectedCustomerCommand = new RelayCommand(SetSelectedCustomer);
            ClearCustomerListViewCommand = new RelayCommand(ClearCustomerListView);

            SetSelectedOrderCommand = new RelayCommand(SetSelectedOrder);
            ClearOrderListViewCommand = new RelayCommand(ClearOrderListView);

            SetSelectedAddressCommand = new RelayCommand(SetSelectedAddress);
            ClearAddressListViewCommand = new RelayCommand(ClearAddressListView);
            RemoveSelectedOrderLineCommand = new RelayCommand(()=>OrderHandler.DeleteOrderLine(SelectedItemOrderLine));
            SetSelectedProductCommand = new RelayCommand(SetSelectedProduct);
            ClearProductListViewCommand = new RelayCommand(ClearProductListView);

            AddProductToOrderCommand = new RelayCommand(OrderHandler.AddOrderLineToOrder);

            SubmitSaveButtonCommand = new RelayCommand(OrderHandler.SaveOrder);
            SubmitDeleteButtonCommand = new RelayCommand(OrderHandler.DeleteOrder);
            if (Singleton.Order.Id!=0) OrderHandler.PageChangeFill();

            IsVisibleOverlaySearch = Visibility.Collapsed;
            IsVisibleSearchAddress = Visibility.Collapsed;
            CloseOverlaySearch();
        }
        #region INotify

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}