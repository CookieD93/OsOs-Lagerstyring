using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class CountryCode
    {
        public string Iso { get; set; }
        public string Country { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public CountryCode(string iso, string country)
        {
            Iso = iso;
            Country = country;
        }

        protected bool Equals(CountryCode other)
        {
            return string.Equals(Iso, other.Iso) && string.Equals(Country, other.Country) && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CountryCode) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Iso != null ? Iso.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Country != null ? Country.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Id;
                return hashCode;
            }
        }
    }
}
