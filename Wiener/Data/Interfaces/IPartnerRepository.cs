using Wiener.Models.Entities;

namespace Wiener.Data.Interfaces
{
    public interface IPartnerRepository
    {
        Task<IEnumerable<Partner>> GetAllAsync();
        Task<Partner?> GetByIdAsync(int id);
        Task<int> CreateAsync(Partner partner);
        Task<bool> UpdateAsync(Partner partner);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<PartnerType>> GetPartnerTypesAsync();
    }
}
