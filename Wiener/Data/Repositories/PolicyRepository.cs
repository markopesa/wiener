using Dapper;
using Microsoft.Data.SqlClient;
using Wiener.Data.Interfaces;
using Wiener.Models.Entities;

namespace Wiener.Data.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly string _connectionString;

        public PolicyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(
                "DefaultConnection"
            ) ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task<IEnumerable<Policy>> GetByPartnerIdAsync(
            int partnerId
        )
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT * FROM Policies 
                WHERE PartnerId = @PartnerId AND IsDeleted = 0
                ORDER BY DateCreated DESC";

            return await connection.QueryAsync<Policy>(
                sql,
                new { PartnerId = partnerId }
            );
        }

        public async Task<int> CreateAsync(Policy policy)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Policies (
                    PartnerId, PolicyNumber, PolicyAmount, 
                    DateCreated, IsDeleted
                )
                VALUES (
                    @PartnerId, @PolicyNumber, @PolicyAmount, 
                    GETUTCDATE(), 0
                );
                SELECT CAST(SCOPE_IDENTITY() as int)";

            return await connection.ExecuteScalarAsync<int>(sql, policy);
        }

        public async Task<int> GetPolicyCountByPartnerIdAsync(int partnerId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT COUNT(*) 
                FROM Policies 
                WHERE PartnerId = @PartnerId AND IsDeleted = 0";

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { PartnerId = partnerId }
            );
        }

        public async Task<decimal> GetTotalAmountByPartnerIdAsync(
            int partnerId
        )
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT ISNULL(SUM(PolicyAmount), 0) 
                FROM Policies 
                WHERE PartnerId = @PartnerId AND IsDeleted = 0";

            return await connection.ExecuteScalarAsync<decimal>(
                sql,
                new { PartnerId = partnerId }
            );
        }
    }
}