using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;

namespace Automation
{
    static class MysqlWork
    {
        static string connection_string = "server='localhost';userid='root';password='root';database='automation';";

        static public void insert_module(string module)
        {
            using (MySqlConnection connection = new MySqlConnection(connection_string))
            {
                try { connection.Open(); }
                catch (MySqlException ex) { return; }
                MySqlCommand command = new MySqlCommand($"insert into module_name (name) values (@name)", connection);
                command.Parameters.AddWithValue("@name", module);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        
        static public void insert_module_result(string module, int serial_number, List<Automation.AllTests.Result> result)
        {
            
            bool full_test_result = true;
            foreach (Automation.AllTests.Result r in result)
            {
                if (!r.is_test_norm)
                    full_test_result = false;
            }

            using (MySqlConnection connection = new MySqlConnection(connection_string))
            {
                try { connection.Open(); }
                catch (MySqlException ex) { return; }
                MySqlCommand command = new MySqlCommand($"insert into module_result (module_name_id, serial_number, date_result, full_test_result, result) values (@module_name_id, @serial_number, @date_result, @full_test_result, @result)", connection);
                command.Parameters.AddWithValue("@module_name_id", MysqlWork.select_module(module));
                command.Parameters.AddWithValue("@serial_number", serial_number);
                command.Parameters.AddWithValue("@date_result", DateTime.Today);
                command.Parameters.AddWithValue("@full_test_result", full_test_result);
                command.Parameters.AddWithValue("@result", JsonConvert.SerializeObject(result));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        static public int select_module(string module)
        {
            int output = 0;
            using (MySqlConnection connection = new MySqlConnection(connection_string))
            {
                try { connection.Open(); }
                catch (MySqlException ex) { return 0; }
                
                MySqlCommand command = new MySqlCommand($"select id from module_name where name = @name", connection);
                command.Parameters.AddWithValue("@name", module);
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    output = reader.GetInt32(0);
                }
                connection.Close();
            }
            return output;
        }

        static public bool try_get_connection(string module)
        {
            using (MySqlConnection connection = new MySqlConnection(connection_string))
            {
                try { connection.Open(); }
                catch (MySqlException ex) { return false; }
                connection.Close();
            }
            return true;
        }
    }
}
