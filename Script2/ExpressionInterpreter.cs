using System.Linq.Expressions;
using System.Reflection;

namespace Script2;

/// <summary>
/// 表达式树解释器，用于 IL2CPP 环境中执行表达式树
/// 不使用 Expression.Compile()，而是直接解释执行表达式树
/// </summary>
public class ExpressionInterpreter
{
    private readonly Script2Environment _env;
    private readonly Stack<Dictionary<ParameterExpression, object>> _variableScopes;

    private class BreakException : Exception
    {
        public object Value { get; }

        public BreakException(object value)
        {
            Value = value;
        }
    }

    public ExpressionInterpreter(Script2Environment env)
    {
        _env = env;
        _variableScopes = new Stack<Dictionary<ParameterExpression, object>>();
    }

    /// <summary>
    /// 解释执行表达式树
    /// </summary>
    public static object Interpret(Expression expr, Script2Environment env)
    {
        var interpreter = new ExpressionInterpreter(env);
        var result = interpreter.Visit(expr);
        return result;
    }

    /// <summary>
    /// 访问表达式树节点并执行
    /// </summary>
    public object Visit(Expression expr)
    {
        return expr.NodeType switch
        {
            ExpressionType.Lambda => VisitLambda((LambdaExpression)expr),
            ExpressionType.Constant => VisitConstant((ConstantExpression)expr),
            ExpressionType.Call => VisitMethodCall((MethodCallExpression)expr),
            ExpressionType.NewArrayInit => VisitNewArray((NewArrayExpression)expr),
            ExpressionType.New => VisitNew((NewExpression)expr),
            ExpressionType.Convert => VisitConvert((UnaryExpression)expr),
            ExpressionType.Add => VisitBinary((BinaryExpression)expr),
            ExpressionType.AddChecked => VisitBinary((BinaryExpression)expr),
            ExpressionType.Subtract => VisitBinary((BinaryExpression)expr),
            ExpressionType.SubtractChecked => VisitBinary((BinaryExpression)expr),
            ExpressionType.Multiply => VisitBinary((BinaryExpression)expr),
            ExpressionType.MultiplyChecked => VisitBinary((BinaryExpression)expr),
            ExpressionType.Divide => VisitBinary((BinaryExpression)expr),
            ExpressionType.Modulo => VisitBinary((BinaryExpression)expr),
            ExpressionType.GreaterThan => VisitBinary((BinaryExpression)expr),
            ExpressionType.GreaterThanOrEqual => VisitBinary((BinaryExpression)expr),
            ExpressionType.LessThan => VisitBinary((BinaryExpression)expr),
            ExpressionType.LessThanOrEqual => VisitBinary((BinaryExpression)expr),
            ExpressionType.Equal => VisitBinary((BinaryExpression)expr),
            ExpressionType.NotEqual => VisitBinary((BinaryExpression)expr),
            ExpressionType.AndAlso => VisitBinary((BinaryExpression)expr),
            ExpressionType.OrElse => VisitBinary((BinaryExpression)expr),
            ExpressionType.Block => VisitBlock((BlockExpression)expr),
            ExpressionType.Assign => VisitAssign((BinaryExpression)expr),
            ExpressionType.Try => VisitTry((TryExpression)expr),
            ExpressionType.Throw => VisitThrow((UnaryExpression)expr),
            ExpressionType.Parameter => VisitParameter((ParameterExpression)expr),
            ExpressionType.MemberAccess => VisitMemberAccess((MemberExpression)expr),
            ExpressionType.Conditional => VisitConditional((ConditionalExpression)expr),
            ExpressionType.Loop => VisitLoop((LoopExpression)expr),
            ExpressionType.Goto => VisitGoto((GotoExpression)expr),
            ExpressionType.IsFalse => VisitIsFalse((UnaryExpression)expr),
            _ => throw new NotSupportedException($"Expression type '{expr.NodeType}' is not supported by the interpreter.")
        };
    }

    private object VisitLambda(LambdaExpression lambda)
    {
        // Lambda 表达式的 body 才是真正要执行的代码
        var result = Visit(lambda.Body);
        return result;
    }

    private object VisitConstant(ConstantExpression constant)
    {
        return constant.Value;
    }

    private object VisitMethodCall(MethodCallExpression call)
    {
        var instance = call.Object != null ? Visit(call.Object) : null;
        var args = call.Arguments.Select(Visit).ToArray();

