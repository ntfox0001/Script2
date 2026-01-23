namespace Script2;

public static class ObjectExtensions
{
    /// <summary>
    /// 将对象转为字符串，其中bool类型会转为小写的true/false，null转为空字符串
    /// </summary>
    internal static string ToLowercaseString(this object obj)
    {
        return obj switch
        {
            bool b => b.ToString().ToLower(), // 专门处理bool类型
            string s => $"\"{s}\"",
            null => string.Empty,             // 处理null
            _ => obj.ToString()               // 其他类型默认转换
        };
    }
}