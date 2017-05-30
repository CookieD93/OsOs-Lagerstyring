using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using OsOs.Model;
using OsOs.Utilities;
using OsOs.ViewModel;


namespace OsOs.Handler
{
    class DataHandler
    {
        #region Product Region

        //Database handling for Products
        //used for adding a new product to the Database

        public async void AddProduct()
        {
            bool productExists = false;
            Product product = new Product(ViewModel.ProductName, ViewModel.ProductBarcode, ViewModel.ProductDescription,
                null)
            { FK_Unit = ViewModel.UnitId };
            foreach (Product pro in Singleton.GetInstance().Products)
            {
                if (pro.Name == product.Name)
                {
                    productExists = true;
                    break;
                }
            }
            if (!productExists)
            {
                Product productd = await PersistencyService.PostToDatabase(product, "Products");
                productd.Unit = ViewModel.Unit;
                Singleton.GetInstance().Products.Add(productd);
                ViewModel.UnsetSelectedProduct();
            }
            else
            {
                MessageDialogHelper.Show("Produktet eksisterer allerede.", "Fejl i tilføjelse af produkt.");
            }
        }

        //used for removing a specific product from the Database

        #region Remove Product Function

        public async void RemoveProduct()
        {
            // Create the message dialog and set its content
            var messageDialog =
                new MessageDialog("Er du sikker på at du vil slette Produktet: " + ViewModel.SelectedProduct.Name +
                                  "?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Ja",
                new UICommandInvokedHandler(this.CommandInvokedHandlerProduct)));
            messageDialog.Commands.Add(new UICommand("Nej", null));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void CommandInvokedHandlerProduct(IUICommand command)
        {
            
            await PersistencyService.DeleteFromDatabase("products", $"{ViewModel.SelectedProduct.Id}");
            Singleton.GetInstance().Products.Remove(ViewModel.SelectedProduct);
            ViewModel.UnsetSelectedProduct();
        }

        #endregion

        //used for making changes to an already existing product
        public async void UpdateProduct()
        {
            Product product = new Product(ViewModel.ProductName, ViewModel.ProductBarcode, ViewModel.ProductDescription,
                null) {FK_Unit = ViewModel.UnitId ,Id = ViewModel.ProductId};
            Product tempProduct = Singleton.GetInstance().Products.First(x => x.Id == product.Id);
            if (!tempProduct.Equals(product))
            {
                int i = Singleton.GetInstance().Products.IndexOf(tempProduct);
                await PersistencyService.PutToDatabase(product, "products", $"{product.Id}",false);
                Singleton.GetInstance().Products[i] = product;
                MessageDialogHelper.Show("Produktet blev opdateres", "Succes");
            }
            else if (tempProduct.Equals(product))
            {
                MessageDialogHelper.Show("Ingen ændringer blev fortaget", "Fejl");
            }
        }

        //used for getting the entire database down into a collection // LAZY
        public async void ReadProducts()
        {
            //            V= await PersistencyService.GetFromDatabase<Product>("products");
        }

        #endregion

        //Database handling for users3
        //used for adding a new user to the Database
        public async void AddUser()
        {

            if (Singleton.GetInstance().Employees.All(x => x.Username.ToLower() == ViewModel.Username.ToLower()))
            {
                MessageDialogHelper.Show("Brugernavnet eksistere allerede","Fejl");
                return;
            }
            if (ViewModel.Username == null || ViewModel.EmployeeName == null || ViewModel.Employee_Function == null)
            {
                MessageDialogHelper.Show("Udfyld venligst alle felter","Fejl");
                return;
            }

            int logincode = ViewModel.LoginCode ?? default(int);
            string password = Crypt.ConvertData(ViewModel.Password);

            Employee emp = new Employee(null, ViewModel.EmployeeName, ViewModel.Username, password, logincode)
                {FK_Employee_funct = ViewModel.FKFunctionId};
            await PersistencyService.PostToDatabase(emp, "employees", false);
            
            emp = new Employee(ViewModel.Employee_Function, ViewModel.EmployeeName, ViewModel.Username, "", logincode);
            Singleton.GetInstance().Employees.Add(emp);

            MessageDialogHelper.Show("Brugeren blev tilføjet - Husk at noter koden ned til din kollega!","Kode: "+ViewModel.Password);

            ViewModel.ClearEmployeeFunctionsListView();
            ClearEmployeeValues();
            password = null;
        }

