using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

public class clsKetNoi : IDisposable
{
    private SqlConnection con;

    public clsKetNoi()
    {
        string connStr = ConfigurationManager.ConnectionStrings["project"].ConnectionString;
        con = new SqlConnection(connStr);

        if (con.State == ConnectionState.Closed)
            con.Open();
    }

    public DataTable LoadTable(string sql)
    {
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            cmd.CommandType = CommandType.Text;
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    public DataTable LoadTable(string sql, string[] name, object[] value, int thamso)
    {
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            for (int i = 0; i < thamso; i++)
            {
                cmd.Parameters.AddWithValue(name[i], value[i]);
            }

            cmd.CommandType = CommandType.Text;
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }

    public int UpdateTable(string sql)
    {
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            return cmd.ExecuteNonQuery();
        }
    }

    public int UpdateTable(string sql, string[] name, object[] value, int thamso)
    {
        using (SqlCommand cmd = new SqlCommand(sql, con))
        {
            for (int i = 0; i < thamso; i++)
            {
                cmd.Parameters.AddWithValue(name[i], value[i]);
            }

            return cmd.ExecuteNonQuery();
        }
    }

    // 👇 QUAN TRỌNG: Giải phóng kết nối SQL
    public void Dispose()
    {
        if (con != null)
        {
            if (con.State != ConnectionState.Closed)
                con.Close();

            con.Dispose();
            con = null;
        }
    }
}
