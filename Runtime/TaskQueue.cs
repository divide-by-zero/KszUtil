using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace KszUtil
{
    public static class TaskQueue<T>
    {
        private const int dupulicateCnt = 1; //���d�� �Ӗ�������ꍇ�Ɩ����ꍇ�������
        private static readonly List<KeyValuePair<T, Func<CancellationToken, UniTask>>> _queue = new List<KeyValuePair<T, Func<CancellationToken, UniTask>>>();
        private static readonly object _queueLock = new object();

        private static CancellationTokenSource cts;
        private static bool isRunning = false;

        public static void Clear()
        {
            lock (_queueLock)
            {
                cts?.Cancel();
                _queue.Clear();
            }
        }

        public static void Enqueue(T key, Func<CancellationToken, UniTask> t)
        {
            lock (_queueLock)
            {
                _queue.Add(new KeyValuePair<T, Func<CancellationToken, UniTask>>(key, t));
                if (isRunning) return;

                // Debug.Log("�����ꂽ�L���[���܂��ɍŏ��̈������APool�����Ǔ�������I");
                isRunning = true;
                UniTask.Void(async () =>
                {
                    cts = new CancellationTokenSource();
                    var startTime = DateTime.Now;
                    while (true)
                    {
                        var list = Dequeue(dupulicateCnt);
                        if (list == null) break;
                        try
                        {
                            await UniTask.WhenAll(list.Select(func => func.Value(cts.Token)));
                            if (DateTime.Now - startTime > TimeSpan.FromSeconds(0.016f))
                            {
                                await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
                                startTime = DateTime.Now;
                            }
                        }
                        catch
                        {

                        }
                    }
                    isRunning = false;
                });
            }
        }

        public static void Dequeue(T key)
        {
            Dequeue(new[]
            {
                key
            });
        }

        public static void Dequeue(IEnumerable<T> keys)
        {
            lock (_queueLock)
            {
                foreach (var key in keys)
                {
                    var peek = _queue.FirstOrDefault(pair => pair.Key.Equals(key));
                    if (peek.Key != null)
                    {
                        // Debug.Log($"�L���[�ɂ���{peek.Key}���L���[�����菜���܂�");
                        _queue.Remove(peek);
                    }
                }
            }
        }

        private static KeyValuePair<T, Func<CancellationToken, UniTask>>[] Dequeue(int cnt = 1)
        {
            lock (_queueLock)
            {
                var list = _queue.Take(cnt).ToArray();
                if (list.Any() == false) return null;
                _queue.RemoveRange(0, list.Length);
                return list;
            }
        }

        private static KeyValuePair<T, Func<CancellationToken, UniTask>>[] Peek(int cnt = 1)
        {
            lock (_queueLock)
            {
                var list = _queue.Take(cnt);
                if (list.Any() == false) return null;
                return list.ToArray();
            }
        }
    }
}
