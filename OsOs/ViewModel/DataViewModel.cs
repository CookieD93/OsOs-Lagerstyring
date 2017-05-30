using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.PointOfService;
using Windows.Devices.SmartCards;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using OsOs.Annotations;
using OsOs.Handler;
using OsOs.Model;
using OsOs.Utilities;

namespace OsOs.ViewModel
{
    class DataViewModel : INotifyPropertyChanged
    {
        #region ViewVisibilityProps-For-General
        private ICommand _oneDeleteButtonToRuleThemAllCommand;
        public ICommand OneDeleteButtonToRuleThemAllCommand
        {
            get { return _oneDeleteButtonToRuleThemAllCommand; }
            set { _oneDeleteButtonToRuleThemAllCommand = value; OnPropertyChanged();}
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
                RefreshFilteredUnits();
                RefreshFilteredProducts();
                RefreshFilteredCustomers();
                RefreshFilteredLocations();
                RefreshFilteredEmployeeFuntions();
                RefreshFilteredEmployees();
                RefreshFilteredCountries();
            }
        }
        private void CloseOverlaySearch()
        {
            IsVisibleOverlaySearch = Visibility.Collapsed;
            OverlaySearchTerm = "";
            IsVisibleUnitList = Visibility.Collapsed;
            ToggleVisibilityUnitList = false;

            IsVisibleProductList = Visibility.Collapsed;
            ToggleVisibilityProductList = false;

            IsVisibleCustomerList = Visibility.Collapsed;
            ToggleVisibilityCustomerList = false;

            IsVisibleLocationList = Visibility.Collapsed;
            ToggleVisibilityLocationList = false;

            IsVisibleEmployeeFunctionsList = Visibility.Collapsed;
            ToggleVisibilityEmployeeFunctionsList = false;

            IsVisibleEmployeeList = Visibility.Collapsed;
            ToggleVisibilityEmployeeList = false;

            IsVisibleCountryCodeList = Visibility.Collapsed;
            ToggleVisibilityCountryCodeList = false;
        }
        #endregion

        #region ViewVisibilityProps-For-Units
        private Visibility _isVisibleUnitList;
        
        public Visibility IsVisibleUnitList
        {
            get { return _isVisibleUnitList; }
            set { _isVisibleUnitList = value; OnPropertyChanged(); }
        }
        
        private bool _toggleVisibilityUnitList;
        
        private Unit _selectedItemUnit;
        
