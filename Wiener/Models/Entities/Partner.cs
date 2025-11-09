using System.ComponentModel.DataAnnotations;

namespace Wiener.Models.Entities
{
    public class Partner : BaseEntity<int>
    {
        [Required, MinLength(2), MaxLength(255)]
        public string FirstName { get; set; }

        [Required, MinLength(2), MaxLength(255)]
        public string LastName { get; set; }

        public string? Address { get; set; }

        [Required, StringLength(20, MinimumLength = 20)]
        public string PartnerNumber { get; set; }

        [StringLength(11)]
        public string? CroatianPIN { get; set; }

        [Required]
        public int PartnerTypeId { get; set; }

        [Required, EmailAddress, MaxLength(255)]
        public string CreatedByUser { get; set; }

        [Required]
        public bool IsForeign { get; set; }

        [MinLength(10), MaxLength(20)]
        public string? ExternalCode { get; set; }

        [Required, RegularExpression("^[MFN]$")]
        public char Gender { get; set; }


        public PartnerType? PartnerType { get; set; }
        public List<Policy>? Policies { get; set; }
    }
}
