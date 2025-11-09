using Dapper;
using Microsoft.Data.SqlClient;
using Wiener.Data.Interfaces;
using Wiener.Models.Entities;

namespace Wiener.Data.Repositories
{
    public class PartnerRepository : IPartnerRepository
    {
        private readonly string _connectionString;

        public PartnerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(
                "DefaultConnection"
            ) ?? throw new ArgumentNullException("Connection string not found");
        }
        public async Task<IEnumerable<PartnerType>> GetPartnerTypesAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT * FROM PartnerTypes 
                WHERE IsDeleted = 0 
                ORDER BY Name";

            return await connection.QueryAsync<PartnerType>(sql);
        }
        public async Task<IEnumerable<Partner>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT p.*, pt.Name as PartnerTypeName
                FROM Partners p
                LEFT JOIN PartnerTypes pt ON p.PartnerTypeId = pt.Id
                WHERE p.IsDeleted = 0
                ORDER BY p.DateCreated DESC";

            var partners = await connection.QueryAsync<Partner,
                PartnerType, Partner>(
                sql,
                (partner, partnerType) =>
                {
                    partner.PartnerType = partnerType;
                    return partner;
                },
                splitOn: "PartnerTypeName"
            );

            return partners;
        }

        public async Task<Partner?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT * FROM Partners 
                WHERE Id = @Id AND IsDeleted = 0";

            return await connection.QueryFirstOrDefaultAsync<Partner>(
                sql,
                new { Id = id }
            );
        }

        public async Task<int> CreateAsync(Partner partner)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Partners (
                    FirstName, LastName, Address, PartnerNumber, 
                    CroatianPIN, PartnerTypeId, CreatedByUser, 
                    IsForeign, ExternalCode, Gender, 
                    DateCreated, IsDeleted
                )
                VALUES (
                    @FirstName, @LastName, @Address, @PartnerNumber, 
                    @CroatianPIN, @PartnerTypeId, @CreatedByUser, 
                    @IsForeign, @ExternalCode, @Gender, 
                    GETUTCDATE(), 0
                );
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, partner);
        }

        public async Task<bool> UpdateAsync(Partner partner)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Partners 
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Address = @Address,
                    PartnerNumber = @PartnerNumber,
                    CroatianPIN = @CroatianPIN,
                    PartnerTypeId = @PartnerTypeId,
                    IsForeign = @IsForeign,
                    ExternalCode = @ExternalCode,
                    Gender = @Gender,
                    DateUpdated = GETUTCDATE()
                WHERE Id = @Id AND IsDeleted = 0";

            var rowsAffected = await connection.ExecuteAsync(sql, partner);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Partners 
                SET IsDeleted = 1, 
                    DateDeleted = GETUTCDATE() 
                WHERE Id = @Id";

            var rowsAffected = await connection.ExecuteAsync(
                sql,
                new { Id = id }
            );
            return rowsAffected > 0;
        }
    }
}