using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ClassLib4Net.Data.Paging
{
	/// <summary>
	/// 封装了SqlDataAdapter中的Fill分页算法，对于小数据量的分页比较方便。
	/// 但是大数据量的分页不推荐使用这个类。
	/// </summary>    
	public class SqlDataAdapterPagingWrapper
	{

		#region 获得分页数据的方法
		/// <summary>
		/// 获取分页数据
		/// </summary>
		/// <param name="sql">取数sql语句</param>
		/// <param name="pageIndex">起始的页面编号</param>
		/// <param name="pageSize">页面的记录数</param>
		/// <param name="connectionString">
		/// 数据库连接对象，如果连接对象未打开，本方法将会打开，
		/// 无论如何，本方法不会关闭连接，所以调用方应该显示关闭连接。
		/// </param>
		/// <returns>指定条件的分页数据</returns> 
		[System.Obsolete("大数据量时请不要用此方法")]
		public static DataSet GetDataSetByPage(string sql, int pageIndex, int pageSize, string connectionString)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				return GetDataSetByPage(sql, pageIndex, pageSize, conn);
			}
		}
		/// <summary>
		///     获取分页数据
		/// </summary>
		/// <param name="sql">取数sql语句</param>
		/// <param name="pageIndex">0 起始的页面编号</param>
		/// <param name="pageSize">页面的记录数</param>
		/// <param name="conn">
		///     数据库连接对象，如果连接对象未打开，本方法将会打开，
		///     无论如何，本方法不会关闭连接，所以调用方应该显示关闭连接。
		/// </param>
		/// <returns>指定条件的分页数据</returns>
		[System.Obsolete("大数据量时请不要用此方法")]
		public static DataSet GetDataSetByPage(string sql, int pageIndex, int pageSize, SqlConnection conn)
		{
			int startRecord = pageIndex * pageSize;
			SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
			DataSet ds = new DataSet();
			adapter.SelectCommand.CommandTimeout = 45;

			try
			{
				adapter.Fill(ds, startRecord, pageSize, "Table0");
				return ds;
			}
			finally
			{
				conn.Close();
			}
		}

		#endregion

		#region 获取分页数据的记录总数的方法

		/// <summary>
		///     获取分页数据的记录总数
		/// </summary>
		/// <param name="tableName">表名，可以是子查询</param>
		/// <param name="whereCondition">
		///     查询条件，可以是子查询，可以不带"where"前缀关键字。
		/// </param>
		/// <param name="groupBy">分组条件，可以不带"group by"关键字</param>
		/// <param name="connectionString">
		///     数据库连接字符串
		/// </param>
		/// <returns>分页数据的记录总数</returns>
		public static long GetDataCount(string tableName, string whereCondition, string groupBy, string connectionString)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				return GetDataCount(tableName, whereCondition, groupBy, conn);
			}
		}

		/// <summary>
		///     获取分页数据的记录总数
		/// </summary>
		/// <param name="tableName">表名，可以是子查询</param>
		/// <param name="whereCondition">
		///     查询条件，可以是子查询，不带"where"前缀关键字也可以。
		/// </param>
		/// <param name="groupBy">分组条件，可以不带"group by"关键字</param>
		/// <param name="conn">
		///     数据库连接对象，如果连接对象未打开，本方法将会打开，
		///     无论如何，本方法不会关闭连接，所以调用方应该显示关闭连接。
		/// </param>
		/// <returns>分页数据的记录总数</returns>
		public static long GetDataCount(string tableName, string whereCondition, string groupBy, SqlConnection conn)
		{
			if (string.IsNullOrEmpty(tableName))
			{
				return 0;
			}
			StringBuilder sql = new StringBuilder(" SELECT COUNT (*) FROM ");
			sql.Append(tableName);

			if (!string.IsNullOrEmpty(whereCondition))
			{
				if (!whereCondition.Trim().ToLower().StartsWith("where"))
				{
					sql.Append(" WHERE");
				}
				sql.Append(" ");
				sql.Append(whereCondition);
			}

			if (!string.IsNullOrEmpty(groupBy))
			{
				if (!groupBy.Trim().ToLower().StartsWith("group by"))
				{
					sql.Append(" GROUP BY");
				}
				sql.Append(" ");
				sql.Append(groupBy);
			}

			return GetDataCount(sql.ToString(), conn);
		}

		/// <summary>
		///     获取记录总数
		/// </summary>
		/// <param name="sql">
		///     形如"select count(*) from tablename" 的Sql语句
		/// </param>
		/// <param name="connectionString">
		///     数据库连接字符串
		/// </param>
		/// <returns>记录总数</returns>
		public static long GetDataCount(string sql, string connectionString)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				return GetDataCount(sql, conn);
			}
		}

		/// <summary>
		///     获取记录总数
		/// </summary>
		/// <param name="sql">
		///     形如"select count(*) from tablename" 的Sql语句
		/// </param>
		/// <param name="conn">
		///     数据库连接对象，如果连接对象未打开，本方法将会打开，
		///     无论如何，本方法不会关闭连接，所以调用方应该显示关闭连接。
		/// </param>
		/// <returns>记录总数</returns>
		public static long GetDataCount(string sql, SqlConnection conn)
		{

			try
			{
				if (conn.State == ConnectionState.Closed)
				{
					conn.Open();
				}
				SqlCommand sqlCommand = new SqlCommand(sql, conn);
				object rst = sqlCommand.ExecuteScalar();

				if (rst == null)
				{
					return 0;
				}

				return Convert.ToInt64(rst);
			}
			finally
			{
				conn.Close();
			}
		}

		#endregion

	}
}
