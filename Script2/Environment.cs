using System.Data;

namespace Script2;

public class Script2Environment
{
    /// <summary>
    /// 是否使用解释器模式
    /// true: 使用解释器模式（兼容 IL2CPP，但速度较慢）
    /// false: 使用编译模式（快速，但需要 .NET 运行时支持 Expression.Compile）
    /// 默认为 false，在 Unity IL2CPP 环境中请手动设置为 true
    /// </summary>
    public bool UseInterpreterMode { get; private set; } = false;
    // 根环境，所有子环境共享
    private readonly Script2Environment _rootEnv = null;

    // 变量字典（当前环境的本地变量）
    private readonly Dictionary<string, object> _variables = new();

    // 函数字典（根环境拥有，子环境共享）
    // 改为存储 Delegate，以便更好地处理不同类型的委托
    private readonly Dictionary<string, Func<object[], object>> _functions = new();

    // 打印回调接口
    public Action<string> OnPrint { get; set; }

    /// <summary>
    /// 构造函数 - 创建根环境
    /// </summary>
    public Script2Environment(bool useInterpreterMode)
    {
        UseInterpreterMode = useInterpreterMode;
        // 根环境的 _rootEnv 为 null
        Setup();
    }

    /// <summary>
    /// 私有构造函数 - 创建子环境
    /// </summary>
    private Script2Environment(Script2Environment rootEnv)
    {
        _rootEnv = rootEnv;
        _functions = rootEnv._functions; // 共享函数字典
        _variables = new Dictionary<string, object>(); // 独立的本地变量
    }

