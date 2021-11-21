using AntJoin.Core.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace AntJoin.Core.Domains
{
    public class EntityBase<TKey> : IEntity<TKey> where TKey : struct
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column(Name = "ID", IsPrimaryKey = true)]
        public virtual TKey ID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(Name = "CreateTime")]
        public virtual DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column(Name = "LastModifyTime")]
        public virtual DateTime? LastModifyTime { get; set; } = DateTime.Now;
    }


    public class EntityBase : EntityBase<int>
    {
    }


    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [Column(Name = "ID", IsPrimaryKey = true)]
        TKey ID { get; set; }
    }

    public interface IEntity
    {
    }

}
