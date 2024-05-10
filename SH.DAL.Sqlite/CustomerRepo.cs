using Microsoft.Data.Sqlite;
using SH.Models;
using SH.Models.Customer;
using SH.Models.Models;
using System.Text;

namespace SH.DAL.Sqlite
{
    public class CustomerRepo : ICustomerRepo
    {
        #region Ctor/Fields

        private const string DOB_NAME = "SH_Customers.db";
        private readonly string _dbPath;
        private readonly string _readOnlyConnectionString;

        public CustomerRepo()
        {
            if (Directory.Exists(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName))
            {
                _dbPath = Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, DOB_NAME);
            }
            else
            {
                _dbPath = Path.Join(Environment.CurrentDirectory, DOB_NAME);
            }

            _readOnlyConnectionString = $"Data Source = {_dbPath}; Mode = ReadOnly";

            //create sqlite db if not exists
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = 'Customers'";
            using var reader = cmd.ExecuteReader(); 
            if (reader != null && !reader.HasRows) 
            { 
                var command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE IF NOT EXISTS Customers (CustomerId TEXT PRIMARY KEY, FullName TEXT, DateOfBirth TEXT)";
                command.ExecuteNonQuery();

                //Create Unique Index
                command.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS ux_CustomerId ON Customers (CustomerId)";
                command.ExecuteNonQuery();

                //Create Unique Index
                command.CommandText = "CREATE INDEX IF NOT EXISTS ix_DoB ON Customers (DateOfBirth)";
                command.ExecuteNonQuery();

                //Seed
                command.CommandText = 
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('2FCA73A8-0C0F-4EF7-8A3D-4EC124B14B2B','Jackie Doe','2012-01-01');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('2FEC6CE1-9566-4FF1-B484-3D7FC52987AE','James Doe','1968-04-15');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('664B597B-87B4-4B2E-9634-26D8B04E11AD','Jimmy Doe','1969-01-26');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('6D331645-5EF0-451A-AE22-7DBB09555B31','Jasmine Doe','2012-09-26');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('7FFD32E8-3DC7-4620-A0E3-F0AFA8E43B7C','Josie Doe','2011-02-24');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('B6C8D04D-C698-46A5-AAF7-9C7CA199D1DF','Jethro Doe','1967-01-26');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('D6775BF4-E393-4533-AA02-7BB575E32F72','Jason Doe','1968-01-26');" +
                    "INSERT INTO [Customers] ([CustomerId],[FullName],[DateOfBirth]) VALUES ('E20F441F-4A6B-4E4C-9669-A9C11281B3BB','Jane Doe','2011-02-24');";
                command.ExecuteNonQuery();

            }
        }

        #endregion

        public async Task<CustomerDto> AddCustomerAsync(CreateCustomerDto dto)
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            using var cmd = conn.CreateCommand();

            conn.Open();

            var customerId = Guid.NewGuid();
            cmd.CommandText = "INSERT INTO Customers (CustomerId, FullName, DateOfBirth) VALUES (@CustomerId, @FullName, @DateOfBirth)";
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@FullName", dto.FullName);
            cmd.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth.ToString(Constants.DOB_FORMAT));
            await cmd.ExecuteNonQueryAsync();

            CustomerDto newCustomer = await GetCustomerByIdAsync(customerId.ToString());
            return newCustomer;
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(string customerId)
        {
            using var conn = new SqliteConnection(_readOnlyConnectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();

            // Note: SqlLite is case sensitive!! CustomerId Guid is stored as uppercase text.
            cmd.CommandText = $"SELECT CustomerId, FullName, DateOfBirth, ProfileImage FROM Customers WHERE CustomerId = '{customerId.ToUpper()}';";

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
                    dto.CustomerId = reader.GetGuid(0);
                    dto.FullName = reader.GetString(1);
                    dto.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2));
                    dto.ProfileImage = reader[3].GetType() == typeof(DBNull) ? null : Encoding.ASCII.GetBytes(reader.GetString(3));

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

        public async Task<List<CustomerDto>> GetCustomersByAgeAsync(DateOnly beginDate, DateOnly endDate)
        {
            var dtoList = new List<CustomerDto>();
            using var conn = new SqliteConnection(_readOnlyConnectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();

            var startDateString = beginDate.ToString(Constants.DOB_FORMAT);
            var endDateString = endDate.ToString(Constants.DOB_FORMAT); 

            // Note: SqlLite is case sensitive!! CustomerId Guid is stored as uppercase text.
            cmd.CommandText = $"SELECT CustomerId, FullName, DateOfBirth, ProfileImage FROM Customers WHERE DateOfBirth >= '{startDateString}' AND DateOfBirth <= '{endDateString}';";

            cmd.Prepare();
            var reader = await cmd.ExecuteReaderAsync();
            try
            {
                if (!reader.HasRows)
                {
                    return dtoList;
                }

                CustomerDto dto = new CustomerDto();
                while (reader.Read())
                {
                    dto.CustomerId = reader.GetGuid(0);
                    dto.FullName = reader.GetString(1);
                    dto.DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2));
                    dto.ProfileImage = reader[3].GetType() == typeof(DBNull) ? null : Encoding.ASCII.GetBytes(reader.GetString(3));

                    dtoList.Add(dto);
                }

                return dtoList;
            }
            finally
            {
                reader.Close();
                conn.Close();
            }
        }

        public async Task PersistImageToCustomerDBAsync (Guid customerId, byte[] imageByteArray)
        {
            using var conn = new SqliteConnection($"Data Source={_dbPath}");
            using var cmd = conn.CreateCommand();

            conn.Open();

            cmd.CommandText = "UPDATE Customers SET ProfileImage = @imageByteArray WHERE CustomerId = @customerId";
            cmd.Parameters.AddWithValue("@imageByteArray", imageByteArray);
            cmd.Parameters.AddWithValue("customerId", customerId.ToString().ToUpper());
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
