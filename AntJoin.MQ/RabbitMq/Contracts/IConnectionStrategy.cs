﻿using Polly;

namespace AntJoin.MQ.Contracts
{
    public interface IConnectionStrategy
    {
        /// <summary>
        /// 创建连接策略
        /// </summary>
        /// <returns></returns>
        ISyncPolicy CreatePolicy();
    }
}