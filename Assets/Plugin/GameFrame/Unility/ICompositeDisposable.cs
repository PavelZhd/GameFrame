using System;
using System.Collections.Generic;

namespace GameFrame.Utils
{
    public interface ICompositeDisposable: IDisposable
    {
        IDisposable Add(IDisposable disposable);
    }

    public static class CompositeDisposable { 
        public static ICompositeDisposable Create()
        {
            return new instanceClass();
        }

        private class instanceClass : ICompositeDisposable { 
            private List<IDisposable> _disposables = new List<IDisposable>();

            public IDisposable Add(IDisposable disposable)
            {
                _disposables.Add(disposable);
                return disposable;
            }

            public void Dispose()
            {
                foreach (IDisposable disposable in _disposables) {
                    disposable.Dispose();
                }
                _disposables.Clear();
            }
        }

        public static IDisposable AddTo(this IDisposable disposable, ICompositeDisposable composite)
        {
            return composite.Add(disposable);
        }
    }
}