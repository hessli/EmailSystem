namespace EmailSys.Impl
{
    public interface InterceptorConfig
    {  
        string TagName { get; }

        /// <summary>
        /// 最大数
        /// </summary>
        int MaxCount { get;  }
        /// <summary>
        /// 频率
        /// </summary>
        int Frequency { get; }
    }
}
