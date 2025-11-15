# LiteEventHub - Ver 2

LiteEventHub là implementation của Observer Pattern với focus vào simplicity, type safety và performance.

## Đặc điểm chính

✅ **Static class** - Không cần instance, truy cập trực tiếp  
✅ **Type Safety** - Kiểm tra type nghiêm ngặt, không cho phép trộn kiểu  
✅ **IDisposable Pattern** - Auto cleanup với using statement  
✅ **Idempotent Dispose** - An toàn khi dispose nhiều lần  
✅ **Zero Allocation** - Tối ưu performance, không alloc garbage trong Publish  
✅ **Thread-safe Dispose** - Sử dụng Interlocked.Exchange  

---

## API Reference

### Subscribe (No Parameter)

```csharp
IDisposable Subscribe(string key, Action handler)
```

Subscribe vào event không có parameter. Trả về IDisposable handle.

**Example:**
```csharp
var subscription = LiteEventHub.Subscribe("OnGameStart", () =>
{
    Debug.Log("Game started!");
});

// Cleanup
subscription.Dispose();
```

### Subscribe (With Parameter)

```csharp
IDisposable Subscribe<T>(string key, Action<T> handler)
```

Subscribe vào event có 1 parameter. Trả về IDisposable handle.

**Example:**
```csharp
var subscription = LiteEventHub.Subscribe<int>("OnScoreChanged", (score) =>
{
    Debug.Log($"New score: {score}");
});

// Cleanup
subscription.Dispose();
```

### Publish (No Parameter)

```csharp
void Publish(string key)
```

Phát event không có parameter tới tất cả subscribers.

**Example:**
```csharp
LiteEventHub.Publish("OnGameStart");
```

### Publish (With Parameter)

```csharp
void Publish<T>(string key, T arg)
```

Phát event có parameter tới tất cả subscribers.

**Example:**
```csharp
LiteEventHub.Publish("OnScoreChanged", 1000);
```

### Unsubscribe

```csharp
void Unsubscribe(string key, Action handler)
void Unsubscribe<T>(string key, Action<T> handler)
```

Unsubscribe khỏi event (alternative to Dispose).

**Example:**
```csharp
void MyHandler(int value) { }

LiteEventHub.Subscribe<int>("OnEvent", MyHandler);
LiteEventHub.Unsubscribe<int>("OnEvent", MyHandler);
```

### Clear

```csharp
void Clear()              // Clear all events
void Clear(string key)    // Clear specific event
```

Xóa subscriptions.

**Example:**
```csharp
LiteEventHub.Clear("OnGameStart");  // Clear specific
LiteEventHub.Clear();               // Clear all
```

---

## Usage Patterns

### 1. Basic Usage

```csharp
using UnityEngine;
using ObserverPattern;

public class GameManager : MonoBehaviour
{
    private IDisposable gameStartSub;
    private IDisposable scoreSub;

    private void OnEnable()
    {
        gameStartSub = LiteEventHub.Subscribe("Game.Start", OnGameStart);
        scoreSub = LiteEventHub.Subscribe<int>("Score.Changed", OnScoreChanged);
    }

    private void OnDisable()
    {
        gameStartSub?.Dispose();
        scoreSub?.Dispose();
    }

    private void OnGameStart()
    {
        Debug.Log("Game started!");
    }

    private void OnScoreChanged(int newScore)
    {
        Debug.Log($"Score: {newScore}");
    }
}
```

### 2. Using Statement (Auto Dispose)

```csharp
private void TemporarySubscription()
{
    using (var sub = LiteEventHub.Subscribe("TempEvent", () => 
    {
        Debug.Log("This will auto dispose");
    }))
    {
        LiteEventHub.Publish("TempEvent");
    } // Auto disposed here
}
```

### 3. CompositeDisposable Pattern

```csharp
public class UIController : MonoBehaviour
{
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();

    private void OnEnable()
    {
        subscriptions.Add(LiteEventHub.Subscribe("UI.Show", OnShow));
        subscriptions.Add(LiteEventHub.Subscribe("UI.Hide", OnHide));
        subscriptions.Add(LiteEventHub.Subscribe<string>("UI.UpdateText", OnUpdateText));
    }

    private void OnDisable()
    {
        subscriptions.Dispose(); // Dispose all at once
    }

    private void OnShow() { }
    private void OnHide() { }
    private void OnUpdateText(string text) { }
}
```

### 4. Event Constants (Recommended)

```csharp
public static class GameEvents
{
    public const string GAME_START = "Game.Start";
    public const string GAME_PAUSE = "Game.Pause";
    public const string GAME_END = "Game.End";
    public const string SCORE_CHANGED = "Score.Changed";
    public const string PLAYER_DIED = "Player.Died";
}

// Usage
LiteEventHub.Subscribe(GameEvents.GAME_START, OnGameStart);
LiteEventHub.Publish(GameEvents.GAME_START);
```

### 5. Complex Data with Struct/Class

```csharp
public struct PlayerData
{
    public string Name;
    public int Health;
    public int Level;
}

// Subscribe
LiteEventHub.Subscribe<PlayerData>("Player.Updated", (data) =>
{
    Debug.Log($"{data.Name} - HP: {data.Health}, Level: {data.Level}");
});

// Publish
var playerData = new PlayerData 
{ 
    Name = "Hero", 
    Health = 100, 
    Level = 5 
};
LiteEventHub.Publish("Player.Updated", playerData);
```

---

## Type Safety

LiteEventHub **không cho phép trộn kiểu** trên cùng một key:

