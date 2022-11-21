namespace QueryPack.Utils
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class MethodFactory
    {
        /// <summary>
        /// arg1 - instance of caller
        /// arg2 - array of args
        /// </summary>
        public static Func<object, object[], T> CreateGenericMethod<T>(MethodInfo method)
        {
            if (!method.IsGenericMethod)
                throw new NotSupportedException();

            var instanceParameter = Expression.Parameter(typeof(object), "target");
            var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

            var call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            var lambda = Expression.Lambda<Func<object, object[], T>>(
                Expression.Convert(call, typeof(T)),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType)).ToArray();
        }
    }
}