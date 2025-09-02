using System.ComponentModel.DataAnnotations;

namespace AutoFilter.DemoDb
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
