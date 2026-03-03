using System;
using System.Collections.Generic;
using System.Windows.Media;
using GameOnWPF.WPFEngine;

namespace GameOnWPF.Components.Update
{
    public sealed class GameUpdateCycle
    {
        public static GameUpdateCycle Instance { get; } = new();

        private TimeSpan m_lastTime;

        private GameUpdateCycle() { }

        public event Action<double>? Updated;

        public void Start()
        {
            m_lastTime = TimeSpan.Zero;
            CompositionTarget.Rendering += OnRendering;
        }

        public void Stop()
        {
            CompositionTarget.Rendering -= OnRendering;
        }

        private void OnRendering(object? sender, EventArgs e)
        {
            var args = (RenderingEventArgs)e;

            if (m_lastTime == TimeSpan.Zero)
            {
                m_lastTime = args.RenderingTime;
                return;
            }

            double deltaTime = (args.RenderingTime - m_lastTime).TotalSeconds;

            m_lastTime = args.RenderingTime;

            Updated?.Invoke(deltaTime);
        }
    }
}
