using Microsoft.Data.Sqlite;
using SH.Models;
using SH.Models.Customer;
using SH.Models.Models;
using System.Data.Common;
using System.Formats.Asn1;
using System.IO;
using System.Reflection.PortableExecutable;

namespace SH.DAL.Sqlite
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly string _dbPath;
        private readonly string _readOnlyConnectionString;

        public CustomerRepo()
        {
            if (Directory.Exists(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName))
            {
                _dbPath = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "JCM_Customers.db");
            }
            else
            {
                _dbPath = Path.Join(Environment.CurrentDirectory, "JCM_Customers.db");
            }

            _readOnlyConnectionString = $"Data Source = {_dbPath}; Mode = ReadOnly";

            //create sqlite db if not exists
            //using var conn = new SqliteConnection("Data Source=C:\\Customers.db");
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table'";
            using var reader = cmd.ExecuteReader(); 
            if (reader != null && !reader.HasRows) 
            { 
                var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Customers (CustomerId TEXT PRIMARY KEY, FullName TEXT, DateOfBirth NUMERIC)";
                command.ExecuteNonQuery();

                //Create Unique Index
                command.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS ux_CustomerId ON Customers (CustomerId)";
                command.ExecuteNonQuery();

                //Create Unique Index
                command.CommandText = "CREATE INDEX IF NOT EXISTS ix_DoB ON Customers (DateOfBirth)";
                command.ExecuteNonQuery();
            }
        }
        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto)
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            using var cmd = conn.CreateCommand();

            conn.Open();

            var customerId = Guid.NewGuid();
            cmd.CommandText = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth) VALUES (@CustomerId, @FullName, @DateOfBirth)";
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@FullName", dto.FullName);
            cmd.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
            await cmd.ExecuteNonQueryAsync();

            CustomerDto newCustomer = await GetCustomerByIdAsync(customerId.ToString());
            return newCustomer;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(string customerId)
        {

            using var conn = new SqliteConnection(_readOnlyConnectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();

            // Note:  SqlLite is case senstive!! CustomerId Guid is stored as uppercase text.
            cmd.CommandText = $"SELECT CustomerId, FullName, DateOfBirth FROM Customers WHERE CustomerId = '{customerId.ToUpper()}';";

            cmd.Prepare();
            var reader = await cmd.ExecuteReaderAsync();
            try
            {
                if (!reader.HasRows)
                {
                    return null;
                }

                CustomerDto dto = new CustomerDto();
                var counter = 0;
                while (reader.Read())
                {
                    if (counter > 1)
                    {
                        throw new Exception("More than one record found");
                    }
                    dto = new CustomerDto(
                        reader.GetGuid(0),
                        reader.GetString(1),
                        DateOnly.FromDateTime(reader.GetDateTime(2))
                    );

                    counter++;
                }
                return dto;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
        }

        public Task<(List<CustomerDto>, PaginationMetadata)> GetCustomersAsync(int age, int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<List<CustomerDto>> GetCustomersByAgeAsync(int age)
        {
            throw new NotImplementedException();
        }
    }
}
