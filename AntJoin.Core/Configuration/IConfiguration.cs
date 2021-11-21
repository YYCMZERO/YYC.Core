namespace AntJoin.Core.Configuration
{
    /// <summary>
    /// 配置抽象接口。
    /// </summary>
    public interface IConfiguration
    {
        ///// <summary>
        ///// 配置更改回调事件。
        ///// </summary>
        //event EventHandler<ConfigurationChangeEventArgs> ConfigChanged;

        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string GetValue(string key);

        /// <summary>
        /// 获取配置项。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        TValue GetValue<TValue>(string key);

        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        string GetDefaultValue(string key, string defaultValue);

        /// <summary>
        /// 获取配置项，如果值为 null 则取参数 <see cref="defaultValue"/> 值。
        /// </summary>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        TValue GetDefaultValue<TValue>(string key, TValue defaultValue);
    }
}