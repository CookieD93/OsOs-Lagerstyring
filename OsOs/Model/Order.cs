using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public byte Status { get; set; }
        public Customer Customer { get; set; }
        public ObservableCollection<Order_Line> Order_Line { get; set; }
        public Address Address { get; set; }
        public Employee Employee { get; set; }
        public int FK_Employee_Id { get; set; }

        public int FK_Customer_Id { get; set; }
        public int? FK_Address_Id { get; set; }


        public Order(DateTime orderDate, byte status, Customer customer, ObservableCollection<Order_Line> odrLines, Address address)
        {
            Date = orderDate;
            Status = status;
            Customer = customer;
            Order_Line = odrLines;
            Address = address;
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Date)}: {Date}, {nameof(Status)}: {Status}, {nameof(Customer)}: {Customer}, {nameof(Order_Line)}: {Order_Line}, {nameof(Address)}: {Address}";
        }

        protected bool Equals(Order other)
        {
            return Id == other.Id && Date.Equals(other.Date) && Status == other.Status && Equals(Customer, other.Customer) && Equals(Order_Line, other.Order_Line) && Equals(Address, other.Address) && Equals(Employee, other.Employee) && FK_Employee_Id == other.FK_Employee_Id && FK_Customer_Id == other.FK_Customer_Id && FK_Address_Id == other.FK_Address_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Order) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Status.GetHashCode();
                hashCode = (hashCode * 397) ^ (Customer != null ? Customer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Order_Line != null ? Order_Line.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Address != null ? Address.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Employee != null ? Employee.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FK_Employee_Id;
                hashCode = (hashCode * 397) ^ FK_Customer_Id;
                hashCode = (hashCode * 397) ^ FK_Address_Id.GetHashCode();
                return hashCode;
            }
        }
    }
}