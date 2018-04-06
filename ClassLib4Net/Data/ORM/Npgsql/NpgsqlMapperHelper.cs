using ClassLib4Net;
using ClassLib4Net.Data.ORM;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

/*
* 简单的ORM框架
* 熊学浩
* 2016-11-24
*/
namespace ClassLib4Net.Data.ORM.Npgsql
{
    /// <summary>
    /// Npgsql数据库映射
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NpgsqlMapperHelper<T> : DataRowMapperHelper<T> where T : class
    {
        #region Load
        /// <summary>
        /// 是现有的列
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected virtual bool IsExistingColumn(string columnName, NpgsqlDataReader reader)
        {
            for(int i = 0; i < reader.VisibleFieldCount && i < reader.FieldCount; i++)
            {
                if(null != reader[i] && DBNull.Value != reader[i])
                    if(reader.GetName(i).Equals(columnName)) return true;
            }
            return false;
        }

        /// <summary>
        /// 对象关系映射
        /// </summary>
        /// <param name="t">object</param>
        /// <param name="reader">使用前必须 reader.Read()</param>
        protected virtual void DoLoad(T t, NpgsqlDataReader reader)
        {
            object o = null;
            foreach(var p in t.GetType().GetProperties())
            {
                if(null != p && p.CanWrite)
                {
                    NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                    if(null != column)
                    {
                        if(column.CanLoad)
                        {
                            string columnName = string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name;
                            if(IsExistingColumn(columnName, reader))
                            {
                                o = reader[columnName];
                                if(o != DBNull.Value)
                                    p.SetValue(t, o);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 从NpgsqlDataReader装载对象
        /// </summary>
        /// <param name="reader">使用前必须 reader.Read()</param>
        /// <returns></returns>
        public T Load(NpgsqlDataReader reader)
        {
            T t = CreateDomainObject();
            DoLoad(t, reader);
            return t;
        }
        /// <summary>
        /// 从NpgsqlDataReader装载对象
        /// </summary>
        /// <param name="reader">data reader</param>
        /// <returns>object list</returns>
        public virtual IList<T> LoadAll(NpgsqlDataReader reader)
        {
            IList<T> domainObjects = new List<T>();
            while(reader.Read())
            {
                domainObjects.Add(Load(reader));
            }
            return domainObjects;
        }
        #endregion

        #region Private property
        private T _entity;
        private T entity
        {
            get
            {
                if(null == _entity)
                {
                    _entity = CreateDomainObject();
                }
                return _entity;
            }
            set { _entity = value; }
        }
        private string _tableName;
        public string tableName
        {
            get
            {
                if(string.IsNullOrWhiteSpace(_tableName))
                {
                    _tableName = DataTableMapperHelper<T>.GetCustomAttribute(entity)?.Name ?? entity.GetType().Name;
                }
                return string.Concat(columnNameSign[0], _tableName, columnNameSign[1]);
            }
            //set { _tableName = value; }
        }
        private string _DBConnectionString;
        public string DBConnectionString
        {
            get
            {
                return _DBConnectionString;
            }
            set { _DBConnectionString = value; }
        }
        #endregion

        public string[] columnNameSign = new string[2] { "\"", "\"" };
        public string paramNameSign = "@";

        public NpgsqlMapperHelper() { }
        public NpgsqlMapperHelper(string connectionString)
        {
            DBConnectionString = connectionString;
        }
        public NpgsqlMapperHelper(T t)
        {
            _entity = t;
        }
        public NpgsqlMapperHelper(T t, string connectionString)
        {
            _entity = t;
            DBConnectionString = connectionString;
        }

        #region CanLoadColumnNames
        private IList<string> _canLoadColumnNames;
        public virtual IList<string> CanLoadColumnNames(T t)
        {
            IList<string> columnNames = new List<string>();
            foreach(var p in t.GetType().GetProperties())
            {
                if(null != p && p.CanWrite)
                {
                    NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                    if(null != column && column.CanLoad)
                    {
                        string columnName = string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name;
                        columnNames.Add(string.Concat(columnNameSign[0], columnName, columnNameSign[1]));
                    }
                }
            }
            return columnNames;
        }
        public virtual IList<string> CanLoadColumnNames()
        {
            if(null == _canLoadColumnNames || !_canLoadColumnNames.Any<string>())
                _canLoadColumnNames = CanLoadColumnNames(entity);
            return _canLoadColumnNames;
        }
        private string _canLoadColumnNamesString;
        public virtual string CanLoadColumnNamesString()
        {
            if(string.IsNullOrWhiteSpace(_canLoadColumnNamesString))
            {
                IList<string> columnNames = CanLoadColumnNames();
                if(null != columnNames && columnNames.Count > 0)
                    _canLoadColumnNamesString = string.Join(",", columnNames);
            }
            return _canLoadColumnNamesString;
        }

        public virtual string ColumnName(string propertyName)
        {
            if(string.IsNullOrWhiteSpace(propertyName)) return string.Empty;
            foreach(var p in entity.GetType().GetProperties())
            {
                if(null != p && p.CanWrite)
                {
                    if(string.Compare(propertyName, p.Name) == 0)
                    {
                        NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                        if(null != column)
                        {
                            string columnName = string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name;
                            return string.Concat(columnNameSign[0], columnName, columnNameSign[1]);
                        }
                    }
                }
            }
            return string.Empty;
        }
        #endregion

        #region ScheduledCommands
        protected virtual string GetInsertCommand(T t, out string IdentityParameterName, out IList<NpgsqlParameter> parameters)
        {
            IdentityParameterName = string.Empty;
            parameters = new List<NpgsqlParameter>();
            if(null == t) return string.Empty;

            string IdentityColumn = string.Empty;
            NpgsqlParameter parameter;
            StringBuilder columns = new StringBuilder();
            StringBuilder parameterColumns = new StringBuilder();
            foreach(PropertyInfo p in ((TypeInfo)(t.GetType())).DeclaredProperties)
            {
                NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                if(null != column)
                {
                    if(column.CanInsert)
                    {
                        if(column.CanDefaultValue || column.IsIdentity) continue;
                        else
                        {
                            parameter = new NpgsqlParameter() { ParameterName = string.Format(paramNameSign + "{0}", string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name), NpgsqlDbType = column.DbType };
                            object _value = p.GetValue(t);
                            if(null == _value || DBNull.Value == _value)
                            {
                                parameter.Value = DBNull.Value;
                            }
                            else if(p.PropertyType.IsEnum)
                            {
                                parameter.Value = Convert.ToInt32(_value);
                            }
                            else
                            {
                                parameter.Value = _value;
                            }

                            columns.AppendFormat("{0},", string.Concat(columnNameSign[0], (string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name), columnNameSign[1]));
                            parameterColumns.AppendFormat("{0},", parameter.ParameterName);

                            parameters.Add(parameter);
                        }
                    }

                    if(column.IsIdentity)
                    {
                        IdentityColumn = string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name;
                        IdentityParameterName = string.Format(paramNameSign + "{0}", IdentityColumn);
                        parameters.Add(new NpgsqlParameter() { ParameterName = IdentityParameterName, NpgsqlDbType = column.DbType, Direction = ParameterDirection.ReturnValue }); //ParameterDirection.Output
                    }
                }
            }

            if(null != parameters && parameters.Count > 0)
            {
                return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columns.ToString().Trim(new Char[] { ' ', ',' }), parameterColumns.ToString().Trim(new Char[] { ' ', ',' })) + (string.IsNullOrWhiteSpace(IdentityParameterName) ? string.Empty : string.Format(" RETURNING {0};", IdentityColumn));// SELECT {0}=SCOPE_IDENTITY(); //RETURNING id;
            }
            return string.Empty;
        }
        protected virtual string GetSelectAllCommand()
        {
            if(null == entity) return string.Empty;

            StringBuilder columns = new StringBuilder();
            foreach(PropertyInfo p in ((TypeInfo)(entity.GetType())).DeclaredProperties)
            {
                if(null != p && p.CanWrite)
                {
                    NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                    if(null != column && column.CanLoad)
                    {
                        columns.AppendFormat("{0},", string.Concat(columnNameSign[0], (string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name), columnNameSign[1]));
                    }
                }
            }

            return string.Format("SELECT {0} FROM {1};", columns.ToString().Trim(new Char[] { ' ', ',' }), tableName);
        }
        protected virtual string GetUpdateCommand(T t, out IList<NpgsqlParameter> parameters)
        {
            parameters = new List<NpgsqlParameter>();
            if(null == t) return string.Empty;

            NpgsqlParameter parameter;
            StringBuilder wherecolumns = new StringBuilder();
            StringBuilder parameterColumns = new StringBuilder();

            bool existPrimaryKeyOrIdentity = false;
            foreach(PropertyInfo p in ((TypeInfo)(t.GetType())).DeclaredProperties)
            {
                NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                if(null != column)
                {
                    parameter = new NpgsqlParameter() { SourceColumn = (string.IsNullOrWhiteSpace(column.Name) ? p.Name : column.Name), NpgsqlDbType = column.DbType };
                    parameter.ParameterName = string.Format(paramNameSign + "{0}", parameter.SourceColumn);

                    object _value = p.GetValue(t);
                    if(null == _value || DBNull.Value == _value)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else if(p.PropertyType.IsEnum)
                    {
                        parameter.Value = Convert.ToInt32(_value);
                    }
                    else
                    {
                        parameter.Value = _value;
                    }


                    if(column.IsPrimaryKey || column.IsIdentity)
                    {
                        if(column.CanLoad)
                        {
                            wherecolumns.AppendFormat("{0}={1} AND ", string.Concat(columnNameSign[0], parameter.SourceColumn, columnNameSign[1]), parameter.ParameterName);

                            parameters.Add(parameter);
                            existPrimaryKeyOrIdentity = true;
                        }
                    }

                    if(column.CanUpdate)
                    {
                        parameterColumns.AppendFormat("{0}={1},", string.Concat(columnNameSign[0], parameter.SourceColumn, columnNameSign[1]), parameter.ParameterName);

                        parameters.Add(parameter);
                    }

                }
            }

            if(!existPrimaryKeyOrIdentity)
            {
                throw new TargetParameterCountException("Not found Column Of PrimaryKey Or Identity");
            }

            if(null != parameters && parameters.Count > 0)
            {
                return string.Format("UPDATE {0} SET {1} WHERE {2};", tableName, parameterColumns.ToString().Trim(new Char[] { ' ', ',' }), StringHelper.Replace(wherecolumns.ToString(), @"(\s+)[a-zA-Z0-9]+(\s*)$", ""));
            }
            return string.Empty;
        }
        #endregion

        #region ExecuteCommands
        public object Insert(T t)
        {
            string IdentityParameterName;
            IList<NpgsqlParameter> parameters;
            string commandText = GetInsertCommand(t, out IdentityParameterName, out parameters);
            object result = NpgsqlHelper.ExecuteScalar(DBConnectionString, CommandType.Text, commandText, parameters.ToArray());
            object id = null;
            if(!string.IsNullOrWhiteSpace(IdentityParameterName))
            {
                if(parameters.Where(x => x.ParameterName.Equals(IdentityParameterName)).Any())
                {
                    if(parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.Output)).Any())
                        id = parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.Output)).Select(x => x.Value).FirstOrDefault();
                    else if(parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.ReturnValue)).Any())
                        id = parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.ReturnValue)).Select(x => x.Value).FirstOrDefault();
                    else if(parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.InputOutput)).Any())
                        id = parameters.Where(x => x.ParameterName.Equals(IdentityParameterName) && x.Direction.Equals(ParameterDirection.InputOutput)).Select(x => x.Value).FirstOrDefault();
                }
            }

            if(null == id || DBNull.Value == id)
                if(null != result && result != DBNull.Value)
                    id = result;

            foreach(PropertyInfo p in ((TypeInfo)(t.GetType())).DeclaredProperties)
            {
                NpgsqlColumnMapperAttribute column = NpgsqlColumnMapperHelper<T>.GetCustomAttribute(p);
                if(null != column)
                {
                    if(column.IsIdentity)
                    {
                        if(null != id && DBNull.Value != id)
                        {
                            switch(p.PropertyType.Name)
                            {
                                case "Int32": p.SetValue(t, Convert.ToInt32(id)); break;
                                case "long":
                                case "Int64": p.SetValue(t, Convert.ToInt64(id)); break;
                                default: p.SetValue(t, id); break;
                            }
                        }
                    }
                    else
                        continue;
                }
            }

            return result;
        }

        public IList<T> LoadAll()
        {
            IList<T> list = new List<T>();
            using(NpgsqlDataReader reader = NpgsqlHelper.ExecuteReader(DBConnectionString, CommandType.Text, GetSelectAllCommand()))
            {
                list = LoadAll(reader);
            }
            return list;
        }

        public int Update(T t)
        {
            IList<NpgsqlParameter> parameters;
            string commandText = GetUpdateCommand(t, out parameters);
            int result = NpgsqlHelper.ExecuteNonQuery(DBConnectionString, CommandType.Text, commandText, parameters.ToArray());
            return result;
        }

        #endregion





    }
}