        // Generate the Quick-Login code
        public int GetLoginCode(string username, string password)
        {
            string x = Crypt.ConvertData(username + password);
            //if (x.GetHashCode() < 0)
            //    return x.GetHashCode() * -1;
            return x.GetHashCode();
        }

        //used for removing a specific user from the Database

        #region RemoveUser Function

        public async void RemoveUser()
        {
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Er du sikker på, at du vil slette denne bruger: " + ViewModel.EmployeeName + "?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Slet", new UICommandInvokedHandler(this.CommandInvokedHandlerUser)));
            messageDialog.Commands.Add(new UICommand("Annuller", null));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void CommandInvokedHandlerUser(IUICommand command)
        {
            await PersistencyService.DeleteFromDatabase("employees", $"{ViewModel.EmployeeId}");
            Singleton.GetInstance().Employees.Remove(ViewModel.SelectedItemEmployee);
            ViewModel.PasswordResetButtonView = Visibility.Collapsed;
            ViewModel.ClearFormView = Visibility.Collapsed;
            ClearEmployeeValues();
        }

        #endregion

        //used for making changes to an aleady existing user
        public async void UpdateUser()
        {
            Employee employee = ViewModel.SelectedItemEmployee;
            Employee tempEmpolyee = new Employee(ViewModel.Employee_Function, ViewModel.EmployeeName, ViewModel.Username, employee.Password, ViewModel.LoginCode??default(int));

            if (!employee.Equals(tempEmpolyee))
            {

                employee.Name = ViewModel.EmployeeName;
                employee.Username = ViewModel.Username;
                employee.LoginCode = ViewModel.LoginCode ?? default(int);

                int i = Singleton.GetInstance().Employees.IndexOf(employee);
                Singleton.GetInstance().Employees[i] = employee;

                tempEmpolyee.FK_Employee_funct = employee.Employee_Function.Id;
                tempEmpolyee.Employee_Function = null;
                tempEmpolyee.Id = employee.Id;

                var task = PersistencyService.PutToDatabase(tempEmpolyee, "employees", $"{ViewModel.EmployeeId}", false);
                await task;
                MessageDialogHelper.Show("Medarbejderen blev opdateret", "Succes");
            }
            else 
            {
                MessageDialogHelper.Show("Ingen ændringer blev foretaget", "Fejl");
            }
        }

        // Reset Password for Employee
        public async void ResetPassword()
        {
            string newPassword = RandomString(6);
            Employee emp = ViewModel.SelectedItemEmployee;
            Employee tempEmp = new Employee(null, emp.Name,emp.Username,Crypt.ConvertData(newPassword),emp.LoginCode);

            int i = Singleton.GetInstance().Employees.IndexOf(emp);
            emp.Password = Crypt.ConvertData(newPassword);
            Singleton.GetInstance().Employees[i] = emp;

            tempEmp.Id = emp.Id;
            tempEmp.FK_Employee_funct = emp.Employee_Function.Id;

            var task = PersistencyService.PutToDatabase(tempEmp, "employees", $"{ViewModel.EmployeeId}", false);
            await task;

            if(task.IsCompleted)
                MessageDialogHelper.Show("Noter den nye adgangskode ned til din kollega", "Ny kode: "+ newPassword);
            else   
                MessageDialogHelper.Show("Noget gik galt - Ingen værdier blev ændret","Fejl");

        }

        public void ClearEmployeeValues()
        {
            ViewModel.Username = null;
            ViewModel.EmployeeName = null;
            ViewModel.Function = null;
            ViewModel.SelectedItemEmployee = null;
            ViewModel.FKFunctionId = null;
            ViewModel.Employee_Function = null;
            ViewModel.LoginCode = null;
            ViewModel.Password = RandomString(6);
            ViewModel.EmployeeId = -1;
            ViewModel.UpdateEmployeeView = Visibility.Collapsed;
            ViewModel.CreateEmployeeView = Visibility.Visible;
            ViewModel.ClearFormView = Visibility.Collapsed;
            ViewModel.DeleteEmployeeView = Visibility.Collapsed;
            ViewModel.PasswordResetButtonView = Visibility.Collapsed;
            ViewModel.ClearEmployeeFunctionsListView();
        }

        //used for getting the entire database down into a collection
        public async void ReadUsers()
        {
            //            V = await PersistencyService.GetFromDatabase<Empolyee>("employees");
        }

        //Database handling for Locations
        //used for adding a new location to the Database
        public async void AddLocation()
        {
            // Use foreach loop to generate locations
            //            await PersistencyService.PostToDatabase(new Location(), "pruduct_location");
            int RowNumber = ViewModel.Row;
            string RowString = "";
            string SpaceString = "";
            if (RowNumber <= 9)
            {
                RowString = "0" + RowNumber;
            }
            else
            {
                RowString = $"{RowNumber}";
            }
            int[] startEnd = AlphabetCounter.FindStartEndValue(Char.ToUpper(ViewModel.StartHeight), Char.ToUpper(ViewModel.EndHeight));
            char[] charArray = AlphabetCounter.LettersToCreate(Char.ToUpper(ViewModel.StartHeight), Char.ToUpper(ViewModel.EndHeight));
            for (int i = ViewModel.StartSpace; i <= ViewModel.EndSpace; i++)
            {
                if (i <= 9)
                {
                    SpaceString = "0" + i;
                }
                else
                {
                    SpaceString = $"{i}";
                }
                for (int j = startEnd[0]; j <= startEnd[1]; j++)
                {

                    Product_Location location = new Product_Location($"{RowString}{SpaceString}{charArray[j]}", false,
                        null);
                    Singleton.GetInstance().Locations.Add(location);
                    Product_Location resultLocation = await PersistencyService.PostToDatabase(location, "Product_Location", false);
                    if (j == startEnd[1] && i == ViewModel.EndSpace)
                    {
                        if (resultLocation != null)
                        {
                            MessageDialogHelper.Show("Lokation(er) oprettet!", "Succes");
                        }
                        else
                        {
                            MessageDialogHelper.Show("Der Skete en fejl", "Fejl!");
                        }
                    }
                }
            }
        }

        //used for removing a specific location from the Database
        public async void RemoveLocation()
        {
            if (ViewModel.LocationAmount == 0)
            {
                // Create the message dialog and set its content
                var messageDialog =
                    new MessageDialog("Er du sikker på at du vil slette Lokationen : " + ViewModel.LocationId +
                                      "?");

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                messageDialog.Commands.Add(new UICommand("Ja", new UICommandInvokedHandler(this.CommandInvokedHandlerLocation)));
                messageDialog.Commands.Add(new UICommand("Nej", null));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }
            else
            {
                MessageDialogHelper.Show(
                    $"Kan ikke slette den valgte lokation da der ligger {ViewModel.LocationAmount} {ViewModel.LocationProduct} på lokationen,\n fjern dem fra lokationen og prøv igen",
                    "Fejl ved Sletning af Lokation");
            }
        }

        private async void CommandInvokedHandlerLocation(IUICommand command)
        {
            await PersistencyService.DeleteFromDatabase("Product_location", $"{ViewModel.LocationId}");
            Singleton.GetInstance().Locations.Remove(Singleton.GetInstance().Locations.First(x => x.Location == ViewModel.LocationId));

        }

        //used for making changes to an aleady existing location or reserving a location
        public async void UpdateLocation()
        {

            if (ViewModel.LocationAmount==0)
            {
                int? nullint = ViewModel.ProductId;
                if (!ViewModel.LocationReserved) nullint = null;

                Product_Location location = new Product_Location(ViewModel.LocationId, ViewModel.LocationReserved, null) {Product = ViewModel.SelectedProduct,FK_Product_Id = nullint };
                Product_Location tempLocation = Singleton.GetInstance()
                    .Locations.First(x => x.Location == location.Location);
                if (!tempLocation.Equals(location))
                {
                    int i = Singleton.GetInstance().Locations.IndexOf(tempLocation);
                    Singleton.GetInstance().Locations[i] = location;
                    location.Product = null;
                    await PersistencyService.PutToDatabase(location, "Product_location", $"{location.Location}");
                    //MessageDialogHelper.Show("Lokationen blev opdateret", "Succes");
                }
                else if (tempLocation.Equals(location))
                {
                    MessageDialogHelper.Show("Ingen ændringer blev fortaget", "Fejl");
                }
            }
            else
            {
                MessageDialogHelper.Show("Kan ikke opdatere lokation da der ligger et produkt", "Fejl i opdatering");
            }
        }

        //used for getting the entire database down into a collection
        public async void ReadLocations()
        {
            //            V = await PersistencyService.GetFromDatabase<Location>("Product_location");
        }

        //Database handling for Customer
        //used for adding a new product to the Database
        public async void AddCustomer()
        {
            //            await PersistencyService.PostToDatabase(new Customer(),"customers" );
            bool customerExists = false;
            Address address = new Address(ViewModel.CustomerName, ViewModel.Address1, ViewModel.Address2,
                ViewModel.PostalCode, ViewModel.Town, ViewModel.CountryCode, ViewModel.Phone, ViewModel.Email)
            {
                FK_Country_Id = ViewModel.CountryId
            };
            Customer customer = new Customer(address, ViewModel.CustomerName);
            foreach (Customer cust in Singleton.GetInstance().Customers)
            {
                if (cust.Name == customer.Name)
                {
                    customerExists = true;
                    break;
                }
            }
            if (!customerExists)
            {
               Address addressToUpload = new Address(ViewModel.CustomerName, ViewModel.Address1, ViewModel.Address2,
                ViewModel.PostalCode, ViewModel.Town,null, ViewModel.Phone, ViewModel.Email)
                {
                    FK_Country_Id = ViewModel.CountryId
                };
                addressToUpload = await PersistencyService.PostToDatabase(addressToUpload, "addresses");
                address.Id = addressToUpload.Id;
                customer.FK_Address_Id = addressToUpload.Id;
                Singleton.GetInstance().Customers.Add(customer);
                Singleton.GetInstance().Addresses.Add(address);
                Customer customerToUpload = new Customer(null,customer.Name) {FK_Address_Id = addressToUpload.Id};
               int i = Singleton.GetInstance().Customers.IndexOf(customer);
                customerToUpload =  await PersistencyService.PostToDatabase(customerToUpload, "customers");
                Singleton.GetInstance().Customers[i].Id = customerToUpload.Id;
            }
            else
            {
                MessageDialogHelper.Show("Kunden eksistere allerede", "Fejl i oprettelse af Kunden");
            }
        }

        //used for removing a specific product from the Database

        #region Remove Customer Function

        public async void RemoveCustomer()
        {
            // Create the message dialog and set its content
            var messageDialog =
                new MessageDialog("Er du sikker på du vil slette Kunden: " + ViewModel.Customer.Name +
                                  "?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Ja",
                new UICommandInvokedHandler(this.CommandInvokedHandlerCustomer)));
            messageDialog.Commands.Add(new UICommand("Nej", null));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void CommandInvokedHandlerCustomer(IUICommand command)
        {
            await PersistencyService.DeleteFromDatabase("Customers", $"{ViewModel.Customer.Id}");
            await PersistencyService.DeleteFromDatabase("addresses", $"{ViewModel.Customer.FK_Address_Id}",false);
            Singleton.GetInstance().Customers.Remove(ViewModel.Customer);
            Singleton.GetInstance().Addresses.Remove(ViewModel.CustomerAddress);
        }

        #endregion

        //used for making changes to an already existing Customer
        public async void UpdateCustomer()
        {
            Address address = new Address(ViewModel.CustomerName, ViewModel.Address1, ViewModel.Address2,
                ViewModel.PostalCode, ViewModel.Town,ViewModel.CountryCode, ViewModel.Phone,
                ViewModel.Email) {FK_Country_Id = ViewModel.CountryId, Id = ViewModel.FKAddressId};
            Customer customer = new Customer(address, ViewModel.CustomerName) {FK_Address_Id = ViewModel.FKAddressId, Id = ViewModel.CustomerId};
            Customer tempCustomer = Singleton.GetInstance().Customers.First(x => x.Id == customer.Id);
            if (!tempCustomer.Equals(customer))
            {
                int i = Singleton.GetInstance().Customers.IndexOf(tempCustomer);
                await PersistencyService.PutToDatabase(customer, "Customers", $"{customer.Id}",false);
                await PersistencyService.PutToDatabase(address, "addresses",$"{address.Id}",false);
                Singleton.GetInstance().Customers[i] = customer;
                MessageDialogHelper.Show("Kunden blev opdateret", "Succes");
            }
            else if (tempCustomer.Equals(customer))
            {
                MessageDialogHelper.Show("Ingen ændringer blev fortaget", "Fejl");
            }
            //await PersistencyService.PutToDatabase(customer, "customer", $"{customer.Id}");
        }

        //used for getting the entire database down into a collection
        public async void ReadCustomer()
        {
            //            V = await PersistencyService.GetFromDatabase<Customer>("customers");
        }

        //Database handling for Units
        //used for adding a new unit to the Database
        public async void AddUnit()
        {
            //            await PersistencyService.PostToDatabase(new Unit(), "units");
            bool unitExists = false;
            Unit unit = new Unit(ViewModel.UnitName);

            foreach (Unit uni in Singleton.GetInstance().Units)
            {
                if (uni.Unit1 == unit.Unit1)
                {
                    unitExists = true;
                    break;
                }
            }
            if (!unitExists)
            {
                //                await PersistencyService.PostToDatabase(unit, "Units");
                Unit tempunit = await PersistencyService.PostToDatabase(unit, "Units");
                Singleton.GetInstance().Units.Add(tempunit);
                ViewModel.UnsetSelectedunit();
            }
            else
            {
                MessageDialogHelper.Show("En Enhed med dette navn eksisterer allerede.", "Fejl i tilføjelse af Enhed");
            }
        }

        //used for removing a specific unit from the Database
        public async void RemoveUnit()
        {
            var countResult = Singleton.GetInstance().Products.Count(p => p.FK_Unit == ViewModel.SelectedItemUnit.Id);
            if (countResult == 0)
            {
                var messageDialog =
                    new MessageDialog("Er du sikker på du vil slette enheden: " + ViewModel.SelectedItemUnit.Unit1 +
                                      "?");

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                messageDialog.Commands.Add(new UICommand("Ja",
                    new UICommandInvokedHandler(this.CommandInvokedHandlerUnit)));
                messageDialog.Commands.Add(new UICommand("Nej", null));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }
            else
            {
                MessageDialogHelper.Show(
                    "Denne Enhed bliver brugt på et eller flere produkter, og kan derfor ikke slettes",
                    "Fejl i sletning af Enhed");
            }
        }

        private async void CommandInvokedHandlerUnit(IUICommand command)
        {
            await PersistencyService.DeleteFromDatabase("units", $"{ViewModel.SelectedItemUnit.Id}");
            Singleton.GetInstance().Units.Remove(ViewModel.SelectedItemUnit);
            ViewModel.UnsetSelectedunit();
        }


        //used for making changes to an already existing Unit
        public async void UpdateUnit(Unit unit)
        {
            await PersistencyService.PutToDatabase(unit, "units", $"{unit.Id}");
        }

        //used for getting the entire database down into a collection
        public async void ReadUnits()
        {
            //            V = await PersistencyService.GetFromDatabase<Unit>("units");
        }

        // Random string generator (For password)
        private Random random;
        public string RandomString(int Length)
        {
            random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, Length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public DataViewModel ViewModel { get; set; }
        private HashConverter Crypt;

        public DataHandler(DataViewModel viewModel)
        {
            Crypt = new HashConverter();
            ViewModel = viewModel;
        }
    }
}