using System;
using System.Data;
using System.Data.OleDb;
using System.Collections;

namespace ClassLib4Net.Data
{
	/// <summary>
	/// 为进行 OleDbServer 数据库访问而做的封装
	/// </summary>
	public sealed class OleDbHelper
	{
		#region private utility methods & constructors

		//将类得默认构造函数置为私有，以防止用户创建类的实例
		private OleDbHelper() { }

		/// <summary>
		/// 此方法用力将OleDbParameter数组挂接到OleDbcommand 
		/// 
		/// This method will assign a value of DbNull to any parameter with a direction of
		/// InputOutput and a value of null.  
		/// 
		/// This behavior will prevent default values from being used, but
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput)
		/// where the user provided no input value.
		/// </summary>
		/// <param name="command">The command to which the parameters will be added</param>
		/// <param name="commandParameters">An array of OleDbParameters to be added to command</param>
		private static void AttachParameters(OleDbCommand command, OleDbParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters != null)
			{
				foreach (OleDbParameter p in commandParameters)
				{
					if (p != null)
					{
						// Check for derived output value with no value assigned
						if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && (p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
		}

		/// <summary>
		/// This method assigns an array of values to an array of OleDbParameters
		/// </summary>
		/// <param name="commandParameters">Array of OleDbParameters to be assigned values</param>
		/// <param name="parameterValues">Array of objects holding the values to be assigned</param>
		private static void AssignParameterValues(OleDbParameter[] commandParameters, object[] parameterValues)
		{
			if ((commandParameters == null) || (parameterValues == null))
			{
				// Do nothing if we get no data
				return;
			}

			// We must have the same number of values as we pave parameters to put them in
			if (commandParameters.Length != parameterValues.Length)
			{
				throw new ArgumentException("Parameter count does not match Parameter Value count.");
			}

			// Iterate through the OleDbParameters, assigning the values from the corresponding position in the 
			// value array
			for (int i = 0, j = commandParameters.Length; i < j; i++)
			{
				// If the current array value derives from IDbDataParameter, then assign its Value property
				if (parameterValues[i] is IDbDataParameter)
				{
					IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
					if (paramInstance.Value == null)
					{
						commandParameters[i].Value = DBNull.Value;
					}
					else
					{
						commandParameters[i].Value = paramInstance.Value;
					}
				}
				else if (parameterValues[i] == null)
				{
					commandParameters[i].Value = DBNull.Value;
				}
				else
				{
					commandParameters[i].Value = parameterValues[i];
				}
			}
		}

		/// <summary>
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
		/// to the provided command
		/// </summary>
		/// <param name="command">The OleDbCommand to be prepared</param>
		/// <param name="connection">A valid OleDbConnection, on which to execute this command</param>
		/// <param name="transaction">A valid OleDbTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
		private static void PrepareCommand(OleDbCommand command, OleDbConnection connection, OleDbTransaction transaction, CommandType commandType, string commandText, OleDbParameter[] commandParameters, out bool mustCloseConnection)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			// If the provided connection is not open, we will open it
			if (connection.State != ConnectionState.Open)
			{
				mustCloseConnection = true;
				connection.Open();
			}
			else
			{
				mustCloseConnection = false;
			}

			// Associate the connection with the command
			command.Connection = connection;

			// Set the command text (stored procedure name or OleDb statement)
			command.CommandText = commandText;

			// If we were provided a transaction, assign it
			if (transaction != null)
			{
				if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				command.Transaction = transaction;
			}

			// Set the command type
			command.CommandType = commandType;

			// Attach the command parameters if they are provided
			if (commandParameters != null)
			{
				AttachParameters(command, commandParameters);
			}
			return;
		}

		#endregion private utility methods & constructors

		#region ExecuteNonQuery

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset and takes no parameters) against the database specified in 
		/// the connection string
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteNonQuery(connectionString, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset) against the database specified in the connection string 
		/// using the provided parameters
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

			// Create & open a OleDbConnection, and dispose of it after we are done
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns no resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored prcedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset and takes no parameters) against the provided OleDbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteNonQuery(connection, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset) against the specified OleDbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Finally, execute the command
			int retval = cmd.ExecuteNonQuery();

