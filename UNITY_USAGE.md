# Script2 在 Unity 中的使用指南

## 快速开始

### 1. 安装

#### 方式一：手动添加 DLL（推荐用于快速测试）

1. 从 [GitHub Releases](https://github.com/ntfox0001/Script2/releases) 下载最新版本的 NuGet 包
2. 将 `.nupkg` 文件改名为 `.zip`
3. 解压 ZIP 文件
4. 找到 `lib/netstandard2.1/Script2.dll`
5. 创建 Unity 项目文件夹：`Assets/Plugins/Script2/`
6. 将 `Script2.dll` 复制到该文件夹

#### 方式二：使用 NuGetForUnity

1. 在 Unity 中安装 NuGetForUnity 插件
2. 打开 **NuGet → Manage NuGet Packages**
3. 搜索 `Script2`
4. 点击 Install

### 2. 配置执行模式

Script2 提供两种执行模式：
- **编译模式**：执行速度快，但不支持 IL2CPP
- **解释器模式**：兼容 IL2CPP，但执行速度较慢

在 Unity 项目中，根据构建目标选择合适的执行模式：

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
        // IL2CPP 构建：使用解释器模式（兼容）
        _script = new Script2(useInterpreterMode: true);
#endif

        Debug.Log("Script2 initialized");
    }
}
```

### 3. 基本使用

```csharp
using UnityEngine;
using Script2;

public class Script2Example : MonoBehaviour
{
    private Script2 _script;

