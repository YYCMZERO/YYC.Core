using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AntJoin.Repository
{
    internal static class Extension
    {
        /// <summary>
        /// 获取查询条件个数
        /// </summary>
        /// <param name="expression">谓词表达式,范例1：t => t.Name == "A" ，结果1。
        /// 范例2：t => t.Name == "A" &amp;&amp; t.Age =1 ，结果2。</param>
        internal static int GetConditionCount(LambdaExpression expression)
        {
            if (expression == null)
            {
                return 0;
            }
            var result = expression.ToString().Replace("AndAlso", "|").Replace("OrElse", "|");
            return result.Split('|').Count();
        }


        /// <summary>
        /// 获取lambda表达式的值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        internal static object Value<T>(this Expression<Func<T, bool>> expression)
        {
            return GetValue(expression);
        }


        /// <summary>
        /// 安全转换为字符串，去除两端空格，当值为null时返回""
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static string SafeString(this object input)
        {
            return input == null ? string.Empty : input.ToString().Trim();
        }


        /// <summary>
        /// 获取值,范例：t => t.Name == "A",返回 A
        /// </summary>
        /// <param name="expression">表达式,范例：t => t.Name == "A"</param>
        private static object GetValue(Expression expression)
        {
            if (expression == null)
                return null;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    return GetValue(((LambdaExpression)expression).Body);
                case ExpressionType.Convert:
                    return GetValue(((UnaryExpression)expression).Operand);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThanOrEqual:
                    return GetValue(((BinaryExpression)expression).Right);
                case ExpressionType.Call:
                    return GetMethodCallExpressionValue(expression);
                case ExpressionType.MemberAccess:
                    return GetMemberValue((MemberExpression)expression);
                case ExpressionType.Constant:
                    return GetConstantExpressionValue(expression);
                case ExpressionType.Not:
                    if (expression.Type == typeof(bool))
                        return false;
                    return null;
            }

            return null;
        }

        /// <summary>
        /// 获取方法调用表达式的值
        /// </summary>
        private static object GetMethodCallExpressionValue(Expression expression)
        {
            var methodCallExpression = (MethodCallExpression)expression;
            var value = GetValue(methodCallExpression.Arguments.FirstOrDefault());
            if (value != null)
            {
                return value;
            }
            if (methodCallExpression.Object == null)
            {
                return methodCallExpression.Type.InvokeMember(methodCallExpression.Method.Name,
                    BindingFlags.InvokeMethod, null, null, null);
            }
            return GetValue(methodCallExpression.Object);
        }

        /// <summary>
        /// 获取属性表达式的值
        /// </summary>
        private static object GetMemberValue(MemberExpression expression)
        {
            if (expression == null)
            {
                return null;
            }
            var field = expression.Member as FieldInfo;
            if (field != null)
            {
                var constValue = GetConstantExpressionValue(expression.Expression);
                return field.GetValue(constValue);
            }

            var property = expression.Member as PropertyInfo;
            if (property == null)
            {
                return null;
            }
            if (expression.Expression == null)
            {
                return property.GetValue(null);
            }
            var value = GetMemberValue(expression.Expression as MemberExpression);
            if (value == null)
            {
                if (property.PropertyType == typeof(bool))
                {
                    return true;
                }
                return null;
            }

            return property.GetValue(value);
        }

        /// <summary>
        /// 获取常量表达式的值
        /// </summary>
        private static object GetConstantExpressionValue(Expression expression)
        {
            var constantExpression = (ConstantExpression)expression;
            return constantExpression.Value;
        }
    }
}
