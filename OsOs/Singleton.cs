using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsOs.Model;

namespace OsOs
{
    class Singleton
    {
        private static Singleton _instance = new Singleton();

        #region Collections

        //collections kører vi i Eager, da det kun er en lille database vi arbejder i
        //Vi er klar over at det kan være en dårlig idé at køre Eager hvis man har 
        //millioner af posts i sin database.
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product_Location> Locations { get; set; }
        public ObservableCollection<Customer> Customers { get; set; }
        public ObservableCollection<Employee> Employees { get; set; }
        public ObservableCollection<Address> Addresses { get; set; }
        public ObservableCollection<Log> Logs { get; set; }
        public ObservableCollection<Unit> Units { get; set; }
        public ObservableCollection<Order_Line> OrderLines { get; set; }
        public ObservableCollection<CountryCode> CountryCodes { get; set; }
        public ObservableCollection<Employee_Function> EmployeeFunctions { get; set; }

        #endregion

        private Singleton()
        {
        }

        public static Singleton GetInstance()
        {
            return _instance;
        }

        // Get/Set employee ID and Function
        public int EmployeeId { get; set; }
        public int EmployeeFunction { get; set; }

        public Order Order { get; set ; } = new Order(DateTime.MinValue,0,null,null,null) {Id = 0};
        public bool Editing { get; set; }

        public async Task FetchCollections()
        {
            //await LoadEmployeesAndFunctions();
            LoadAddressCustomersOrders();
            LoadUnitsProductsLocations();
            LoadOrderLinesCountryCodes();
            Logs = await PersistencyService.GetFromDatabase<Log>("Logs");
            Task[] tasks = new Task[]
                {LoadAddressCustomersOrders(), LoadUnitsProductsLocations(), LoadOrderLinesCountryCodes()};
            Task.WaitAll(tasks);
             ChainTogether();
        }

        public async Task LoadEmployeesAndFunctions()
        {
            Employees = await PersistencyService.GetFromDatabase<Employee>("Employees");
            EmployeeFunctions = await PersistencyService.GetFromDatabase<Employee_Function>("Employee_function");
            ChainEmp();
        }
        public async Task LoadAddressCustomersOrders()
        {
            Addresses = await PersistencyService.GetFromDatabase<Address>("Addresses");
            Customers = await PersistencyService.GetFromDatabase<Customer>("customers");
            Orders = await PersistencyService.GetFromDatabase<Order>("orders");
        }

        public async Task LoadUnitsProductsLocations()
        {
            Units = await PersistencyService.GetFromDatabase<Unit>("Units");
            Products = await PersistencyService.GetFromDatabase<Product>("products");
            Locations = await PersistencyService.GetFromDatabase<Product_Location>("Product_Location");
        }

        public async Task LoadOrderLinesCountryCodes()
        {
            OrderLines = await PersistencyService.GetFromDatabase<Order_Line>("Order_Line");
            CountryCodes = await PersistencyService.GetFromDatabase<CountryCode>("CountryCodes");
        }

