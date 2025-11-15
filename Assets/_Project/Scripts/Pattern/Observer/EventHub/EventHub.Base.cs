using System;
using System.Collections.Generic;
using System.Threading;

namespace Virtuesky.Pattern
{
    /// <summary>
    /// Static event hub sử dụng Observer Pattern với type safety và IDisposable pattern.
    /// Hỗ trợ subscribe/publish events với hoặc không có parameter.
    /// </summary>
    public static partial class EventHub
    {
        /// <summary>
        /// Dictionary lưu trữ tất cả events với key là tên event và value là delegate.
        /// Sử dụng một dictionary duy nhất để quản lý cả Action và Action<T>.
        /// </summary>
        private static readonly Dictionary<string, Delegate> eventDictionary = new Dictionary<string, Delegate>();

        /// <summary>
        /// Clear tất cả subscriptions trong event hub.
        /// Removes tất cả events và handlers khỏi dictionary.
        /// </summary>
        /// <remarks>
        /// Sử dụng method này khi cần reset toàn bộ event system,
        /// ví dụ khi chuyển scene hoặc kết thúc game.
        /// </remarks>
        /// <example>
        /// LiteEventHub.Clear(); // Remove all events
        /// </example>
        public static void Clear()
        {
            eventDictionary.Clear();
        }

        /// <summary>
        /// Clear tất cả subscriptions của một event cụ thể.
        /// Remove event key và tất cả handlers của nó khỏi dictionary.
        /// </summary>
        /// <param name="key">Tên event cần clear</param>
        /// <remarks>
        /// Method này safe khi gọi với key không tồn tại (không throw exception).
        /// </remarks>
        /// <example>
        /// LiteEventHub.Clear("OnGameStart"); // Remove specific event
        /// </example>
        public static void Clear(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                eventDictionary.Remove(key);
            }
        }

        /// <summary>
        /// Sealed class implement IDisposable pattern cho subscription không có parameter.
        /// Đảm bảo Dispose() là idempotent (gọi nhiều lần vẫn safe).
        /// </summary>
        private sealed class Subscription : IDisposable
        {
            // Handler được lưu để có thể unsubscribe sau
            private Action handler;

            // Event key để biết unsubscribe từ event nào
            private readonly string key;

            /// <summary>
            /// Constructor lưu lại key và handler để dùng khi Dispose.
            /// </summary>
            public Subscription(string key, Action handler)
            {
                this.key = key;
                this.handler = handler;
            }

            /// <summary>
            /// Dispose unsubscribe handler khỏi event.
            /// Sử dụng Interlocked.Exchange để đảm bảo thread-safe và idempotent.
            /// </summary>
            /// <remarks>
            /// - Gọi nhiều lần Dispose() vẫn safe (idempotent)
            /// - Thread-safe với Interlocked.Exchange
            /// - Chỉ unsubscribe một lần duy nhất
            /// </remarks>
            public void Dispose()
            {
                // Interlocked.Exchange atomically:
                // 1. Lấy giá trị hiện tại của handler
                // 2. Set handler = null
                // 3. Trả về giá trị cũ
                // Thread-safe operation, đảm bảo chỉ một thread có thể lấy được handler khác null
                var handlerToRemove = Interlocked.Exchange(ref handler, null);

                // Chỉ unsubscribe nếu handler chưa null (lần gọi Dispose đầu tiên)
                if (handlerToRemove != null)
                {
                    RemoveListener(key, handlerToRemove);
                }
                // Nếu handler đã null (Dispose lần 2+), không làm gì cả
            }
        }

        /// <summary>
        /// Sealed class implement IDisposable pattern cho subscription có parameter.
        /// Đảm bảo Dispose() là idempotent (gọi nhiều lần vẫn safe).
        /// </summary>
        private sealed class Subscription<T> : IDisposable
        {
            // Handler được lưu để có thể unsubscribe sau
            private Action<T> handler;

            // Event key để biết unsubscribe từ event nào
            private readonly string key;

            /// <summary>
            /// Constructor lưu lại key và handler để dùng khi Dispose.
            /// </summary>
            public Subscription(string key, Action<T> handler)
            {
                this.key = key;
                this.handler = handler;
            }

            /// <summary>
            /// Dispose unsubscribe handler khỏi event.
            /// Sử dụng Interlocked.Exchange để đảm bảo thread-safe và idempotent.
            /// </summary>
            /// <remarks>
            /// - Gọi nhiều lần Dispose() vẫn safe (idempotent)
            /// - Thread-safe với Interlocked.Exchange
            /// - Chỉ unsubscribe một lần duy nhất
            /// </remarks>
            public void Dispose()
            {
                // Interlocked.Exchange atomically:
                // 1. Lấy giá trị hiện tại của handler
                // 2. Set handler = null
                // 3. Trả về giá trị cũ
                // Thread-safe operation, đảm bảo chỉ một thread có thể lấy được handler khác null
                var handlerToRemove = Interlocked.Exchange(ref handler, null);

                // Chỉ unsubscribe nếu handler chưa null (lần gọi Dispose đầu tiên)
                if (handlerToRemove != null)
                {
                    RemoveListener(key, handlerToRemove);
                }
                // Nếu handler đã null (Dispose lần 2+), không làm gì cả
            }
        }
    }
}