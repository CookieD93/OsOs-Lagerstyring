using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public Unit Unit { get; set; }
        public int? FK_Unit { get; set; }

        public Product(string name, string barcode, string description, Unit unit)
        {
           
            Name = name;
            Barcode = barcode;
            Description = description;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Barcode)}: {Barcode}, {nameof(Description)}: {Description}, {nameof(Unit)}: {Unit}";
        }

        protected bool Equals(Product other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Barcode, other.Barcode) && string.Equals(Description, other.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Product) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Barcode != null ? Barcode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
