using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Product_Location
    {
        [Key]
        public string Location { get; set; }
        public string Batch { get; set; }
        public Product Product { get; set; }
        public int? Quantity { get; set; }
        public Boolean Reserved { get; set; }
        public DateTime? Date { get; set; }
        public int? FK_Product_Id { get; set; }


        public Product_Location(string locationId, bool reservedForProduct, DateTime? date)
        {
            Location = locationId;
            Reserved = reservedForProduct;
            Date = date;
            Quantity = 0;
        }

        public override string ToString()
        {
            return $"{nameof(Location)}: {Location}, {nameof(Batch)}: {Batch}, {nameof(Product)}: {Product}, {nameof(Quantity)}: {Quantity }, {nameof(Reserved)}: {Reserved}, {nameof(Date)}: {Date}";
        }

        protected bool Equals(Product_Location other)
        {
            return string.Equals(Location, other.Location) && string.Equals(Batch, other.Batch) && Equals(Product, other.Product) && Quantity == other.Quantity && Reserved == other.Reserved && Date.Equals(other.Date) && FK_Product_Id == other.FK_Product_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product_Location) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Batch != null ? Batch.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Product != null ? Product.GetHashCode() : 0);
                hashCode = (int) ((hashCode * 397) ^ Quantity);
                hashCode = (hashCode * 397) ^ Reserved.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ FK_Product_Id.GetHashCode();
                return hashCode;
            }
        }
    }
}
