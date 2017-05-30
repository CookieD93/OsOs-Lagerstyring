using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using OsOs.Model;
using OsOs.Utilities;
using OsOs.ViewModel;

namespace OsOs.Handler
{
    class WareHouseHandler
    {
        public async void Withdraw(Product_Location selectLocation=null)
        {
            Product_Location selectedLocation ;

            if (selectLocation ==null)
            {
            selectedLocation = ViewModel.Location;
            }
            else
            {
            selectedLocation = selectLocation;
            }
            selectedLocation.FK_Product_Id = selectedLocation.Product.Id;

            if (selectedLocation.Quantity >= ViewModel.Amount)
            {
                selectedLocation.Quantity -= ViewModel.Amount;
                if (!selectedLocation.Reserved && selectedLocation.Quantity == 0)
                {
                    selectedLocation.Product = null;
                    selectedLocation.FK_Product_Id = null;
                    selectedLocation.Batch = null;
                    selectedLocation.Date = null;
                }
                Product_Location tempSingletonProductLocation =
                    Singleton.GetInstance().Locations.First(x => x.Location == selectedLocation.Location);
                int index = Singleton.GetInstance().Locations.IndexOf(tempSingletonProductLocation);
                Singleton.GetInstance().Locations[index] = selectedLocation ;

                Product_Location locationToUpload = new Product_Location(selectedLocation.Location,selectedLocation.Reserved,selectedLocation.Date) {FK_Product_Id = selectedLocation.FK_Product_Id,Batch = selectedLocation.Batch,Quantity = selectedLocation.Quantity};
                await PersistencyService.PutToDatabase(locationToUpload, "Product_Location", $"{selectedLocation.Location}",false);
                MessageDialogHelper.Show("Withdrawl is complete", "Succes");
            }
            else
            {
                MessageDialogHelper.Show("The amount is to much or the selected location is empty", "Error 40");
            }
        }

        //Denne metode kigger på om lokationen allerede har det produkt man vil ligge ind eller om den er tom.
        //Er dette statement sandt, så ligger den den givne mængde til. Er lokationen tom sætter den også lokationens
        //produkt til at være det produkt som der tilføjes.
        //Ligger der allerede et produkt på lokationen som ikke er det givne produkt, gives der en fejl.
        public void AddToLocation(Product_Location selectLocation=null)
        {
            Product_Location location;

            if (selectLocation == null)
            {
                location = ViewModel.Location;
            }
            else
            {
                location = selectLocation;
            }
            int? nullableint = ViewModel.Amount;
            if (location.Product != null)
            {
                location.FK_Product_Id = location.Product.Id;
                location.Product = null;
            }
            location.Batch = ViewModel.BatchNo;
            location.Date = ViewModel.DateToday;
            //location.Date = null;
            if (location.FK_Product_Id == null || location.FK_Product_Id == ViewModel.Product.Id)
            {
                if (location.Quantity == null) location.Quantity = 0;
                location.Quantity = location.Quantity + nullableint;
                if (location.FK_Product_Id == null)
                {
                    location.FK_Product_Id = ViewModel.Product.Id;
                }
                //persistency.savelocation
                PersistencyService.PutToDatabase(location, "Product_location", $"{location.Location}");
                Product_Location locationForSingleton = location;
                locationForSingleton.Product = ViewModel.Product;
                Singleton.GetInstance().Locations.Add(locationForSingleton);
            }
            else if (location.FK_Product_Id != ViewModel.Product.Id)
            {
                MessageDialogHelper.Show(
                    $"Der ligger allerede et produkt af typen {ViewModel.Products.First(x => x.Id == location.FK_Product_Id)} på denne lokation",
                    "Fejl");
            }
        }

        //Denne metode tager to lokationer (fra og til) og en mængde som skal flyttes.
        //Den tager og siger at hvis produktet på lokationerne er det samme produkt, så ligger den mængden
        //til "til-lokationen" og trækker mængden fra "fra-lokationen".
        //Hvis den første If ikke bliver opfyldt så ser den om produktet er forskelligt og ikke er null, så tager den
        //og giver en fejl om at der allerede ligger et produkt på lokationen.
        //Som en sidste ting kigger den på om der overhovedet ligger et produkt, hvis der ikke gør tager den og ligger produktet dér.
        public async void MoveToLocation()
        {
            Product_Location fromLocation = ViewModel.Location;
            Product_Location toLocation = ViewModel.SecondLocation;
            if (fromLocation.Quantity>=ViewModel.Amount)
            {
                if (toLocation.FK_Product_Id != ViewModel.Product.Id)
                {
                    MessageDialogHelper.Show(
                        $"Der ligger allerede et produkt af typen {ViewModel.Products.First(x => x.Id == toLocation.FK_Product_Id)} på denne lokation",
                        "Fejl");
                }
                else
                {
                    ViewModel.BatchNo = fromLocation.Batch;
                    Withdraw(fromLocation);
                    AddToLocation(toLocation);
                }
            }
        }

        public void OrderProcess(Order order, Product product, int amountPicked)
        {
            List<Order_Line> lines = (List<Order_Line>) order.Order_Line.Where(x => x.Product == product);

            if (lines[0].OrderedAmount >= amountPicked)
            {
                lines[0].PickedAmount += amountPicked;
                if (lines[0].OrderedAmount == lines[0].PickedAmount)
                {
                    MessageDialogHelper.Show("Covota forfilled", "Succes");
                }
                else
                {
                    int difference = lines[0].OrderedAmount - lines[0].PickedAmount;
                    MessageDialogHelper.Show($"there is still missing {difference} of {product}", "Missing items");
                }
            }
            else
            {
                MessageDialogHelper.Show("Quantity is too big", "Error");
            }
        }

        public WareHouseViewModel ViewModel { get; set; }

        public WareHouseHandler(WareHouseViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}