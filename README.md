# Script2

一个轻量级的脚本解析器，使用 C# 编写，基于表达式树（Expression Tree）实现脚本解析和执行。

## 特性

- **动态执行**: 通过表达式树实现动态脚本解析和执行
- **函数支持**: 支持函数声明、调用、递归和闭包
- **变量作用域**: 支持变量作用域隔离，子环境共享函数
- **内置数学函数**: 内置 MathF 数学函数，如 Sin, Cos, Sqrt, Pow 等
- **自定义函数**: 支持注册自定义 C# 函数到脚本环境
- **流程控制**: 支持 if-else 条件判断和 while 循环
- **数组支持**: 支持数组字面量、索引读写、for-in 遍历和丰富的数组操作函数
- **类型安全**: 支持运行时类型检查和类型转换

## 快速开始

```csharp
using Script2;

// 创建脚本对象
var script = new Script2(useInterpreterMode: false);

// 执行简单表达式
var result = script.Execute("3 + 5 * 2");
Console.WriteLine(result); // 输出: 13

// 定义变量
script.Execute("var x = 10");

// 定义函数
script.Execute(@"
add(a, b) {
    return a + b;
}
");

// 调用函数
var sum = script.Execute("add(3, 5)");
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
| `array` | `[1, 2, 3]`, `["a", "b"]` | 数组（动态列表，支持混合类型） |

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
1. `()` 括号、`[]` 索引访问
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

### 数组

Script2 支持数组类型，使用 `List<object>` 作为内部实现，支持混合类型元素。

#### 创建数组

```javascript
// 整数数组
var arr = [1, 2, 3];

// 混合类型数组
var mixed = [1, "hello", true];

// 空数组
var empty = [];

// 嵌套数组
var matrix = [[1, 2], [3, 4]];
```

#### 索引读写

```javascript
var arr = [10, 20, 30];

// 读取元素（支持负索引）
arr[0];    // 返回 10
arr[-1];   // 返回 30（最后一个元素）

// 修改元素
arr[1] = 99;
```

#### for-in 遍历

```javascript
var arr = [1, 2, 3];
var sum = 0;
for item in arr {
    sum = sum + item;
}
// sum = 6
```

#### 内置数组函数

| 函数 | 描述 | 示例 |
|------|------|------|
| `len(arr)` | 获取数组长度 | `len([1, 2, 3])` → `3` |
| `push(arr, value)` | 尾部追加元素，返回新长度 | `push(arr, 4)` → `4` |
| `pop(arr)` | 移除并返回末尾元素 | `pop([1, 2, 3])` → `3` |
| `remove(arr, index)` | 按索引移除元素，返回被移除的值 | `remove(arr, 1)` → `2` |
| `insert(arr, index, value)` | 按索引插入元素，返回新长度 | `insert(arr, 1, 99)` → `4` |
| `contains(arr, value)` | 检查数组是否包含值 | `contains(arr, 2)` → `true` |
| `indexOf(arr, value)` | 查找值的索引，未找到返回 -1 | `indexOf(arr, 2)` → `1` |
| `slice(arr, start, end)` | 返回子数组（end 可选，默认到末尾，支持负索引） | `slice(arr, 1, 3)` → `[2, 3]` |
| `join(arr, sep)` | 用分隔符拼接数组元素为字符串 | `join(arr, ", ")` → `"1, 2, 3"` |
| `sort(arr)` | 返回排序后的新数组（原数组不变） | `sort([3, 1, 2])` → `[1, 2, 3]` |

#### 数组操作示例

```javascript
// 动态构建数组
var arr = [];
var i = 0;
while (i < 5) {
    push(arr, i * 10);
    i = i + 1;
}
// arr = [0, 10, 20, 30, 40]

// 数组切片
var sub = slice(arr, 1, 3);  // [10, 20]

// 查找元素
var idx = indexOf(arr, 20);  // 2

// 拼接为字符串
join(arr, "-");  // "0-10-20-30-40"
```

### 内置常量

Script2 预定义了以下数学常量：

| 常量 | 值 | 说明 |
|------|-----|------|
| `PI` | `3.1415926` | 圆周率 |
| `E` | `2.71828` | 自然常数 |

```javascript
// 使用示例
PI * 2;      // 返回 6.2831852
E * 2;       // 返回 5.43656
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