			// Detach the OleDbParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			if (mustCloseConnection)
				connection.Close();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns no resultset) against the specified OleDbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset and takes no parameters) against the provided OleDbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteNonQuery(transaction, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns no resultset) against the specified OleDbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Finally, execute the command
			int retval = cmd.ExecuteNonQuery();

			// Detach the OleDbParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns no resultset) against the specified 
		/// OleDbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An int representing the number of rows affected by the command</returns>
		public static int ExecuteNonQuery(OleDbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
			}
		}



		#endregion ExecuteNonQuery

		#region ExecuteNonQueryReturnValue
		/// <summary>
		/// 执行一个 OleDb 命令，返回指定 Output 参数的值
		/// </summary>
		/// <param name="connectionString">a valid connection string for a OleDbConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-OleDb command</param>
		/// <param name="outParamName">返回参数的名称</param>
		/// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数值</returns>
		public static object ExecuteNonQueryReturnValue(string connectionString, CommandType commandType, string commandText, string outParamName, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				connection.Open();
				return ExecuteNonQueryReturnValue(connection, commandType, commandText, outParamName, commandParameters);
			}
		}

		/// <summary>
		/// 执行一个 OleDb 命令，返回指定 Output 参数的值
		/// </summary>
		/// <param name="connection">OleDbConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-OleDb command</param>
		/// <param name="outParamName">返回参数的名称</param>
		/// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数值</returns>
		public static object ExecuteNonQueryReturnValue(OleDbConnection connection, CommandType commandType, string commandText, string outParamName, params OleDbParameter[] commandParameters)
		{
			using (OleDbCommand cmd = new OleDbCommand())
			{
				bool mustCloseConnection = false;
				PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);

				if (!cmd.Parameters.Contains(outParamName))
				{
					throw new ArgumentException("OleDbCommand 不包含此参数 " + outParamName);
				}

				//增加输入输出参数的支持
				//updated by liangguangming

				if (cmd.Parameters[outParamName].Direction != ParameterDirection.Output && cmd.Parameters[outParamName].Direction != ParameterDirection.InputOutput)
				{
					throw new ArgumentException("参数 " + outParamName + " 不是输出参数");
				}

				int val = cmd.ExecuteNonQuery();

				object returnValue = cmd.Parameters[outParamName].Value;

				cmd.Parameters.Clear();
				if (mustCloseConnection)
				{
					connection.Close();
				}
				return returnValue;
			}
		}
		/// <summary>
		/// 执行一个 存储过程 命令，返回指定 Output 参数的值
		/// </summary>
		/// <param name="connectionString">a valid connection string for a OleDbConnection</param>
		/// <param name="spName">the stored procedure name</param>
		/// <param name="outParamName">返回参数的名称</param>
		/// <param name="parameterValues">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数值</returns>
		public static object ExecuteNonQueryReturnValue(string connectionString, string spName, string outParamName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteNonQueryReturnValue(connectionString, CommandType.StoredProcedure, spName, outParamName, commandParameters);
			}
			else
			{
				return ExecuteNonQueryReturnValue(connectionString, CommandType.StoredProcedure, spName, outParamName, (OleDbParameter[])null);
			}
		}
		/// <summary>
		/// 执行一个 存储过程 命令，返回指定 Output 参数的值
		/// </summary>
		/// <param name="connection">OleDbConnection</param>
		/// <param name="spName">the stored procedure name or T-OleDb command</param>
		/// <param name="outParamName">返回参数的名称</param>
		/// <param name="parameterValues">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数值</returns>
		public static object ExecuteNonQueryReturnValue(OleDbConnection connection, string spName, string outParamName, params object[] parameterValues)
		{
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			if ((parameterValues != null) && (parameterValues.Length > 0))
			{

				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteNonQueryReturnValue(connection, CommandType.StoredProcedure, spName, outParamName, commandParameters);
			}
			else
			{
				return ExecuteNonQueryReturnValue(connection, CommandType.StoredProcedure, spName, outParamName, (OleDbParameter[])null);
			}
		}
		/// <summary>
		/// 执行一个事务 存储过程 命令，返回指定 Output 参数的值
		/// </summary>
		/// <param name="transaction">事务</param>
		/// <param name="spName">the stored procedure name</param>
		/// <param name="outParamName">返回参数的名称</param>
		/// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数</returns>
		public static object ExecuteNonQueryReturnValue(OleDbTransaction transaction, string spName, string outParamName, params OleDbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			bool mustCloseConnection = false;
			OleDbCommand cmd = new OleDbCommand();
			PrepareCommand(cmd, transaction.Connection, transaction, CommandType.StoredProcedure, spName, commandParameters, out mustCloseConnection);
			if (!cmd.Parameters.Contains(outParamName))
			{
				throw new ArgumentException("OleDbCommand 不包含此参数 " + outParamName);
			}
			//增加输入输出参数的支持
			//updated by liangguangming

			if (cmd.Parameters[outParamName].Direction != ParameterDirection.Output && cmd.Parameters[outParamName].Direction != ParameterDirection.InputOutput)
			{
				throw new ArgumentException("参数 " + outParamName + " 不是输出参数");
			}

			int val = cmd.ExecuteNonQuery();
			object returnValue = cmd.Parameters[outParamName].Value;
			return returnValue;
		}
		#endregion ExecuteNonQueryReturnValue

		#region ExecuteNonQueryReturnArray
		/// <summary>
		/// 执行一个 OleDb 命令，返回指定 Output 参数数组
		/// </summary>
		/// <param name="connectionString">数据库链接字符串</param>
		/// <param name="commandType">指定如何解释命令字符串。(stored procedure, text, etc.)</param>
		/// <param name="commandText">要对数据源执行的 Transact-OleDb 语句或存储过程</param>
		/// <param name="outParamNameList">返回参数的名称数组</param>
		/// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回参数数组</returns>
		public static object[] ExecuteNonQueryReturnArray(string connectionString, CommandType commandType, string commandText, string[] outParamNameList, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				connection.Open();
				return ExecuteNonQueryReturnArray(connection, commandType, commandText, outParamNameList, commandParameters);
			}
		}
		/// <summary>
		/// 执行一个 OleDb 命令，返回指定 Output 参数数组
		/// </summary>
		/// <param name="connection">OleDbConnection</param>
		/// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">the stored procedure name or T-OleDb command</param>
		/// <param name="outParamNameList">返回参数的名称数组</param>
		/// <param name="commandParameters">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回object数组</returns>
		public static object[] ExecuteNonQueryReturnArray(OleDbConnection connection, CommandType commandType, string commandText, string[] outParamNameList, params OleDbParameter[] commandParameters)
		{
			int returnCount = outParamNameList.Length;
			object[] returnArray = new object[returnCount];
			using (OleDbCommand cmd = new OleDbCommand())
			{
				bool mustCloseConnection = false;
				PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
				for (int iParam = 0; iParam < returnCount; iParam++)
				{
					string outParamName = outParamNameList[iParam];
					if (!cmd.Parameters.Contains(outParamName))
					{
						throw new ArgumentException("OleDbCommand 不包含此参数 " + outParamName);
					}
					if (cmd.Parameters[outParamName].Direction == ParameterDirection.Output || cmd.Parameters[outParamName].Direction == ParameterDirection.InputOutput)
					{

					}
					else throw new ArgumentException("参数 " + outParamName + " 不是输出参数");
				}

				int val = cmd.ExecuteNonQuery();

				for (int iParam = 0; iParam < returnCount; iParam++)
				{
					string outParamName = outParamNameList[iParam];
					returnArray[iParam] = cmd.Parameters[outParamName].Value;
				}

				cmd.Parameters.Clear();
				if (mustCloseConnection)
				{
					connection.Close();
				}
				return returnArray;
			}
		}

		/// <summary>
		/// 执行一个 存储过程 命令，返回指定 Output 参数数组
		/// </summary>
		/// <param name="connectionString">a valid connection string for a OleDbConnection</param>
		/// <param name="spName">存储过程名</param>
		/// <param name="outParamNameList">返回参数的名称数组</param>
		/// <param name="parameterValues">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回object数组</returns>
		public static object[] ExecuteNonQueryReturnArray(string connectionString, string spName, string[] outParamNameList, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteNonQueryReturnArray(connectionString, CommandType.StoredProcedure, spName, outParamNameList, commandParameters);
			}
			else
			{
				return ExecuteNonQueryReturnArray(connectionString, CommandType.StoredProcedure, spName, outParamNameList, (OleDbParameter[])null);
			}
		}
		/// <summary>
		/// 执行一个 存储过程 命令，返回指定 Output 参数数组
		/// </summary>
		/// <param name="connection">OleDbConnection</param>
		/// <param name="spName">存储过程名</param>
		/// <param name="outParamNameList">返回参数的名称数组</param>
		/// <param name="parameterValues">an array of OleDbParamters used to execute the command</param>
		/// <returns>返回object数组</returns>
		public static object[] ExecuteNonQueryReturnArray(OleDbConnection connection, string spName, string[] outParamNameList, params object[] parameterValues)
		{
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteNonQueryReturnArray(connection, CommandType.StoredProcedure, spName, outParamNameList, commandParameters);
			}
			else
			{
				return ExecuteNonQueryReturnArray(connection, CommandType.StoredProcedure, spName, outParamNameList, (OleDbParameter[])null);
			}
		}
		#endregion ExecuteNonQueryReturnArray

		#region ExecuteDataset

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteDataset(connectionString, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");

			// Create & open a OleDbConnection, and dispose of it after we are done
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteDataset(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the provided OleDbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteDataset(connection, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the specified OleDbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);

				// Detach the OleDbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				if (mustCloseConnection)
					connection.Close();

				// Return the dataset
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the specified OleDbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the provided OleDbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteDataset(transaction, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the specified OleDbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Create the DataAdapter & DataSet
			using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
			{
				DataSet ds = new DataSet();

				// Fill the DataSet using default values for DataTable names, etc
				da.Fill(ds);

				// Detach the OleDbParameters from the command object, so they can be used again
				cmd.Parameters.Clear();

				// Return the dataset
				return ds;
			}
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the specified 
		/// OleDbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A dataset containing the resultset generated by the command</returns>
		public static DataSet ExecuteDataset(OleDbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteDataset

		#region ExecuteReader

		/// <summary>
		/// This enum is used to indicate whether the connection was provided by the caller, or created by OleDbHelper, so that
		/// we can set the appropriate CommandBehavior when calling ExecuteReader()
		/// </summary>
		private enum OleDbConnectionOwnership
		{
			/// <summary>Connection is owned and managed by OleDbHelper</summary>
			Internal,
			/// <summary>Connection is owned and managed by the caller</summary>
			External
		}

		/// <summary>
		/// Create and prepare a OleDbCommand, and call ExecuteReader with the appropriate CommandBehavior.
		/// </summary>
		/// <remarks>
		/// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
		/// 
		/// If the caller provided the connection, we want to leave it to them to manage.
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection, on which to execute this command</param>
		/// <param name="transaction">A valid OleDbTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="connectionOwnership">Indicates whether the connection parameter was provided by the caller, or created by OleDbHelper</param>
		/// <returns>OleDbDataReader containing the results of the command</returns>
		private static OleDbDataReader ExecuteReader(OleDbConnection connection, OleDbTransaction transaction, CommandType commandType, string commandText, OleDbParameter[] commandParameters, OleDbConnectionOwnership connectionOwnership)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			bool mustCloseConnection = false;
			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			try
			{
				PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

				// Create a reader
				OleDbDataReader dataReader;

				// Call ExecuteReader with the appropriate CommandBehavior
				if (connectionOwnership == OleDbConnectionOwnership.External)
				{
					dataReader = cmd.ExecuteReader();
				}
				else
				{
					dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}

				// Detach the OleDbParameters from the command object, so they can be used again.
				// HACK: There is a problem here, the output parameter values are fletched 
				// when the reader is closed, so if the parameters are detached from the command
				// then the OleDbReader can磘 set its values. 
				// When this happen, the parameters can磘 be used again in other command.
				bool canClear = true;
				foreach (OleDbParameter commandParameter in cmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}

				if (canClear)
				{
					cmd.Parameters.Clear();
				}

				return dataReader;
			}
			catch
			{
				if (mustCloseConnection)
					connection.Close();
				throw;
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteReader(connectionString, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			OleDbConnection connection = null;
			try
			{
				connection = new OleDbConnection(connectionString);
				connection.Open();

				// Call the private overload that takes an internally owned connection in place of the connection string
				return ExecuteReader(connection, null, commandType, commandText, commandParameters, OleDbConnectionOwnership.Internal);
			}
			catch
			{
				// If we fail to return the OleDbDatReader, we need to close the connection ourselves
				if (connection != null) connection.Close();
				throw;
			}

		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the provided OleDbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteReader(connection, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the specified OleDbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			// Pass through the call to the private overload using a null transaction value and an externally owned connection
			return ExecuteReader(connection, (OleDbTransaction)null, commandType, commandText, commandParameters, OleDbConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the specified OleDbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset and takes no parameters) against the provided OleDbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteReader(transaction, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a resultset) against the specified OleDbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///   OleDbDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Pass through to private overload, indicating that the connection is owned by the caller
			return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, OleDbConnectionOwnership.External);
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a resultset) against the specified
		/// OleDbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  OleDbDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>A OleDbDataReader containing the resultset generated by the command</returns>
		public static OleDbDataReader ExecuteReader(OleDbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				AssignParameterValues(commandParameters, parameterValues);

				return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteReader

		#region ExecuteScalar

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
		/// the connection string. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteScalar(connectionString, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset) against the database specified in the connection string 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			// Create & open a OleDbConnection, and dispose of it after we are done
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				connection.Open();

				// Call the overload that takes a connection in place of the connection string
				return ExecuteScalar(connection, commandType, commandText, commandParameters);
			}
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a 1x1 resultset) against the database specified in 
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connectionString, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset and takes no parameters) against the provided OleDbConnection. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteScalar(connection, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset) against the specified OleDbConnection 
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbConnection connection, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connection == null) throw new ArgumentNullException("connection");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();

			bool mustCloseConnection = false;
			PrepareCommand(cmd, connection, (OleDbTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

			// Execute the command & return the results
			object retval = cmd.ExecuteScalar();

			// Detach the OleDbParameters from the command object, so they can be used again
			cmd.Parameters.Clear();

			if (mustCloseConnection)
				connection.Close();

			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a 1x1 resultset) against the specified OleDbConnection 
		/// using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbConnection connection, string spName, params object[] parameterValues)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// Pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
			}
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset and takes no parameters) against the provided OleDbTransaction. 
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbTransaction transaction, CommandType commandType, string commandText)
		{
			// Pass through the call providing null for the set of OleDbParameters
			return ExecuteScalar(transaction, commandType, commandText, (OleDbParameter[])null);
		}

		/// <summary>
		/// Execute a OleDbCommand (that returns a 1x1 resultset) against the specified OleDbTransaction
		/// using the provided parameters.
		/// </summary>
		/// <remarks>
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new OleDbParameter("@prodid", 24));
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters used to execute the command</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbTransaction transaction, CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

			// Create a command and prepare it for execution
			OleDbCommand cmd = new OleDbCommand();
			bool mustCloseConnection = false;
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

			// Execute the command & return the results
			object retval = cmd.ExecuteScalar();

			// Detach the OleDbParameters from the command object, so they can be used again
			cmd.Parameters.Clear();
			return retval;
		}

		/// <summary>
		/// Execute a stored procedure via a OleDbCommand (that returns a 1x1 resultset) against the specified
		/// OleDbTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
		/// </summary>
		/// <remarks>
		/// This method provides no access to output parameters or the stored procedure's return value parameter.
		/// 
		/// e.g.:  
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
		/// </remarks>
		/// <param name="transaction">A valid OleDbTransaction</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="parameterValues">An array of objects to be assigned as the input values of the stored procedure</param>
		/// <returns>An object containing the value in the 1x1 resultset generated by the command</returns>
		public static object ExecuteScalar(OleDbTransaction transaction, string spName, params object[] parameterValues)
		{
			if (transaction == null) throw new ArgumentNullException("transaction");
			if (transaction != null && transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			// If we receive parameter values, we need to figure out where they go
			if ((parameterValues != null) && (parameterValues.Length > 0))
			{
				// PPull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
				OleDbParameter[] commandParameters = OleDbHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);

				// Assign the provided values to these parameters based on parameter order
				AssignParameterValues(commandParameters, parameterValues);

				// Call the overload that takes an array of OleDbParameters
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
			}
			else
			{
				// Otherwise we can just call the SP without params
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
			}
		}

		#endregion ExecuteScalar
	}

	/// <summary>
	/// OleDbHelperParameterCache provides functions to leverage a static cache of procedure parameters, 
	/// and the ability to discover parameters for stored procedures at run-time.
	/// </summary>
	public sealed class OleDbHelperParameterCache
	{
		#region private methods, variables, and constructors

		//因为类只提供静态方法，因此将默认得构造函数置为私有，以防止用户调用默认构造函数创建实例
		private OleDbHelperParameterCache() { }

		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

		/// <summary>
		/// Resolve at run time the appropriate set of OleDbParameters for a stored procedure
		/// </summary>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
		/// <returns>The parameter array discovered.</returns>
		private static OleDbParameter[] DiscoverSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			OleDbCommand cmd = new OleDbCommand(spName, connection);
			cmd.CommandType = CommandType.StoredProcedure;

			connection.Open();
			OleDbCommandBuilder.DeriveParameters(cmd);
			connection.Close();

			if (!includeReturnValueParameter)
			{
				cmd.Parameters.RemoveAt(0);
			}

			OleDbParameter[] discoveredParameters = new OleDbParameter[cmd.Parameters.Count];

			cmd.Parameters.CopyTo(discoveredParameters, 0);

			// Init the parameters with a DBNull value
			foreach (OleDbParameter discoveredParameter in discoveredParameters)
			{
				discoveredParameter.Value = DBNull.Value;
			}
			return discoveredParameters;
		}

		/// <summary>
		/// Deep copy of cached OleDbParameter array
		/// </summary>
		/// <param name="originalParameters"></param>
		/// <returns></returns>
		private static OleDbParameter[] CloneParameters(OleDbParameter[] originalParameters)
		{
			OleDbParameter[] clonedParameters = new OleDbParameter[originalParameters.Length];

			for (int i = 0, j = originalParameters.Length; i < j; i++)
			{
				clonedParameters[i] = (OleDbParameter)((ICloneable)originalParameters[i]).Clone();
			}

			return clonedParameters;
		}

		#endregion private methods, variables, and constructors

		#region caching functions

		/// <summary>
		/// Add parameter array to the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <param name="commandParameters">An array of OleDbParamters to be cached</param>
		public static void CacheParameterSet(string connectionString, string commandText, params OleDbParameter[] commandParameters)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			string hashKey = connectionString + ":" + commandText;

			paramCache[hashKey] = commandParameters;
		}

		/// <summary>
		/// Retrieve a parameter array from the cache
		/// </summary>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="commandText">The stored procedure name or T-OleDb command</param>
		/// <returns>An array of OleDbParamters</returns>
		public static OleDbParameter[] GetCachedParameterSet(string connectionString, string commandText)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

			string hashKey = connectionString + ":" + commandText;

			OleDbParameter[] cachedParameters = paramCache[hashKey] as OleDbParameter[];
			if (cachedParameters == null)
			{
				return null;
			}
			else
			{
				return CloneParameters(cachedParameters);
			}
		}

		#endregion caching functions

		#region Parameter Discovery Functions

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OleDbParameters</returns>
		public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName)
		{
			return GetSpParameterSet(connectionString, spName, false);
		}

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connectionString">A valid connection string for a OleDbConnection</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		public static OleDbParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
		{
			if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <returns>An array of OleDbParameters</returns>
		internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName)
		{
			return GetSpParameterSet(connection, spName, false);
		}

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <remarks>
		/// This method will query the database for this information, and then store it in a cache for future requests.
		/// </remarks>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		internal static OleDbParameter[] GetSpParameterSet(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			using (OleDbConnection clonedConnection = (OleDbConnection)((ICloneable)connection).Clone())
			{
				return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
			}
		}

		/// <summary>
		/// Retrieves the set of OleDbParameters appropriate for the stored procedure
		/// </summary>
		/// <param name="connection">A valid OleDbConnection object</param>
		/// <param name="spName">The name of the stored procedure</param>
		/// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
		/// <returns>An array of OleDbParameters</returns>
		private static OleDbParameter[] GetSpParameterSetInternal(OleDbConnection connection, string spName, bool includeReturnValueParameter)
		{
			if (connection == null) throw new ArgumentNullException("connection");
			if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

			string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

			OleDbParameter[] cachedParameters;

			cachedParameters = paramCache[hashKey] as OleDbParameter[];
			if (cachedParameters == null)
			{
				OleDbParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
				paramCache[hashKey] = spParameters;
				cachedParameters = spParameters;
			}

			return CloneParameters(cachedParameters);
		}

		#endregion Parameter Discovery Functions

	}
}
