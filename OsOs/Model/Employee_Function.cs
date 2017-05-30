using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsOs.Model
{
    public class Employee_Function
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public Employee_Function(string description)
        {
            Description = description;
        }

        public override string ToString()
        {
            return $"{nameof(Description)}: {Description}";
        }
    }
}