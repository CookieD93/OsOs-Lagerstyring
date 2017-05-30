using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Employee_Function Employee_Function { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Username { get; set; }
        [StringLength(256)]
        public string Password { get; set; }

        public int LoginCode { get; set; }
        public int? FK_Employee_funct { get; set; }

        public Employee(Employee_Function employee_Function, string name, string username, string password, int loginCode)
        {
            Employee_Function = employee_Function;
            Name = name;
            Username = username;
            Password = password;
            LoginCode = loginCode;
        }

        public override string ToString()
        {
            return $"{nameof(Employee_Function)}: {Employee_Function}, {nameof(Name)}: {Name}, {nameof(Username)}: {Username}, {nameof(Password)}: {Password}, {nameof(LoginCode)}: {LoginCode}";
        }

        protected bool Equals(Employee other)
        {
            return Id == other.Id && Equals(Employee_Function, other.Employee_Function) && string.Equals(Name, other.Name) && string.Equals(Username, other.Username) && string.Equals(Password, other.Password) && LoginCode == other.LoginCode && FK_Employee_funct == other.FK_Employee_funct;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Employee) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Employee_Function != null ? Employee_Function.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Username != null ? Username.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ LoginCode;
                hashCode = (hashCode * 397) ^ FK_Employee_funct.GetHashCode();
                return hashCode;
            }
        }
    }
}
