namespace AntJoin.Repository
{
    /// <summary>
    /// 实体映射配置
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public abstract class EntityMap<TEntity> : EntityTypeMap<TEntity>, IEntityRegister where TEntity : class
    {
    }
}
