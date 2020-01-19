﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    public abstract class AudioPlayer: MonoBehaviour
    {
        public virtual AudioEventObject AudioEventObject { get; }
        public abstract void PlayAudio();
        public abstract void PauseAudio();
        public abstract void UnpauseAudio();
        public abstract void StopAudio();
        public virtual void TerminateAudio() { }
    }
}