using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neusoft.HISFC.Models.SqlSugar
{
    /// <summary>
    ///   (^_^)
    ///   /| |\
    ///    | |
    /// 本类由代码生成器自动生成，请勿手动修改
    /// 由[少司命]定制专属守护
    /// 表名注释：通用调试日志表 - 用于排查系统运行时各类BUG，支持全链路逻辑追踪
    /// 数据表名：sys_debug_log
    /// 生成时间：2026-01-21 15:26:47
    /// </summary>
    public class SysDebugLog
    {
        /// <summary>
        /// 字段说明:日志唯一标识 (建议存储GUID字符串)
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:否
        /// 字段名称:log_id
        /// </summary>
        public string LogId { get; set; }

        /// <summary>
        /// 字段说明:日志记录逻辑时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:log_time
        /// </summary>
        public DateTime LogTime { get; set; }

        /// <summary>
        /// 字段说明:所属模块/类名 (如 MzPackage)
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:module_name
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 字段说明:函数或方法名
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:function_name
        /// </summary>
        public string FunctionName { get; set; }

        /// <summary>
        /// 字段说明:执行步骤序号 (用于大型方法的流程追踪)
        /// 数据类型:NUMBER
        /// 字段长度:10
        /// 是否可空:是
        /// 字段名称:step_no
        /// </summary>
        public decimal StepNo { get; set; }

        /// <summary>
        /// 字段说明:日志级别 (DEBUG/INFO/WARN/ERROR/FATAL)
        /// 数据类型:VARCHAR2
        /// 字段长度:20
        /// 是否可空:是
        /// 字段名称:log_level
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        /// 字段说明:主要日志描述内容
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:log_message
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 字段说明:通用业务键1 (如处方号、流水号)
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:biz_key1
        /// </summary>
        public string BizKey1 { get; set; }

        /// <summary>
        /// 字段说明:通用业务键2 (如药品代码、患者ID)
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:biz_key2
        /// </summary>
        public string BizKey2 { get; set; }

        /// <summary>
        /// 字段说明:通用业务键3 (扩展备用)
        /// 数据类型:VARCHAR2
        /// 字段长度:200
        /// 是否可空:是
        /// 字段名称:biz_key3
        /// </summary>
        public string BizKey3 { get; set; }

        /// <summary>
        /// 字段说明:业务类型标识 (如 DOSE_CALC, PAY_FLOW)
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:biz_type
        /// </summary>
        public string BizType { get; set; }

        /// <summary>
        /// 字段说明:方法入口参数详细信息 (CLOB)
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:input_params
        /// </summary>
        public string InputParams { get; set; }

        /// <summary>
        /// 字段说明:方法出口返回值或结果 (CLOB)
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:output_result
        /// </summary>
        public string OutputResult { get; set; }

        /// <summary>
        /// 字段说明:执行中间状态或上下文快照数据 (CLOB)
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:context_data
        /// </summary>
        public string ContextData { get; set; }

        /// <summary>
        /// 字段说明:系统或自定义错误编码
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:error_code
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 字段说明:详细错误异常文本
        /// 数据类型:VARCHAR2
        /// 字段长度:4000
        /// 是否可空:是
        /// 字段名称:error_message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 字段说明:完整的程序异常调用堆栈 (CLOB)
        /// 数据类型:CLOB
        /// 字段长度:2147483647
        /// 是否可空:是
        /// 字段名称:stack_trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 字段说明:应用服务器节点名称/主机名
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:server_name
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 字段说明:调用方客户端IP地址
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:client_ip
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        /// 字段说明:当前正在操作的用户ID
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:operator_id
        /// </summary>
        public string OperatorId { get; set; }

        /// <summary>
        /// 字段说明:当前运行的系统线程ID
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:thread_id
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// 字段说明:链路追踪ID (串联同一次完整业务请求)
        /// 数据类型:VARCHAR2
        /// 字段长度:100
        /// 是否可空:是
        /// 字段名称:trace_id
        /// </summary>
        public string TraceId { get; set; }

        /// <summary>
        /// 字段说明:父级日志ID (用于函数嵌套深度调用追踪)
        /// 数据类型:VARCHAR2
        /// 字段长度:50
        /// 是否可空:是
        /// 字段名称:parent_log_id
        /// </summary>
        public string ParentLogId { get; set; }

        /// <summary>
        /// 字段说明:该记录存入数据库的物理时间
        /// 数据类型:DATE
        /// 字段长度:7
        /// 是否可空:是
        /// 字段名称:create_time
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
