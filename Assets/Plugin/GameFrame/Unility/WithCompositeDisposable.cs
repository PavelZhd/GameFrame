using System;
using GameFrame.Utils;
using UnityEngine;

namespace GameFrame.Utility
{
    public class WithCompositeDisposable : ICompositeDisposable
    {
        protected ICompositeDisposable disposable { get; private set; }
        public WithCompositeDisposable()
        {
            disposable = CompositeDisposable.Create();
        }
        public void Dispose() { 
            disposable.Dispose();
        }

        public IDisposable Add(IDisposable disposable)
        {
            return this.disposable.Add(disposable);
        }
    }
}