        // 处理 Script2Environment 的方法调用
        if (call.Method.DeclaringType == typeof(Script2Environment))
        {
            if (instance is not Script2Environment envInstance)
            {
                envInstance = _env;
            }

            if (call.Method.Name == nameof(Script2Environment.GetVariableValue))
            {
                var varName = args[0] as string;
                return envInstance.GetVariableValue(varName);
            }
            else if (call.Method.Name == nameof(Script2Environment.SetVariableValue))
            {
                var varName = args[0] as string;
                var value = args[1];
                envInstance.SetVariableValue(varName, value);
                return null;
            }
            else if (call.Method.Name == nameof(Script2Environment.CallFunction))
            {
                var fn = args[0] as string;
                var fnArgs = args[1] as object[];
                if (fnArgs == null)
                {
                    throw new InvalidCastException($"Expected object[] for function arguments, but got {args[1]?.GetType()}");
                }
                return envInstance.CallFunction(fn, fnArgs);
            }
            else if (call.Method.Name == nameof(Script2Environment.CreateChildEnvironment))
            {
                return envInstance.CreateChildEnvironment();
            }
            else if (call.Method.Name == nameof(Script2Environment.RegisterFunction))
            {
                var name = args[0] as string;
                var func = args[1] as Delegate;
                envInstance.RegisterFunction(name, func);
                return null;
            }
        }

        // 默认使用反射调用
        if (instance == null)
        {
            return call.Method.Invoke(null, args);
        }
        else
        {
            return call.Method.Invoke(instance, args);
        }
    }

    private object VisitNewArray(NewArrayExpression newArray)
    {
        var elements = newArray.Expressions.Select(Visit).ToArray();
        return elements;
    }

    private object VisitConvert(UnaryExpression convert)
    {
            // 处理 LambdaExpression -> Delegate 的转换
        if (convert.Type == typeof(Delegate) && convert.Operand is LambdaExpression lambdaExpr)
        {
            // 创建一个包装的委托，它会使用解释器来执行 Lambda 表达式
            // 需要捕获当前的 _env
            var capturedEnv = _env;

            // 获取 Lambda 表达式的参数数量
            var paramCount = lambdaExpr.Parameters.Count;

            // 创建一个委托，当被调用时，使用解释器来执行 Lambda 表达式
            var func = new Func<object[], object>(args =>
            {
                // 为了处理函数参数，我们需要设置参数的值
                // 但当前的实现不支持这个，我们需要添加参数传递
                // 暂时，我们只能处理无参数的 Lambda
                if (args.Length != paramCount)
                {
                    throw new ArgumentException($"Expected {paramCount} arguments, but got {args.Length}");
                }

                // 我们需要将参数传递给解释器
                // 这需要修改解释器以支持参数传递
                // 作为临时解决方案，我们可以使用 Environment 来存储参数
                var childEnv = capturedEnv.CreateChildEnvironment();
                for (int i = 0; i < args.Length; i++)
                {
                    childEnv.SetVariableValue(lambdaExpr.Parameters[i].Name ?? $"arg{i}", args[i]);
                }

                // 使用子环境来执行 Lambda 表达式
                var paramInterpreter = new ExpressionInterpreter(childEnv);
                return paramInterpreter.Visit(lambdaExpr.Body);
            });

            // 将 func 转换为正确的委托类型
            return func;
        }

        var operand = Visit(convert.Operand);

        if (operand == null)
            return null;

        if (convert.Type == typeof(float))
        {
            return Convert.ToSingle(operand);
        }
        else if (convert.Type == typeof(int))
        {
            return Convert.ToInt32(operand);
        }
        else if (convert.Type == typeof(object))
        {
            return operand;
        }
        else if (convert.Type == typeof(Delegate))
        {
            // 如果 operand 已经是 Delegate 类型，直接返回
            if (operand is Delegate del)
                return del;
            throw new InvalidCastException($"Cannot convert {operand.GetType()} to Delegate");
        }

        return Convert.ChangeType(operand, convert.Type);
    }

    private object VisitBinary(BinaryExpression binary)
    {
        var left = Visit(binary.Left);
        var right = Visit(binary.Right);

