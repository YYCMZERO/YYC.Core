using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AntJoin.Repository
{
    /// <summary>
    /// 映射配置
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class EntityTypeMap<TEntity> : IEntityTypeConfiguration<TEntity>, IEntityRegister where TEntity : class
    {
        /// <summary>
        /// 模型生成器
        /// </summary>
        protected ModelBuilder ModelBuilder { get; private set; }

        /// <summary>
        /// 映射配置
        /// </summary>
        /// <param name="builder">模型生成器</param>
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);


        /// <summary>
        /// 将当前实体类映射对象注册到数据上下文模型构建器中
        /// </summary>
        /// <param name="builder">上下文模型构建器</param>
        public void RegistTo(ModelBuilder builder)
        {
            builder.ApplyConfiguration(this);
        }
    }
}
