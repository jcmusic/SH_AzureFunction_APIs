using Microsoft.Data.Sqlite;
using System.Data;

namespace SH.DAL.Sqlite
{
    internal static class ExtensionMethods
    {
        public static R Single<R>(this SqliteDataReader reader, Func<SqliteDataReader, R> selector)
        {
            R result = default(R);
            if (!reader.HasRows)
                throw new DataException("no rows returned from query");
            if (reader.Read())
                result = selector(reader);
            if (reader.Read())
                throw new DataException("multiple rows returned from query");
            return result;
        }
    }
}
