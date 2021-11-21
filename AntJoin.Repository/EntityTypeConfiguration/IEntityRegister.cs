using Microsoft.EntityFrameworkCore;

namespace AntJoin.Repository
{
    /// <summary>
    /// 映射
    /// </summary>
    public interface IEntityRegister
    {
        /// <summary>
        /// 映射配置
        /// </summary>
        /// <param name="builder">模型生成器</param>
        void RegistTo(ModelBuilder builder);
    }
}
