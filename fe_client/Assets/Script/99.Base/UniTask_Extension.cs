using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


 public class ManualUniTaskRunner
    {
        private struct PendingDelay
        {
            public float Remaining;
            public UniTaskCompletionSource Tcs;
        }
        private readonly List<PendingDelay> m_delays = new List<PendingDelay>();

        /// <summary> 연출용 대기. Tick()이 호출될 때만 시간이 누적됨. </summary>
        private UniTask Delay(float seconds)
        {
            if (seconds <= 0f) return UniTask.CompletedTask;
            var tcs = new UniTaskCompletionSource();
            m_delays.Add(new PendingDelay { Remaining = seconds, Tcs = tcs });
            return tcs.Task;
        }

        /// <summary> 밀리초 단위. </summary>
        public UniTask DelayMS(int ms) => Delay(ms / 1000f);

        /// <summary> 외부 Update에서 호출. deltaTime만큼만 시간이 흐름. </summary>
        public void Tick(float deltaTime)
        {
            for (int i = m_delays.Count - 1; i >= 0; i--)
            {
                var d = m_delays[i];
                d.Remaining -= deltaTime;
                if (d.Remaining <= 0f)
                {
                    m_delays.RemoveAt(i);
                    d.Tcs.TrySetResult();
                }
                else
                {
                    m_delays[i] = d;
                }
            }
        }
    }