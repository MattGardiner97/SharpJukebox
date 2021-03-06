﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for SeekBar.xaml
    /// </summary>
    public partial class SeekBar : ProgressBar
    {
        private DoubleAnimation _anim;
        private Storyboard _storyboard;

        public event Action<double> Seeked;

        public SeekBar()
        {
            InitializeComponent();
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var clickPositionX = Mouse.GetPosition((ProgressBar)sender).X;
            var gridWidth = ((ProgressBar)sender).RenderSize.Width;

            double targetFraction = clickPositionX / gridWidth;
            Seeked?.Invoke(targetFraction);
        }

        public void StartAnimation(TimeSpan TrackLength)
        {
            StartAnimation(0, 1, TrackLength);
        }

        public void StartAnimation(double Start, TimeSpan TrackLength)
        {
            //return;

            double remainingPercent = 1 - Start;
            TimeSpan remainingLength = TrackLength * remainingPercent;
            StartAnimation(Start, 1, remainingLength);
        }

        private void StartAnimation(double Start, double End, TimeSpan Length)
        {
            if (_storyboard != null)
                _storyboard.Stop();

            _storyboard = new Storyboard();

            _anim = new DoubleAnimation(Start, End, new Duration(Length));
            _anim.FillBehavior = FillBehavior.Stop;

            _storyboard.Children.Add(_anim);

            Storyboard.SetTarget(_anim, this);
            Storyboard.SetTargetProperty(_anim, new PropertyPath(ProgressBar.ValueProperty));

            _storyboard.Begin();
        }

        public void PauseAnimation()
        {
            if (_anim == null || _storyboard == null)
                return;

            _storyboard.Pause();
        }

        public void ResumeAnimation()
        {
            if (_anim == null || _storyboard == null)
                return;

            _storyboard.Resume();
        }

        public void StopAnimation()
        {
            if (_anim == null || _storyboard == null)
                return;

            _storyboard.Stop();
        }
    }
}