    private void Setup()
    {
        _variables["PI"] = 3.1415926f;
        _variables["E"] = 2.71828;

        // 注册 print 函数（支持多个参数）
        // 使用 RegisterFunction 而不是 RegisterFunc，因为我们需要直接处理 args 数组
        AddFunc("print", new Func<object[], object>(args =>
        {
            var output = string.Join("", args.Select(arg => arg?.ToString() ?? "null"));
            if (OnPrint != null)
            {
                OnPrint(output);
            }
            else
            {
                Console.WriteLine(output);
            }
            return VoidValue.Instance;
        }));

        // 注册 concat 函数（字符串连接）
        AddFunc("concat", new Func<object[], object>(args =>
        {
            return string.Join("", args.Select(arg => arg?.ToString() ?? ""));
        }));

        // 注册 format 函数（格式化字符串）
        AddFunc("format", new Func<object[], object>(args =>
        {
            if (args.Length == 0)
                return "";

            var formatString = args[0]?.ToString() ?? "";
            if (args.Length == 1)
                return formatString;

            var values = args.Skip(1).Select(arg => arg?.ToString() ?? "null").ToArray<object>();
            return string.Format(formatString, values);
        }));

        // 注册 len 函数（获取数组长度）
        AddFunc("len", new Func<object[], object>(args =>
        {
            if (args.Length != 1)
                throw new InvalidOperationException($"Function 'len' expects 1 argument, got {args.Length}.");
            if (args[0] is List<object> list)
                return (float)list.Count;
            throw new InvalidOperationException($"Cannot get length of non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 push 函数（向数组末尾添加元素）
        AddFunc("push", new Func<object[], object>(args =>
        {
            if (args.Length != 2)
                throw new InvalidOperationException($"Function 'push' expects 2 arguments (array, value), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                if (args[1] is VoidValue)
                    throw new InvalidOperationException("Cannot push void value to array.");
                list.Add(args[1]);
                return (float)list.Count;
            }
            throw new InvalidOperationException($"Cannot push to non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 remove 函数（删除数组指定索引的元素）
        AddFunc("remove", new Func<object[], object>(args =>
        {
            if (args.Length != 2)
                throw new InvalidOperationException($"Function 'remove' expects 2 arguments (array, index), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                var idx = Convert.ToInt32(args[1]);
                if (idx < 0 || idx >= list.Count)
                    throw new IndexOutOfRangeException($"Array index {idx} is out of range (array length: {list.Count}).");
                var removed = list[idx];
                list.RemoveAt(idx);
                return removed;
            }
            throw new InvalidOperationException($"Cannot remove from non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 insert 函数（在数组指定位置插入元素）
        AddFunc("insert", new Func<object[], object>(args =>
        {
            if (args.Length != 3)
                throw new InvalidOperationException($"Function 'insert' expects 3 arguments (array, index, value), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                var idx = Convert.ToInt32(args[1]);
                if (idx < 0 || idx > list.Count)
                    throw new IndexOutOfRangeException($"Array index {idx} is out of range for insertion (array length: {list.Count}).");
                if (args[2] is VoidValue)
                    throw new InvalidOperationException("Cannot insert void value to array.");
                list.Insert(idx, args[2]);
                return (float)list.Count;
            }
            throw new InvalidOperationException($"Cannot insert into non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 pop 函数（移除并返回数组末尾元素）
        AddFunc("pop", new Func<object[], object>(args =>
        {
            if (args.Length != 1)
                throw new InvalidOperationException($"Function 'pop' expects 1 argument (array), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                if (list.Count == 0)
                    return null;
                var last = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return last;
            }
            throw new InvalidOperationException($"Cannot pop from non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 contains 函数（检查数组是否包含指定值）
        AddFunc("contains", new Func<object[], object>(args =>
        {
            if (args.Length != 2)
                throw new InvalidOperationException($"Function 'contains' expects 2 arguments (array, value), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                return list.Contains(args[1]);
            }
            throw new InvalidOperationException($"Cannot check contains on non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 indexOf 函数（查找值的索引，未找到返回 -1）
        AddFunc("indexOf", new Func<object[], object>(args =>
        {
            if (args.Length != 2)
                throw new InvalidOperationException($"Function 'indexOf' expects 2 arguments (array, value), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                return (float)list.IndexOf(args[1]);
            }
            throw new InvalidOperationException($"Cannot indexOf on non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 slice 函数（返回子数组，end 可选，默认到末尾）
        AddFunc("slice", new Func<object[], object>(args =>
        {
            if (args.Length < 2 || args.Length > 3)
                throw new InvalidOperationException($"Function 'slice' expects 2 or 3 arguments (array, start, [end]), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                var start = Convert.ToInt32(args[1]);
                if (start < 0) start = list.Count + start;
                var end = args.Length == 3 ? Convert.ToInt32(args[2]) : list.Count;
                if (end < 0) end = list.Count + end;
                start = Math.Max(0, Math.Min(start, list.Count));
                end = Math.Max(0, Math.Min(end, list.Count));
                return new List<object>(list.GetRange(start, end - start));
            }
            throw new InvalidOperationException($"Cannot slice non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 join 函数（用分隔符拼接数组元素为字符串）
        AddFunc("join", new Func<object[], object>(args =>
        {
            if (args.Length != 2)
                throw new InvalidOperationException($"Function 'join' expects 2 arguments (array, separator), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                return string.Join(args[1]?.ToString() ?? "", list.Select(v => v?.ToString() ?? "null"));
            }
            throw new InvalidOperationException($"Cannot join non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));

        // 注册 sort 函数（返回排序后的新数组，原数组不变）
        AddFunc("sort", new Func<object[], object>(args =>
        {
            if (args.Length != 1)
                throw new InvalidOperationException($"Function 'sort' expects 1 argument (array), got {args.Length}.");
            if (args[0] is List<object> list)
            {
                var sorted = new List<object>(list);
                sorted.Sort((a, b) =>
                {
                    if (a is float af && b is float bf) return af.CompareTo(bf);
                    if (a is string as_ && b is string bs) return string.Compare(as_, bs, StringComparison.Ordinal);
                    return string.Compare(a?.ToString(), b?.ToString(), StringComparison.Ordinal);
                });
                return sorted;
            }
            throw new InvalidOperationException($"Cannot sort non-array value of type {args[0]?.GetType().Name ?? "null"}.");
        }));
    }

    /// <summary>
    /// 获取变量值（先查本地变量，找不到则查询根环境）
    /// </summary>
    public object GetVariableValue(string varName)
    {
        // 先查本地变量
        if (_variables.TryGetValue(varName, out var value))
            return value;

        // 如果不是根环境，尝试从根环境获取
        if (_rootEnv != null)
        {
            return _rootEnv.GetVariableValue(varName);
        }

        throw new InvalidOperationException($"Variable '{varName}' is not defined.");
    }

    /// <summary>
    /// 设置变量值（设置到当前环境的本地变量）
    /// </summary>
    public void SetVariableValue(string varName, object value)
    {
        // 检查是否为void值
        if (value is VoidValue)
        {
            throw new InvalidOperationException(
                $"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
        }

        _variables[varName] = value;
    }

    /// <summary>
    /// 获取数组的指定索引元素（支持负索引）
    /// </summary>
    public object GetArrayItem(object array, object index)
    {
        if (array is List<object> list)
        {
            var idx = Convert.ToInt32(index);
            if (idx < 0) idx = list.Count + idx;
            if (idx < 0 || idx >= list.Count)
                throw new IndexOutOfRangeException($"Array index {idx} is out of range (array length: {list.Count}).");
            return list[idx];
        }
        throw new InvalidOperationException($"Cannot index into non-array value of type {array?.GetType().Name ?? "null"}.");
    }

    /// <summary>
    /// 设置数组的指定索引元素（支持负索引）
    /// </summary>
    public void SetArrayItem(object array, object index, object value)
    {
        if (value is VoidValue)
        {
            throw new InvalidOperationException("Cannot assign void value to array element.");
        }

        if (array is List<object> list)
        {
            var idx = Convert.ToInt32(index);
            if (idx < 0) idx = list.Count + idx;
            if (idx < 0 || idx >= list.Count)
                throw new IndexOutOfRangeException($"Array index {idx} is out of range (array length: {list.Count}).");
            list[idx] = value;
            return;
        }
        throw new InvalidOperationException($"Cannot index into non-array value of type {array?.GetType().Name ?? "null"}.");
    }

    public object CallFunction(string fn, object[] args)
    {
        object result;
        if (_functions.TryGetValue(fn, out var func))
        {
            try
            {
                // // 获取委托的 Invoke 方法
                // var invokeMethod = func.GetType().GetMethod("Invoke");
                // var parameters = invokeMethod!.GetParameters();
                //
                // if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]))
                // {
                //     // 解释器模式：委托期望一个 object[] 参数
                //     result = invokeMethod.Invoke(func, new object[] { args });
                // }
                // else if (parameters.Length == args.Length)
                // {
                //     // 编译模式：委托期望多个 object 参数
                //     result = invokeMethod.Invoke(func, args);
                // }
                // else
                // {
                //     throw new InvalidOperationException(
                //         $"Function '{fn}' delegate signature mismatch. " +
                //         $"Delegate expects {parameters.Length} parameters, but {args.Length} arguments were provided.");
                // }
                if (UseInterpreterMode)
                {
                    // Console.WriteLine($"委托类型：{func.GetType()}");
                    // Console.WriteLine($"委托需要的参数数量：{func.Method.GetParameters().Length}");
                    // foreach (var param in func.Method.GetParameters())
                    // {
                    //     Console.WriteLine($"参数类型：{param.ParameterType}，参数名：{param.Name}");
                    // }
                    // result = func.DynamicInvoke(new object[]{args});
                    result = func(new object[]{args});
                }
                else
                {
                    result = func(args);
                }
                
            }
            catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException != null)
            {
                // 解包内部异常
                throw tie.InnerException;
            }
        }
        else
        {
            var methodInfo = FindMathMethod(fn, args);
            if (methodInfo == null)
                throw new InvalidOperationException($"Function '{fn}' is not supported.");

            // 统一转换为 float
            var convertedArgs = args.Select(arg => Convert.ChangeType(arg, typeof(float))).ToArray();

            result = methodInfo.Invoke(null, convertedArgs);
        }

        // 不在这里检查返回值是否为 void，让调用方决定如何处理
        return result;
    }

    /// <summary>
    /// 检查两个值是否可以进行相等性比较
    /// </summary>
    public static void CheckTypesForEquality(object left, object right)
    {
        if (left == null && right == null)
            return;

        if (left == null || right == null)
            throw new InvalidOperationException(
                $"Type mismatch: cannot compare null with {right ?? left}");

        Type leftType = left.GetType();
        Type rightType = right.GetType();

        // 检查类型是否匹配
        bool typesMatch = (leftType == rightType) ||
                          (leftType == typeof(float) && rightType == typeof(float)) ||
                          (leftType == typeof(string) && rightType == typeof(string)) ||
                          (leftType == typeof(bool) && rightType == typeof(bool));

        if (!typesMatch)
        {
            // 将类型名称统一为常见名称
            string GetTypeName(Type t)
            {
                if (t == typeof(float)) return "float";
                if (t == typeof(string)) return "string";
                if (t == typeof(bool)) return "bool";
                return t.Name;
            }

            throw new InvalidOperationException(
                $"Type mismatch: cannot compare {GetTypeName(leftType)} with {GetTypeName(rightType)}. " +
                "Logical equality (==) and inequality (!=) operations require both operands to be of the same type.");
        }
    }

    /// <summary>
    /// 检查是否存在指定函数
    /// </summary>
    public bool HasFunction(string name)
    {
        return _functions.ContainsKey(name);
    }

    /// <summary>
    /// 注册自定义函数（只能在根环境中注册）
    /// </summary>
    public void RegisterFunction(string name, Delegate func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        if (!_functions.TryAdd(name, func.DynamicInvoke))
        {
            throw new DuplicateNameException($"Function already exists: {name}");
        }
    }

    /// <summary>
    /// 创建子环境（共享函数字典，独立的本地变量）
    /// </summary>
    public Script2Environment CreateChildEnvironment()
    {
        var newEnv = new Script2Environment(_rootEnv ?? this)
        {
            UseInterpreterMode = UseInterpreterMode
        };
        return newEnv;
    }

    /// <summary>
    /// 在Math类中查找匹配的方法重载
    /// </summary>
    private static System.Reflection.MethodInfo FindMathMethod(string methodName, object[] args)
    {
        var methods = typeof(MathF).GetMethods(
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static);

        // 只查找 float 类型的重载
        var matchingMethods = methods.Where(m =>
            m.Name == methodName &&
            m.GetParameters().Length == args.Length &&
            m.GetParameters().All(p => p.ParameterType == typeof(float))
        ).ToList();

        if (matchingMethods.Count == 0)
            return null;

        return matchingMethods[0];
    }

    // 单个参数的函数注册（只能在根环境中注册）
    public void RegisterFunc<TR>(string funcName, Func<TR> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 0 })
                throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");

            return func();
        }));
    }

    // 单个参数的函数注册（只能在根环境中注册）
    public void RegisterFunc<T1, TR>(string funcName, Func<T1, TR> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 1 })
                throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");

            var arg = Convertor.ConvertToTargetType<T1>(args[0]);
            return func(arg);
        }));
    }

    // 两个参数的函数注册（只能在根环境中注册）
    public void RegisterFunc<T1, T2, TR>(string funcName, Func<T1, T2, TR> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 2 })
                throw new InvalidOperationException($"Function '{funcName}' expects 2 arguments.");

            var (arg1, arg2) = Convertor.ConvertToTargetTypes<T1, T2>(args[0], args[1], funcName);
            return func(arg1, arg2);
        }));
    }

    // 三个参数的函数注册（只能在根环境中注册）
    public void RegisterFunc<T1, T2, T3, TR>(string funcName, Func<T1, T2, T3, TR> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 3 })
                throw new InvalidOperationException($"Function '{funcName}' expects 3 arguments.");

            var (arg1, arg2, arg3) = Convertor.ConvertToTargetTypes<T1, T2, T3>(args[0], args[1], args[2], funcName);
            return func(arg1, arg2, arg3);
        }));
    }

    // 四个参数的函数注册（只能在根环境中注册）
    public void RegisterFunc<T1, T2, T3, T4, TR>(string funcName, Func<T1, T2, T3, T4, TR> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 4 })
                throw new InvalidOperationException($"Function '{funcName}' expects 4 arguments.");

            var (arg1, arg2, arg3, arg4) =
                Convertor.ConvertToTargetTypes<T1, T2, T3, T4>(args[0], args[1], args[2], args[3], funcName);
            return func(arg1, arg2, arg3, arg4);
        }));
    }

    // 无返回值的函数注册 - 0个参数（只能在根环境中注册）
    public void RegisterFunc(string funcName, Action func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 0 })
                throw new InvalidOperationException($"Function '{funcName}' expects 0 arguments.");

            func();
            return VoidValue.Instance;
        }));
    }

    // 无返回值的函数注册 - 1个参数（只能在根环境中注册）
    public void RegisterFunc<T1>(string funcName, Action<T1> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 1 })
                throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");

            var arg = Convertor.ConvertToTargetType<T1>(args[0]);
            func(arg);
            return VoidValue.Instance;
        }));
    }

    // 无返回值的函数注册 - 2个参数（只能在根环境中注册）
    public void RegisterFunc<T1, T2>(string funcName, Action<T1, T2> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 2 })
                throw new InvalidOperationException($"Function '{funcName}' expects 2 arguments.");

