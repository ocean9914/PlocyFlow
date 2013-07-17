using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace PlocyFlow.DAL.Entity
{
    public class DBHelper
    {
        public static readonly string mysqlConnection = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        private static MySqlCommand GetCommand(MySqlConnection conn, string sql)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return new MySqlCommand(sql, conn);
        }

        public static MySqlDataReader GetReader(MySqlConnection conn, string sql)
        {
            return GetCommand(conn, sql).ExecuteReader(CommandBehavior.CloseConnection);
        }
        public static MySqlDataReader GetReader(string con, string sql)
        {
            return GetReader(new MySqlConnection(mysqlConnection), sql);
        }
        public static MySqlDataReader GetReader(MySqlConnection conn, string proc, MySqlParameter[] parameters)
        {
            var command = GetCommand(conn, proc);
            command.CommandType = CommandType.StoredProcedure;
            foreach (var p in parameters)
            {
                command.Parameters.Add(p);
            }
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static DataSet GetDataSet(string sql)
        {
            DataSet ds = new DataSet();
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, mysqlConnection);
            adapter.Fill(ds);

            return ds;
        }
        public static DataTable GetDataTable(string sql)
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, mysqlConnection);
                adapter.Fill(dt);
                adapter.Dispose();
            }
            catch { }
            return dt;
        }
        public static DataTable GetDataTable(string sql,MySqlParameter[] paras)
        {
            DataTable dt = null;
            try
            {
                dt = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(sql, mysqlConnection);
                foreach (MySqlParameter p in paras)
                {
                    adapter.SelectCommand.Parameters.Add(p);
                }
                adapter.Fill(dt);
                adapter.Dispose();
            }
            catch { }
            return dt;
        }
        public static object GetObject(string sql)
        {
            object o = null;
            if (sql != null && sql.Trim() != "")
            {
                using (MySqlConnection con = new MySqlConnection(mysqlConnection))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(sql, con);
                    o = cmd.ExecuteScalar();
                    con.Close();
                    cmd.Dispose();
                    con.Dispose();
                }
            }
            return o;
        }
        public static object ExcuteScala(MySqlConnection conn, string sql)
        {
            return GetCommand(conn, sql).ExecuteScalar();
        }

        public static int NonQuery(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(mysqlConnection))
            {
                return GetCommand(connection, sql).ExecuteNonQuery();
            }
        }

        public static int NonQuery(string proc, MySqlParameter[] arges)
        {
            using (MySqlConnection connection = new MySqlConnection(mysqlConnection))
            {
                MySqlCommand command = GetCommand(connection, proc);
                command.CommandType = CommandType.StoredProcedure;
                if (arges != null && arges.Count() > 0)
                {
                    command.Parameters.AddRange(arges);
                }
                var result = command.ExecuteNonQuery();
                return result;
            }
        }
        public static int NonExcuteQuery(string sql, MySqlParameter[] arges)
        {
            using (MySqlConnection connection = new MySqlConnection(mysqlConnection))
            {
                MySqlCommand command = GetCommand(connection, sql);
                command.CommandType = CommandType.Text;
                if (arges != null && arges.Count() > 0)
                {
                    command.Parameters.AddRange(arges);
                }
                var result = command.ExecuteNonQuery();
                return result;
            }
        }
    }
    public class DBHelperTran : IDisposable
    {
        private string _connection;
        private bool _hasAct;
        private bool _hasErr;
        public bool HasErr { get { return _hasErr; } }
        private MySqlConnection con;
        private MySqlTransaction tran;
        public DBHelperTran(string MysqlConStr)
        {
            _connection = MysqlConStr;
            _hasAct = false;
            _hasErr = false;
            con = new MySqlConnection(_connection);
            con.Open();
            tran = con.BeginTransaction();
        }
        public int ExcuteSQL(string sql)
        {
            int result = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;
                    result = cmd.ExecuteNonQuery();
                    _hasAct = true;
                }
                else
                {
                    _hasErr = true;
                }
            }
            catch (Exception err)
            {
                //Tencent.OA.App.Logger.Write(err);
                _hasErr = true;
                result = -1;
            }
            return result;
        }
        public int ExcuteSQL(string sql, MySqlParameter[] paras)
        {
            int result = 0;
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = con;
                    cmd.Transaction = tran;
                    cmd.CommandText = sql;
                    if (paras != null)
                    {
                        foreach (MySqlParameter p in paras)
                        {
                            cmd.Parameters.Add(p);
                        }
                    }
                    result = cmd.ExecuteNonQuery();
                    _hasAct = true;
                }
                else
                    _hasErr = true;
            }
            catch (Exception err)
            {
                _hasErr = true;
                result = -1;
                //Tencent.OA.App.Logger.Write(err);
            }
            return result;
        }
        public void Commit()
        {
            if (_hasAct && !_hasErr)
                tran.Commit();
            else
                if (_hasAct)
                    tran.Rollback();
            _hasAct = false;
            _hasErr = false;
        }
        public void Roback()
        {
            if (_hasAct)
                tran.Rollback();
            _hasAct = false;
            _hasErr = false;
        }
        public void Dispose()
        {
            if (tran != null)
                tran.Dispose();
            if (con != null)
                con.Dispose();
        }
    }
}
