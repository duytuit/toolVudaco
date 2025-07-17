using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace toolVudaco
{
    public static class SqlHelper
    {
        public static async Task<SqlCommand> BuildPagedCommandWithTotalAsync(
            SqlConnection connection,
            string tableName,
            string[] fields,
            int? skip = null,
            int? take = null,
            Dictionary<string, object> whereEquals = null,
            Dictionary<string, string> whereLikes = null,
            Dictionary<string, IEnumerable<object>> whereInList = null,
            List<(string Field, DateTime From, DateTime To)> dateRangeList = null,
            List<string> orderByList = null,
            CancellationToken cancellationToken = default)
        {
            var cmd = connection.CreateCommand();

            // Kiểm tra cột deleted_at
            cmd.CommandText = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.COLUMNS 
            WHERE TABLE_NAME = @table AND COLUMN_NAME = 'deleted_at'";
            cmd.Parameters.AddWithValue("@table", tableName);
            var hasDeletedAt = (int)(await cmd.ExecuteScalarAsync(cancellationToken)) > 0;

            var whereClauses = new List<string>();

            if (hasDeletedAt)
                whereClauses.Add("deleted_at IS NULL");

            if (whereEquals != null)
            {
                foreach (var kv in whereEquals)
                {
                    var param = $"@eq_{kv.Key}";
                    whereClauses.Add($"{kv.Key} = {param}");
                    cmd.Parameters.AddWithValue(param, kv.Value ?? DBNull.Value);
                }
            }

            if (whereLikes != null)
            {
                foreach (var kv in whereLikes)
                {
                    var param = $"@like_{kv.Key}";
                    whereClauses.Add($"{kv.Key} LIKE {param}");
                    cmd.Parameters.AddWithValue(param, $"%{kv.Value}%");
                }
            }

            if (whereInList != null)
            {
                foreach (var kv in whereInList)
                {
                    var paramNames = new List<string>();
                    int i = 0;
                    foreach (var val in kv.Value)
                    {
                        var param = $"@in_{kv.Key}_{i++}";
                        paramNames.Add(param);
                        cmd.Parameters.AddWithValue(param, val);
                    }
                    whereClauses.Add($"{kv.Key} IN ({string.Join(", ", paramNames)})");
                }
            }

            if (dateRangeList != null)
            {
                for (int i = 0; i < dateRangeList.Count; i++)
                {
                    var dr = dateRangeList[i];
                    var fromParam = $"@dateFrom{i}";
                    var toParam = $"@dateTo{i}";
                    whereClauses.Add($"{dr.Field} BETWEEN {fromParam} AND {toParam}");
                    cmd.Parameters.AddWithValue(fromParam, dr.From);
                    cmd.Parameters.AddWithValue(toParam, dr.To);
                }
            }

            var whereClause = whereClauses.Count > 0 ? $"WHERE {string.Join(" AND ", whereClauses)}" : "";
            var orderClause = (orderByList != null && orderByList.Count > 0)
                ? $"ORDER BY {string.Join(", ", orderByList)}"
                : "ORDER BY (SELECT NULL)";
            var offset = skip ?? 0;
            var fetch = take ?? 20;

            var fieldList = string.Join(", ", fields.Select(f => $"t.{f}"));

            cmd.CommandText = $@"
            SELECT {fieldList}, COUNT(*) OVER() AS TotalRows
            FROM [{tableName}] t
            {whereClause}
            {orderClause}
            OFFSET {offset} ROWS FETCH NEXT {fetch} ROWS ONLY";

            return cmd;
        }

        public static async Task<DataTable> ExecuteQueryDataTableAsync(SqlCommand cmd, CancellationToken cancellationToken = default)
        {
            var dt = new DataTable();
            SqlDataReader reader = null;
            try
            {
                reader = await cmd.ExecuteReaderAsync(cancellationToken);
                dt.Load(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Dispose(); // dùng Dispose thay vì DisposeAsync
            }
            return dt;
        }
    }
}
