using AntJoin.Core.Extensions;

namespace AntJoin.Core.Exceptions
{
    #region 系统级别错误
    /// <summary>
    /// 系统级别错误编码(1~50)
    /// </summary>
    public enum SysErrorCode
    {
        /// <summary>
        /// 系统缓存出错
        /// </summary>
        CACHE_ERROR = 1,

        /// <summary>
        /// 无效的IP
        /// </summary>
        INVALID_IP = 2,

        /// <summary>
        /// 请勿重复调用
        /// </summary>
        REPEATE_CALL = 3,

        /// <summary>
        /// 缺少系统参数
        /// </summary>
        LACK_SYSPARA = 4,

        /// <summary>
        /// 暂不支持该库
        /// </summary>
        INVALID_DB = 5
    }
    #endregion

    #region API公共错误
    /// <summary>
    /// API公共错误码(201-250)
    /// </summary>
    public enum ApiErrorCode
    {
        /// <summary>
        /// 缺少参数ID
        /// </summary>
        LACK_ID = 201,

        /// <summary>
        /// 参数page无效
        /// </summary>
        INVALID_PAGE = 202,

        /// <summary>
        /// 参数pageSize无效
        /// </summary>
        INVALID_PAGE_SIZE = 203,

        /// <summary>
        /// 参数pageSize无效，支持到10000
        /// </summary>
        INVALID_PAGE_SIZE_BIG = 204,

        /// <summary>
        /// 缺少参数startDate
        /// </summary>
        LACK_START_DATE = 205,

        /// <summary>
        /// 缺少参数endDate
        /// </summary>
        LACK_END_DATE = 206,

        /// <summary>
        /// 用户不存在
        /// </summary>
        USER_NON_EXISTENT = 207,
        /// <summary>
        /// 请求失败
        /// </summary>
        REQUEST_API_ERROR = 208,

        /// <summary>
        /// 数据操作失败
        /// </summary>
        DATA_EXECUTE_ERROR = 209,

        /// <summary>
        /// 数据不存在
        /// </summary>
        DATA_NON_EXISTENT = 210,

        /// <summary>
        /// 数据已经存在
        /// </summary>
        DATA_EXISTENT = 211,

        /// <summary>
        /// 不支持的请求
        /// </summary>
        NOT_SUPPORT_REQUEST = 212,

        /// <summary>
        /// 请求参数错误
        /// </summary>
        REQUEST_PARAMS_ERROR = 213,

        /// <summary>
        /// 功能未开发或开发中
        /// </summary>
        DEVELOPING = 999,
    }
    #endregion

    #region 登录错误
    public enum UserErrorCode
    {
        /// <summary>
        /// 获取一键登录的手机号码失败
        /// </summary>
        [EnumDescription("获取一键登录的手机号码失败")]
        OneKeyGetPhone = 301,

        /// <summary>
        /// token失效或不存在
        /// </summary>
        [EnumDescription("token失效或不存在")]
        OneKeyToken = 302,

        /// <summary>
        /// 获取配置失败
        /// </summary>
        [EnumDescription("获取配置失败")]
        OneKeyConfig = 303,

        /// <summary>
        /// 同一号码连续两次提交认证间隔过短
        /// </summary>
        [EnumDescription("同一号码连续两次提交认证间隔过短")]
        OneKeyTimeShort = 304,

        /// <summary>
        /// appKey自然日认证消耗超过限制
        /// </summary>
        [EnumDescription("appKey自然日认证消耗超过限制")]
        OneKeyConsumptionLimit = 305,

        /// <summary>
        /// 账户余额不足
        /// </summary>
        [EnumDescription("账户余额不足")]
        OneKeyCreditLow = 306,

        /// <summary>
        /// 未开通认证服务
        /// </summary>
        [EnumDescription("未开通认证服务")]
        OneKeyServiceNotAvailable = 307,

        /// <summary>
        /// 服务器未知错误
        /// </summary>
        [EnumDescription("服务器未知错误")]
        OneKeyServerUnknownRrror = 308,

