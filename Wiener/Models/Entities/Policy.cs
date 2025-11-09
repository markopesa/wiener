using System.ComponentModel.DataAnnotations;

namespace Wiener.Models.Entities
{
    public class Policy : BaseEntity<int>
    {
        [Required]
        public int PartnerId { get; set; }

        [Required, MinLength(10), MaxLength(15)]
        public string PolicyNumber { get; set; }

        [Required]
        public decimal PolicyAmount { get; set; }

        public Partner? Partner { get; set; }
    }
}
