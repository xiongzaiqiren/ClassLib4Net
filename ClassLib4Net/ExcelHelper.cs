using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Reflection;
using System.IO;
using System.Data.OleDb;

namespace ClassLib4Net
{
	/// <summary>
	/// 调用Com组件Excel帮助类
	/// </summary>
	public class ExcelHelper
	{
		private static object missing = Missing.Value;

		#region 将dataset数据集生成excel单个sheet保存到服务器
		
		#endregion

		#region 将dataset数据集生成excel多个sheet保存到服务器
		
		#endregion

		

		#region 将dataset数据生成excel下载到客户端
		
		#endregion

		#region 将dataset数据下载到客户端
		
		#endregion

		#region 将dataset数据下载到客户端
		
		#endregion

		#region 内部成员方法

		/// <summary>
		/// 根据指定值返回所在数组索引号
		/// </summary>
		/// <param name="list"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static int GetRangIndex(string[] list, string obj)
		{
			for (int k = 0; k < list.Length; k++)
			{
				if (list[k] == obj)
				{
					return k;
				}
			}
			return -1;
		}
        
		#endregion

		#region 导出与生成Excel 20150515
		/// <summary>
		/// 生成Excel报表
		/// </summary>
		/// <param name="ds">需要生成数据的的DataSet</param>
		/// <param name="exportPath">输出路径</param>
		/// <param name="columnName">数据库字段名</param>
		/// <param name="columnTitle">改变数据库字段名的名字</param>
		/// <param name="exportFileName">导出Excle文件名（短文件名，无需后缀）</param>
		/// <param name="errorMessage">出错信息</param>
		/// <returns>返回路径</returns>
		public static string Export(DataSet ds, string[] columnName, string[] columnTitle, string exportPath, string exportFileName, out string errorMessage)
		{
			string rtn = string.Empty;
			int i = 0;
			int j = 0;
			int outLength = 0; //需要显示的字段数
			outLength = columnName.Length;
			string outExcelString = string.Empty;
			string excelDataString = string.Empty;
			errorMessage = string.Empty;
			string tableTab = "\t"; //制表位
			string enterTab = "\n"; //软回车

			//定义表对象与行对像，同时用DataSet对其值进行初始化 
			System.Data.DataTable execDt = ds.Tables[0];
			DataRow execDr;

			try
			{
				if (outLength != 0)
				{
					for (i = 0; i <= execDt.Columns.Count - 1; i++)
					{
						if (i >= outLength)
						{
							break;
						}
						if (execDt.Columns[i].ColumnName.ToString() == columnName[i].ToString())
						{
							outExcelString += columnTitle[i] + tableTab;
						}
					}
				}
				else
				{
					for (i = 0; i <= execDt.Columns.Count - 1; i++)
					{
						outExcelString += execDt.Columns[i].ColumnName.ToString() + tableTab;
					}
				}

				outExcelString = outExcelString + enterTab;

				for (i = 0; i <= execDt.Rows.Count - 1; i++)
				{
					execDr = execDt.Rows[i];
					excelDataString = string.Empty;
					for (j = 0; j <= execDt.Columns.Count - 1; j++)
					{
						if (j >= outLength)
						{
							break;
						}
						excelDataString = excelDataString + execDr[j].ToString().Trim() + tableTab;
					}
					excelDataString += enterTab;
					outExcelString = outExcelString + excelDataString;
				}
				//生成文件
				System.Byte[] outByte;
				outByte = System.Text.Encoding.Default.GetBytes(outExcelString);

				System.IO.FileStream outFileStream;
				exportFileName = string.IsNullOrEmpty(exportFileName)
					? DateTime.Now.ToString("yyyy-MM-dd HH-mm")
					: exportFileName;
				exportPath += "\\" + exportFileName + ".xls";
				outFileStream = new System.IO.FileStream(exportPath, System.IO.FileMode.Create);

				outFileStream.Write(outByte, 0, outByte.Length);
				outFileStream.Close();

				//释放资源
				execDt.Dispose();
				GC.Collect();

				rtn = exportFileName + ".xls"; //返回值
				errorMessage = "ok"; //导出成功
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
				rtn = string.Empty;
			}
			return rtn;
		}

