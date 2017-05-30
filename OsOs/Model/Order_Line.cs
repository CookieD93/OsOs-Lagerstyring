using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OsOs.Annotations;

namespace OsOs.Model
{
    class Order_Line : INotifyPropertyChanged
    {

        public Order Order { get; set; }
        public Product Product { get; set; }
        public int OrderedAmount { get; set; }

        public int PickedAmount { get; set; }

        [StringLength(16)]
        public string BatchNo { get; set; }
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FK_Order_Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int FK_Product_Id { get; set; }

        public Order_Line(Order order, Product product, int orderedAmount, int pickedAmount, string batchNo)
        {
            Order = order;
            Product = product;
            OrderedAmount = orderedAmount;
            PickedAmount = pickedAmount;
            BatchNo = batchNo;
        }

        public override string ToString()
        {
            return $"{nameof(Product)}: {Product}, {nameof(OrderedAmount)}: {OrderedAmount}, {nameof(PickedAmount)}: {PickedAmount}, {nameof(BatchNo)}: {BatchNo}";
        }

        protected bool Equals(Order_Line other)
        {
            return Equals(Order, other.Order) && Equals(Product, other.Product) && OrderedAmount == other.OrderedAmount && PickedAmount == other.PickedAmount && string.Equals(BatchNo, other.BatchNo) && FK_Order_Id == other.FK_Order_Id && FK_Product_Id == other.FK_Product_Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Order_Line) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Order != null ? Order.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Product != null ? Product.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ OrderedAmount;
                hashCode = (hashCode * 397) ^ PickedAmount;
                hashCode = (hashCode * 397) ^ (BatchNo != null ? BatchNo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ FK_Order_Id;
                hashCode = (hashCode * 397) ^ FK_Product_Id;
                return hashCode;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
