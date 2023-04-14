using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Wafey
{
    public class DBHelper
    {

        public MySqlConnection _connection;
        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;

        
        public DBHelper()
        {
            _server = "145.44.233.180";
            _database = "wafey";
            _uid = "root";
            _password = "";

            _connection = new MySqlConnection();
            _connection.ConnectionString = String.Format("server={0};Port=80;database={1};uid={2};password={3}", _server, _database, _uid, _password);
        }

        public void Open()
        {
            try
            {
                _connection.Open();
                Console.WriteLine("Connection is opened.");
            }
            catch (MySqlException ex)
            {
                throw new Exception("Cannot connect to server.", ex);
            }
        }

        public void Close()
        {
            try
            {
                var temp = _connection.State.ToString();
                if (_connection.State == ConnectionState.Open && temp == "Open")
                {
                    _connection.Close();
                    Console.WriteLine("Connection is closed.");
                }
                else
                {
                    Console.WriteLine("There was no connection open.");
                }
            }
            catch (MySqlException ex)
            {
                throw new Exception("Cannot disconnect from server.", ex);
            }
        }

        public int ExecuteStoredProcedure(string name, params MySqlParameter[] commandParamters)
        {
            int count = 0;
            using (var cmd = new MySqlCommand())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = _connection;
                cmd.CommandText = name;
                cmd.Parameters.AddRange(commandParamters);
                count = cmd.ExecuteNonQuery();
            }
            return count;
        }

        public List<T> getData<T>(string link)
        {

            //list for T
            List<T> dataList = new List<T>();

            //http request to webpage
            String URL = link;
            try
            {
                string result2 = new WebClient().DownloadString(URL);

                //convert page info into an object
                dynamic response = JsonConvert.DeserializeObject(result2);

                foreach (var item in response)
                {

                    //convert array into a string
                    string json = JsonConvert.SerializeObject(item);

                    //convert string info into an T object
                    T dataObject = JsonConvert.DeserializeObject<T>(json);

                    //add user to T list
                    dataList.Add(dataObject);

                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error something went wrong");
            }

            return dataList;

        }

        public int DBconnectie(String email, String firstname, String Username, String password, String salt)
        {

            String URL = "http://145.44.233.180/AddUser.php?Email=" + email + "&FirstName=" + firstname +
                         "&UserName=" + Username + "&Password=" + password + "&Salt=" + salt;

            string result = new WebClient().DownloadString(URL);

            int intje;

            if (int.TryParse(result, out intje))
            {
                return intje;
            }
            return 0;
        }

        public bool DeleteUser(int userID)
        {

            String URL = "http://145.44.233.180/deleteuser.php?id=" + userID;

            string result = new WebClient().DownloadString(URL);

            if (result.Equals("success"))
            {
                return true;
            }
            return false;
        }

        public bool DeleteElement(string url)
        {
            string result = new WebClient().DownloadString(url);

            if (result.Equals("Success"))
            {
                return true;
            }
            return false;
        }

        public bool ChangeElement(string url)
        {
            string result = new WebClient().DownloadString(url);

            if (result.Equals("Success"))
            {
                return true;
            }
            return false;
        }

    }
}
