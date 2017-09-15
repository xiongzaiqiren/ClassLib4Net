/*
* 简单的ORM框架
* 熊学浩
* 2016-11-15
*/
namespace ClassLib4Net.Data.ORM
{
    /// <summary>
    /// 数据加载模式
    /// </summary>
    public enum LoadDataMode
    {
        /// <summary>
        /// 数据表
        /// </summary>
        Table = 1,
        /// <summary>
        /// 复杂查询
        /// </summary>
        ComplexQuery = 2,
        /// <summary>
        /// 视图
        /// </summary>
        View = 3,
        /// <summary>
        /// 存储过程返回结果
        /// </summary>
        StoredProcedure = 4,
        /// <summary>
        /// 枚举
        /// </summary>
        Enums = 5,
        /// <summary>
        /// 静态数据
        /// </summary>
        StaticData = 6,
        /// <summary>
        /// 静态数据
        /// </summary>
        XmlDocument = 7,
        /// <summary>
        /// 静态数据
        /// </summary>
        JsonObject = 8,

        /// <summary>
        /// 其它
        /// </summary>
        Other = 0,
    }

    /// <summary>
    /// 执行操作
    /// </summary>
    public enum ExecuteOperation
    {
        /// <summary>
        /// 插入
        /// </summary>
        Insert = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
        /// <summary>
        /// 查询
        /// </summary>
        Select = 3,
        /// <summary>
        /// 修改
        /// </summary>
        Update = 4,

    }

    /// <summary>
    /// 多个条件之间的连接词
    /// </summary>
    public enum WhereConj
    {
        And = 1,
        Or = 2,
    }

    /// <summary>
    /// 对具体条件使用操作符
    /// </summary>
    public enum WhereOperator
    {
        Equal = 1,
        NotEqual = 2,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 3,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 4,
        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterOrEqual = 5,
        /// <summary>
        /// 小于等于
        /// </summary>
        LessOrEqual = 6,
        Between = 7,
        Like = 8,
        In = 9,

        IsNull = 10,
        IsNotNull = 11,
    }



}
