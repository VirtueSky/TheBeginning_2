using System;

namespace Virtuesky.Pattern
{
    public static partial class EventHub
    {
        /// <summary>
        /// Subscribe vào event có 1 parameter generic.
        /// </summary>
        /// <typeparam name="T">Type của parameter</typeparam>
        /// <param name="key">Tên event (không được null hoặc empty)</param>
        /// <param name="handler">Action<T> handler sẽ được gọi với parameter khi event được publish</param>
        /// <returns>IDisposable handle để unsubscribe. Gọi Dispose() để hủy subscription</returns>
        /// <exception cref="ArgumentNullException">Khi key hoặc handler là null/empty</exception>
        /// <exception cref="InvalidOperationException">Khi key đã tồn tại với type khác (ví dụ Action hoặc Action<string> khi T là int)</exception>
        /// <example>
        /// var subscription = LiteEventHub.Subscribe<int>("OnScoreChanged", (score) => Debug.Log($"Score: {score}"));
        /// LiteEventHub.Publish("OnScoreChanged", 100);
        /// subscription.Dispose(); // Unsubscribe
        /// </example>
        public static IDisposable AddListener<T>(this string key, Action<T> handler)
        {
            // Validate parameters
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            // Kiểm tra xem key đã tồn tại chưa
            if (eventDictionary.TryGetValue(key, out var existing))
            {
                // Type safety: Đảm bảo key existing phải cùng type (Action<T>)
                // Nếu key đã có Action hoặc Action<OtherType> thì throw exception
                if (existing != null && !(existing is Action<T>))
                {
                    throw new InvalidOperationException(
                        $"Type mismatch for key '{key}': Expected Action<{typeof(T).Name}>, but found {existing.GetType().Name}");
                }

                // Combine delegates: Thêm handler mới vào multicast delegate
                eventDictionary[key] = (Action<T>)existing + handler;
            }
            else
            {
                // Key chưa tồn tại, tạo mới
                eventDictionary[key] = handler;
            }

            // Trả về Subscription<T> handle để có thể Dispose sau này
            return new Subscription<T>(key, handler);
        }

        public static IDisposable AddListener<T>(this EventName eventName, Action<T> handler)
        {
            return AddListener<T>(eventName.ToString(), handler);
        }

        /// <summary>
        /// Unsubscribe khỏi event có parameter.
        /// Alternative cho Dispose() - có thể dùng method này hoặc gọi Dispose() trên IDisposable handle.
        /// </summary>
        /// <typeparam name="T">Type của parameter (phải match với type khi Subscribe)</typeparam>
        /// <param name="key">Tên event</param>
        /// <param name="handler">Handler đã subscribe trước đó</param>
        /// <remarks>
        /// Method này safe khi gọi với key/handler không tồn tại (không throw exception).
        /// Nếu sau khi remove handler mà không còn handler nào, event key sẽ bị remove khỏi dictionary.
        /// </remarks>
        /// <example>
        /// void ScoreHandler(int score) { Debug.Log($"Score: {score}"); }
        /// LiteEventHub.Subscribe<int>("OnScore", ScoreHandler);
        /// LiteEventHub.Unsubscribe<int>("OnScore", ScoreHandler); // Remove subscription
        /// </example>
        public static void RemoveListener<T>(this string key, Action<T> handler)
        {
            // Null/empty check - silent return để tránh exception
            if (string.IsNullOrEmpty(key) || handler == null)
                return;

            // Tìm và remove handler khỏi multicast delegate
            if (eventDictionary.TryGetValue(key, out var existing) && existing is Action<T> action)
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

        public static void RemoveListener<T>(this EventName eventName, Action<T> handler)
        {
            RemoveListener(eventName.ToString(), handler);
        }

        /// <summary>
        /// Publish event có parameter.
        /// Gọi tất cả handlers đã subscribe vào event này với parameter được truyền vào.
        /// </summary>
        /// <typeparam name="T">Type của parameter</typeparam>
        /// <param name="key">Tên event cần publish</param>
        /// <param name="arg">Parameter truyền cho handlers</param>
        /// <remarks>
        /// - Method này zero-allocation (không tạo garbage) khi có thể
        /// - Nếu key không tồn tại hoặc null/empty, method sẽ silent return
        /// - Tất cả handlers được gọi theo thứ tự subscribe với cùng parameter
        /// - Parameter được pass by value (struct) hoặc by reference (class)
        /// </remarks>
        /// <example>
        /// LiteEventHub.Subscribe<int>("OnScore", (score) => Debug.Log($"Score: {score}"));
        /// LiteEventHub.Publish("OnScore", 100); // Triggers all subscribed handlers với arg = 100
        /// </example>
        public static void Raise<T>(this string key, T arg)
        {
            // Null/empty check - silent return
            if (string.IsNullOrEmpty(key))
                return;

            // Tìm delegate trong dictionary và invoke với parameter
            if (eventDictionary.TryGetValue(key, out var delegateValue))
            {
                // Type check và invoke với parameter
                if (delegateValue is Action<T> action)
                {
                    action.Invoke(arg); // Zero-allocation call
                }
            }
            // Không tìm thấy key - silent return (không throw exception)
        }

        public static void Raise<T>(this EventName eventName, T arg)
        {
            Raise(eventName.ToString(), arg);
        }
    }
}