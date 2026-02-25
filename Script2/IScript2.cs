namespace Script2;

public interface IScript2
{
    /// <summary>
    /// 打印回调接口，用于自定义 print 函数的输出行为
    /// </summary>
    Action<string> OnPrint { get; set; }

    object Execute(string expression);
    void RegisterFunc<TR>(string funcName, Func<TR> func);
    void RegisterFunc<T1, TR>(string funcName, Func<T1, TR> func);
    void RegisterFunc<T1, T2, TR>(string funcName, Func<T1, T2, TR> func);
    void RegisterFunc<T1, T2, T3, TR>(string funcName, Func<T1, T2, T3, TR> func);
    void RegisterFunc<T1, T2, T3, T4, TR>(string funcName, Func<T1, T2, T3, T4, TR> func);

    void RegisterFunc(string funcName, Action func);
    void RegisterFunc<T1>(string funcName, Action<T1> func);
    void RegisterFunc<T1, T2>(string funcName, Action<T1, T2> func);
    void RegisterFunc<T1, T2, T3>(string funcName, Action<T1, T2, T3> func);
    void RegisterFunc<T1, T2, T3, T4>(string funcName, Action<T1, T2, T3, T4> func);
}