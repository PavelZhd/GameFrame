using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GameFrame.SignalBus
{
    public interface ISignalSender
    {
        void SendSignal<T>(string name, T payload);
        void SendSignal(string name);
    }
    public interface ISubscriptionProvider
    {
        IDisposable Subscribe(string name, Action callback);
        IDisposable Subscribe<T>(string name, Action<T> callback);
    }
    public class SignalBus: ISignalSender, ISubscriptionProvider, IDisposable
    {
        private static SignalBus _instance;
        public static SignalBus Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SignalBus();
                }
                return _instance;
            }
        }

        private readonly SubscriptionManager rootSubscriptionManager = new SubscriptionManager();

        public void Dispose()
        {
            rootSubscriptionManager.Dispose();
        }

        public void SendSignal<T>(string name, T payload)
        {
            rootSubscriptionManager.TriggerPath(SplitQueue(name), payload);
        }

        public void SendSignal(string name)
        {
            rootSubscriptionManager.TriggerPath(SplitQueue(name), null);
        }

        public IDisposable Subscribe(string name, Action callback)
        {
            return rootSubscriptionManager.AddListener(SplitQueue(name), _ => callback());
        }

        public IDisposable Subscribe<T>(string name, Action<T> callback)
        {
            return rootSubscriptionManager.AddListener(SplitQueue(name), (pl) => {
                if (pl is T typed)
                    callback(typed);
            });
        }

        private class SubscriptionManager: IDisposable
        {
            List<Action<object>> listeners = new List<Action<object>>();
            Dictionary<string, SubscriptionManager> branches = new Dictionary<string, SubscriptionManager>();
            public void TriggerPath(Queue<string> steps, object payload)
            {
                foreach (var callback in listeners) {
                    callback(payload);
                }
                if (steps.TryDequeue(out var step)) {
                    if (branches.TryGetValue(step, out var branch)) { 
                        branch.TriggerPath(steps, payload);
                    }
                }
            }
            public IDisposable AddListener(Queue<string> steps, Action<object> listener) {
                if (steps.TryDequeue(out var step))
                {
                    if (!branches.TryGetValue(step, out var branch))
                    {
                        branch = new SubscriptionManager();
                        branches.Add(step, branch);
                    }
                    return branch.AddListener(steps, listener);
                } else
                {
                    var thisLestener = listener;
                    listeners.Add(thisLestener);
                    return Disposable.Create(() => {
                        listeners.Remove(thisLestener);
                    });
                }
            }

            public void Dispose()
            {
                listeners.Clear();
                foreach (var branch in branches)
                {
                    branch.Value.Dispose();
                }
                branches.Clear();
            }
        }

        private static Queue<string> SplitQueue(string source, string separator = "/")
        {
            return source.Split(separator).Where(x=>!string.IsNullOrWhiteSpace(x)).ToQueue();
        }

    }

    public static class QuaueHelpers
    {
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            var result = new Queue<T>();
            foreach (var item in source) {
                result.Enqueue(item);
            }
            return result;
        }
    }
}