        return binary.NodeType switch
        {
            ExpressionType.Add => Add(left, right),
            ExpressionType.AddChecked => Add(left, right),
            ExpressionType.Subtract => Subtract(left, right),
            ExpressionType.SubtractChecked => Subtract(left, right),
            ExpressionType.Multiply => Multiply(left, right),
            ExpressionType.MultiplyChecked => Multiply(left, right),
            ExpressionType.Divide => Divide(left, right),
            ExpressionType.Modulo => Modulo(left, right),
            ExpressionType.GreaterThan => Compare(left, right, true),
            ExpressionType.GreaterThanOrEqual => CompareGreaterThanOrEqual(left, right),
            ExpressionType.LessThan => Compare(left, right, false),
            ExpressionType.LessThanOrEqual => CompareLessThanOrEqual(left, right),
            ExpressionType.Equal => Equal(left, right),
            ExpressionType.NotEqual => NotEqual(left, right),
            ExpressionType.AndAlso => AndAlso(left, right),
            ExpressionType.OrElse => OrElse(left, right),
            ExpressionType.Assign => left,
            _ => throw new NotSupportedException($"Binary expression type '{binary.NodeType}' is not supported.")
        };
    }

    private object VisitAssign(BinaryExpression assign)
    {
        // 右侧先计算
        var value = Visit(assign.Right);

        // 左侧是 ParameterExpression（局部变量赋值）
        if (assign.Left is ParameterExpression paramExpr)
        {
            // 从变量作用域中找到变量并赋值
            foreach (var scope in _variableScopes)
            {
                if (scope.ContainsKey(paramExpr))
                {
                    scope[paramExpr] = value;
                    return value;
                }
            }
            throw new InvalidOperationException($"Variable '{paramExpr.Name}' is not defined in current scope.");
        }

        // 左侧需要是方法调用（如 SetVariableValue）
        if (assign.Left is MethodCallExpression call)
        {
            VisitMethodCall(call);
        }

        return value;
    }

    private object VisitBlock(BlockExpression block)
    {
        // 创建新的变量作用域
        var variableValues = new Dictionary<ParameterExpression, object>();

        // 如果 block 有变量，初始化它们（如果需要）
        foreach (var var in block.Variables)
        {
            // 局部变量初始化为 null
            variableValues[var] = null;
        }

        // 将变量作用域压栈
        _variableScopes.Push(variableValues);

        object result = null;

        foreach (var expr in block.Expressions)
        {
            result = Visit(expr);
        }

        // 弹出变量作用域
        _variableScopes.Pop();

        return result;
    }

    private object VisitTry(TryExpression tryExpr)
    {
        try
        {
            var result = Visit(tryExpr.Body);
            return result;
        }
        catch (ReturnValueException ex)
        {
            if (tryExpr.Handlers.Count > 0)
            {
                var catchBlock = tryExpr.Handlers[0];
                var exceptionVar = catchBlock.Variable;

                // 直接返回 ReturnValueException 的 ReturnValue
                return ex.ReturnValue;
            }
            throw;
        }
    }

    private object VisitThrow(UnaryExpression throwExpr)
    {
        var exception = Visit(throwExpr.Operand);
        if (exception is Exception ex)
        {
            throw ex;
        }
        throw new ArgumentException("Throw expression must evaluate to an Exception.", nameof(throwExpr));
    }

    private object VisitParameter(ParameterExpression parameter)
    {
        // 处理环境参数
        if (parameter.Type == typeof(Script2Environment))
        {
            return _env;
        }

        // 从变量作用域中查找
        foreach (var scope in _variableScopes)
        {
            if (scope.TryGetValue(parameter, out var value))
            {
                return value;
            }
        }

        // 对于函数参数，尝试从当前环境中获取变量值
        if (!string.IsNullOrEmpty(parameter.Name))
        {
            try
            {
                return _env.GetVariableValue(parameter.Name);
            }
            catch (InvalidOperationException)
            {
                // 变量未定义，继续返回 null
            }
        }

        // 对于函数参数，在实际调用函数时会通过闭包处理
        // 这里暂时返回 null
        return null;
    }

    private object VisitMemberAccess(MemberExpression member)
    {
        if (member.Member is FieldInfo field)
        {
            var instance = member.Expression != null ? Visit(member.Expression) : null;
            return field.GetValue(instance);
        }
        else if (member.Member is PropertyInfo property)
        {
            var instance = member.Expression != null ? Visit(member.Expression) : null;
            return property.GetValue(instance);
        }

        throw new NotSupportedException($"Member type '{member.Member.MemberType}' is not supported.");
    }

