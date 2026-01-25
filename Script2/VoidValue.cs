namespace Script2;

// 特殊的void标记值
public class VoidValue
{
    private VoidValue()
    {
    }

    public static readonly VoidValue Instance = new VoidValue();
}