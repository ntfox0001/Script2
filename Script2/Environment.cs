namespace Script2
{
    // 特殊的void标记值
    public class VoidValue
    {
        private VoidValue() { }
        public static readonly VoidValue Instance = new VoidValue();
    }

    // 用于实现 return 语句的提前返回异常
    public class ReturnValueException : Exception
    {
        public object ReturnValue { get; }

        public ReturnValueException(object returnValue)
        {
            ReturnValue = returnValue;
        }
    }

    public class Script2Environment
    {
        // 根环境，所有子环境共享
        private readonly Script2Environment _rootEnv = null;

        // 变量字典（当前环境的本地变量）
        private readonly Dictionary<string, object> _variables = new();

        // 函数字典（根环境拥有，子环境共享）
        private readonly Dictionary<string, Func<object[], object>> _functions = new();

        /// <summary>
        /// 构造函数 - 创建根环境
        /// </summary>
        public Script2Environment()
        {
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
                throw new InvalidOperationException($"Cannot assign void value to variable '{varName}'. Functions that don't return a value cannot be assigned to variables.");
            }
            _variables[varName] = value;
        }

        public object CallFunction(string fn, object[] args)
        {
            object result;
            if (_functions.TryGetValue(fn, out var func))
            {
                try
                {
                    result = func(args);
                }
                catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException != null)
                {
                    // 解包内部异常，让调用者能看到原始异常
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

            _functions[name] = func.DynamicInvoke;
        }

        /// <summary>
        /// 创建子环境（共享函数字典，独立的本地变量）
        /// </summary>
        public Script2Environment CreateChildEnvironment()
        {
            return new Script2Environment(_rootEnv ?? this);
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

            _functions[funcName] = args =>
            {
                if (args is not { Length: 0 })
                    throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");
                
                return func();
            };
        }
        
        // 单个参数的函数注册（只能在根环境中注册）
        public void RegisterFunc<T1, TR>(string funcName, Func<T1, TR> func)
        {
            if (_rootEnv != null)
                throw new InvalidOperationException("Functions can only be registered in the root environment.");

            _functions[funcName] = args =>
            {
                if (args is not { Length: 1 })
                    throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");

                var arg = Convertor.ConvertToTargetType<T1>(args[0]);
                return func(arg);
            };
        }

        // 两个参数的函数注册（只能在根环境中注册）
        public void RegisterFunc<T1, T2, TR>(string funcName, Func<T1, T2, TR> func)
        {
            if (_rootEnv != null)
                throw new InvalidOperationException("Functions can only be registered in the root environment.");

            _functions[funcName] = args =>
            {
                if (args is not { Length: 2 })
                    throw new InvalidOperationException($"Function '{funcName}' expects 2 arguments.");

                var (arg1, arg2) = Convertor.ConvertToTargetTypes<T1, T2>(args[0], args[1], funcName);
                return func(arg1, arg2);
            };
        }

        // 三个参数的函数注册（只能在根环境中注册）
        public void RegisterFunc<T1, T2, T3, TR>(string funcName, Func<T1, T2, T3, TR> func)
        {
            if (_rootEnv != null)
                throw new InvalidOperationException("Functions can only be registered in the root environment.");

            _functions[funcName] = args =>
            {
                if (args is not { Length: 3 })
                    throw new InvalidOperationException($"Function '{funcName}' expects 3 arguments.");

                var (arg1, arg2, arg3) = Convertor.ConvertToTargetTypes<T1, T2, T3>(args[0], args[1], args[2], funcName);
                return func(arg1, arg2, arg3);
            };
        }

        // 四个参数的函数注册（只能在根环境中注册）
        public void RegisterFunc<T1, T2, T3, T4, TR>(string funcName, Func<T1, T2, T3, T4, TR> func)
        {
            if (_rootEnv != null)
                throw new InvalidOperationException("Functions can only be registered in the root environment.");

            _functions[funcName] = args =>
            {
                if (args is not { Length: 4 })
                    throw new InvalidOperationException($"Function '{funcName}' expects 4 arguments.");

                var (arg1, arg2, arg3, arg4) = Convertor.ConvertToTargetTypes<T1, T2, T3, T4>(args[0], args[1], args[2], args[3], funcName);
                return func(arg1, arg2, arg3, arg4);
            };
        }
    }
}