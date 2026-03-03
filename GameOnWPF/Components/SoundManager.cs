using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace GameOnWPF.Components
{
    internal class SoundManager
    {
        // Background player (looping)
        private readonly MediaPlayer _environmentPlayer = new MediaPlayer();

        // Footstep data
        private readonly List<Uri> _footstepList = new List<Uri>();
        private readonly Random _random = new Random();

        public SoundManager()
        {
            LoadFootSteps();
            PlayBackground();
        }

        #region Background

        private void PlayBackground()
        {
            _environmentPlayer.Open(new Uri("SFX/29811401-wondrous-waters-119518.mp3", UriKind.Relative));

            _environmentPlayer.MediaEnded += (s, e) =>
            {
                _environmentPlayer.Position = TimeSpan.Zero;
                _environmentPlayer.Play();
            };

            _environmentPlayer.Volume = 0.5;
            _environmentPlayer.Play();
        }

        #endregion

        #region Footsteps

        private void LoadFootSteps()
        {
            _footstepList.Add(new Uri("SFX/Footsteps_Walk_Grass_Mono_01.wav", UriKind.Relative));
            _footstepList.Add(new Uri("SFX/Footsteps_Walk_Grass_Mono_04.wav", UriKind.Relative));
            _footstepList.Add(new Uri("SFX/Footsteps_Walk_Grass_Mono_05.wav", UriKind.Relative));
        }

        public void PlayRandomStep()
        {
            if (_footstepList.Count < 2)
                return;

            int index = _random.Next(1, _footstepList.Count);

            Uri clip = _footstepList[index];

            PlayOneShot(clip);

            _footstepList[index] = _footstepList[0];
            _footstepList[0] = clip;
        }

        private void PlayOneShot(Uri uri, double volume = 0.6)
        {
            MediaPlayer player = new MediaPlayer();
            player.Open(uri);

            player.Volume = 0.55 + _random.NextDouble() * 0.1; // slight variation

            player.MediaEnded += (s, e) =>
            {
                player.Close();
            };

            player.Play();
        }

        #endregion
    }
}