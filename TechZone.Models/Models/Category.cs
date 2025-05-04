using System.ComponentModel.DataAnnotations;

namespace TechZone.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The Name is Required")]
        [MaxLength(50,ErrorMessage ="Max Length Is 50")]
        public string name { get; set; }
        [Required]
        [Range(0,200,ErrorMessage ="the range between 0 => 200")]
        public int DisplayOrder { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<Product>? Products  { get; set; }
    }
}