### 内置字符串函数

| 函数 | 描述 | 示例 |
|------|------|------|
| `concat(...)` | 连接多个值（自动转字符串） | `concat("Hello", " ", "World")` → `"Hello World"` |
| `format(fmt, ...)` | 格式化字符串，使用 `{0}`, `{1}` 等占位符 | `format("Score: {0}", 95)` → `"Score: 95"` |

#### concat 函数

`concat` 函数支持任意数量的参数，会自动将参数转换为字符串后连接：

```javascript
concat("Hello", " ", "World"); // 返回 "Hello World"
concat("The value is: ", 42); // 返回 "The value is: 42"
concat("Result: ", true);     // 返回 "Result: True"
```

#### format 函数

`format` 函数使用 C# 的字符串格式化语法，支持占位符：

```javascript
// 基本使用
format("Hello {0}", "World");           // 返回 "Hello World"
format("{0} + {1} = {2}", 2, 3, 5);   // 返回 "2 + 3 = 5"

// 使用变量
var name = "Alice";
var age = 25;
format("Name: {0}, Age: {1}", name, age); // 返回 "Name: Alice, Age: 25"

// 重复使用占位符
format("{0} {1} {0}", "Hello", "World"); // 返回 "Hello World Hello"
```

### 自定义函数注册

可以在 C# 代码中注册自定义函数到脚本环境：

```csharp
var script = new Script2(useInterpreterMode: false);

// 注册无参数函数
script.RegisterFunc("GetCurrentTime", () => DateTime.Now);

// 注册单参数函数
script.RegisterFunc<string, string>("Uppercase", (s) => s.ToUpper());

// 注册双参数函数
script.RegisterFunc<int, int, int>("Add", (a, b) => a + b);

// 在脚本中调用
script.Execute("Add(10, 20)"); // 返回 30
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
- 字符串连接和格式化使用 `concat()` 和 `format()` 函数

```javascript
// 字符串连接
concat("Hello", " ", "World"); // 返回 "Hello World"

// 字符串格式化
format("Score: {0}", 95);     // 返回 "Score: 95"
format("{0} + {1} = {2}", 2, 3, 5); // 返回 "2 + 3 = 5"
```

## 运行环境要求

- .NET 7.0 或更高版本
- 依赖库: Superpower (解析器组合子库)
- **Unity IL2CPP**: 支持使用解释器模式（开发中）

## 执行模式选择

Script2 提供两种执行模式，由开发者根据运行环境选择：

### 编译模式（默认，推荐用于 .NET 环境）

使用 `Expression.Compile()` 编译表达式树，执行速度最快。

```csharp
var script = new Script2(useInterpreterMode: false);
var result = script.Execute("3 + 5 * 2");
```

**适用场景**：
- ✅ .NET / .NET Core 环境
- ✅ Unity Editor 模式
- ✅ Unity Mono 后端
- ❌ Unity IL2CPP 后端（不支持 `Expression.Compile()`）

### 解释器模式（兼容 IL2CPP，开发中）

不编译表达式树，直接解释执行，兼容 IL2CPP 但速度较慢。

**当前状态**：⚠️ 解释器模式仍在开发中，基础框架已完成，但部分功能尚需完善。

```csharp
var script = new Script2(useInterpreterMode: true);
var result = script.Execute("3 + 5 * 2");
```

**适用场景**（待完善后）：
- ✅ Unity IL2CPP 后端
- ✅ 需要跨平台的 Unity 项目
- ⚠️ 执行速度比编译模式慢约 10-100 倍（预期）

### Unity 项目推荐配置

```csharp
using UnityEngine;
using Script2;

public class Script2Setup : MonoBehaviour
{
    private Script2 _script;

    void Awake()
    {
#if UNITY_EDITOR || !ENABLE_IL2CPP
        // Editor 或 Mono 构建：使用编译模式（快速）
        _script = new Script2(useInterpreterMode: false);
#else
        // IL2CPP 构建：解释器模式仍在开发中，暂不推荐
        // _script = new Script2(useInterpreterMode: true);
        Debug.LogWarning("Script2 IL2CPP support is under development. Please use Mono backend for now.");
#endif

        Debug.Log($"Script2 initialized");
    }
}
```

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！
