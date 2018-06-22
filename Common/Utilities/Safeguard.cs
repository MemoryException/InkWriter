using System;

namespace Utilities
{
    public static class Safeguard
    {
        public static void EnsureNotNull(string parameterName, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}
