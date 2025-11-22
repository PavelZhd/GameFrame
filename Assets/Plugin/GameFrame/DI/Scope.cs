using System;
using System.Collections.Generic;
using GameFrame.Utils;

namespace GameFrame.DI
{
    public interface IScope: ICompositeDisposable
    {
        void RegisterInstance<T>(T instance);
        void RegisterDisposable<T>(T instance) where T: IDisposable;
        void RegisterConstructor<T>(Func<IScope, T> constructor);
        IScope CreateChild();
        T Resolve<T>();
    }
    public class Scope : IScope
    {
        private Scope parentScope;
        private Dictionary<Type, object> _instances;
        private Dictionary<Type, Func<IScope,object>> _constructors;
        private List<IDisposable> _disposables;

        public static IScope Root()
        {
            return new Scope();
        }

        private Scope()
        {
            _instances = new Dictionary<Type, object>();
            _constructors = new Dictionary<Type, Func<IScope, object>>();
            _disposables = new List<IDisposable>();
        }

        public IDisposable Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
            return disposable;
        }

        public IScope CreateChild()
        {
            var newScope = new Scope();
            newScope.parentScope = this;
            return newScope;
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }

        public void RegisterConstructor<T>(Func<IScope, T> constructor)
        {
            _constructors.Add(typeof(T), s => constructor(s));
        }

        public void RegisterInstance<T>(T instance)
        {
            _instances.Add(typeof(T), instance);
        }
        public void RegisterDisposable<T>(T instance) where T : IDisposable
        {
            _instances.Add(typeof(T), instance);
            _disposables.Add(instance);
        }
        public T Resolve<T>()
        {
            return ResplveInternal<T>(this);
        }

        private T ResplveInternal<T>( Scope initialScope)
        {
            var result = ResplveInternal(typeof(T), initialScope);
            if (result is T typed)
                return typed;
            throw new Exception($"Resolve result type mismatch! Expected: {typeof(T).Name}, received: {result.GetType().Name}");
        }
        private object ResplveInternal(Type tt, Scope initialScope)
        {
            if (_instances.TryGetValue(tt, out var instance))
                return instance;
            if (_constructors.TryGetValue(tt, out var constructor))
            {
                var constructed = constructor(initialScope);
                if (constructed == null)
                    throw new Exception($"Resolve failed! Constructor returned null. Expected type: {tt.Name}");
                initialScope._instances.Add(tt, constructed);
                if (constructed is IDisposable disposable)
                    initialScope.Add(disposable);
                return constructed;
            }
            if (parentScope != null)
                return parentScope.ResplveInternal(tt, initialScope);
            throw new Exception($"Resolve failed! Unable to locate instance or constructor of type {tt.Name}");
        }

        
    }
}