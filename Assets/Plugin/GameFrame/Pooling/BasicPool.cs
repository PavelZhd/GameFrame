using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrame.Pool
{
    public class BasicPool<T> : IPool<T>
    {
        protected readonly Stack<T> _stashedItems;
        private readonly Func<T> _itemConstructor;

        public BasicPool(Func<T> itemConstructor)
        {
            _stashedItems = new Stack<T>();
            this._itemConstructor = itemConstructor;
        }

        public virtual T Fetch()
        {
            if (!_stashedItems.TryPop(out var item))
            {
                return _itemConstructor();
            }
            return item;
        }

        public virtual void Release(T item)
        {
            _stashedItems.Push(item);
        }
    }

    public class DisposablePool<T> : BasicPool<T>, IDisposable
        where T : IDisposable
    {
        public bool isDisposed { get; private set;}
        public DisposablePool(Func<T> itemConstructor) : base(itemConstructor)
        {
            isDisposed = false;
        }

        public override T Fetch()
        {
            if (!isDisposed)
                return base.Fetch();
            else
                return default(T);
        }

        public override void Release(T item)
        {
            if (!isDisposed)
                base.Release(item);
            else
                item.Dispose();
        }

        public void Dispose()
        {
            isDisposed = true;
            while (_stashedItems.TryPop(out var item))
            {
                item.Dispose();
            }
        }
    }
}