            var (arg1, arg2) = Convertor.ConvertToTargetTypes<T1, T2>(args[0], args[1], funcName);
            func(arg1, arg2);
            return VoidValue.Instance;
        }));
    }

    // 无返回值的函数注册 - 3个参数（只能在根环境中注册）
    public void RegisterFunc<T1, T2, T3>(string funcName, Action<T1, T2, T3> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 3 })
                throw new InvalidOperationException($"Function '{funcName}' expects 3 arguments.");

            var (arg1, arg2, arg3) = Convertor.ConvertToTargetTypes<T1, T2, T3>(args[0], args[1], args[2], funcName);
            func(arg1, arg2, arg3);
            return VoidValue.Instance;
        }));
    }

    // 无返回值的函数注册 - 4个参数（只能在根环境中注册）
    public void RegisterFunc<T1, T2, T3, T4>(string funcName, Action<T1, T2, T3, T4> func)
    {
        if (_rootEnv != null)
            throw new InvalidOperationException("Functions can only be registered in the root environment.");

        AddFunc(funcName, new Func<object[], object>(args =>
        {
            if (args is not { Length: 4 })
                throw new InvalidOperationException($"Function '{funcName}' expects 4 arguments.");

            var (arg1, arg2, arg3, arg4) =
                Convertor.ConvertToTargetTypes<T1, T2, T3, T4>(args[0], args[1], args[2], args[3], funcName);
            func(arg1, arg2, arg3, arg4);
            return VoidValue.Instance;
        }));
    }

    private void AddFunc(string funcName, Func<object[], object> f)
    {
        _functions[funcName] = UseInterpreterMode ? f.DynamicInvoke : f;
    }
}