        public bool ToggleVisibilityUnitList
        {
            get { return _toggleVisibilityUnitList; }
            set
            {
                _toggleVisibilityUnitList = value;
                if (_toggleVisibilityUnitList)
                {
                    OverlaySearchHeader = "Søg enhed";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleUnitList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }

        public ICommand SetSelectedUnitCommand { get; set; }
        

        private void SetSelectedUnit()
        {
            if (SelectedItemUnit != null)
            {
                UnitId = SelectedItemUnit.Id;
                UnitName = SelectedItemUnit.Unit1;
                Unit = SelectedItemUnit;
                OneDeleteButtonToRuleThemAllCommand = new RelayCommand(dataHandler.RemoveUnit);
            }

            CloseOverlaySearch();
            ClearUnitListView();
        }

        public void UnsetSelectedunit()
        {
            UnitId = 0;
            UnitName = null;
        }
        public ICommand ClearUnitListViewCommand { get; set; }
        
        public Unit SelectedItemUnit
        {
            get { return _selectedItemUnit; }
            set { _selectedItemUnit = value; OnPropertyChanged(); }
        }
        public void ClearUnitListView()
        {
            CloseOverlaySearch();
            SelectedItemUnit = null;
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
                ProductId = SelectedItemProduct.Id;
                ProductName = SelectedItemProduct.Name;
                ProductDescription = SelectedItemProduct.Description;
                // UnitName = (SelectedItemProduct.Unit == null) ? null : SelectedItemProduct.Unit.Unit1;
                // --- A ---  --------------- B ----------------   - C -  -------------- D --------------
                // Hvis B == true, så er A = C, else er A = D
                // Nedeunder er Visual Studios forkortede version, som virker, men som jeg ikke forstår /Philip
                UnitName = SelectedItemProduct.Unit?.Unit1;
                UnitId = SelectedItemProduct.Unit.Id;
                ProductBarcode = SelectedItemProduct.Barcode;
                if (!EditingLoaction) OneDeleteButtonToRuleThemAllCommand = new RelayCommand(dataHandler.RemoveProduct);
                SubmitProduct = new RelayCommand(dataHandler.UpdateProduct);
            }
            
            CloseOverlaySearch();
            ClearProductListView();
        }

        public void UnsetSelectedProduct()
        {
            ProductId = 0;
            ProductName = null;
            ProductDescription = null;
            UnitName = null;
            ProductBarcode = null;
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
                Customer = SelectedItemCustomer;
                CustomerAddress = SelectedItemCustomer.Address;
                FKAddressId = SelectedItemCustomer.FK_Address_Id;
                CustomerId = SelectedItemCustomer.Id;
                CustomerName = SelectedItemCustomer.Name;
                Address1 = SelectedItemCustomer.Address.Address1;
                Address2 = SelectedItemCustomer.Address.Address2;
                PostalCode = SelectedItemCustomer.Address.Postal;
                Town = SelectedItemCustomer.Address.Town;
                CountryIso = SelectedItemCustomer.Address.CountryCode.Iso;
                CountryName = SelectedItemCustomer.Address.CountryCode.Country;
                Phone = SelectedItemCustomer.Address.Phone;
                Email = SelectedItemCustomer.Address.Email;
                OneDeleteButtonToRuleThemAllCommand = new RelayCommand(dataHandler.RemoveCustomer);
                SubmitCustomer = new RelayCommand(dataHandler.UpdateCustomer);
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

        #region ViewVisibilityProps-For-CountryCode
        private Visibility _isVisibleCountryCodeList;

        public Visibility IsVisibleCountryCodeList
        {
            get { return _isVisibleCountryCodeList; }
            set { _isVisibleCountryCodeList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityCountryCodeList;

        private CountryCode _selectedItemCountryCode;

        public bool ToggleVisibilityCountryCodeList
        {
            get { return _toggleVisibilityCountryCodeList; }
            set
            {
                _toggleVisibilityCountryCodeList = value;
                if (_toggleVisibilityCountryCodeList)
                {
                    OverlaySearchHeader = "Søg land";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleCountryCodeList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        public ICommand SetSelectedCountryCodeCommand { get; set; }

        private void SetSelectedCountryCode()
        {
            if (SelectedItemCountryCode != null)
            {
                CountryCode = SelectedItemCountryCode;
                CountryId = SelectedItemCountryCode.Id;
                CountryIso = SelectedItemCountryCode.Iso;
                CountryName = SelectedItemCountryCode.Country;
            }
            ClearCountryCodeListView();
            CloseOverlaySearch();
        }
        public ICommand ClearCountryCodeListViewCommand { get; set; }

        public CountryCode SelectedItemCountryCode
        {
            get { return _selectedItemCountryCode; }
            set { _selectedItemCountryCode = value; OnPropertyChanged(); }
        }
        public void ClearCountryCodeListView()
        {
            CloseOverlaySearch();
            SelectedItemCountryCode = null;
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
                LocationAmount = SelectedItemLocation.Quantity;
                if (SelectedItemLocation.Reserved)
                {
                    ToggleVisibilitySearchProduct = true;
                    IsVisibleSearchProduct = Visibility.Visible;
                    ProductName = SelectedItemLocation.Product.Name;
                }
                else if (!SelectedItemLocation.Reserved)
                {
                    ToggleVisibilitySearchProduct = false;
                    IsVisibleSearchProduct = Visibility.Collapsed;
                }
                EditingLoaction = true;
                OneDeleteButtonToRuleThemAllCommand = new RelayCommand(dataHandler.RemoveLocation);
                SubmitLocation = new RelayCommand(dataHandler.UpdateLocation);
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

        private bool _toggleVisibilitySearchProduct;
        public bool ToggleVisibilitySearchProduct
        {
            get { return _toggleVisibilitySearchProduct; }
            set
            {
                _toggleVisibilitySearchProduct = value;
                if (_toggleVisibilitySearchProduct)
                {
                    IsVisibleSearchProduct = Visibility.Visible;
                    LocationReserved = true;
                }
                else
                {
                    IsVisibleSearchProduct = Visibility.Collapsed;
                    UnsetSelectedProduct();
                    ClearProductListView();
                    LocationReserved = false;

                }
                OnPropertyChanged();
            }
        }

        #endregion

        #region ViewVisibilityProps-For-Employee
        private Visibility _isVisibleEmployeeList;
        private Visibility _passwordResetButtonView;
        private Visibility _clearFormView;
        private Visibility _createEmployeeView;
        private Visibility _updateEmployeeView;
        private Visibility _deleteEmployeeView;
        public ICommand ResetPasswordCommand { get; set; }
        public ICommand ClearFormCommand { get; set; }
        public Visibility DeleteEmployeeView
        {
            get { return _deleteEmployeeView; }
            set { _deleteEmployeeView = value; OnPropertyChanged(); }
        }

        public Visibility UpdateEmployeeView
        {
            get { return _updateEmployeeView; }
            set { _updateEmployeeView = value; OnPropertyChanged(); }
        }

        public Visibility CreateEmployeeView
        {
            get { return _createEmployeeView; }
            set { _createEmployeeView = value; OnPropertyChanged(); }
        }
        
        public Visibility ClearFormView
        {
            get { return _clearFormView; }
            set
            {
                if(Username != null || Function != null || EmployeeName != null)
                     _clearFormView = Visibility.Visible;
                else 
                    _clearFormView=Visibility.Collapsed;
                OnPropertyChanged();
            }
        }
        public Visibility PasswordResetButtonView
        {
            get { return _passwordResetButtonView; }
            set { _passwordResetButtonView = value; OnPropertyChanged(); }
        }

        public Visibility IsVisibleEmployeeList
        {
            get { return _isVisibleEmployeeList; }
            set { _isVisibleEmployeeList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityEmployeeList;

        private Employee _selectedItemEmployee;

        public bool ToggleVisibilityEmployeeList
        {
            get { return _toggleVisibilityEmployeeList; }
            set
            {
                _toggleVisibilityEmployeeList = value;
                if (_toggleVisibilityEmployeeList)
                {
                    OverlaySearchHeader = "Søg medarbejder";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleEmployeeList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        public ICommand SetSelectedEmployeeCommand { get; set; }
        private void SetSelectedEmployee()
        {
            if (SelectedItemEmployee != null)
            {
                EmployeeId = SelectedItemEmployee.Id;
                EmployeeName = SelectedItemEmployee.Name;
                Username = SelectedItemEmployee.Username;
                Function = SelectedItemEmployee.Employee_Function.Description;
                Employee_Function = SelectedItemEmployee.Employee_Function;
                Password = "Adgangskoden er krypteret";
                PasswordResetButtonView=Visibility.Visible;
                ClearFormView=Visibility.Visible;
                DeleteEmployeeView=Visibility.Visible;
                CreateEmployeeView=Visibility.Collapsed;
                UpdateEmployeeView=Visibility.Visible;
                LoginCode = SelectedItemEmployee.LoginCode;
                FKFunctionId = SelectedItemEmployee.Employee_Function.Id;
            }
            CloseOverlaySearch();
        }

        public ICommand ClearEmployeeListViewCommand { get; set; }

        public Employee SelectedItemEmployee
        {
            get { return _selectedItemEmployee; }
            set { _selectedItemEmployee = value;
                OnPropertyChanged(); }
        }
        public void ClearEmployeeListView()
        {
            CloseOverlaySearch();
            SelectedItemEmployee = null;
        }
        #endregion

        #region ViewVisibilityProps-For-EmployeeFunctions
        private Visibility _isVisibleEmployeeFunctionsList;

        public Visibility IsVisibleEmployeeFunctionsList
        {
            get { return _isVisibleEmployeeFunctionsList; }
            set { _isVisibleEmployeeFunctionsList = value; OnPropertyChanged(); }
        }

        private bool _toggleVisibilityEmployeeFunctionsList;

        private Employee_Function _selectedItemEmployeeFunctions;

        public bool ToggleVisibilityEmployeeFunctionsList
        {
            get { return _toggleVisibilityEmployeeFunctionsList; }
            set
            {
                _toggleVisibilityEmployeeFunctionsList = value;
                if (_toggleVisibilityEmployeeFunctionsList)
                {
                    OverlaySearchHeader = "Søg funktion";
                    IsVisibleOverlaySearch = Visibility.Visible;
                    IsVisibleEmployeeFunctionsList = Visibility.Visible;
                }
                OnPropertyChanged();
            }
        }
        public ICommand SetSelectedEmployeeFunctionsCommand { get; set; }

        private void SetSelectedEmployeeFunctions()
        {
            if (SelectedItemEmployeeFunctions != null)
            {
                Function = SelectedItemEmployeeFunctions.Description;
                Employee_Function = SelectedItemEmployeeFunctions;
                FKFunctionId = SelectedItemEmployeeFunctions.Id;
            }
            CloseOverlaySearch();
        }
        public ICommand ClearEmployeeFunctionsListViewCommand { get; set; }

        public Employee_Function SelectedItemEmployeeFunctions
        {
            get { return _selectedItemEmployeeFunctions; }
            set { _selectedItemEmployeeFunctions = value; OnPropertyChanged(); }
        }
        public void ClearEmployeeFunctionsListView()
        {
            CloseOverlaySearch();
            SelectedItemEmployeeFunctions = null;
        }
        #endregion

        #region Product Properties
        private string _productName;
        private string _productBarcode;
        private string _productDescription;
        private Unit _productUnit;
        private ObservableCollection<Product> _filteredProducts;
        public int ProductId { get; set; }
        public Product SelectedProduct { get; set; }
        public CountryCode CountryCode { get; set; }

        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; OnPropertyChanged(); }
        }

        public string ProductBarcode
        {
            get { return _productBarcode; }
            set { _productBarcode = value; OnPropertyChanged(); }
        }

        public string ProductDescription
        {
            get { return _productDescription; }
            set { _productDescription = value; OnPropertyChanged();}
        }

        public Unit ProductUnit
        {
            get { return _productUnit; }
            set { _productUnit = value; OnPropertyChanged();}
        }

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
        
        #region Employee Properties
        public int EmployeeId { get; set; }
        private int _unitId;
        private string _unitName;
        private string _employeeName;
        private string _username;
        private string _function;
        private string _password;
        private int _loginCode;

        public int? FKFunctionId
        {
            get { return _fkFunctionId; }
            set { _fkFunctionId = value; OnPropertyChanged(); }
        }


        public string EmployeeName
        {
            get { return _employeeName; }
            set { _employeeName = value;
                    ClearFormView =Visibility.Visible;
                    OnPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value;
                    OnPropertyChanged();
                    GenerateQuickCode();
                    ClearFormView = Visibility.Visible;
            }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }


        public int? LoginCode
        {
            get { return _loginCode; }
            set { _loginCode = value ?? default(int); OnPropertyChanged(); }
        }

        public string Function
        {
            get { return _function; }
            set { _function = value;
                    OnPropertyChanged();
                    ClearFormView = Visibility.Visible;
            }
        }

        public Employee_Function Employee_Function
        {
            get { return _employeeFunction; }
            set { _employeeFunction = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Employee> Employees { get; set; }
        private ObservableCollection<Employee> _filteredEmployees;
        public ObservableCollection<Employee> FilteredEmployees
        {
            get { return _filteredEmployees; }
            set { _filteredEmployees = value; OnPropertyChanged();}
        }

        private void RefreshFilteredEmployees()
        {
            var result = Singleton.GetInstance().Employees.Where(e => e.Username.ToLower().Contains(OverlaySearchTerm));
            if (FilteredEmployees == null || FilteredEmployees.Count != result.Count())
            {
                FilteredEmployees = new ObservableCollection<Employee>(result);
            }
        }
        private ObservableCollection<Employee_Function> _filteredEmployeeFunctions;

        public ObservableCollection<Employee_Function> FilteredEmployeeFunctions
        {
            get { return _filteredEmployeeFunctions; }
            set { _filteredEmployeeFunctions = value; OnPropertyChanged(); }
        }

        private void RefreshFilteredEmployeeFuntions()
        {
            var result =
                Singleton.GetInstance()
                    .EmployeeFunctions.Where(ef => ef.Description.ToLower().Contains(OverlaySearchTerm));
            if (FilteredEmployeeFunctions == null || FilteredEmployeeFunctions.Count != result.Count())
            {
                FilteredEmployeeFunctions = new ObservableCollection<Employee_Function>(result);
            }
        }

        private void GenerateQuickCode()
        {
            if (Username != null)
                LoginCode = dataHandler.GetLoginCode(Username, Password);
            else
                LoginCode = null;
        }

        #endregion

        #region Customer Properties
        public Address CustomerAddress { get; set; }
        public Customer Customer { get; set; }
        private ObservableCollection<Customer> _filteredCustomers;
        private ObservableCollection<CountryCode> _filteredCountries;

        private int _customerId;
        private string _customerName;
        private string _address1;
        private string _address2;
        private int _postalCode;
        private string _town;
        private string _city;
        private int _countryId;
        private string _countryIso;
        private string _countryName;
        private string _phone;
        private string _email;
        private int _fkAddressId;
        
        public int FKAddressId
        {
            get { return _fkAddressId; }
            set { _fkAddressId = value; OnPropertyChanged();}
        }

        public int CustomerId
        {
            get { return _customerId; }
            set { _customerId = value; OnPropertyChanged();}
        }

        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; OnPropertyChanged(); }
        }
        public string Address1
        {
            get { return _address1; }
            set { _address1 = value; OnPropertyChanged();}
        }

        public string Address2
        {
            get { return _address2; }
            set { _address2 = value; OnPropertyChanged(); }
        }

        public int PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; OnPropertyChanged(); }
        }

        public string Town
        {
            get { return _town; }
            set { _town = value; OnPropertyChanged(); }
        }

        public string City
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(); }
        }

        public int CountryId
        {
            get { return _countryId; }
            set { _countryId = value; OnPropertyChanged(); }
        }

        public string CountryIso
        {
            get { return _countryIso; }
            set { _countryIso = value; OnPropertyChanged(); }
        }

        public string CountryName
        {
            get { return _countryName; }
            set { _countryName = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(); }
        }
        public ObservableCollection<CountryCode> FilteredCountries
        {
            get { return _filteredCountries; }
            set { _filteredCountries = value; OnPropertyChanged();}
        }

        private void RefreshFilteredCountries()
        {
            var result =
                Singleton.GetInstance().CountryCodes.Where(
                    cc => cc.Country.ToLower().Contains(OverlaySearchTerm) ||
                    cc.Iso.ToLower().Contains(OverlaySearchTerm));
            
            if (FilteredCountries == null || FilteredCountries.Count != result.Count())
            {
                FilteredCountries = new ObservableCollection<CountryCode>(result);
            }
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
        #endregion

        #region Location Properties
        private ObservableCollection<Product_Location> _filteredLocations;
        private string _locationId;
        public string LocationId
        {
            get { return _locationId; }
            set { _locationId = value; OnPropertyChanged(); }
        }

        public bool EditingLoaction { get; set; }
        public string LocationBatchNo { get; set; }
        public int? LocationAmount { get; set; }
        public Boolean LocationReserved { get; set; }
        public DateTime LocationDateTime { get; set; }
        public Product LocationProduct { get; set; }
        public int Row { get; set; }
        public int StartSpace { get; set; }
        public int EndSpace { get; set; }
        public char StartHeight { get; set; }
        public char EndHeight { get; set; }
        public ObservableCollection<Product_Location> FilteredLocations
        {
            get { return _filteredLocations; }
            set { _filteredLocations = value; OnPropertyChanged(); }
        }

        public void RefreshFilteredLocations()
        {
            var result = Singleton.GetInstance().Locations.Where(l => l.Location.ToLower().Contains(OverlaySearchTerm));
            if (FilteredLocations == null || FilteredLocations.Count != result.Count())
            {
                FilteredLocations = new ObservableCollection<Product_Location>(result);
            }
        }

        #endregion

        #region Unit Properties
        private ObservableCollection<Unit> _filteredUnits;
        private ICommand _submitProduct;
        private Employee_Function _employeeFunction;
        private ICommand _submitCustomer;
        private int? _fkFunctionId;
        public Unit Unit { get; set; }

        public int UnitId
        {
            get { return _unitId; }
            set { _unitId = value; OnPropertyChanged(); }
        }
        public string UnitName
        {
            get { return _unitName; }
            set { _unitName = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Unit> FilteredUnits
        {
            get { return _filteredUnits; }
            set { _filteredUnits = value; OnPropertyChanged();}
        }
        private void RefreshFilteredUnits()
        {
            var result = Singleton.GetInstance().Units.Where(u => u.Unit1.ToLower().Contains(OverlaySearchTerm));
            if (FilteredUnits == null || FilteredUnits.Count != result.Count())
            {
                FilteredUnits = new ObservableCollection<Unit>(result);
            }
        }
        #endregion

        public DataHandler dataHandler { get; set; }

        public ICommand SubmitProduct
        {
            get { return _submitProduct; }
            set { _submitProduct = value; OnPropertyChanged(); }
        }
        private ICommand _submitLocation;
        public ICommand SubmitEmployee { get; set; }
        public ICommand RemoveEmployee { get; set; }
        public ICommand SubmitUnit { get; set; }
        public ICommand UpdateEmplCommand { get; set; } 
        public ICommand SubmitCustomer
        {
            get { return _submitCustomer; }
            set { _submitCustomer = value; OnPropertyChanged(); }
        }

        public ICommand SubmitLocation
        {
            get { return _submitLocation; }
            set { _submitLocation = value; OnPropertyChanged(); }
        }
        public ICommand SubmitGenerateLocations { get; set; }

        public DataViewModel()
        {
            dataHandler = new DataHandler(this);
            SubmitProduct = new RelayCommand(dataHandler.AddProduct);
            SubmitEmployee = new RelayCommand(dataHandler.AddUser);
            RemoveEmployee=new RelayCommand(dataHandler.RemoveUser);
            SubmitUnit = new RelayCommand(dataHandler.AddUnit);
            SubmitCustomer = new RelayCommand(dataHandler.AddCustomer);
            SubmitGenerateLocations = new RelayCommand(dataHandler.AddLocation);
            ResetPasswordCommand = new RelayCommand(dataHandler.ResetPassword);
            ClearFormCommand = new RelayCommand(dataHandler.ClearEmployeeValues);
            UpdateEmplCommand = new RelayCommand(dataHandler.UpdateUser);
            FilteredUnits = Singleton.GetInstance().Units;
            FilteredProducts = Singleton.GetInstance().Products;
            FilteredCustomers = Singleton.GetInstance().Customers;
            FilteredCountries = Singleton.GetInstance().CountryCodes;
            FilteredLocations = Singleton.GetInstance().Locations;
            FilteredEmployees = Singleton.GetInstance().Employees;
            FilteredEmployeeFunctions = Singleton.GetInstance().EmployeeFunctions;
            
            CloseOverlaySearchCommand = new RelayCommand(CloseOverlaySearch);

            SetSelectedUnitCommand = new RelayCommand(SetSelectedUnit);
            ClearUnitListViewCommand = new RelayCommand(ClearUnitListView);

            SetSelectedProductCommand = new RelayCommand(SetSelectedProduct);
            ClearProductListViewCommand = new RelayCommand(ClearProductListView);

            SetSelectedCustomerCommand = new RelayCommand(SetSelectedCustomer);
            ClearCustomerListViewCommand = new RelayCommand(ClearCustomerListView);

            SetSelectedCountryCodeCommand = new RelayCommand(SetSelectedCountryCode);
            ClearCountryCodeListViewCommand = new RelayCommand(ClearCountryCodeListView);

            SetSelectedLocationCommand = new RelayCommand(SetSelectedLocation);
            ClearLocationListViewCommand = new RelayCommand(ClearLocationListView);
           
            SetSelectedEmployeeCommand = new RelayCommand(SetSelectedEmployee);
            ClearEmployeeListViewCommand = new RelayCommand(ClearProductListView);

            SetSelectedEmployeeFunctionsCommand = new RelayCommand(SetSelectedEmployeeFunctions);
            ClearEmployeeFunctionsListViewCommand = new RelayCommand(ClearEmployeeFunctionsListView);
            IsVisibleOverlaySearch = Visibility.Collapsed;
            IsVisibleSearchProduct = Visibility.Collapsed;
            CloseOverlaySearch();

            Password = dataHandler.RandomString(6);
            PasswordResetButtonView = Visibility.Collapsed;
            ClearFormView = Visibility.Collapsed;
            CreateEmployeeView=Visibility.Visible;
            UpdateEmployeeView=Visibility.Collapsed;
            DeleteEmployeeView = Visibility.Collapsed;
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