        /// <summary>
        /// 账号错误
        /// </summary>
        [EnumDescription("账号错误")]
        AccountError = 309,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [EnumDescription("验证码错误")]
        VerifyCodeError = 310,

        /// <summary>
        /// 验证码和密码必须有一个不为空
        /// </summary>
        [EnumDescription("验证码和密码必须有一个不为空")]
        VerifyCodeOrPasswordError = 311,

        /// <summary>
        /// 账号或密码错误
        /// </summary>
        [EnumDescription("账号或密码错误")]
        AccountPasswordError = 312,

        /// <summary>
        /// 参数错误
        /// </summary>
        [EnumDescription("参数错误")]
        ParamsError = 313,


        /// <summary>
        /// 获取用户openid失败
        /// </summary>
        [EnumDescription("获取用户openid失败")]
        GetWxOpenidError = 314,


        /// <summary>
        /// 用户已存在
        /// </summary>
        [EnumDescription("用户已存在")]
        UserIsExist = 315,

        /// <summary>
        /// 当前IP注册用户数过多
        /// </summary>
        [EnumDescription("当前IP注册用户数过多")]
        RegisterIpMost = 316,

        /// <summary>
        /// 手机号码不能为空
        /// </summary>
        [EnumDescription("手机号码不能为空")]
        SmsPhoneEmpty = 317,


        /// <summary>
        /// 账号已绑定，无需重复绑定
        /// </summary>
        [EnumDescription("账号已绑定，无需重复绑定")]
        AccountBound = 318,

        /// <summary>
        /// 账号已被绑定，无法绑定
        /// </summary>
        [EnumDescription("账号已被绑定，无法绑定")]
        AccountBeUsed = 319,

        /// <summary>
        /// 数据更新失败
        /// </summary>
        [EnumDescription("数据更新失败")]
        UpdateError = 320,

        /// <summary>
        /// 您的实名认证正在审核中
        /// </summary>
        [EnumDescription("您的实名认证正在审核中")]
        VerifyReview = 321,

        /// <summary>
        /// 您已实名认证
        /// </summary>
        [EnumDescription("您已实名认证")]
        Certified = 322,

        /// <summary>
        /// 请求过于频繁
        /// </summary>
        [EnumDescription("请求过于频繁")]
        RequestFrequent = 323,

        /// <summary>
        /// 发送短信失败
        /// </summary>
        [EnumDescription("发送短信失败")]
        SmsFail = 324,

        /// <summary>
        /// 短信发送过于频繁
        /// </summary>
        [EnumDescription("短信发送过于频繁")]
        SmsMobileMost = 325,

        /// <summary>
        /// 余额不足
        /// </summary>
        [EnumDescription("余额不足")]
        BalanceNotEnough = 326,

        /// <summary>
        /// 未找到支付配置数据
        /// </summary>
        [EnumDescription("未找到支付配置数据")]
        Pay_Data_Unfind = 327,

        /// <summary>
        /// 支付失败
        /// </summary>
        [EnumDescription("支付失败")]
        Pay_Request_Fail = 328,

        /// <summary>
        /// 未知的支付方式
        /// </summary>
        [EnumDescription("未知的支付方式")]
        Pay_Method_Unknown = 329,

        /// <summary>
        /// 订单状态无效
        /// </summary>
        [EnumDescription("订单状态无效")]
        Order_Status_Error = 330,

        /// <summary>
        /// 退款金额不能大于订单金额
        /// </summary>
        [EnumDescription("退款金额不能大于订单金额")]
        Refund_Price_Error = 331,


        /// <summary>
        /// 获取云信accid失败
        /// </summary>
        [EnumDescription("获取云信accid失败")]
        GetYunXinAccIdFail = 332,

        /// <summary>
        /// 账号不存在
        /// </summary>
        [EnumDescription("账号不存在")]
        UserIsNotExist = 333,
    }
    #endregion
}
