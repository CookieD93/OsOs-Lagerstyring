using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;
using Windows.UI.Popups;
using OsOs.Model;
using OsOs.Utilities;
using OsOs.ViewModel;

namespace OsOs.Handler
{
    class OrderHandler
    {
        public OrderViewModel OrderViewModel { get; set; }
        public OrderHandler(OrderViewModel orderViewModel)
        {
            OrderViewModel = orderViewModel;
        }
        public void PageChangeFill()
        {
            OrderViewModel.SelectedItemOrder = Singleton.GetInstance().Order;
            OrderViewModel.SetSelectedOrder();
            Singleton.GetInstance().Order = new Order(DateTime.MinValue, 0, null, null, null) { Id = 0 };
            Singleton.GetInstance().Editing = false;
        }
        public async void AddOrderLineToOrder()
        {
            if (OrderViewModel.OrderId == 0)
            {
                SaveOrder();
            }

            Order order1 = Singleton.GetInstance().Orders.First(x => x.Id == OrderViewModel.OrderId);
            Order_Line line = new Order_Line(order1, OrderViewModel.SelectedProduct, OrderViewModel.Amount, 0,
                    OrderViewModel.Batch) {FK_Order_Id = order1.Id, FK_Product_Id = OrderViewModel.SelectedProduct.Id};
            bool productAlreadyOnList = false;
            foreach (Order_Line orderLine in OrderViewModel.OrderLines)
            {
                if (orderLine.Product.Name == OrderViewModel.SelectedProduct.Name)
                {
                    productAlreadyOnList = true;
                }
            }
            if (!productAlreadyOnList)
            {
                order1.Order_Line.Add(line);
                OrderViewModel.OrderLines.Add(line);

                Order_Line uploadingLine = new Order_Line(null, null, line.OrderedAmount, 0,
                        line.BatchNo)
                    {FK_Order_Id = line.FK_Order_Id, FK_Product_Id = line.FK_Product_Id};
                await PersistencyService.PostToDatabase(uploadingLine, "Order_Line",false);
            }
            else
            {
                MessageDialogHelper.Show("Produktet er allerede i ordren","Fejl i tilføjelse af ordre-linje!");
            }
        }

        #region Delete Order Function

        public async void DeleteOrder()
        {
            // Create the message dialog and set its content
            var messageDialog =
                new MessageDialog("Er du sikker på at du vil slette Ordren: " + OrderViewModel.OrderId +
                                  " og alle dens linjer?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Ja",
                new UICommandInvokedHandler(this.CommandInvokedHandlerOrder)));
            messageDialog.Commands.Add(new UICommand("Nej", null));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void CommandInvokedHandlerOrder(IUICommand command)
        {
            foreach (Order_Line line in OrderViewModel.OrderLines)
            {
                await
                    PersistencyService.DeleteFromDatabase("Order_line", $"{line.FK_Order_Id},{line.FK_Product_Id}",
                        false);
                Singleton.GetInstance().OrderLines.Remove(line);
            }
            OrderViewModel.OrderLines.Clear();
            Order order = OrderViewModel.Orders.First(x => x.Id == OrderViewModel.OrderId);
            await PersistencyService.DeleteFromDatabase("Orders", $"{order.Id}");
            Singleton.GetInstance().Orders.Remove(order);
            Singleton.GetInstance().Order = new Order(DateTime.MinValue, 0, null, null, null) { Id = 0 };
        }
        #endregion
        public void LoadOrder(Order order)
        {
            OrderViewModel.OrderLines.Clear();
            foreach (Order_Line line in order.Order_Line)
            {
                OrderViewModel.OrderLines.Add(line);
            }
        }

        public async void SaveOrder()
        {
            Order ord = new Order(OrderViewModel.OrderDate, OrderViewModel.OrderStatus, OrderViewModel.OrderCustomer,
                OrderViewModel.OrderLines, OrderViewModel.OrderAddress)
            {
                Id = OrderViewModel.OrderId,
                FK_Customer_Id = OrderViewModel.OrderCustomer.Id,
                FK_Address_Id = OrderViewModel.OrderAddress.Id,
                FK_Employee_Id = Singleton.GetInstance().EmployeeId
            };
            if (ord.Id != 0)
            {
                Order ords = Singleton.GetInstance().Orders.First(x => x.Id == ord.Id);
                if (!ords.Equals(ord))
                {
                    int i = Singleton.GetInstance().Orders.IndexOf(ord);
                    Singleton.GetInstance().Orders[i] = ord;
                    OrderViewModel.Orders.Add(ord);

                    Order uploadOrder = new Order(ord.Date, ord.Status, null,
                        null, null)
                    {
                        Id = ord.Id,
                        FK_Customer_Id = ord.FK_Customer_Id,
                        FK_Address_Id = ord.FK_Address_Id,
                        FK_Employee_Id = ord.FK_Employee_Id
                    };
                    await PersistencyService.PutToDatabase(uploadOrder, "orders", $"{ord.Id}");
                    MessageDialogHelper.Show("Ordren er gemt", "Gemt");
                }
                else if (ords.Equals(ord))
                {
                    MessageDialogHelper.Show("Ingen ændringer blev fortaget", "Fejl");
                }
            }
            else
            {

                Order uploadOrder = new Order(ord.Date, ord.Status, null,null, null)
                {
                    Id = ord.Id,
                    FK_Customer_Id = ord.FK_Customer_Id,
                    FK_Address_Id = ord.FK_Address_Id,
                    FK_Employee_Id = ord.FK_Employee_Id
                };
                uploadOrder = await PersistencyService.PostToDatabase(uploadOrder, "Orders");

                OrderViewModel.OrderId = uploadOrder.Id;
                ord.Id = uploadOrder.Id;
                Singleton.GetInstance().Orders.Add(ord);
            }
        }

        public async void DeleteOrderLine(Order_Line line)
        {
            await PersistencyService.DeleteFromDatabase("Order_line", $"{line.FK_Order_Id},{line.FK_Product_Id}", false);
            Singleton.GetInstance().OrderLines.Remove(line);
            OrderViewModel.OrderLines.Remove(line);
        }
    }
}