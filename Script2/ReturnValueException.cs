namespace Script2;

// 用于实现 return 语句的提前返回异常
public class ReturnValueException : Exception
{
    public object ReturnValue { get; }

    public ReturnValueException(object returnValue)
    {
        ReturnValue = returnValue;
    }
}