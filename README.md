# BulletHellDemo

> 2D 弹幕射击游戏 Demo ｜ Unity 2022.3.62f1 ｜ C# ｜ 27 个自研脚本 / 约 1770 行

一个完整的单机弹幕射击小游戏，包含选机、商店、排行榜、战斗、结算五个环节。核心难点在**弹道数学**与**配置驱动的弹幕系统**：五种子弹运动方式、八种发射点位、单发与散弹两种开火模式全部由配置数据决定，改数值不改代码。

## 技术栈

| 分类 | 内容 |
|---|---|
| 引擎 | Unity 2022.3.62f1、NGUI（UI 层）、2D 物理 |
| 语言 | C#（泛型、接口实现、委托与 Lambda、反射、枚举、List/字典） |
| 数据 | XML 序列化（`XmlSerializer`）、自定义字典序列化 |
| 数学 | 四元数（Slerp / LookRotation / AngleAxis）、向量、屏幕与世界坐标互转 |

## 核心实现

### 1. 四元数驱动的五种弹道

子弹运动方式由 `BulletInfo.type` 字段决定，运行时 `switch` 分派，新增弹道类型只需加一个分支与一份配置：

| type | 弹道 | 实现 |
|---|---|---|
| 1 | 直线 | `Translate(Vector3.forward * speed * dt)` |
| 2 | 正弦摆动 | `Translate(Vector3.right * dt * Mathf.Sin(time * roundSpeed) * rightSpeed)` |
| 3 | 右抛物线 | `rotation *= Quaternion.AngleAxis(roundSpeed * dt, Vector3.up)` |
| 4 | 左抛物线 | 同上，角度取负 |
| 5 | 追踪弹 | `Quaternion.Slerp(rotation, Quaternion.LookRotation(玩家位置 - 自身位置), k)` |

### 2. 八方位发射点与扇面散弹

`E_Pos_Type` 枚举定义 8 个发射方位（左上/上/右上/左/右/下/左下/右下），每个方位对应一组屏幕坐标与初始方向：

- 用 `Camera.main.ScreenToWorldPoint()` 把屏幕坐标换算成世界坐标，保证发射点始终贴着屏幕边缘；
- 散弹模式下按方位动态计算扇面夹角：**角落发射点按 `90°/(n+1)` 均分，正方向发射点按 `180°/(n+1)` 均分**，再用 `Quaternion.AngleAxis` 逐颗旋转出射方向。

### 3. 飞机操作与边界处理

- `Input.GetAxisRaw` 读取输入，无输入时 `Quaternion.identity` 回正，有输入时用 `AngleAxis(±30, Vector3.forward)` 构造左右倾斜角，再由 `Quaternion.Slerp` 平滑过渡；
- 用 `Camera.main.WorldToScreenPoint()` 判断是否飞出屏幕，越界则回退到**上一帧记录的位置**，而不是简单钳制坐标。

### 4. 点击摧毁子弹

```csharp
Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo,
                1000, 1 << LayerMask.NameToLayer("Bullet"))
```
屏幕点 → 世界射线 → 按 LayerMask 只命中子弹层，命中后调用 `BulletObj.Dead()`。

### 5. 泛型面板基类

```csharp
public abstract class BasePanel<T> : MonoBehaviour where T : class
{
    private static T instance;
    public static T Instance => instance;
    protected virtual void Awake() { instance = this as T; }
    public abstract void Init();      // 子类必须实现，用于注册控件事件
}
```
泛型约束 + `as` 转型 + 抽象方法强制实现 + 静态单例入口，四件事在一个 30 行的类里完成。子面板只需 `class GamePanel : BasePanel<GamePanel>` 即可获得单例与生命周期。

### 6. 字典的 XML 序列化

`XmlSerializer` 原生不支持 `Dictionary`，这里通过**继承 + 实现接口**解决：

```csharp
public class SerializerDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public void WriteXml(XmlWriter writer) { /* 逐对序列化 key / value */ }
    public void ReadXml(XmlReader reader)  { /* 逐对反序列化并 Add */ }
}
```

### 7. 三级存档兜底

`XmlDataMar.LoadData(Type, string)` 按顺序尝试：

1. `Application.persistentDataPath`（玩家存档目录）
2. `Application.streamingAssetsPath`（随包发布的默认配置）
3. 都不存在 → `Activator.CreateInstance(type)` 反射生成默认对象

保证首次运行、配置文件缺失时都不会抛异常。

### 8. 运行时状态与配置数据隔离

发射点的 `num / cd / delay` 在运行时会被消耗，代码里先从配置对象拷贝到成员变量再修改：

```csharp
// 不能直接修改数据当中的内容，应拿变量临时存储，避免污染配置
nowNum = fireInfo.num;
nowCd  = fireInfo.cd;
```

## 游戏内容

- **四种道具**：无敌、加血、防御罩、加速，用 `storeList[i].num` 做库存，使用时扣减；
- **商店**：左右加减数量、上限 10、总价实时结算、余额校验、购买后写回 XML；
- **排行榜**：最多保留 10 条，超出自动移除最旧记录；
- **血量显示**：`List<GameObject>` 用 `SetActive(i < hp)` 控制血格数量。

## 操作说明

| 按键 | 功能 |
|---|---|
| W / A / S / D 或方向键 | 移动 |
| Shift | 加速 |
| 鼠标左键 | 点击摧毁子弹 |
| 1 / 2 / 3 / 4 | 使用无敌 / 加血 / 防御 / 加速道具 |

## 目录结构

```
Assets/Scripts/
├── Data/          配置数据与管理器（GameDataMgr、BulletData、FireData、StoreData…）
├── Object/        运行时对象（PlayerObj、BulletObj、FireObj）
├── UI/            NGUI 面板（BeginScene 选机商店排行、GameScene 战斗结算）
└── XML/           XmlDataMar、SerializerDictionary
```

## 说明

- UI 基于 **NGUI**，`Assets/NGUI` 目录为第三方插件，不计入自研代码；
- 本项目未使用对象池，子弹为 `Instantiate` + `Destroy` 直接创建销毁，弹幕密集时存在优化空间（后续可接入对象池）。
