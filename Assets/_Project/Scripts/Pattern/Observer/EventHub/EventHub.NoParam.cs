using System;

namespace Virtuesky.Pattern
{
    public static partial class EventHub
    {
        /// <summary>
        /// Subscribe vào event không có parameter.
        /// </summary>
        /// <param name="key">Tên event (không được null hoặc empty)</param>
        /// <param name="handler">Action handler sẽ được gọi khi event được publish</param>
        /// <returns>IDisposable handle để unsubscribe. Gọi Dispose() để hủy subscription</returns>
        /// <exception cref="ArgumentNullException">Khi key hoặc handler là null/empty</exception>
        /// <exception cref="InvalidOperationException">Khi key đã tồn tại với type khác (ví dụ Action<T>)</exception>
        /// <example>
        /// var subscription = LiteEventHub.Subscribe("OnGameStart", () => Debug.Log("Started"));
        /// subscription.Dispose(); // Unsubscribe
        /// </example>
        public static IDisposable AddListener(this string key, Action handler)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // Kiểm tra xem key đã tồn tại chưa
            if (eventDictionary.TryGetValue(key, out var existing))
            {
                // Type safety: Đảm bảo key existing phải cùng type (Action)
                // Nếu key đã có Action<T> thì throw exception
                if (existing != null && !(existing is Action))
                {
                    throw new InvalidOperationException(
                        $"Type mismatch for key '{key}': Expected Action, but found {existing.GetType().Name}");
                }

                // Combine delegates: Thêm handler mới vào multicast delegate
                eventDictionary[key] = (Action)existing + handler;
            }
            else
            {
                // Key chưa tồn tại, tạo mới
                eventDictionary[key] = handler;
            }

            // Trả về Subscription handle để có thể Dispose sau này
            return new Subscription(key, handler);
        }

        public static IDisposable AddListener(this EventName eventName, Action handler)
        {
            return AddListener(eventName.ToString(), handler);
        }

        /// <summary>
        /// Unsubscribe khỏi event không có parameter.
        /// Alternative cho Dispose() - có thể dùng method này hoặc gọi Dispose() trên IDisposable handle.
        /// </summary>
        /// <param name="key">Tên event</param>
        /// <param name="handler">Handler đã subscribe trước đó</param>
        /// <remarks>
        /// Method này safe khi gọi với key/handler không tồn tại (không throw exception).
        /// Nếu sau khi remove handler mà không còn handler nào, event key sẽ bị remove khỏi dictionary.
        /// </remarks>
        /// <example>
        /// void MyHandler() { Debug.Log("Event"); }
        /// LiteEventHub.Subscribe("OnEvent", MyHandler);
        /// LiteEventHub.Unsubscribe("OnEvent", MyHandler); // Remove subscription
        /// </example>
        public static void RemoveListener(this string key, Action handler)
        {
            // Null/empty check - silent return để tránh exception
            if (string.IsNullOrEmpty(key) || handler == null)
                return;

            // Tìm và remove handler khỏi multicast delegate
            if (eventDictionary.TryGetValue(key, out var existing) && existing is Action action)
            {
                // Remove handler khỏi delegate chain
                action -= handler;

                // Nếu không còn handler nào, remove key khỏi dictionary
                if (action == null || action.GetInvocationList().Length == 0)
                {
                    eventDictionary.Remove(key);
                }
                else
                {
                    // Còn handlers khác, update lại dictionary
                    eventDictionary[key] = action;
                }
            }
        }

        public static void RemoveListener(this EventName eventName, Action handler)
        {
            RemoveListener(eventName.ToString(), handler);
        }

        /// <summary>
        /// Publish event không có parameter.
        /// Gọi tất cả handlers đã subscribe vào event này.
        /// </summary>
        /// <param name="key">Tên event cần publish</param>
        /// <remarks>
        /// - Method này zero-allocation (không tạo garbage) khi có thể
        /// - Nếu key không tồn tại hoặc null/empty, method sẽ silent return
        /// - Tất cả handlers được gọi theo thứ tự subscribe
        /// - Nếu handler throw exception, các handlers tiếp theo vẫn được gọi
        /// </remarks>
        /// <example>
        /// LiteEventHub.Subscribe("OnGameStart", () => Debug.Log("Started"));
        /// LiteEventHub.Publish("OnGameStart"); // Triggers all subscribed handlers
        /// </example>
        public static void Raise(this string key)
        {
            // Null/empty check - silent return
            if (string.IsNullOrEmpty(key))
                return;

            // Tìm delegate trong dictionary và invoke
            if (eventDictionary.TryGetValue(key, out var delegateValue))
            {
                // Type check và invoke
                if (delegateValue is Action action)
                {
                    action.Invoke(); // Zero-allocation call
                }
            }
            // Không tìm thấy key - silent return (không throw exception)
        }

        public static void Raise(this EventName eventName)
        {
            Raise(eventName.ToString());
        }
    }
}