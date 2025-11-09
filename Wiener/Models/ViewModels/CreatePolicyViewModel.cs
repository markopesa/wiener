using System.ComponentModel.DataAnnotations;
using Wiener.Data.Interfaces;
using Wiener.Models.Entities;

namespace Wiener.Models.ViewModels
{
    public class CreatePolicyViewModel
    {
        public int PartnerId { get; set; }
        public string? PartnerName { get; set; }

        [Required(ErrorMessage = "Broj police je obavezan")]
        [MinLength(10, ErrorMessage = "Broj police mora imati minimalno 10 znakova")]
        [MaxLength(15, ErrorMessage = "Broj police može imati maksimalno 15 znakova")]
        [Display(Name = "Broj police")]
        public string PolicyNumber { get; set; }

        [Required(ErrorMessage = "Iznos police je obavezan")]
        [Range(0.01, double.MaxValue,
            ErrorMessage = "Iznos police mora biti veći od 0")]
        [Display(Name = "Iznos police")]
        public decimal PolicyAmount { get; set; }

        public static async Task<CreatePolicyViewModel> PrepareAsync(
            int partnerId,
            IPartnerRepository partnerRepository
        )
        {
            var partner = await partnerRepository.GetByIdAsync(partnerId);

            if (partner == null)
                throw new ArgumentException("Partner nije pronađen");

            return new CreatePolicyViewModel
            {
                PartnerId = partnerId,
                PartnerName = $"{partner.FirstName} {partner.LastName}"
            };
        }

        public async Task<int> SavePolicyAsync(
            IPolicyRepository policyRepository
        )
        {
            var policy = new Policy
            {
                PartnerId = PartnerId,
                PolicyNumber = PolicyNumber,
                PolicyAmount = PolicyAmount
            };

            return await policyRepository.CreateAsync(policy);
        }
    }
}