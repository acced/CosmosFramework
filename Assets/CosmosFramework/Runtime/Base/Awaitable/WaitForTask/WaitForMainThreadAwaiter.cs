﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Cosmos.Awaitable
{
    public class WaitForMainThreadAwaiter:CoroutineAwaiter<WaitForMainThreadAwaiter>
    {
        public WaitForMainThreadAwaiter()
        {
            Instruction = default(WaitForMainThreadAwaiter);
        }
        public override void OnCompleted(Action continuation)
        {
            base.OnCompleted(continuation);
            if (SynchronizationContext.Current != null)
            {
                IsCompleted = true;
            }
            else
            {
                CoroutineAwaiterMonitor.Instance.PostToMainThread(state =>
                {
                    IsCompleted = true;
                });
            }
        }
    }
}
