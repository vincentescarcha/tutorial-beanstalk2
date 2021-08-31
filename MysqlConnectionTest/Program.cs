using MySqlConnector;
using Renci.SshNet;
using System;
using System.IO;

namespace MysqlConnectionTest
{
    class Program
    {
        static string SshHostName = "ec2-18-163-214-96.ap-east-1.compute.amazonaws.com";
        static string SshUserName = "ubuntu";
        static string SshKeyFile = @"C:\Users\VincentEscarcha\Downloads\keypair-1-vincent.pem";

        static string Server = "127.0.0.1";
        static uint Port = 3306;
        static string UserID = "root";
        static string Password = "admin";
        static string DataBase = "FoodShop";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ConnectionInfo cnnInfo;
            using (var stream = new FileStream(SshKeyFile, FileMode.Open, FileAccess.Read))
            {
                var file = new PrivateKeyFile(stream);
                var authMethod = new PrivateKeyAuthenticationMethod(SshUserName, file);
                cnnInfo = new ConnectionInfo(SshHostName, 22, SshUserName, authMethod);
            }

            using (var client = new SshClient(cnnInfo))
            {
                client.Connect();
                if (client.IsConnected)
                {
                    var forwardedPort = new ForwardedPortLocal("127.0.0.1", Server, Port);
                    client.AddForwardedPort(forwardedPort);
                    forwardedPort.Start();

                    string connStr = $"Server = {forwardedPort.BoundHost};Port = {forwardedPort.BoundPort};Database = {DataBase};Uid = {UserID};Pwd = {Password};";

                    using (MySqlConnection cnn = new MySqlConnection(connStr))
                    {
                        cnn.Open();

                        MySqlCommand cmd = new MySqlCommand("SELECT * FROM test_table1 LIMIT 25;", cnn);

                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                            Console.WriteLine($"{reader.GetInt32(0)}, {reader.GetString(1)}");

                        Console.WriteLine("Ok");

                        cnn.Close();
                    }

                    client.Disconnect();
                }

            }
        }
    }
}
