using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsOs.Model
{
    class Log
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Product_Id { get; set; }

        [Required]
        [StringLength(5)]
        public string Product_Location { get; set; }

        
        [StringLength(16)]
        public string Product_Batch { get; set; }

        
        public int Balance { get; set; }

        public int Amount { get; set; }
        public int Order_Id { get; set; }
        public int Employee_Id { get; set; }
        public DateTime Timestamp { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Description)}: {Description}, {nameof(Product_Id)}: {Product_Id}, {nameof(Product_Location)}: {Product_Location}, {nameof(Product_Batch)}: {Product_Batch}, {nameof(Balance)}: {Balance}, {nameof(Amount)}: {Amount}, {nameof(Order_Id)}: {Order_Id}, {nameof(Employee_Id)}: {Employee_Id}, {nameof(Timestamp)}: {Timestamp}";
        }

    }
}
