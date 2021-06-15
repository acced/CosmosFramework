﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Cosmos.Download
{
    public class WaitForDownload<T> : CustomYieldInstruction
        where T :class
    {
        /// <summary>
        /// T类型为Task、Coroutine;
        /// </summary>
        public T DownloadTask { get;}
        public override bool keepWaiting { get; }
    }
}
