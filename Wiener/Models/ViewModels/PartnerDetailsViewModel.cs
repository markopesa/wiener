using Wiener.Data.Interfaces;

namespace Wiener.Models.ViewModels
{
    public class PartnerDetailsViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PartnerNumber { get; set; }
        public string CroatianPIN { get; set; }
        public string PartnerType { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedByUser { get; set; }
        public bool IsForeign { get; set; }
        public string ExternalCode { get; set; }
        public char Gender { get; set; }
        public int PolicyCount { get; set; }
        public decimal TotalPolicyAmount { get; set; }

        public static async Task<PartnerDetailsViewModel?> LoadAsync(
            int partnerId,
            IPartnerRepository partnerRepository,
            IPolicyRepository policyRepository
        )
        {
            var partner = await partnerRepository.GetByIdAsync(partnerId);
            if (partner == null) return null;

            var policyCount = await policyRepository
                .GetPolicyCountByPartnerIdAsync(partnerId);
            var totalAmount = await policyRepository
                .GetTotalAmountByPartnerIdAsync(partnerId);

            return new PartnerDetailsViewModel
            {
                Id = partner.Id,
                FullName = $"{partner.FirstName} {partner.LastName}",
                Address = partner.Address ?? "N/A",
                PartnerNumber = partner.PartnerNumber,
                CroatianPIN = partner.CroatianPIN ?? "N/A",
                PartnerType = partner.PartnerType?.Name ?? "N/A",
                DateCreated = partner.DateCreated,
                CreatedByUser = partner.CreatedByUser,
                IsForeign = partner.IsForeign,
                ExternalCode = partner.ExternalCode ?? "N/A",
                Gender = partner.Gender,
                PolicyCount = policyCount,
                TotalPolicyAmount = totalAmount
            };
        }
    }
}