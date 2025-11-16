using System;
using UnityEngine;
using VirtueSky.Pattern;

public class LiteEventHubExample : MonoBehaviour
{
    private IDisposable subscription1;
    private IDisposable subscription2;
    private IDisposable subscription3;

    private void Start()
    {
        Debug.Log("=== LiteEventHub Example ===\n");

        // Test 1: Subscribe without parameter
        TestNoParameter();

        // Test 2: Subscribe with parameter
        TestWithParameter();

        // Test 3: Type safety check
        TestTypeSafety();

        // Test 4: IDisposable pattern
        TestDisposablePattern();

        // Test 5: Multiple subscriptions
        TestMultipleSubscriptions();

        // Test 6: Idempotent Dispose
        TestIdempotentDispose();
    }

    #region Test Cases

    private void TestNoParameter()
    {
        Debug.Log("\n--- Test 1: No Parameter ---");

        subscription1 = EventHub.AddListener("OnGameStart", () => { Debug.Log("Game Started!"); });

        EventHub.Raise("OnGameStart");
    }

    private void TestWithParameter()
    {
        Debug.Log("\n--- Test 2: With Parameter ---");

        subscription2 =
            EventHub.AddListener<int>("OnScoreChanged", (score) => { Debug.Log($"Score changed to: {score}"); });

        EventHub.Raise("OnScoreChanged", 100);
        EventHub.Raise("OnScoreChanged", 250);
    }

    private void TestTypeSafety()
    {
        Debug.Log("\n--- Test 3: Type Safety ---");

        // Subscribe with int parameter
        EventHub.AddListener<int>("OnPlayerLevel", (level) => { Debug.Log($"Player level: {level}"); });

        EventHub.Raise("OnPlayerLevel", 5);

        // Try to subscribe with different type to same key - should throw exception
        try
        {
            EventHub.AddListener<string>("OnPlayerLevel", (name) => { Debug.Log($"This should not work: {name}"); });
        }
        catch (System.InvalidOperationException ex)
        {
            Debug.LogWarning($"Expected exception caught: {ex.Message}");
        }

        // Try to subscribe Action to key with Action<int> - should throw exception
        try
        {
            EventHub.AddListener("OnPlayerLevel", () => { Debug.Log("This should not work either"); });
        }
        catch (System.InvalidOperationException ex)
        {
            Debug.LogWarning($"Expected exception caught: {ex.Message}");
        }
    }

    private void TestDisposablePattern()
    {
        Debug.Log("\n--- Test 4: IDisposable Pattern ---");

        // Using pattern - auto dispose
        using (var sub = EventHub.AddListener("OnTempEvent", () => { Debug.Log("Temporary event handler"); }))
        {
            EventHub.Raise("OnTempEvent");
        } // Auto disposed here

        // Try to publish after dispose - should not call handler
        Debug.Log("Publishing after dispose (should be silent):");
        EventHub.Raise("OnTempEvent");
    }

    private void TestMultipleSubscriptions()
    {
        Debug.Log("\n--- Test 5: Multiple Subscriptions ---");

        var sub1 = EventHub.AddListener("OnMultiEvent", () => Debug.Log("Handler 1"));
        var sub2 = EventHub.AddListener("OnMultiEvent", () => Debug.Log("Handler 2"));
        var sub3 = EventHub.AddListener("OnMultiEvent", () => Debug.Log("Handler 3"));

        Debug.Log("Publishing with 3 handlers:");
        EventHub.Raise("OnMultiEvent");

        // Dispose middle handler
        sub2.Dispose();

        Debug.Log("\nPublishing after disposing handler 2:");
        EventHub.Raise("OnMultiEvent");

        // Dispose all
        sub1.Dispose();
        sub3.Dispose();

        Debug.Log("\nPublishing after disposing all (should be silent):");
        EventHub.Raise("OnMultiEvent");
    }

    private void TestIdempotentDispose()
    {
        Debug.Log("\n--- Test 6: Idempotent Dispose ---");

        var sub = EventHub.AddListener("OnIdempotentTest", () => { Debug.Log("Idempotent test handler"); });

        EventHub.Raise("OnIdempotentTest");

        // Dispose multiple times - should be safe
        Debug.Log("Disposing multiple times (should be safe):");
        sub.Dispose();
        sub.Dispose();
        sub.Dispose();
        Debug.Log("Successfully disposed 3 times without error");

        // Publish after dispose - should be silent
        Debug.Log("Publishing after dispose (should be silent):");
        EventHub.Raise("OnIdempotentTest");
    }

    #endregion

    #region Advanced Examples

    private void AdvancedExample1_GameFlow()
    {
        Debug.Log("\n=== Advanced Example 1: Game Flow ===");

        var startSub = EventHub.AddListener("Game.Start", () => { Debug.Log("Initializing game systems..."); });

        var pauseSub = EventHub.AddListener("Game.Pause", () => { Debug.Log("Game paused"); });

        var endSub = EventHub.AddListener<int>("Game.End",
            (finalScore) => { Debug.Log($"Game ended! Final score: {finalScore}"); });

        // Simulate game flow
        EventHub.Raise("Game.Start");
        EventHub.Raise("Game.Pause");
        EventHub.Raise("Game.End", 9999);

        // Cleanup
        startSub.Dispose();
        pauseSub.Dispose();
        endSub.Dispose();
    }

    private void AdvancedExample2_UnsubscribeMethod()
    {
        Debug.Log("\n=== Advanced Example 2: Unsubscribe Method ===");

        void ScoreHandler(int score)
        {
            Debug.Log($"Score: {score}");
        }

        // Subscribe
        EventHub.AddListener<int>("Score.Update", ScoreHandler);

        EventHub.Raise("Score.Update", 100);

        // Unsubscribe using method
        EventHub.RemoveListener<int>("Score.Update", ScoreHandler);

        Debug.Log("After unsubscribe (should be silent):");
        EventHub.Raise("Score.Update", 200);
    }

    #endregion

    private void OnDestroy()
    {
        // Manual cleanup if needed
        subscription1?.Dispose();
        subscription2?.Dispose();
        subscription3?.Dispose();

        // Clear all remaining subscriptions
        EventHub.Clear();

        Debug.Log("\n=== Cleanup Complete ===");
    }
}