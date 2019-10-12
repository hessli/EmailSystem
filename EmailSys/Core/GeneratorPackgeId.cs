

namespace EmailSys.Core
{
    internal static class GeneratorPackgeId
    {
        private static object _synPackeIdObj = new object();
        private static uint _packageId;
        internal static uint GetPakcageId()
        {
            uint packageId;
            lock (_synPackeIdObj)
            {
                _packageId += 1u;
                if (_packageId == 0u)
                {
                    _packageId += 1u;
                }
                packageId = _packageId;
            }
            return packageId;
        }
    }
}