```csharp
// ✅ OK - Subscribe Action
LiteEventHub.Subscribe("MyEvent", () => { });

// ❌ ERROR - Không thể subscribe Action<int> vào key đã có Action
LiteEventHub.Subscribe<int>("MyEvent", (x) => { }); 
// Throws: InvalidOperationException

// ✅ OK - Subscribe Action<int>
LiteEventHub.Subscribe<int>("Score", (score) => { });

// ❌ ERROR - Không thể subscribe Action<string> vào key đã có Action<int>
LiteEventHub.Subscribe<string>("Score", (name) => { });
// Throws: InvalidOperationException
```

**Lợi ích**: Phát hiện lỗi sớm, tránh runtime confusion.

---

## Performance Best Practices

### 1. Zero Allocation in Publish

```csharp
// ✅ Good - No allocation
LiteEventHub.Publish("OnEvent");
LiteEventHub.Publish("OnScore", 100);

// ❌ Avoid - Creates closure
LiteEventHub.Subscribe("OnEvent", () => 
{
    var temp = GetExpensiveData(); // Allocates
});

// ✅ Better - Use cached handler
private void CachedHandler()
{
    var temp = GetExpensiveData();
}

LiteEventHub.Subscribe("OnEvent", CachedHandler);
```

### 2. Unsubscribe in OnDisable/OnDestroy

```csharp
// ✅ Always cleanup
private void OnDestroy()
{
    subscription?.Dispose();
    // or
    LiteEventHub.Clear("MyEvent");
}
```

### 3. Use CompositeDisposable for Multiple Subscriptions

```csharp
// ✅ Efficient cleanup
private readonly CompositeDisposable subs = new CompositeDisposable();

private void OnEnable()
{
    subs.Add(LiteEventHub.Subscribe("Event1", Handler1));
    subs.Add(LiteEventHub.Subscribe("Event2", Handler2));
    subs.Add(LiteEventHub.Subscribe("Event3", Handler3));
}

private void OnDisable()
{
    subs.Dispose(); // One call disposes all
}
```

---

## Error Handling

### Type Mismatch

```csharp
try
{
    LiteEventHub.Subscribe<int>("MyKey", (x) => { });
    LiteEventHub.Subscribe<string>("MyKey", (x) => { }); // Different type
}
catch (InvalidOperationException ex)
{
    Debug.LogError($"Type mismatch: {ex.Message}");
}
```

### Null Safety

```csharp
// ❌ Throws ArgumentNullException
LiteEventHub.Subscribe(null, () => { });
LiteEventHub.Subscribe("", () => { });
LiteEventHub.Subscribe("Key", null);

// ✅ Safe - Does nothing
LiteEventHub.Unsubscribe(null, () => { });
LiteEventHub.Publish(null);
```

---

## Testing

```csharp
[Test]
public void TestEventPublish()
{
    int callCount = 0;
    
    using (LiteEventHub.Subscribe("TestEvent", () => callCount++))
    {
        LiteEventHub.Publish("TestEvent");
        Assert.AreEqual(1, callCount);
        
        LiteEventHub.Publish("TestEvent");
        Assert.AreEqual(2, callCount);
    }
    
    // After dispose
    LiteEventHub.Publish("TestEvent");
    Assert.AreEqual(2, callCount); // No change
}

[Test]
public void TestTypeSafety()
{
    LiteEventHub.Subscribe<int>("Key", (x) => { });
    
    Assert.Throws<InvalidOperationException>(() =>
    {
        LiteEventHub.Subscribe<string>("Key", (x) => { });
    });
    
    LiteEventHub.Clear();
}

[Test]
public void TestIdempotentDispose()
{
    var sub = LiteEventHub.Subscribe("Event", () => { });
    
    // Should not throw
    Assert.DoesNotThrow(() =>
    {
        sub.Dispose();
        sub.Dispose();
        sub.Dispose();
    });
}
```

---

## Migration from Ver 1

### Ver 1 (Observer)
```csharp
Observer.Instance.Subscribe("OnEvent", Handler);
Observer.Instance.Notify("OnEvent");
Observer.Instance.Unsubscribe("OnEvent", Handler);
```

### Ver 2 (LiteEventHub)
```csharp
var sub = LiteEventHub.Subscribe("OnEvent", Handler);
LiteEventHub.Publish("OnEvent");
sub.Dispose(); // or LiteEventHub.Unsubscribe("OnEvent", Handler);
```

### Key Changes
- `Instance.` → Static access
- `Notify` → `Publish`
- Returns `IDisposable` → Use Dispose() for cleanup
- Type safety enforced

---

## FAQ

**Q: Có thể subscribe nhiều handlers vào cùng key không?**  
A: Có, tất cả handlers cùng type sẽ được gọi khi Publish.

**Q: Thread-safe không?**  
A: Dispose() thread-safe với Interlocked.Exchange. Dictionary access cần lock nếu multi-thread.

**Q: Publish có allocate garbage không?**  
A: Không, Publish được optimize zero-allocation.

**Q: Làm sao để dùng nhiều hơn 1 parameter?**  
A: Dùng struct/class/tuple:
```csharp
var data = (Score: 100, Name: "Player");
LiteEventHub.Publish("Event", data);
```

**Q: Có cần Clear() trong OnDestroy không?**  
A: Nên Dispose() từng subscription. Clear() chỉ dùng khi cần clear tất cả.

---

## Files

- **LiteEventHub.cs** - Core implementation
- **LiteEventHubExample.cs** - Basic examples & tests
- **LiteEventHubAdvancedUsage.cs** - Advanced patterns (CompositeDisposable)
- **README_LiteEventHub.md** - This documentation

---

## Summary

LiteEventHub Ver 2 cung cấp:
- 🎯 Simple & Clean API
- 🛡️ Type Safety & Error Handling
- ⚡ Zero-Allocation Performance
- 🧹 IDisposable Pattern
- 📦 Production-Ready Code

**Best for**: Production code, long-term maintenance, team projects.
