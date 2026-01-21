namespace Script2
{
    public class Script2Environment
    {
        public Dictionary<string, Func<object[], object>> Functions { get; } = new();
        public Dictionary<string, object> Variables { get; } = new();

        /// <summary>
        /// 安全地获取变量值
        /// </summary>
        public object GetVariableValue(string varName)
        {
            if (!Variables.TryGetValue(varName, out var value))
                throw new InvalidOperationException($"Variable '{varName}' is not defined.");
    
            return value;
        }

        public object CallFunction(string fn, object[] args)
        {
            if (Functions.TryGetValue(fn, out var func))
            {
                return func(args);
            }

            var methodInfo = FindMathMethod(fn, args);
            if (methodInfo == null)
                throw new InvalidOperationException($"Function '{fn}' is not supported.");

            // 统一转换为 float
            var convertedArgs = args.Select(arg => Convert.ChangeType(arg, typeof(float))).ToArray();
    
            return methodInfo.Invoke(null, convertedArgs);
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

        // 单个参数的函数注册
        public void RegisterFunc<TR, T1>(string funcName, Func<T1, TR> func)
        {
            Functions[funcName] = args =>
            {
                if (args == null || args.Length != 1)
                    throw new InvalidOperationException($"Function '{funcName}' expects 1 argument.");

                var arg = Convertor.ConvertToTargetType<T1>(args[0]);
                return func(arg);
            };
        }

        // 两个参数的函数注册
        public void RegisterFunc<TR, T1, T2>(string funcName, Func<T1, T2, TR> func)
        {
            Functions[funcName] = args =>
            {
                if (args == null || args.Length != 2)
                    throw new InvalidOperationException($"Function '{funcName}' expects 2 arguments.");

                var (arg1, arg2) = Convertor.ConvertToTargetTypes<T1, T2>(args[0], args[1], funcName);
                return func(arg1, arg2);
            };
        }
    }
}