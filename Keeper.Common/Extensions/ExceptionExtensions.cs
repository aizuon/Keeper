using System;
using System.Runtime.ExceptionServices;

namespace Keeper.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static Exception Rethrow(this Exception @this)
        {
            ExceptionDispatchInfo.Capture(@this).Throw();
            return null;
        }
    }
}