		/// <summary>
		/// 导出Excle
		/// </summary>
		/// <typeparam name="T">类对象</typeparam>
		/// <param name="list">对象数据集合</param>
		/// <param name="column">类字段，字段对应列名</param>
		/// <param name="exportFileName">导出Excle文件名（短文件名，无需后缀）</param>
		public static void Export<T>(List<T> list, Dictionary<string, string> column, string exportFileName)
		{
			if (list == null || list.Count == 0 || column == null || column.Count == 0)
			{
				return;
			}

			StringWriter sw = new StringWriter();
			//-------------------------------表头读取开始------------------------------------------------
			string title = string.Empty;
			foreach (KeyValuePair<string, string> kvp in column)
			{
				title += kvp.Value + "\t";
			}
			title = title.Substring(0, title.LastIndexOf("\t"));
			sw.WriteLine(title);
			//-------------------------------表头读取结束--------------------------------------------------------
			//--------------------------------数据读取start----------------------------------------------------------------------------------
			Type objType = typeof(T);
			BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
			//反射标识 
			PropertyInfo[] propInfoArr = objType.GetProperties(bf); //获取映射列表
			foreach (T model in list)
			{
				System.Text.StringBuilder data = new System.Text.StringBuilder();
				foreach (string key in column.Keys)
				{
					foreach (PropertyInfo propInfo in propInfoArr)
					{
						if (key == propInfo.Name) //判断头相对应的字段 
						{
							PropertyInfo modelProperty = model.GetType().GetProperty(propInfo.Name);
							if (modelProperty != null)
							{
								object objResult = modelProperty.GetValue(model, null); //获取值                        
								data.Append(((objResult == null) ? string.Empty : objResult) + "\t");
							}
						}
					}
				}
				var temp = data.ToString();
				temp = temp.Substring(0, temp.LastIndexOf("\t"));
				sw.WriteLine(temp);
			}
			//------------------------------------------end----------------------------------------------------------------------------------
			sw.Close(); //读取数据结束
			//-----------------------------------输出excel-------------------------------------------------------------
			System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + exportFileName + ".xls");
			System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
			System.Web.HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
			System.Web.HttpContext.Current.Response.Write(sw.ToString());
			System.Web.HttpContext.Current.Response.End();
			//-------------------------------------------------------------------------------------------------------------               
		}

		#endregion

		#region 读取与操作Excel工作表 20150515
		/// <summary>
		/// 获取Excel工作表名
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static String[] GetExcelSheetNames(ExcelConnectionString connection)
		{
			List<String> listTableName = new List<String>();
			using (OleDbConnection con = new OleDbConnection(connection.ConnectionString))
			{
				if (con.State != ConnectionState.Open) con.Open();
				System.Data.DataTable dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
				if (dt != null && dt.Rows != null)
					foreach (DataRow row in dt.Rows)
						listTableName.Add(row["TABLE_NAME"].ToString().Replace("$", ""));
				return listTableName.ToArray();
			}
		}

		/// <summary>
		/// 获取数据集
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="SheetName"></param>
		/// <returns></returns>
		public static DataSet GetDS(ExcelConnectionString connection, string SheetName)
		{
			if (string.IsNullOrWhiteSpace(SheetName)) return GetDS(connection);
			string _SheetName = (SheetName.LastIndexOf("$") == SheetName.Length - 1) ? SheetName : (SheetName + "$");
			string select = string.Format("SELECT * FROM [{0}];", _SheetName);
			using (OleDbConnection con = new OleDbConnection(connection.ConnectionString))
			{
				if (con.State != ConnectionState.Open) con.Open();
				DataSet ds = new DataSet();
				using (OleDbDataAdapter oda = new OleDbDataAdapter(select, con))
				{
					oda.Fill(ds, SheetName);
					return ds;
				}
			}
		}

