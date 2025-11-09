using Dapper;
using Microsoft.Data.SqlClient;

namespace Wiener.Data.Seed
{
    public class DatabaseSeeder
    {
        private readonly string _connectionString;

        public DatabaseSeeder(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(
                "DefaultConnection"
            ) ?? throw new ArgumentNullException("Connection string not found");
        }

        public async Task SeedAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string checkSql =
                "SELECT COUNT(*) FROM Partners WHERE IsDeleted = 0";
            var count = await connection.ExecuteScalarAsync<int>(checkSql);

            if (count > 0)
            {
                Console.WriteLine("Baza već sadrži podatke. Seed preskočen.");
                return;
            }

            Console.WriteLine("Baza je prazna.");

            await SeedPartnerTypesAsync(connection);
            await SeedPartnersAsync(connection);

            await SeedPoliciesAsync(connection);

            Console.WriteLine("Seedano");
        }

        private async Task SeedPartnerTypesAsync(SqlConnection connection)
        {
            const string checkSql = "SELECT COUNT(*) FROM PartnerTypes";
            var count = await connection.ExecuteScalarAsync<int>(checkSql);

            if (count == 0)
            {
                const string sql = @"
                    INSERT INTO PartnerTypes (Id, Name, DateCreated, IsDeleted)
                    VALUES 
                    (1, 'Personal', GETUTCDATE(), 0),
                    (2, 'Legal', GETUTCDATE(), 0)";

                await connection.ExecuteAsync(sql);
                Console.WriteLine("PartnerTypes seedirani.");
            }
        }

        private async Task SeedPartnersAsync(SqlConnection connection)
        {
            const string sql = @"
                INSERT INTO Partners (
                    FirstName, LastName, Address, PartnerNumber, 
                    CroatianPIN, PartnerTypeId, CreatedByUser, 
                    IsForeign, ExternalCode, Gender, 
                    DateCreated, IsDeleted
                )
                VALUES 
                ('Marko', 'Marković', 'Ilica 123, Zagreb', 
                 '12345678901234567890', '12345678901', 1, 
                 'admin@wiener.hr', 0, 'EXT1234567890', 'M', 
                 GETUTCDATE(), 0),
                
                ('Ana', 'Anić', 'Vukovarska 45, Osijek', 
                 '09876543210987654321', '09876543210', 1, 
                 'admin@wiener.hr', 0, 'EXT0987654321', 'F', 
                 GETUTCDATE(), 0),
                
                ('Petra', 'Petrović', 'Splitska 78, Split', 
                 '11111111112222222222', '11111111111', 1, 
                 'admin@wiener.hr', 0, 'EXT1111111111', 'F', 
                 GETUTCDATE(), 0),
                
                ('Ivan', 'Horvat', 'Rijeka ulica 34, Rijeka', 
                 '33333333334444444444', '33333333333', 1, 
                 'admin@wiener.hr', 1, 'EXT3333333333', 'M', 
                 GETUTCDATE(), 0),
                
                ('Wiener d.o.o.', '', 'Zagrebačka 100, Zagreb', 
                 '55555555556666666666', NULL, 2, 
                 'admin@wiener.hr', 0, 'EXT5555555555', 'N', 
                 GETUTCDATE(), 0)";

            await connection.ExecuteAsync(sql);
            Console.WriteLine("Partners seedirani - 5 partnera dodano.");
        }

        private async Task SeedPoliciesAsync(SqlConnection connection)
        {
            const string sql = @"
                -- Marko Marković - 6 polica (>5 = treba *)
                INSERT INTO Policies (PartnerId, PolicyNumber, 
                                      PolicyAmount, DateCreated, IsDeleted)
                VALUES 
                (1, 'POL0000000001', 1000.00, GETUTCDATE(), 0),
                (1, 'POL0000000002', 1500.00, GETUTCDATE(), 0),
                (1, 'POL0000000003', 800.00, GETUTCDATE(), 0),
                (1, 'POL0000000004', 1200.00, GETUTCDATE(), 0),
                (1, 'POL0000000005', 900.00, GETUTCDATE(), 0),
                (1, 'POL0000000006', 600.00, GETUTCDATE(), 0),
                
                -- Ana Anić - 3 police, ukupno 6000 (>5000 = treba *)
                (2, 'POL0000000007', 2500.00, GETUTCDATE(), 0),
                (2, 'POL0000000008', 2000.00, GETUTCDATE(), 0),
                (2, 'POL0000000009', 1500.00, GETUTCDATE(), 0),
                
                -- Petra Petrović - 2 police, male vrijednosti (ne treba *)
                (3, 'POL0000000010', 500.00, GETUTCDATE(), 0),
                (3, 'POL0000000011', 300.00, GETUTCDATE(), 0)";

            await connection.ExecuteAsync(sql);
          
        }
    }
}