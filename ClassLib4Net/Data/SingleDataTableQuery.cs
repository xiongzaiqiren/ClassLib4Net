using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace ClassLib4Net.Data
{
	/// <summary>
	/// 本类是一个工具类，功能是根据DataRow、DataTable对象生成Sql语句
	/// 注意：只支持DataRow中属性类型如下：
	/// Boolean、Byte、Char、DateTime、Double、Int16、
	/// Int32、Int64、SByte、Single、String、TimeSpan、UInt16、UInt32、UInt64，
	/// 只有这些类型的DataRow属性能作为条件生成Where子句，
	/// 当DataRow数字类型属性为0时或DateTime类型的值为最小时间时，不会作为Where子句中的条件
	/// </summary>
	public class SelectSqlBuilder
	{
		/// <summary>
		/// 根据DataRow对象生成Select Sql语句
		/// </summary>
		/// <param name="row">DataRow对象</param>
		/// <returns>Select Sql语句</returns>
		public static string BuildSelectSql(DataRow row)
		{
			return " SELECT " + BuildSelectList(row.Table)
				+ " FROM " + BuildFromList(row)
				+ " WHERE " + BuildWhere(row);
		}

		/// <summary>
		/// 生成Sql语句的Select List
		/// </summary>
		/// <param name="table">DataTable对象</param>
		/// <returns>Sql语句的Select List 字串</returns>
		public static string BuildSelectList(DataTable table)
		{
			return BuildSelectList(table, false);
		}

		/// <summary>
		/// 生成Sql语句的Select List，如果isChangeColumnName为true,
		/// 则把表明作为Select List中字段别名的前缀，
		/// 如table1.column1字段在Select List中的别名为：table1_column1
		/// </summary>
		/// <param name="table">DataTable对象</param>
		/// <param name="isChangeColumnName">是否把表名作为Select List中字段别名的前缀</param>
		/// <returns>Sql语句的Select List 字串</returns>
		public static string BuildSelectList(DataTable table, bool isChangeColumnName)
		{
			StringBuilder rst = new StringBuilder("");
			foreach (DataColumn column in table.Columns)
			{
				if (rst.Length > 0)
				{
					rst.Append(", ");
				}
				if (table.TableName.Length > 0)
				{
					rst.Append(table.TableName + ".");
				}
				rst.Append(column.ColumnName);
				if (isChangeColumnName)
				{
					rst.Append(" " + table.TableName + "_" + column.ColumnName);
				}
			}
			return rst.ToString();
		}

		private static string BuildFromList(DataRow row)
		{
			return row.Table.TableName;
		}

		/// <summary>
		/// 创建带参数的sql语句的where条件
		/// </summary>
		/// <param name="row">DataRow对象</param>
		/// <returns></returns>
		private static string BuildWhere(DataRow row)
		{
			StringBuilder rst = new StringBuilder("");
			foreach (DataColumn column in row.Table.Columns)
			{
				if (!IsNeedAddToSql(row, column))
				{
					continue;
				}
				if (rst.Length > 0)
				{
					rst.Append(" AND ");
				}
				if (row.Table.TableName.Length > 0)
				{
					rst.Append(row.Table.TableName + ".");
				}
				rst.Append(column.ColumnName);
				rst.Append(" = @");
				rst.Append(row.Table.TableName + "_" + column.ColumnName);
			}
			return rst.ToString();
		}

		/// <summary>
		///		创建Where条件用到的sql参数，
		/// </summary>
		/// <param name="row">参数条件的具体值从此DataRow中取</param>
		/// <returns></returns>
		public static SqlParameter[] BuildParameter(DataRow row)
		{
			ArrayList rst = new ArrayList();
			foreach (DataColumn column in row.Table.Columns)
			{
				if (!IsNeedAddToSql(row, column))
				{
					continue;
				}
				string paramName = "@" + row.Table.TableName + "_" + column.ColumnName;
				rst.Add(new SqlParameter(paramName, row[column.ColumnName]));
			}
			return (SqlParameter[])rst.ToArray(typeof(SqlParameter));
		}

		private static bool IsNeedAddToSql(DataRow row, DataColumn column)
		{
			if (!IsSupportedType(column.DataType))
			{
				return false;
			}
			if (row[column.ColumnName] == DBNull.Value)
			{
				return false;
			}
			if (IsNumberType(column.DataType) && row[column.ColumnName].Equals(0))
			{
				return false;
			}
			if (column.DataType.Equals(typeof(DateTime))
				&& DateTime.MinValue.Equals(row[column.ColumnName])
				)
			{
				return false;
			}
			return true;
		}

		private static bool IsSupportedType(Type type)
		{
			switch (type.Name)
			{
				case "Boolean":
					return true;
				case "Byte":
					return true;
				case "Char":
					return true;
				case "DateTime":
					return true;
				case "Double":
					return true;
				case "Int16":
					return true;
				case "Int32":
					return true;
				case "Int64":
					return true;
				case "SByte":
					return true;
				case "Single":
					return true;
				case "String":
					return true;
				case "TimeSpan":
					return true;
				case "UInt16":
					return true;
				case "UInt32":
					return true;
				case "UInt64":
					return true;
				default:
					return false;
			}
		}

		private static bool IsNumberType(Type type)
		{
			switch (type.Name)
			{
				case "Byte":
					return true;
				case "Double":
					return true;
				case "Int16":
					return true;
				case "Int32":
					return true;
				case "Int64":
					return true;
				case "SByte":
					return true;
				case "Single":
					return true;
				case "UInt16":
					return true;
				case "UInt32":
					return true;
				case "UInt64":
					return true;
				default:
					return false;
			}
		}
	}

	/// <summary>
	/// 本辅助类利用SelectSqlBuilder类实现单个表的数据读取，
	/// 调用方只需指定一个DataRow对象和数据库连接字符串就可以。
	/// 注意：只支持DataRow中属性类型如下：
	/// Boolean、Byte、Char、DateTime、Double、Int16、
	/// Int32、Int64、SByte、Single、String、TimeSpan、UInt16、UInt32、UInt64，
	/// 只有这些类型的DataRow属性能作为条件生成Where子句，
	/// 当DataRow数字类型属性为0时或DateTime类型的值为最小时间时，不会作为Where子句中的条件，
	/// 当你的DataRow对象只有一个数字类型且值为零时，并不能查询出该属性（字段）等于0的记录，
	/// 而是查询出所有的记录。
	/// </summary>
	public class SingleDataTableQuery
	{
		#region GetDataSet
		/// <summary>
		/// 从数据库中取得满足参数paramRow中各值"与/and"条件的DataSet
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <param name="paramRow">DataRow参数值</param>
		/// <returns>符合paramRow条件的DataSet</returns>
		public static DataSet GetDataSet(string connectionString, DataRow paramRow)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return GetDataSet(connection, paramRow);
			}
		}

		/// <summary>
		/// 从数据库中取得满足参数paramRow中各值"与/and"条件的DataSet
		/// </summary>
		/// <param name="connection">数据库连接对象</param>
		/// <param name="paramRow">DataRow参数值</param>
		/// <returns>符合paramRow条件的DataSet</returns>
		public static DataSet GetDataSet(SqlConnection connection, DataRow paramRow)
		{
			DataSet dataSet = (DataSet)paramRow.Table.DataSet.GetType().GetConstructor(new Type[0]).Invoke(null);
			FillDataSet(connection, paramRow, dataSet);
			return dataSet;
		}

		/// <summary>
		/// 从数据库中取得满足参数paramRow中各值"与/and"条件的DataSet,
		/// 并且以orderSql、startRecord和maxRecords作为分页条件
		/// </summary>
		/// <param name="connectionString">数据库连接字符串</param>
		/// <param name="paramRow">DataRow参数值</param>
		/// <param name="orderSql">排序Sql子句</param>
		/// <param name="startRecord">起始记录号，最小值为0</param>
		/// <param name="maxRecords">一次返回的最大记录数</param>
		/// <returns>符合paramRow条件的DataSet</returns>
		public static DataSet GetDataSet(
			string connectionString,
			DataRow paramRow,
			string orderSql,
			int startRecord,
			int maxRecords)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				return GetDataSet(connection, paramRow, orderSql, startRecord, maxRecords);
			}
		}

		/// <summary>
		/// 从数据库中取得满足参数paramRow中各值"与/and"条件的DataSet,
		/// 并且以orderSql、startRecord和maxRecords作为分页条件
		/// </summary>
		/// <param name="connection">数据库连接对象</param>
		/// <param name="paramRow">DataRow参数值</param>
		/// <param name="orderSql">排序Sql子句</param>
		/// <param name="startRecord">起始记录号，最小值为0</param>
		/// <param name="maxRecords">一次返回的最大记录数</param>
		/// <returns>符合paramRow条件的DataSet</returns>
		public static DataSet GetDataSet(
			SqlConnection connection,
			DataRow paramRow,
			string orderSql,
			int startRecord,
			int maxRecords)
		{
			DataSet dataSet = (DataSet)paramRow.Table.DataSet.GetType()
				.GetConstructor(new Type[0]).Invoke(null);
			FillDataSet(connection, paramRow, orderSql, startRecord, maxRecords, dataSet);
			return dataSet;
		}
		#endregion

		#region FillDataSet 功能与GetDataSet()方法相同，只是由调用方传入DataSet，而不是新建DataSet
		/// <summary>
		/// 所有FillDataSet与GetDataSet方法功能相同，只是由调用方传入DataSet，而不是新建DataSet
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="paramRow"></param>
		/// <param name="dataSet"></param>
		public static void FillDataSet(string connectionString, DataRow paramRow, DataSet dataSet)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				FillDataSet(connection, paramRow, dataSet);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="paramRow"></param>
		/// <param name="dataSet"></param>
		public static void FillDataSet(SqlConnection connection, DataRow paramRow, DataSet dataSet)
		{
			CreateDataAdapter(connection, paramRow).Fill(dataSet, paramRow.Table.TableName);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="paramRow"></param>
		/// <param name="orderSql"></param>
		/// <param name="startRecord"></param>
		/// <param name="maxRecords"></param>
		/// <param name="dataSet"></param>
		public static void FillDataSet(
			string connectionString,
			DataRow paramRow,
			string orderSql,
			int startRecord,
			int maxRecords,
			DataSet dataSet)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				FillDataSet(connection, paramRow, orderSql, startRecord, maxRecords, dataSet);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="paramRow"></param>
		/// <param name="orderSql"></param>
		/// <param name="startRecord"></param>
		/// <param name="maxRecords"></param>
		/// <param name="dataSet"></param>
		public static void FillDataSet(
			SqlConnection connection,
			DataRow paramRow,
			string orderSql,
			int startRecord,
			int maxRecords,
			DataSet dataSet)
		{
			CreateDataAdapter(connection, paramRow, orderSql)
				.Fill(dataSet, startRecord, maxRecords, paramRow.Table.TableName);
		}
		#endregion

		#region FillDataTable
		/// <summary>
		/// 所有FillDataSet与GetDataSet方法功能相同，
		/// 只是由调用方传入DataTable，把数据填入此DataTable
		/// </summary>
		/// <param name="connectionString"></param>
		/// <param name="paramRow"></param>
		/// <param name="dataTable"></param>
		public static void FillDataTable(string connectionString, DataRow paramRow, DataTable dataTable)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				FillDataTable(connection, paramRow, dataTable);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="paramRow"></param>
		/// <param name="dataTable"></param>
		public static void FillDataTable(SqlConnection connection, DataRow paramRow, DataTable dataTable)
		{
			CreateDataAdapter(connection, paramRow).Fill(dataTable);
		}
		#endregion

		#region CreateDataAdapter
		private static SqlDataAdapter CreateDataAdapter(
			SqlConnection connection,
			DataRow paramRow)
		{
			return CreateDataAdapter(connection, paramRow, "");
		}

		private static SqlDataAdapter CreateDataAdapter(
			SqlConnection connection,
			DataRow paramRow,
			string orderSql)
		{
			string sql = SelectSqlBuilder.BuildSelectSql(paramRow) + " " + orderSql;
			SqlParameter[] sqlParams = SelectSqlBuilder.BuildParameter(paramRow);
			SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
			foreach (SqlParameter sqlParam in sqlParams)
			{
				adapter.SelectCommand.Parameters.Add(sqlParam);
			}
			return adapter;
		}
		#endregion
	}
}
