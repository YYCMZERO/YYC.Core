using System;
using System.Reflection;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace AntJoin.Core.Threading
{
   /// <summary>
    /// Provides some helper methods to work with async methods.
    /// </summary>
    public static class AsyncHelper
    {
        /// <summary>
        /// Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            return method.ReturnType.IsTaskOrTaskOfT();
        }

        public static bool IsTaskOrTaskOfT(this Type type)
        {
            return type == typeof(Task) || (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>));
        }

        public static bool IsTaskOfT(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        /// Returns void if given type is Task.
        /// Return T, if given type is Task{T}.
        /// Returns given type otherwise.
        /// </summary>
        public static Type UnwrapTask(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (type == typeof(Task))
            {
                return typeof(void);
            }
            return type.IsTaskOfT() ? type.GenericTypeArguments[0] : type;
        }

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="func">A function that returns a result</param>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <returns>Result of the async operation</returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func) => AsyncContext.Run(func);

        /// <summary>
        /// Runs a async method synchronously.
        /// </summary>
        /// <param name="action">An async action</param>
        public static void RunSync(Func<Task> action) => AsyncContext.Run(action);
    }
}