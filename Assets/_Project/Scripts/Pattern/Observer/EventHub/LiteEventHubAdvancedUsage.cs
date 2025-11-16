using System;
using UnityEngine;
using VirtueSky.Pattern;

public class LiteEventHubAdvancedUsage : MonoBehaviour
{
    // Use this pattern for storing subscriptions in Unity components
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();

    private void OnEnable()
    {
        // Subscribe to events and store subscriptions
        subscriptions.Add(EventHub.AddListener("UI.Show", OnUIShow));
        subscriptions.Add(EventHub.AddListener("UI.Hide", OnUIHide));
        subscriptions.Add(EventHub.AddListener<string>("UI.UpdateText", OnUIUpdateText));
        subscriptions.Add(EventHub.AddListener<PlayerData>("Player.DataChanged", OnPlayerDataChanged));
    }

    private void OnDisable()
    {
        // Dispose all subscriptions at once
        subscriptions.Dispose();
    }

    private void OnUIShow()
    {
        Debug.Log("UI Shown");
    }

    private void OnUIHide()
    {
        Debug.Log("UI Hidden");
    }

    private void OnUIUpdateText(string text)
    {
        Debug.Log($"UI Text updated: {text}");
    }

    private void OnPlayerDataChanged(PlayerData data)
    {
        Debug.Log($"Player: {data.Name}, HP: {data.Health}, Score: {data.Score}");
    }

    // Example: Publishing events from different systems
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EventHub.Raise("UI.Show");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EventHub.Raise("UI.Hide");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            EventHub.Raise("UI.UpdateText", "Hello from LiteEventHub!");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            var playerData = new PlayerData
            {
                Name = "Player 1",
                Health = 100,
                Score = 1500
            };
            EventHub.Raise("Player.DataChanged", playerData);
        }
    }
}

// Example data class
public class PlayerData
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Score { get; set; }
}

// Helper class to manage multiple disposables
public class CompositeDisposable : IDisposable
{
    private readonly System.Collections.Generic.List<IDisposable> disposables =
        new System.Collections.Generic.List<IDisposable>();

    private bool isDisposed;

    public void Add(IDisposable disposable)
    {
        if (isDisposed)
        {
            disposable?.Dispose();
            return;
        }

        if (disposable != null)
        {
            disposables.Add(disposable);
        }
    }

    public void Dispose()
    {
        if (isDisposed) return;

        isDisposed = true;
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }

        disposables.Clear();
    }
}