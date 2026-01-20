using System;

namespace GoFire.Kernel.Script2
{
    internal static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    public static class Convertor
    {
        #region 类型转换辅助方法

        /// <summary>
        /// 将原始值转换为目标类型（支持基础类型、枚举、可空类型等）
        /// </summary>
        public static T ConvertToTargetType<T>(object rawValue)
        {
            var targetType = typeof(T);

            // 处理null（仅允许转换到可空类型）
            if (rawValue == null)
            {
                if (targetType.IsNullableType())
                {
                    return default!;
                }

                throw new ArgumentException(
                    $"目标类型 '{targetType.Name}' 不可为null");
            }

            var sourceType = rawValue.GetType();

            // 类型匹配直接返回
            if (targetType.IsAssignableFrom(sourceType))
            {
                return (T)rawValue;
            }

            // 尝试通用转换（支持基础类型、枚举等）
            try
            {
                if (targetType.IsEnum)
                {
                    return (T)Enum.Parse(targetType, rawValue.ToString()!);
                }

                return (T)Convert.ChangeType(rawValue, targetType);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"无法从 '{sourceType.Name}' ({rawValue}) 转换为 '{targetType.Name}'",
                    ex
                );
            }
        }

        /// <summary>
        /// 批量转换两个参数
        /// </summary>
        public static (T1, T2) ConvertToTargetTypes<T1, T2>(object raw1, object raw2, string funcName)
        {
            var param1 = ConvertToTargetType<T1>(raw1);
            var param2 = ConvertToTargetType<T2>(raw2);
            return (param1, param2);
        }

        /// <summary>
        /// 批量转换三个参数
        /// </summary>
        public static (T1, T2, T3) ConvertToTargetTypes<T1, T2, T3>(object raw1, object raw2, object raw3,
            string funcName)
        {
            var param1 = ConvertToTargetType<T1>(raw1);
            var param2 = ConvertToTargetType<T2>(raw2);
            var param3 = ConvertToTargetType<T3>(raw3);
            return (param1, param2, param3);
        }

        #endregion
    }
}