        public void ChainTogether()
        {
            #region CountryCode -> Address

            foreach (Address address in Addresses)
            {
                foreach (CountryCode countryCode in CountryCodes)
                {
                    if (address.FK_Country_Id == countryCode.Id)
                    {
                        address.CountryCode = countryCode;
                    }
                }
            }

            #endregion
            #region Address -> Customer && Address -> Order

            //foreach (Customer customer in Customers)
            //{
            //    foreach (Address address in Addresses)
            //    {
            //        if (customer.FK_Address_Id == address.Id)
            //        {
            //            customer.Address = address;
            //        }
            //    }
            //}
            foreach (Address address in Addresses)
            {
                foreach (Customer customer in Customers)
                {
                    if (customer.FK_Address_Id==address.Id)
                    {
                        customer.Address = address;
                    }
                }
                foreach (Order order in Orders)
                {
                    if (order.FK_Address_Id==address.Id)
                    {
                        order.Address = address;
                    }
                }
            }
            #endregion
            #region Unit -> Product && Product -> Location && Product -> Order Line

            foreach (Product product in Products)
            {
                foreach (Unit unit in Units)
                {
                    if (product.FK_Unit==unit.Id)
                    {
                        product.Unit = unit;
                    }
                }
                foreach (Product_Location productLocation in Locations)
                {
                    if (productLocation.FK_Product_Id==product.Id)
                    {
                        productLocation.Product = product;
                    }
                }
                foreach (Order_Line orderLine in OrderLines)
                {
                    if (product.Id==orderLine.FK_Product_Id)
                    {
                        orderLine.Product = product;
                    }
                }
            }
            #endregion
            //#region Emp_funct -> Employee
            //ChainEmp();
           
            //#endregion
            #region Customer -> Order && Employee -> Order && Order Line -> Order

            foreach (Order order in Orders)
            {
                foreach (Customer customer in Customers)
                {
                    if (order.FK_Customer_Id== customer.Id)
                    {
                        order.Customer = customer;
                    }
                }
                foreach (Employee employee in Employees)
                {
                    if (order.FK_Employee_Id==employee.Id)
                    {
                        order.Employee = employee;
                    }
                }
                foreach (Order_Line orderLine in OrderLines)
                {
                    if (orderLine.FK_Order_Id==order.Id)
                    {
                        order.Order_Line.Add(orderLine);
                    }
                }
            }
            #endregion
            #region Order Populations
            #region Gammel Order foreach virker ikke med ting som ikke blive brugt
            //foreach (Order order in Orders)
            //{
            //    foreach (Employee employee in Employees)
            //    {
            //        if (order.FK_Employee_Id == employee.Id)
            //        {
            //            foreach (Employee_Function employeeFunction in EmployeeFunctions)
            //            {
            //                if (employee.FK_Employee_funct == employeeFunction.Id)
            //                {
            //                    employee.Employee_Function = employeeFunction;
            //                        // tilføjer den rette function til matchende employee
            //                }
            //            }
            //            order.Employee = employee; // tilføjer den rette employee til ordren
            //        }
            //    }
            //    foreach (Customer customer in Customers)
            //    {
            //        if (order.FK_Customer_Id == customer.Id)
            //        {
            //            foreach (Address address in Addresses)
            //            {
            //                if (customer.FK_Address_Id == address.Id)
            //                {
            //                    foreach (CountryCode countryCode in CountryCodes)
            //                    {
            //                        if (address.FK_Country_Id == countryCode.Id)
            //                        {
            //                            address.CountryCode = countryCode;
            //                                // tilføjer den matchende countryCode til den rigtige adresse
            //                        }
            //                    }
            //                    customer.Address = address; // tilføjer adresse til den matchende customer i customers
            //                    order.Address = address; // tilføjer adresse til den matchende ordre i orders
            //                }
            //            }
            //            order.Customer = customer; // tilføjer den matchende customer til den rette ordre
            //        }
            //    }
            //    foreach (Order_Line orderLine in OrderLines)
            //    {
            //        if (order.Id == orderLine.FK_Order_Id)
            //        {
            //            foreach (Product_Location productLocations in Locations)
            //            {
            //                foreach (Product product in Products)
            //                {
            //                    if (orderLine.FK_Product_Id == product.Id)
            //                    {
            //                        foreach (Unit unit in Units)
            //                        {
            //                            if (product.FK_Unit == unit.Id)
            //                            {
            //                                product.Unit = unit; //tilføjer Unit til produkter
            //                            }
            //                        }

            //                        orderLine.Product = product; // tilføjer produkter til ordre linjer
            //                    }
            //                    if (productLocations.FK_Product_Id == product.Id)
            //                    {
            //                        productLocations.Product = product;
            //                    }
            //                }
            //            }
            //            order.Order_Line.Add(orderLine); // tilføjer ordre linjer til ordre
            //        }
            //    }
            //} 
            #endregion

            #endregion
        }

        public void ChainEmp()
        {
            foreach (Employee employee in Employees)
            {
                foreach (Employee_Function employeeFunction in EmployeeFunctions)
                {
                    if (employee.FK_Employee_funct == employeeFunction.Id)
                    {
                        employee.Employee_Function = employeeFunction;
                    }
                }
            }
        }
    }
}