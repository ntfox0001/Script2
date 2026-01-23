# Script2

一个轻量级的脚本解析器，使用 C# 编写，基于表达式树（Expression Tree）实现脚本解析和执行。

## 特性

- **动态执行**: 通过表达式树实现动态脚本解析和执行
- **函数支持**: 支持函数声明、调用、递归和闭包
- **变量作用域**: 支持变量作用域隔离，子环境共享函数
- **内置数学函数**: 内置 MathF 数学函数，如 Sin, Cos, Sqrt, Pow 等
- **自定义函数**: 支持注册自定义 C# 函数到脚本环境
- **流程控制**: 支持 if-else 条件判断和 while 循环
- **类型安全**: 支持运行时类型检查和类型转换

## 快速开始

```csharp
using Script2;

// 创建环境
var env = new Script2Environment();

// 执行简单表达式
var result = Script2Parser.Execute("3 + 5 * 2", env);
Console.WriteLine(result); // 输出: 13

// 定义变量
Script2Parser.Execute("var x = 10", env);

// 定义函数
Script2Parser.Execute(@"
add(a, b) {
    return a + b;
}
", env);

// 调用函数
var sum = Script2Parser.Execute("add(3, 5)", env);
Console.WriteLine(sum); // 输出: 8
```

## 语法参考

### 数据类型

Script2 支持以下基本数据类型：

| 类型 | 示例 | 说明 |
|------|------|------|
| `float` | `3.14`, `10`, `0.5` | 浮点数 |
| `string` | `"hello"`, `"world"` | 字符串（双引号包裹） |
| `bool` | `true`, `false` | 布尔值 |
| `null` | `null` | 空值 |

### 运算符

#### 算术运算符

| 运算符 | 描述 | 示例 |
|--------|------|------|
| `+` | 加法 | `a + b` |
| `-` | 减法 | `a - b` |
| `*` | 乘法 | `a * b` |
| `/` | 除法 | `a / b` |
| `%` | 取模（返回余数） | `a % b` |

#### 比较运算符

| 运算符 | 描述 | 示例 |
|--------|------|------|
| `>` | 大于 | `a > b` |
| `<` | 小于 | `a < b` |
| `>=` | 大于等于 | `a >= b` |
| `<=` | 小于等于 | `a <= b` |
| `==` | 等于（类型敏感） | `a == b` |
| `!=` | 不等于（类型敏感） | `a != b` |

#### 逻辑运算符

| 运算符 | 描述 | 示例 |
|--------|------|------|
| `and` | 逻辑与 | `true and false` |
| `or` | 逻辑或 | `true or false` |
| `not` | 逻辑非 | `not true` |

#### 运算符优先级

从高到低：
1. `()` 括号
2. `not` 逻辑非
3. `*`, `/`, `%` 乘除模
4. `+`, `-` 加减
5. `>`, `<`, `>=`, `<=` 比较
6. `==`, `!=` 相等比较
7. `and` 逻辑与
8. `or` 逻辑或

### 变量声明和赋值

```javascript
// 声明并初始化变量
var x = 10;
var name = "Alice";

// 重新赋值
x = 20;
```

### 函数定义和调用

```javascript
// 定义函数
funcName(param1, param2) {
    // 函数体
    return param1 + param2;
}

// 调用函数
result = funcName(5, 3);
```

#### 函数特性

- 支持无参数函数: `noArgs() { return 42 }`
- 支持多参数函数: `sum(a, b, c) { return a + b + c }`
- 支持 `return` 语句返回值
- 没有显式 `return` 语句的函数返回 `void`（无法赋值给变量）
- 支持递归调用
- 支持闭包（可访问外部变量）

### 条件语句

```javascript
// if 语句
if (condition) {
    // 条件为真时执行
}

// if-else 语句
if (condition) {
    // 条件为真时执行
} else {
    // 条件为假时执行
}
```

#### 嵌套条件示例

```javascript
grade(score) {
    if (score >= 90) {
        return "A";
    } else {
        if (score >= 80) {
            return "B";
        } else {
            return "C";
        }
    }
}
```

### 循环语句

```javascript
// while 循环
while (condition) {
    // 循环体
    statements;
}
```

#### 循环示例

```javascript
// 累加 1 到 n
sum(n) {
    var total = 0;
    var i = 1;
    while (i <= n) {
        total = total + i;
        i = i + 1;
    }
    return total;
}
```

### 内置常量

```javascript
PI   // 3.1415926
E    // 2.71828
```

### 内置数学函数

Script2 内置了 `MathF` 的所有静态方法，参数和返回值均为 `float` 类型：

| 函数 | 描述 | 示例 |
|------|------|------|
| `Abs(x)` | 绝对值 | `Abs(-5)` → `5` |
| `Sqrt(x)` | 平方根 | `Sqrt(16)` → `4` |
| `Pow(x, y)` | x 的 y 次方 | `Pow(2, 3)` → `8` |
| `Sin(x)` | 正弦 | `Sin(0)` → `0` |
| `Cos(x)` | 余弦 | `Cos(0)` → `1` |
| `Min(a, b)` | 最小值 | `Min(3, 5)` → `3` |
| `Max(a, b)` | 最大值 | `Max(3, 5)` → `5` |
| `Round(x)` | 四舍五入 | `Round(3.7)` → `4` |
| `Floor(x)` | 向下取整 | `Floor(3.9)` → `3` |
| `Ceiling(x)` | 向上取整 | `Ceiling(3.1)` → `4` |
| `Log(x)` | 自然对数 | `Log(2.718)` → `1` |
| `Log10(x)` | 以 10 为底的对数 | `Log10(100)` → `2` |
| `Exp(x)` | e 的 x 次方 | `Exp(1)` → `2.718` |

### 自定义函数注册

可以在 C# 代码中注册自定义函数到脚本环境：

```csharp
var env = new Script2Environment();

// 注册无参数函数
env.RegisterFunc("GetCurrentTime", () => DateTime.Now);

// 注册单参数函数
env.RegisterFunc<string, string>("Uppercase", (s) => s.ToUpper());

// 注册双参数函数
env.RegisterFunc<int, int, int>("Add", (a, b) => a + b);

// 在脚本中调用
Script2Parser.Execute("Add(10, 20)", env); // 返回 30
```

## 高级用法

### 斐波那契数列

```javascript
fib(n) {
    if (n <= 1) {
        return n;
    }
    return fib(n - 1) + fib(n - 2);
}
fib(6); // 返回 8
```

### 阶乘

```javascript
factorial(n) {
    if (n <= 1) {
        return 1;
    }
    return n * factorial(n - 1);
}
factorial(5); // 返回 120
```

### 闭包示例

```javascript
var x = 10;
addX(a) {
    return a + x;
}
addX(5); // 返回 15
```

### 类型检查说明

- `==` 和 `!=` 运算符要求两边类型相同
- 不同类型比较会抛出 `InvalidOperationException`
- 字符串连接使用 `+` 可能导致类型错误（当前版本不支持）

## 运行环境要求

- .NET 7.0 或更高版本
- 依赖库: Superpower (解析器组合子库)

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！
