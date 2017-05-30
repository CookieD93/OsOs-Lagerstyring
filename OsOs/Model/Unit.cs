using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Unit
    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Unit1 { get; set; }

        public Unit(string unit1)
        {
            Unit1 = unit1;
        }
        public override string ToString()
        {
            return $"{nameof(Unit1)}: {Unit1}";
        }

        protected bool Equals(Unit other)
        {
            return string.Equals(Unit1, other.Unit1);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Unit) obj);
        }

        public override int GetHashCode()
        {
            return (Unit1 != null ? Unit1.GetHashCode() : 0);
        }
    }
}
