using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Lib
{
    class SqlConnector
    {
        Dictionary<string, MySqlConnection> connections = new Dictionary<string, MySqlConnection>();
        string connctor_property = null;

        public void AddConnector(string key, string ip, ushort port, string database)
        {
            lock (connections)
            {
                connctor_property = @"SERVER=" + ip
                    + ";PORT=" + port
                    + ";DATABASE=" + database
                    + ";UID=123;PWD=456";

                if (!connections.ContainsKey(key))
                {
                    MySqlConnection conn = new MySqlConnection(connctor_property);
                    conn.Open();
                    connections.Add(key, conn);
                }
            }
        }

        public void DelConnector(string key)
        {
            lock (connections)
            {
                if (connections.ContainsKey(key))
                {
                    connections[key].Dispose();
                    connections[key].Close();
                    connections.Remove(key);
                }
            }
        }

        public MySqlConnection Connector(string key)
        {
            lock (connections)
            {
                if (!connections.ContainsKey(key))
                {
                    return null;
                }

                MySqlConnection connection = connections[key];

                if (!connection.Ping())
                {
                    connection.Open();
                }

                return connection;
            }
        }

        ~SqlConnector()
        {
            lock (connections)
            {
                foreach (MySqlConnection item in connections.Values)
                {
                    item.Dispose();
                    item.Close();
                }
            }
        }
    }
}
