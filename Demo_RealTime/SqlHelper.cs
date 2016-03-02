using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
    class SqlHelper
    {
        static string connStr = "server=.;database=hrkf;user=sa;pwd=sasa";
        //执行查询语句，返回一个值
        public static object GetOneData(string sql, CommandType ctype, params SqlParameter[] sps)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand com = new SqlCommand(sql, conn);
            com.CommandType = ctype;
            if (sps != null)
                com.Parameters.AddRange(sps);
            object ob =  com.ExecuteScalar(); 
            conn.Close();
            return ob;
        }
        //执行查询语句，返回结果集
        public static SqlDataReader GetReader(string sql, CommandType ctype, params SqlParameter[] sps)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand com = new SqlCommand(sql, conn);
            com.CommandType = ctype;
            if (sps != null)
                com.Parameters.AddRange(sps);
            SqlDataReader sdr = com.ExecuteReader(CommandBehavior.CloseConnection);
            return sdr;
        }
        //执行查询语句，返回DataSet
        public static DataSet GetDS(string sql, CommandType ctype, params SqlParameter[] sps)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand com = new SqlCommand(sql, conn);
            com.CommandType = ctype;
            if (sps != null)
                com.Parameters.AddRange(sps);
            SqlDataAdapter sda = new SqlDataAdapter(com);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            conn.Close();
            return ds;
        }
        //执行增删改的Sql语句，返回受影响的行
        public static int ExeSql(string sql, CommandType ctype, params SqlParameter[] sps)
        {
            SqlConnection conn = new SqlConnection(connStr);
            conn.Open();
            SqlCommand com = new SqlCommand(sql, conn);
            com.CommandType = ctype;
            if (sps != null)
                com.Parameters.AddRange(sps);
            int i = com.ExecuteNonQuery();
            conn.Close();
            return i;
        }
    }

