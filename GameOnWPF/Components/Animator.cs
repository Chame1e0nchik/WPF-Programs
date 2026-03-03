using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GameOnWPF.Components
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    internal class Animator
    {
        private Image player;

        private Dictionary<Direction, List<string>> animations = new();

        private Direction currentDirection = Direction.Down;

        private DispatcherTimer timer;

        private int frameIndex = 0;
        private bool isWalking = false;

        // Frame animation states
        private readonly int[] idleFrames = { 0, 1 }; // 1-2
        private readonly int[] walkFrames = { 2, 0, 3, 0}; // 1-3-2-4

        public Animator(Image playerImage)
        {
            player = playerImage;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += OnTick;
        }

        public void LoadAnimations()
        {
            animations[Direction.Top] = Load("top");
            animations[Direction.Down] = Load("down");
            animations[Direction.Left] = Load("left");
            animations[Direction.Right] = Load("right");
        }

        private List<string> Load(string dir)
        {
            var list = new List<string>();

            for (int i = 1; i <= 4; i++)
            {
                list.Add($"pack://application:,,,/Sprites/Character/Character_{dir}_{i}.png");
            }
            return list;
        }

        /// <summary>
        /// Called when player is moving
        /// </summary>
        public void Play(Direction direction)
        {
            if (currentDirection != direction || !isWalking)
            {
                currentDirection = direction;
                frameIndex = 0;
                isWalking = true;
            }

            if (!timer.IsEnabled)
                timer.Start();
        }

        /// <summary>
        /// Called when player is idle
        /// </summary>
        public void Stop()
        {
            isWalking = false;

            if (!timer.IsEnabled)
                timer.Start(); // keep idle animation running
        }

        private void OnTick(object? sender, EventArgs e)
        {
            var frames = isWalking ? walkFrames : idleFrames;

            int spriteIndex = frames[frameIndex % frames.Length];
            frameIndex++;

            SetFrame(spriteIndex);
        }

        private void SetFrame(int index)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(animations[currentDirection][index]);
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.EndInit();

            player.Source = bmp;
        }
    }

    public enum Direction
    {
        Top,
        Down,
        Left,
        Right
    }
}