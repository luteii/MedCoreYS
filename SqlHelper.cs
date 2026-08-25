using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace HastaneYonetim
{
    class SqlHelper
    {
        private SqlConnection sqlConnection = null;
        // Kendi veritabanı bağlantı cümleni buraya sabitliyoruz
        private string connString = @"Data Source=.;Initial Catalog=HASTANEYONETIM_DB;Integrated Security=True;TrustServerCertificate=True;";
        public SqlHelper()
        {
            sqlConnection = new SqlConnection(connString);
        }

        public SqlHelper(string sqlConnectionString)
        {
            sqlConnection = new SqlConnection(sqlConnectionString);
        }

        public void ExecuteNonQuery(string commandText, Dictionary<string, object> parameters = null)
        {
            SqlCommand sqlCommand = CreateCommand(commandText, parameters);
            using (sqlCommand.Connection)
            {
                if (sqlCommand.Connection.State == ConnectionState.Closed)
                    sqlCommand.Connection.Open();

                sqlCommand.ExecuteNonQuery();

                if (sqlCommand.Connection.State == ConnectionState.Open)
                    sqlCommand.Connection.Close();
            }
        }

        public DataTable GetTable(string commandText, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();
            SqlCommand sqlCommand = CreateCommand(commandText, parameters);
            using (sqlCommand.Connection)
            {
                if (sqlCommand.Connection.State == ConnectionState.Closed)
                    sqlCommand.Connection.Open();

                SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
                da.Fill(dataTable);

                if (sqlCommand.Connection.State == ConnectionState.Open)
                    sqlCommand.Connection.Close();

                return dataTable;
            }
        }

        public object ExecuteScalar(string commandText, Dictionary<string, object> parameters = null)
        {
            SqlCommand sqlCommand = CreateCommand(commandText, parameters);
            object value;
            using (sqlCommand.Connection)
            {
                if (sqlCommand.Connection.State == ConnectionState.Closed)
                    sqlCommand.Connection.Open();

                value = sqlCommand.ExecuteScalar();

                if (sqlCommand.Connection.State == ConnectionState.Open)
                    sqlCommand.Connection.Close();

                return value;
            }
        }

        private SqlCommand CreateCommand(string commandText, Dictionary<string, object> parameters = null)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.ConnectionString = connString;
            }

            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = commandText;
            sqlCommand.CommandType = CommandType.StoredProcedure;
            AddParametersCommand(sqlCommand, parameters);
            return sqlCommand;
        }

        private void AddParametersCommand(SqlCommand sqlCommand, Dictionary<string, object> parameters = null)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                }
            }
        }
    }

    public static class ComboBoxExtensions
    {
        public static void SetDataSourceWithChooseOption(this System.Windows.Forms.ComboBox cmb, DataTable dt, string displayMember, string valueMember)
        {
            if (dt != null)
            {
                DataRow row = dt.NewRow();
                if (dt.Columns.Contains(displayMember)) row[displayMember] = "Seçiniz";

                if (dt.Columns.Contains(valueMember))
                {
                    Type type = dt.Columns[valueMember].DataType;
                    if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(byte))
                        row[valueMember] = -1;
                    else
                        row[valueMember] = DBNull.Value;
                }
                dt.Rows.InsertAt(row, 0);
            }
            cmb.DataSource = dt;
            cmb.DisplayMember = displayMember;
            cmb.ValueMember = valueMember;
            if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;
        }
    }
}