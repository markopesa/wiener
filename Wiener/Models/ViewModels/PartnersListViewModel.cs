using Wiener.Data.Interfaces;

namespace Wiener.Models.ViewModels
{
    public class PartnerListViewModel
    {
        public List<PartnerListItemViewModel> Partners { get; set; } = new();
        public int? NewlyAddedPartnerId { get; set; }

        public async Task LoadPartnersAsync(
            IPartnerRepository partnerRepository,
            IPolicyRepository policyRepository
        )
        {
            var partners = await partnerRepository.GetAllAsync();

            foreach (var partner in partners)
            {
                var policyCount = await policyRepository
                    .GetPolicyCountByPartnerIdAsync(partner.Id);
                var totalAmount = await policyRepository
                    .GetTotalAmountByPartnerIdAsync(partner.Id);

                Partners.Add(new PartnerListItemViewModel
                {
                    Id = partner.Id,
                    FullName = $"{partner.FirstName} {partner.LastName}",
                    PartnerNumber = partner.PartnerNumber,
                    CroatianPIN = partner.CroatianPIN ?? "N/A",
                    PartnerType = partner.PartnerType?.Name ?? "N/A",
                    DateCreated = partner.DateCreated,
                    IsForeign = partner.IsForeign,
                    Gender = partner.Gender,
                    RequiresFlag = policyCount > 5 || totalAmount > 5000,
                    IsNewlyAdded = NewlyAddedPartnerId.HasValue &&
                        partner.Id == NewlyAddedPartnerId.Value
                });
            }
        }
    }

    public class PartnerListItemViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PartnerNumber { get; set; }
        public string CroatianPIN { get; set; }
        public string PartnerType { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool IsForeign { get; set; }
        public char Gender { get; set; }
        public bool RequiresFlag { get; set; }
        public bool IsNewlyAdded { get; set; }
    }
}