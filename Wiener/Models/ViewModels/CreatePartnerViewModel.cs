using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Wiener.Data.Interfaces;
using Wiener.Models.Entities;

namespace Wiener.Models.ViewModels
{
    public class CreatePartnerViewModel
    {
        [Required(ErrorMessage = "Ime je obavezno")]
        [MinLength(2, ErrorMessage = "Ime mora imati minimalno 2 znaka")]
        [MaxLength(255, ErrorMessage = "Ime može imati maksimalno 255 znakova")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        [MinLength(2, ErrorMessage = "Prezime mora imati minimalno 2 znaka")]
        [MaxLength(255, ErrorMessage = "Prezime može imati maksimalno 255 znakova")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [MaxLength(500, ErrorMessage = "Adresa može imati maksimalno 500 znakova")]
        [Display(Name = "Adresa")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Broj partnera je obavezan")]
        [StringLength(20, MinimumLength = 20,
            ErrorMessage = "Broj partnera mora imati točno 20 znamenki")]
        [RegularExpression(@"^\d{20}$",
            ErrorMessage = "Broj partnera mora sadržavati samo brojeve")]
        [Display(Name = "Broj partnera")]
        public string PartnerNumber { get; set; }

        [StringLength(11, MinimumLength = 11,
            ErrorMessage = "OIB mora imati točno 11 znamenki")]
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "OIB mora sadržavati samo brojeve")]
        [Display(Name = "OIB")]
        public string? CroatianPIN { get; set; }

        [Required(ErrorMessage = "Tip partnera je obavezan")]
        [Display(Name = "Tip partnera")]
        public int PartnerTypeId { get; set; }

        [Required(ErrorMessage = "Email je obavezan")]
        [EmailAddress(ErrorMessage = "Unesite valjan email")]
        [MaxLength(255, ErrorMessage = "Email može imati maksimalno 255 znakova")]
        [Display(Name = "Kreirao korisnik (email)")]
        public string CreatedByUser { get; set; }

        [Display(Name = "Strani partner")]
        public bool IsForeign { get; set; }

        [MinLength(10, ErrorMessage = "Eksterni kod mora imati minimalno 10 znakova")]
        [MaxLength(20, ErrorMessage = "Eksterni kod može imati maksimalno 20 znakova")]
        [Display(Name = "Eksterni kod")]
        public string? ExternalCode { get; set; }

        [Required(ErrorMessage = "Spol je obavezan")]
        [RegularExpression("^[MFN]$",
            ErrorMessage = "Spol mora biti M (muški), F (ženski) ili N (neutralno)")]
        [Display(Name = "Spol")]
        public char Gender { get; set; }

        public List<SelectListItem> PartnerTypes { get; set; } = new();

        public async Task PrepareSelectListsAsync(IPartnerRepository partnerRepository)
        {
            var partnerTypes = await partnerRepository.GetPartnerTypesAsync();

            PartnerTypes = partnerTypes.Select(pt => new SelectListItem
            {
                Value = pt.Id.ToString(),
                Text = pt.Name,
                Selected = pt.Id == PartnerTypeId
            })
            .ToList();
        }

        public async Task<int> SavePartnerAsync(
            IPartnerRepository partnerRepository
        )
        {
            var partner = new Partner
            {
                FirstName = FirstName,
                LastName = LastName,
                Address = Address,
                PartnerNumber = PartnerNumber,
                CroatianPIN = CroatianPIN,
                PartnerTypeId = PartnerTypeId,
                CreatedByUser = CreatedByUser,
                IsForeign = IsForeign,
                ExternalCode = ExternalCode,
                Gender = Gender
            };

            return await partnerRepository.CreateAsync(partner);
        }
    }
}