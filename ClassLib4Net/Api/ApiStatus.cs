

using ClassLib4Net.Extension.Enum;
/**
* 命名空间: ClassLib4Net.Api
*
* 功 能： N/A
* 类 名： ApiStatus
*
* Ver 变更日期 负责人 当前系统用户名 CLR版本 变更内容
* ───────────────────────────────────
* V0.01 2019/11/24 11:17:44 熊仔其人 xxh 4.0.30319.42000 初版
*
* Copyright (c) 2019 熊仔其人 Corporation. All rights reserved.
*┌─────────────────────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．   │
*│　版权所有：熊仔其人 　　　　　　　　　　　　　　　　　　　　　　　 │
*└─────────────────────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLib4Net.Api
{
    /// <summary>
    /// Api处理状态枚举
    /// </summary>
    public enum ApiStatus:int
    {
        /// <summary>
        /// 成功
        /// </summary>
        [MultiLanguage("zh-CN", "成功")]
        [Description("成功")]
        Ok = 0,

        /// <summary>
        /// 服务器处理成功
        /// </summary>
        [MultiLanguage("zh-CN", "服务器处理成功")]
        [Description("服务器处理成功")]
        Success = 200,

        /// <summary>
        /// 已完成
        /// </summary>
        [MultiLanguage("zh-CN", "已完成")]
        [Description("已完成")]
        Completed = 201,

        #region 40xxx

        /// <summary>
        /// 参数无效
        /// </summary>
        [MultiLanguage("zh-CN", "参数无效")]
        [Description("参数无效")]
        InvalidParameter = 40001,

        /// <summary>
        /// 当前值或模型已存在
        /// </summary>
        [MultiLanguage("zh-CN", "当前值或模型已存在")]
        [Description("当前值或模型已存在")]
        Existed = 40010,

        /// <summary>
        /// 当前值或模型不存在
        /// </summary>
        [MultiLanguage("zh-CN", "当前值或模型不存在")]
        [Description("当前值或模型不存在")]
        NotExisted = 40011,

        /// <summary>
        /// Token过期
        /// </summary>
        [MultiLanguage("zh-CN", "Token过期")]
        [Description("Token过期")]
        TokenExpire = 40012,

        /// <summary>
        /// 登录异常
        /// </summary>
        [MultiLanguage("zh-CN", "登录异常")]
        [Description("登录异常")]
        LoginEx = 40100,

        /// <summary>
        /// 未授权的请求
        /// </summary>
        [MultiLanguage("zh-CN", "未授权的请求")]
        [Description("未授权的请求")]
        Unauthorized = 40101,

        /// <summary>
        /// 用户名或者密码错误
        /// </summary>
        [MultiLanguage("zh-CN", "用户名或者密码错误")]
        [Description("用户名或者密码错误")]
        NameOrPasswordError = 40102,

        /// <summary>
        /// 用户禁用
        /// </summary>
        [MultiLanguage("zh-CN", "用户禁用")]
        [Description("用户禁用")]
        UserDisable = 40103,

        /// <summary>
        /// 无效的验证码
        /// </summary>
        [MultiLanguage("zh-CN", "无效的验证码")]
        [Description("无效的验证码")]
        InvalidVerificationCode = 40104,

        #endregion

        #region 50xxx

        /// <summary>
        /// 服务器处理失败
        /// </summary>
        [MultiLanguage("zh-CN", "服务器处理失败")]
        [Description("服务器处理失败")]
        ServerError = 50001,

        /// <summary>
        /// 失败
        /// </summary>
        [MultiLanguage("zh-CN", "失败")]
        [Description("失败")]
        Fail = 50002,

        /// <summary>
        /// 未完成
        /// </summary>
        [MultiLanguage("zh-CN", "未完成")]
        [Description("未完成")]
        InComplete = 50003,

        #endregion


    }


}
