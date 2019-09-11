namespace EmailSys.Core
{
    public enum RuningState
    {
        Runing = 1,

        Stop = 2
    }
    public enum EmailDeliveryMethod
    {
        Network = 1
    }

    public enum Frequency
    {
        /// <summary>
        /// 小时限制
        /// </summary>
        Hour = 1,
        /// <summary>
        /// 按日限制
        /// </summary>
        Day = 2
    }

    public enum ErrorLevel
    {
        Application = 1,
        Emitter = 2,
        Args = 3
    }
}