    void Start()
    {
        // 创建脚本对象
        _script = new Script2(useInterpreterMode: false);
        _script.OnPrint = Debug.Log; // 使用 Unity Debug.Log 输出

        // 执行简单表达式
        var result = _script.Execute("3 + 5 * 2");
        Debug.Log($"Result: {result}"); // 输出: Result: 13

        // 定义变量
        _script.Execute("var x = 10");
        _script.Execute("var y = 20");

        // 定义函数
        _script.Execute(@"
add(a, b) {
    return a + b;
}
");

        // 调用函数
        var sum = _script.Execute("add(3, 5)");
        Debug.Log($"Sum: {sum}"); // 输出: Sum: 8

        // 使用 print 函数
        _script.Execute("print(\"Hello, Unity!\")");
    }
}
```

## Unity 特定配置

### 输出重定向

```csharp
var script = new Script2(useInterpreterMode: false);

// 将 print 函数的输出重定向到 Debug.Log
script.OnPrint = (message) => Debug.Log($"[Script2] {message}");

// 或者使用 Unity Console
script.OnPrint = Debug.Log;
```

### 注册自定义函数

```csharp
// 注册 Unity Vector3 函数
script.RegisterFunc<float, float, float, float>("CreateVector", (x, y, z) => {
    // 返回 Vector3 的长度作为示例
    return new Vector3(x, y, z).magnitude;
});

// 注册获取当前时间的函数
script.RegisterFunc("GetTime", () => Time.time);

// 在脚本中使用
script.Execute("var length = CreateVector(1, 2, 3)");
script.Execute("print(GetTime())");
```

## 性能优化建议

### 1. 预定义脚本

如果脚本是固定的，建议在 `Start()` 中预定义：

```csharp
private Script2 _script;

void Start()
{
    _script = new Script2(useInterpreterMode: false);
    _script.OnPrint = Debug.Log;

    // 预定义所有函数
    _script.Execute(@"
CalculateDamage(attack, defense, level) {
    var baseDamage = attack - defense;
    var levelBonus = baseDamage * level * 0.1;
    return baseDamage + levelBonus;
}

GenerateEnemyWave(level) {
    var count = level * 2 + 1;
    return count;
}
");
}

void Update()
{
    // 只执行需要动态计算的部分
    var damage = _script.Execute("CalculateDamage(100, 50, 5)");
}
```

### 2. 重用环境对象

```csharp
// ❌ 不推荐：每次执行都创建新对象
void Update()
{
    var script = new Script2(useInterpreterMode: false);
    script.Execute("x + 5");
}

// ✅ 推荐：重用脚本对象
private Script2 _script;

void Start()
{
    _script = new Script2(useInterpreterMode: false);
}

void Update()
{
    _script.Execute("x + 5");
}
```

## 实战示例

### 示例 1：游戏规则配置系统

```csharp
public class GameRules : MonoBehaviour
{
    private Script2 _script;

    void Start()
    {
        _script = new Script2(useInterpreterMode: false);
        _script.OnPrint = Debug.Log;

        // 加载游戏规则
        var rules = @"
var baseHealth = 100;
var healthPerLevel = 10;
var baseDamage = 20;
var damagePerLevel = 5;

CalculateMaxHealth(level) {
    return baseHealth + level * healthPerLevel;
}

CalculateDamage(level) {
    return baseDamage + level * damagePerLevel;
}

CalculateReward(level) {
    var health = CalculateMaxHealth(level);
    var damage = CalculateDamage(level);
    return (health + damage) * 0.5;
}
";

        _script.Execute(rules);

        // 使用规则
        var playerLevel = 5;
        var maxHealth = _script.Execute($"CalculateMaxHealth({playerLevel})");
        var damage = _script.Execute($"CalculateDamage({playerLevel})");
        var reward = _script.Execute($"CalculateReward({playerLevel})");

        Debug.Log($"Level {playerLevel}: HP={maxHealth}, DMG={damage}, Reward={reward}");
    }
}
```

### 示例 2：动态事件系统

```csharp
public class EventSystem : MonoBehaviour
{
    private Script2 _script;

    void Start()
    {
        _script = new Script2(useInterpreterMode: false);
        _script.OnPrint = Debug.Log;

        // 注册 Unity 相关函数
        _script.RegisterFunc<float>("DestroyObject", (delay) => {
            Destroy(gameObject, delay);
            return 0;
        });

        _script.RegisterFunc("PlaySound", () => {
            AudioSource.PlayClipAtPoint(null, transform.position);
            return 0;
        });

        // 定义事件脚本
        _script.Execute(@"
OnPlayerDeath(level) {
    print(\"Player died at level: \" + level);
    PlaySound();
    DestroyObject(2.0);
}
");
    }

    public void OnPlayerKilled(int level)
    {
        // 触发事件
        _script.Execute($"OnPlayerDeath({level})");
    }
}
```

### 示例 3：技能系统

```csharp
public class SkillSystem : MonoBehaviour
{
    private Script2 _script;

    void Start()
    {
        _script = new Script2(useInterpreterMode: false);
        _script.OnPrint = Debug.Log;

        // 定义技能计算逻辑
        _script.Execute(@"
// 火球术技能
Fireball(level, baseDamage) {
    var levelMultiplier = 1.0 + level * 0.2;
    var finalDamage = baseDamage * levelMultiplier;
    print(\"Fireball damage: \" + finalDamage);
    return finalDamage;
}

// 治疗术技能
Heal(level, baseHeal) {
    var levelMultiplier = 1.0 + level * 0.15;
    var finalHeal = baseHeal * levelMultiplier;
    print(\"Heal amount: \" + finalHeal);
    return finalHeal;
}

// 护盾技能计算
Shield(level, baseShield) {
    var duration = 5.0 + level * 0.5;
    var shieldAmount = baseShield * (1.0 + level * 0.25);
    print(\"Shield: \" + shieldAmount + \" for \" + duration + \" seconds\");
    return shieldAmount;
}
");
    }

    public void CastFireball(int level)
    {
        var damage = _script.Execute($"Fireball({level}, 50)");
        // 应用伤害逻辑...
    }

    public void CastHeal(int level)
    {
        var healAmount = _script.Execute($"Heal({level}, 30)");
        // 应用治疗逻辑...
    }
}
```

## IL2CPP 构建注意事项

### 执行模式配置

Script2 不会自动检测运行环境，需要你根据构建目标手动配置：

```csharp
private Script2 _script;

void Awake()
{
#if UNITY_EDITOR || !ENABLE_IL2CPP
    // Editor 或 Mono 构建：使用编译模式（快速）
    _script = new Script2(useInterpreterMode: false);
#else
    // IL2CPP 构建：使用解释器模式（兼容）
    _script = new Script2(useInterpreterMode: true);
#endif
}
```

### 测试 IL2CPP 构建建议

1. **在 Editor 中测试解释器模式**：
```csharp
// 临时创建解释器模式的脚本对象，模拟 IL2CPP 环境
var testScript = new Script2(useInterpreterMode: true);
var result = testScript.Execute("3 + 5 * 2");
```

2. **在真机上测试**：
   - 使用 IL2CPP 后端构建项目
   - 部署到设备
   - 验证脚本执行功能正常

### 性能考虑

- **编译模式**：执行速度最快，推荐用于 .NET/Mono 环境
- **解释器模式**：执行速度较慢（约慢 10-100 倍），但完全兼容 IL2CPP

如果性能是关键因素，建议：
1. 在 Editor 中预编译脚本
2. 缓存执行结果
3. 简化脚本逻辑

## 故障排除

### 问题：IL2CPP 构建后脚本无法执行

**解决方案**：
- 确保 `Script2.dll` 已正确放置在 `Assets/Plugins/Script2/`
- 确保 IL2CPP 构建时使用解释器模式：`new Script2(useInterpreterMode: true)`
- 检查是否正确引用了 `System.Linq.Expressions`（IL2CPP 支持表达式树创建，但不支持编译）

### 问题：性能问题

**解决方案**：
- 减少脚本执行频率（缓存结果）
- 简化脚本逻辑
- 预定义复杂函数，避免重复解析

### 问题：无法访问 Unity API

**解决方案**：
```csharp
// 注册 Unity API 包装函数
script.RegisterFunc("Log", (message) => {
    Debug.Log(message);
    return 0;
});

// 在脚本中使用
script.Execute("Log(\"Hello from Script2!\")");
```

## 参考资源

- [Script2 GitHub 仓库](https://github.com/ntfox0001/Script2)
- [Script2 README](README.md)
- [NuGet 包](https://www.nuget.org/packages/Script2)
