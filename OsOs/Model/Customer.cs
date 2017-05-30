using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Address Address { get; set; }
        public string Name { get; set; }
        public int FK_Address_Id { get; set; }

        public Customer(Address address, string name)
        {
            Address = address;
            Name = name;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Address)}: {Address}, {nameof(Name)}: {Name}";
        }

        protected bool Equals(Customer other)
        {
            return Id == other.Id && Equals(Address, other.Address) && string.Equals(Name, other.Name) && FK_Address_Id == other.FK_Address_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Customer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FK_Address_Id;
                return hashCode;
            }
        }
    }
}
