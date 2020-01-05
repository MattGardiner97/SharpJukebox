using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for PlayPauseButton.xaml
    /// </summary>
    public partial class PlayPauseButton : UserControl
    {
        //Resource dictionary keys for MouseDown styles
        private const string MOUSE_DOWN_ELLIPSE_STYLE = "PlayerControl_MouseDownEllipseStyle";
        private const string MOUSE_DOWN_POLY_STYLE = "PlayerControl_MouseDownPolyStyle";
        private const string MOUSE_DOWN_LINE_STYLE = "PlayerControl_MouseDownLineStyle";


        //Stores the default control styles
        private Style _ellipsePreviousStyle;
        private Style _pathPreviousStyle;
        private Style _linePreviousStyle;

        //Play button showing = true, Pause button showing = false
        private PlayButtonDisplayState _displayState = PlayButtonDisplayState.Play;
        public PlayButtonDisplayState DisplayState
        {
            get { return _displayState; }
            set { _displayState = value; UpdateControl(); }
        }

        //Events
        public event Action Pressed;

        public PlayPauseButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ellipsePreviousStyle = borderEllipse.Style;
            _pathPreviousStyle = arrowPoly.Style;
            _linePreviousStyle = bar1.Style;

            var ellipseStyle = Application.Current.FindResource(MOUSE_DOWN_ELLIPSE_STYLE);
            borderEllipse.Style = (Style)ellipseStyle;

            var pathStyle = Application.Current.FindResource(MOUSE_DOWN_POLY_STYLE);
            arrowPoly.Style = (Style)pathStyle;

            var lineStyle = Application.Current.FindResource(MOUSE_DOWN_LINE_STYLE);
            bar1.Style = (Style)lineStyle;
            bar2.Style = (Style)lineStyle;

        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetStyle();
            UpdateControl();
            if (Pressed != null)
                Pressed();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetStyle();
        }

        private void UpdateControl()
        {
            if (_displayState == PlayButtonDisplayState.Pause)
            {
                bar1.Visibility = Visibility.Visible;
                bar2.Visibility = Visibility.Visible;
                arrowPoly.Visibility = Visibility.Collapsed;
            }
            else
            {
                bar1.Visibility = Visibility.Collapsed;
                bar2.Visibility = Visibility.Collapsed;
                arrowPoly.Visibility = Visibility.Visible;
            }
        }

        private void ResetStyle()
        {
            if (_ellipsePreviousStyle != null)
                borderEllipse.Style = _ellipsePreviousStyle;
            if (_pathPreviousStyle != null)
                arrowPoly.Style = _pathPreviousStyle;
            if (_linePreviousStyle != null)
            {
                bar1.Style = _linePreviousStyle;
                bar2.Style = _linePreviousStyle;
            }

        }
    }

    public enum PlayButtonDisplayState { Play, Pause }
}
