using Wiener.Models.Entities;

namespace Wiener.Data.Interfaces
{
    public interface IPolicyRepository
    {
        Task<IEnumerable<Policy>> GetByPartnerIdAsync(int partnerId);
        Task<int> CreateAsync(Policy policy);
        Task<int> GetPolicyCountByPartnerIdAsync(int partnerId);
        Task<decimal> GetTotalAmountByPartnerIdAsync(int partnerId);
    }
}