    private object VisitNew(NewExpression newExpr)
    {
        var args = newExpr.Arguments.Select(Visit).ToArray();

        if (newExpr.Constructor != null)
        {
            return newExpr.Constructor.Invoke(args);
        }

        // 如果没有构造函数，可能是通过转换创建委托
        // 处理 LambdaExpression -> Delegate 的转换
        if (newExpr.Type == typeof(Delegate) && args.Length == 1)
        {
            // 这种情况不应该发生，因为 LambdaExpression -> Delegate 的转换应该通过 Convert 处理
            // 如果发生了，说明表达式树有问题
            throw new InvalidOperationException("Cannot create Delegate directly without constructor. Use Convert instead.");
        }

        throw new NotSupportedException($"New expression without constructor is not supported.");
    }

    private object VisitIsFalse(UnaryExpression isFalseExpr)
    {
        var value = Visit(isFalseExpr.Operand);
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return !(bool)Convert.ChangeType(value, typeof(bool));
    }

    private object VisitConditional(ConditionalExpression conditional)
    {
        var test = Visit(conditional.Test);
        if (test is bool boolTest)
        {
            return boolTest ? Visit(conditional.IfTrue) : Visit(conditional.IfFalse);
        }
        throw new InvalidOperationException("Conditional expression test must return a boolean value.");
    }

    private object VisitLoop(LoopExpression loop)
    {
        var loopEnv = _env;

        while (true)
        {
            try
            {
                var result = Visit(loop.Body);

                // 如果遇到 ReturnValueException，抛出它
                if (result is ReturnValueException ex)
                {
                    throw ex;
                }
            }
            catch (BreakException ex)
            {
                // 遇到 break 语句，退出循环并返回 break 的值
                return ex.Value;
            }
        }
    }

    private object VisitGoto(GotoExpression gotoExpr)
    {
        // 目前只支持 break 语句
        if (gotoExpr.Kind == GotoExpressionKind.Break)
        {
            object value = gotoExpr.Value != null ? Visit(gotoExpr.Value) : null;
            throw new BreakException(value);
        }
        throw new NotSupportedException($"Goto kind {gotoExpr.Kind} is not supported by the interpreter.");
    }

    #region 运算符实现

    private static float Add(object left, object right)
    {
        return Convert.ToSingle(left) + Convert.ToSingle(right);
    }

    private static float Subtract(object left, object right)
    {
        return Convert.ToSingle(left) - Convert.ToSingle(right);
    }

    private static float Multiply(object left, object right)
    {
        return Convert.ToSingle(left) * Convert.ToSingle(right);
    }

    private static float Divide(object left, object right)
    {
        return Convert.ToSingle(left) / Convert.ToSingle(right);
    }

    private static float Modulo(object left, object right)
    {
        return Convert.ToInt32(left) % Convert.ToInt32(right);
    }

    private static bool Compare(object left, object right, bool greater)
    {
        Script2Environment.CheckTypesForEquality(left, right);

        var leftFloat = Convert.ToSingle(left);
        var rightFloat = Convert.ToSingle(right);

        return greater ? leftFloat > rightFloat : leftFloat < rightFloat;
    }

    private static bool CompareGreaterThanOrEqual(object left, object right)
    {
        Script2Environment.CheckTypesForEquality(left, right);

        var leftFloat = Convert.ToSingle(left);
        var rightFloat = Convert.ToSingle(right);

        return leftFloat >= rightFloat;
    }

    private static bool CompareLessThanOrEqual(object left, object right)
    {
        Script2Environment.CheckTypesForEquality(left, right);

        var leftFloat = Convert.ToSingle(left);
        var rightFloat = Convert.ToSingle(right);

        return leftFloat <= rightFloat;
    }

    private static bool Equal(object left, object right)
    {
        Script2Environment.CheckTypesForEquality(left, right);

        if (left == null && right == null)
            return true;

        if (left == null || right == null)
            return false;

        return left.Equals(right);
    }

    private static bool NotEqual(object left, object right)
    {
        return !Equal(left, right);
    }

    private static bool AndAlso(object left, object right)
    {
        var leftBool = Convert.ToBoolean(left);
        if (!leftBool)
            return false;

        return Convert.ToBoolean(right);
    }

    private static bool OrElse(object left, object right)
    {
        var leftBool = Convert.ToBoolean(left);
        if (leftBool)
            return true;

        return Convert.ToBoolean(right);
    }

    #endregion
}
