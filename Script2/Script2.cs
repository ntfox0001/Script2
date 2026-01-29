namespace Script2;

public class Script2 : IScript2
{
    private readonly Script2Environment _env = new();
    public object Execute(string expression)
    {
        return Script2Parser.Execute(expression, _env);
    }

    public void RegisterFunc<TR>(string funcName, Func<TR> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, TR>(string funcName, Func<T1, TR> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2, TR>(string funcName, Func<T1, T2, TR> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2, T3, TR>(string funcName, Func<T1, T2, T3, TR> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2, T3, T4, TR>(string funcName, Func<T1, T2, T3, T4, TR> func)
    {
        _env.RegisterFunc(funcName, func); 
    }

    public void RegisterFunc(string funcName, Action func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1>(string funcName, Action<T1> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2>(string funcName, Action<T1, T2> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2, T3>(string funcName, Action<T1, T2, T3> func)
    {
        _env.RegisterFunc(funcName, func);
    }

    public void RegisterFunc<T1, T2, T3, T4>(string funcName, Action<T1, T2, T3, T4> func)
    {
        _env.RegisterFunc(funcName, func);
    }
}