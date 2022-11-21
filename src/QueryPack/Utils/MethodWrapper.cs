namespace QueryPack.Utils
{
    using System;

    internal interface IMethodWrapper { }

    internal class GenericMethodWrapper<T> : IMethodWrapper
    {
        public GenericMethodWrapper(Type type, Func<object, object[], T> method)
        {
            Type = type;
            Method = method;
        }

        public Type Type { get; }

        /// <summary>
        /// arg1 - instance of caller
        /// arg2 - array of args
        /// </summary>
        public Func<object, object[], T> Method { get; }
    }
}