		/// <summary>
		/// 获取数据集
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static DataSet GetDS(ExcelConnectionString connection)
		{
			string[] SheetNames = GetExcelSheetNames(connection);
			if (SheetNames != null && SheetNames.Length > 0)
			{
				DataSet ds = new DataSet();
				DataSet dsItem;
				foreach (string SheetName in SheetNames)
				{
					dsItem = GetDS(connection, SheetName);
					ds.Tables.Add(dsItem.Tables[0].Copy());
				}
				return ds;
			}
			return null;
		}

		/// <summary>
		/// 移除工作表
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="SheetName"></param>
		/// <returns></returns>
		public static int DropSheet(ExcelConnectionString connection, string SheetName)
		{
			string _SheetName = (SheetName.LastIndexOf("$") == SheetName.Length - 1) ? SheetName : (SheetName + "$");
			string drop = string.Format("Drop Table [{0}];", _SheetName);
			using (OleDbConnection con = new OleDbConnection(connection.ConnectionString))
			{
				using (OleDbCommand cmd = new OleDbCommand(drop, con))
				{
					if (con.State != ConnectionState.Open) con.Open();
					return cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>  
		/// Execute new command  
		/// </summary>  
		/// <param name="command">Command</param>  
		public static int ExecuteCommand(ExcelConnectionString connection, string command)
		{
			using (OleDbConnection con = new OleDbConnection(connection.ConnectionString))
			{
				using (OleDbCommand cmd = new OleDbCommand(command, con))
				{
					if (con.State != ConnectionState.Open) con.Open();
					return cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>  
		/// Generates create table script  
		/// </summary>  
		/// <param name="tableName">Table Name（注意Excel表名最后的字符$）</param>  
		/// <param name="tableDefinition">Table Definition</param>  
		/// <returns>Create table script</returns>  
		private static string GenerateCreateTable(string tableName, Dictionary<string, string> tableDefinition)
		{
			StringBuilder sb = new StringBuilder();
			bool firstcol = true;
			sb.AppendFormat("CREATE TABLE [{0}](", tableName);
			firstcol = true;
			foreach (KeyValuePair<string, string> keyvalue in tableDefinition)
			{
				if (!firstcol)
				{
					sb.Append(",");
				}
				firstcol = false;
				sb.AppendFormat("{0} {1}", keyvalue.Key, keyvalue.Value);
			}

			sb.Append(")");
			return sb.ToString();
		}

		/// <summary>  
		/// Generates insert statement script  
		/// </summary>  
		/// <param name="dr">Data row</param>  
		/// <returns>Insert statement script</returns>  
		private static string GenerateInsertStatement(DataRow dr)
		{
			StringBuilder sb = new StringBuilder();
			bool firstcol = true;
			sb.AppendFormat("INSERT INTO [{0}](", dr.Table.TableName);


			foreach (DataColumn dc in dr.Table.Columns)
			{
				if (!firstcol)
				{
					sb.Append(",");
				}
				firstcol = false;

				sb.Append(dc.Caption);
			}

			sb.Append(") VALUES(");
			firstcol = true;
			for (int i = 0; i <= dr.Table.Columns.Count - 1; i++)
			{
				if (!object.ReferenceEquals(dr.Table.Columns[i].DataType, typeof(int)))
				{
					sb.Append("'");
					sb.Append(dr[i].ToString().Replace("'", "''"));
					sb.Append("'");
				}
				else
				{
					sb.Append(dr[i].ToString().Replace("'", "''"));
				}
				if (i != dr.Table.Columns.Count - 1)
				{
					sb.Append(",");
				}
			}

			sb.Append(")");
			return sb.ToString();
		}

		/// <summary>  
		/// Create Table  
		/// </summary>  
		/// <param name="tableName">Table Name（注意Excel表名最后的字符$）</param>  
		/// <param name="tableDefinition">Table Definition</param>  
		public static int CreateTable(ExcelConnectionString connection, string tableName, Dictionary<string, string> tableDefinition)
		{
			using (OleDbConnection con = new OleDbConnection(connection.ConnectionString))
			{
				string command = GenerateCreateTable(tableName, tableDefinition);
				using (OleDbCommand cmd = new OleDbCommand(command, con))
				{
					if (con.State != ConnectionState.Open) con.Open();
					return cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>  
		/// Add new row  
		/// </summary>  
		/// <param name="dr">Data Row</param>  
		public static int AddNewRow(ExcelConnectionString connection, DataRow dr)
		{
			string command = GenerateInsertStatement(dr);
			return ExecuteCommand(connection, command);
		}

		#endregion
	}

	/// <summary>
	/// Excel数据库链接字符串
	/// </summary>
	public class ExcelConnectionString
	{
		private string _DataSource = string.Empty;
		private bool _HDR = true;
		private int _IMEX = 2;
		private bool _PersistSecurityInfo = false;

		/// <summary>
		/// Data Source即文件路径（完整的物理路径）
		/// </summary>
		public string DataSource { get { return _DataSource; } set { _DataSource = value; } }
		/// <summary>
		/// 参数HDR的值：HDR=Yes，这代表第一行是标题，不做为数据使用 ，如果用HDR=NO，则表示第一行不是标题，做为数据来使用。系统默认的是YES
		/// 缺省为YES
		/// </summary>
		public bool HDR { get { return _HDR; } set { _HDR = value; } }
		/// <summary>
		/// IMEX ( IMport EXport mode )
		/// 当 IMEX=0 时为“汇出模式”，这个模式开启的 Excel 档案只能用来做“写入”用途。
		/// 当 IMEX=1 时为“汇入模式”，这个模式开启的 Excel 档案只能用来做“读取”用途。
		/// 当 IMEX=2 时为“连結模式”，这个模式开启的 Excel 档案可同时支援“读取”与“写入”用途(完全更新能力)。
		/// 缺省为2
		/// </summary>
		public int IMEX { get { return _IMEX; } set { _IMEX = value; } }

		/// <summary>
		/// ADO在数据库连接成功后是否保存密码信息"，
		/// True表示保存，False表示不保存
		/// 缺省为False
		/// </summary>
		public bool PersistSecurityInfo { get { return _PersistSecurityInfo; } set { _PersistSecurityInfo = value; } }


		private string _ConnectionString = "Provider=Microsoft.{0}.OLEDB.{1}; Persist Security Info={2};Data Source={3};Extended Properties=\"Excel {4};HDR={5};IMEX={6}\"";

		/// <summary>
		/// 链接字符串（支持.xls|.xlsx后缀名的Excel文件）
		/// </summary>
		public string ConnectionString
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this._DataSource))
					return "";
				else
				{
					FileInfo fi = new FileInfo(this._DataSource);
					if (fi.Extension.ToLower().Equals(".xls"))
					{
						return string.Format(this._ConnectionString, "Jet", "4.0", this._PersistSecurityInfo, this._DataSource, "8.0", this._HDR ? "YES" : "NO", this._IMEX);
					}
					else if (fi.Extension.ToLower().Equals(".xlsx"))
					{
						return string.Format(this._ConnectionString, "Ace", "12.0", this._PersistSecurityInfo, this._DataSource, "12.0", this._HDR ? "YES" : "NO", this._IMEX);
					}
					return "未识别的文件路径后缀名";
				}
			}
		}

		/*
		 * 对于Excel 97以上版本都用Excel 8.0
		 * 	string fileType = System.IO.Path.GetExtension(filePath);
			if (string.IsNullOrEmpty(fileType)) return null;

			if (fileType == ".xls")
				connStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
			else
				connStr = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
		 
						  "Provider=Microsoft.ACE.OLEDB.12.0; Persist Security Info=False;Data Source=" + @Excfilename + "; Extended Properties=/"Excel 8.0;HDR=Yes;IMEX=2/"";
		 */

		/// <summary>
		/// 带参数的构造函数
		/// </summary>
		/// <param name="DataSource"></param>
		/// <param name="HDR"></param>
		/// <param name="IMEX"></param>
		/// <param name="PersistSecurityInfo"></param>
		public ExcelConnectionString(string DataSource, bool HDR = true, int IMEX = 2, bool PersistSecurityInfo = false)
		{
			this.DataSource = DataSource;
			this.HDR = HDR;
			this.IMEX = IMEX;
			this.PersistSecurityInfo = PersistSecurityInfo;
		}
	}
}