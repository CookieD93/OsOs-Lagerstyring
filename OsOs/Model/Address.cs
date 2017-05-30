using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int Postal { get; set; }
        public string Town { get; set; }
        public CountryCode CountryCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int FK_Country_Id { get; set; }

        public Address(string name="", string address1="", string address2="", int postal=0, string town="", CountryCode countryCode=null, string phone="", string email="")
        {
            Name = name;
            Address1 = address1;
            Address2 = address2;
            Postal = postal;
            Town = town;
            CountryCode = countryCode;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Address1)}: {Address1}, {nameof(Address2)}: {Address2}, {nameof(Postal)}: {Postal}, {nameof(Town)}: {Town}, {nameof(CountryCode)}: {CountryCode}, {nameof(Phone)}: {Phone}, {nameof(Email)}: {Email}";
        }

        protected bool Equals(Address other)
        {
            return Id == other.Id && string.Equals(Name, other.Name) && string.Equals(Address1, other.Address1) && string.Equals(Address2, other.Address2) && Postal == other.Postal && string.Equals(Town, other.Town) && Equals(CountryCode, other.CountryCode) && string.Equals(Phone, other.Phone) && string.Equals(Email, other.Email) && FK_Country_Id == other.FK_Country_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address1 != null ? Address1.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address2 != null ? Address2.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Postal;
                hashCode = (hashCode * 397) ^ (Town != null ? Town.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CountryCode != null ? CountryCode.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Phone != null ? Phone.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FK_Country_Id;
                return hashCode;
            }
        }
    }
}
