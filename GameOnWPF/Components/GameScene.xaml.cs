using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GameOnWPF.Components.Update;

namespace GameOnWPF.Components
{
    public partial class GameScene : UserControl
    {
        private MainWindow mainWindow;

        private const int tileSize = 48;
        private const int mapWidth = 120;
        private const int mapHeight = 68;

        private readonly Uri colorMapUri = new("pack://application:,,,/Sprites/ColorMap.png");

        private Tilemap? tilemap;

        private TranslateTransform m_cameraOffset = null!;

        private Vector _playerPosition;
        private Vector _input;

        private const double speed = 180;

        private Animator animator = null!;
        private SoundManager soundManager;
        private Direction currentDirection = Direction.Down;

        private bool isPaused;

        public GameScene(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(this);

            tilemap = new Tilemap(tileSize, mapWidth, mapHeight);
            ColorMap.Source = new DrawingImage(tilemap.Build(colorMapUri));

            m_cameraOffset = new TranslateTransform();
            ColorMap.RenderTransform = m_cameraOffset;

            animator = new Animator(Player);
            animator.LoadAnimations();

            soundManager = new SoundManager();

            _playerPosition = new Vector(mapWidth * tileSize / 2, mapHeight * tileSize / 2);

            Player.RenderTransform = new TranslateTransform();
            UpdatePlayerScreenPosition();

            UpdateCamera();

            GameUpdateCycle.Instance.Updated += OnUpdate;
            GameUpdateCycle.Instance.Start();
        }

        /// <summary>
        /// Root update cycle
        /// </summary>
        /// <param name="deltaTime"></param>
        private void OnUpdate(double deltaTime)
        {
            // Reset input every frame
            _input = default;

            if (Keyboard.IsKeyDown(Key.D)) _input.X += 1;
            if (Keyboard.IsKeyDown(Key.A)) _input.X -= 1;
            if (Keyboard.IsKeyDown(Key.W)) _input.Y -= 1;
            if (Keyboard.IsKeyDown(Key.S)) _input.Y += 1;

            // Animation is there
            if (_input.LengthSquared > 0)
            {
                if (Math.Abs(_input.X) > Math.Abs(_input.Y))
                    currentDirection = _input.X > 0 ? Direction.Right : Direction.Left;
                else
                    currentDirection = _input.Y > 0 ? Direction.Down : Direction.Top;

                animator.Play(currentDirection);
            }
            else
            {
                animator.Stop();
            }

            PlayerMovement(deltaTime);

            UpdateCamera();
            UpdatePlayerScreenPosition();
        }

        /// <summary>
        /// Updating position of the player method (map actually)
        /// </summary>
        private void UpdatePlayerScreenPosition()
        {
            double canvasCenterX = Screen.ActualWidth / 2;
            double canvasCenterY = Screen.ActualHeight / 2;

            (Player.RenderTransform as TranslateTransform)!.X = canvasCenterX - Player.Width / 2;
            (Player.RenderTransform as TranslateTransform)!.Y = canvasCenterY - Player.Height / 2;
        }

        /// <summary>
        /// Player movement with tile-based collision
        /// </summary>
        private void PlayerMovement(double deltaTime)
        {
            if (_input.LengthSquared == 0)
                return;

            _input.Normalize();

            double moveX = _input.X * speed * deltaTime;
            double moveY = _input.Y * speed * deltaTime;

            // X axis movement
            if (PlayerCollision(_playerPosition.X + moveX, _playerPosition.Y))
                _playerPosition.X += moveX;

            // Y axis movement
            if (PlayerCollision(_playerPosition.X, _playerPosition.Y + moveY))
                _playerPosition.Y += moveY;
        }

        /// <summary>
        /// Player collision check (4 corners)
        /// </summary>
        private bool PlayerCollision(double nextX, double nextY)
        {
            if (tilemap == null)
                return false;

            double w = Player.Width;
            double h = Player.Height;

            return tilemap.IsBlocked(nextX - w / 2, nextY + h / 2) ||
                   tilemap.IsBlocked(nextX + w / 2, nextY + h / 2) ||
                   tilemap.IsBlocked(nextX - 2 / 2, nextY - h / 2) ||
                   tilemap.IsBlocked(nextX + w / 2, nextY - h / 2);
        }

        /// <summary>
        /// Updating camera position
        /// </summary>
        private void UpdateCamera()
        {
            double canvasCenterX = Screen.ActualWidth / 2;
            double canvasCenterY = Screen.ActualHeight / 2;

            m_cameraOffset.X = canvasCenterX - _playerPosition.X;
            m_cameraOffset.Y = canvasCenterY - _playerPosition.Y;
        }

        /// <summary>
        /// Unsubscribing update method and then stop updating
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            GameUpdateCycle.Instance.Updated -= OnUpdate;
            GameUpdateCycle.Instance.Stop();
        }

        private void OnPauseClick(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            PauseOverlay.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
            PauseBackdrop.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
            GameUpdateCycle.Instance.Stop();
        }

        private void OnResumeClick(object sender, RoutedEventArgs e)
        {
            isPaused = !isPaused;
            PauseOverlay.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
            PauseBackdrop.Visibility = isPaused ? Visibility.Visible : Visibility.Collapsed;
            GameUpdateCycle.Instance.Start();
        }

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
        }
    }
}
