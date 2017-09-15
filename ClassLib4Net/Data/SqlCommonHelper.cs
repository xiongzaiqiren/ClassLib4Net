using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace ClassLib4Net.Data
{
	/// <summary>
	/// SQL助手
	/// 熊学浩
	/// 2015-01-21
	/// </summary>
	public class SqlCommonHelper
	{
		#region 执行带参数的SQL语句

		/// <summary>
		/// 执行SQL语句，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <param name="connectionString">链接字符串</param>
		/// <param name="cmdParms">参数</param>
		/// <returns>影响的记录数</returns>
		public static int ExecuteSql(string SQLString, string connectionString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						int rows = cmd.ExecuteNonQuery();
						cmd.Parameters.Clear();
						return rows;
					}
					catch (System.Data.SqlClient.SqlException E)
					{
						throw new Exception(E.Message);
					}
				}
			}
		}
		/// <summary>
		/// 执行SQL语句，SQL事物，返回影响的记录数
		/// </summary>
		/// <param name="SQLString">SQL语句</param>
		/// <param name="connection">链接字符串</param>
		/// <param name="trans">SQL事物</param>
		/// <param name="cmdParms">参数</param>
		/// <returns></returns>
		public static int ExecuteSql(string SQLString, SqlConnection connection, SqlTransaction trans, params SqlParameter[] cmdParms)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				try
				{
					PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
					int rows = cmd.ExecuteNonQuery();
					cmd.Parameters.Clear();
					return rows;
				}
				catch (System.Data.SqlClient.SqlException E)
				{
					throw new Exception(E.Message);
				}
			}
		}

		/// <summary>
		/// 执行多条SQL语句，实现数据库事务。
		/// </summary>
		/// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
		/// <param name="connectionString">链接字符串</param>
		/// <returns></returns>
		public static int ExecuteSqlTran(Hashtable SQLStringList, string connectionString)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction())
				{
					SqlCommand cmd = new SqlCommand();
					try
					{
						int rows = 0;
						//循环
						foreach (DictionaryEntry myDE in SQLStringList)
						{
							string cmdText = myDE.Key.ToString();
							SqlParameter[] cmdParms = (SqlParameter[])myDE.Value;
							PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
							int val = cmd.ExecuteNonQuery();
							cmd.Parameters.Clear();

							trans.Commit();

							rows += val;
						}
						return rows;
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}
		}


		/// <summary>
		/// 执行一条计算查询结果语句，返回查询结果（object）。
		/// </summary>
		/// <param name="SQLString">计算查询结果语句</param>
		/// <returns>查询结果（object）</returns>
		public static object GetSingle(string SQLString, string connectionString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						PrepareCommand(cmd, connection, null, SQLString, cmdParms);
						object obj = cmd.ExecuteScalar();
						cmd.Parameters.Clear();
						if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
						{
							return null;
						}
						else
						{
							return obj;
						}
					}
					catch (System.Data.SqlClient.SqlException e)
					{
						throw new Exception(e.Message);
					}
				}
			}
		}
		public static object GetSingle(string SQLString, SqlConnection connection, SqlTransaction trans, params SqlParameter[] cmdParms)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				try
				{
					PrepareCommand(cmd, connection, trans, SQLString, cmdParms);
					object obj = cmd.ExecuteScalar();
					cmd.Parameters.Clear();
					if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
					{
						return null;
					}
					else
					{
						return obj;
					}
				}
				catch (System.Data.SqlClient.SqlException e)
				{
					throw new Exception(e.Message);
				}
			}
		}

		/// <summary>
		/// 执行查询语句，返回SqlDataReader ( 注意：使用后一定要对SqlDataReader进行Close )
		/// </summary>
		/// <param name="strSQL">查询语句</param>
		/// <returns>SqlDataReader</returns>
		public static SqlDataReader ExecuteReader(string SQLString, string connectionString, params SqlParameter[] cmdParms)
		{
			SqlConnection connection = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand();
			try
			{
				PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				SqlDataReader myReader = cmd.ExecuteReader();
				cmd.Parameters.Clear();
				return myReader;
			}
			catch (System.Data.SqlClient.SqlException e)
			{
				throw new Exception(e.Message);
			}
			finally
			{
				cmd.Dispose();
				connection.Close();
			}

		}

		/// <summary>
		/// 执行查询语句，返回DataSet
		/// </summary>
		/// <param name="SQLString">查询语句</param>
		/// <returns>DataSet</returns>
		public static DataSet Query(string SQLString, string connectionString, params SqlParameter[] cmdParms)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand();
				PrepareCommand(cmd, connection, null, SQLString, cmdParms);
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					DataSet ds = new DataSet();
					try
					{
						da.Fill(ds, "ds");
						cmd.Parameters.Clear();
					}
					catch (System.Data.SqlClient.SqlException ex)
					{
						throw new Exception(ex.Message);
					}
					return ds;
				}
			}
		}


		private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
		{
			if (conn.State != ConnectionState.Open)
				conn.Open();
			cmd.Connection = conn;
			cmd.CommandText = cmdText;
			if (trans != null)
				cmd.Transaction = trans;
			cmd.CommandType = CommandType.Text;//cmdType;
			if (cmdParms != null)
			{
				foreach (SqlParameter parameter in cmdParms)
				{
					if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
					{
						parameter.Value = DBNull.Value;
					}
					cmd.Parameters.Add(parameter);
				}
			}
		}

		#endregion

		#region 数据验证
		/// <summary>
		/// SQL注入关键字符号组,检测字符串是否包含关键字，符号，运算符等不利于数据库安全的因素
		/// 作者：熊学浩
		/// 日期：2013-6-18
		/// </summary>
		/// <param name="_txt">需要验证的字符串</param>
		/// <returns></returns>
		public static bool CheckSql(string _txt)
		{
			string m_EngChsFilter = @"( (and|exec|insert|select|delete|update|count|chr|mid|master|truncate|char|declare|or) ){1}"; //防止SQL注入
			return System.Text.RegularExpressions.Regex.IsMatch(_txt.ToLower(), m_EngChsFilter);
		}

		/// <summary>
		/// 替换字符串中出现的英文单引号为两个连续的单引号，避免sql出错
		/// </summary>
		/// <param name="_txt">需要替换的字符串</param>
		/// <returns></returns>
		public static string ReplaceQuotes(string _txt)
		{
			return System.Text.RegularExpressions.Regex.Replace(_txt, "'", "''");
		}

		/// <summary>
		/// 验证DataSet是否不为空
		/// 作者：熊学浩
		/// 日期：2015-01-21
		/// </summary>
		/// <param name="ds">DataSet</param>
		/// <returns></returns>
		public static bool CheckDS(DataSet ds)
		{
			if (ds != null && ds.Tables != null && ds.Tables.Count > 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// 验证DataTable是否不为空
		/// 作者：熊学浩
		/// 日期：2013-6-18
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <returns></returns>
		public static bool CheckDT(DataTable dt)
		{
			if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// 验证DataRow是否不为空
		/// 作者：熊学浩
		/// 日期：2015-01-21
		/// </summary>
		/// <param name="dr">DataRow</param>
		/// <returns></returns>
		public static bool CheckDR(DataRow dr)
		{
			if (dr != null && dr.ItemArray != null && dr.ItemArray.Length > 0)
				return true;
			else
				return false;
		}

		#endregion
	}
}
