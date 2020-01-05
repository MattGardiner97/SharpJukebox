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
    /// Interaction logic for NextButton.xaml
    /// </summary>
    public partial class NextButton : UserControl
    {
        //Resource dictionary keys for MouseDown styles
        private const string MOUSE_DOWN_ELLIPSE_STYLE = "PlayerControl_MouseDownEllipseStyle";
        private const string MOUSE_DOWN_POLY_STYLE = "PlayerControl_MouseDownPolyStyle";
        private const string MOUSE_DOWN_LINE_STYLE = "PlayerControl_MouseDownLineStyle";


        //Stores the default control styles
        private Style _ellipsePreviousStyle;
        private Style _pathPreviousStyle;
        private Style _linePreviousStyle;

        //Events
        public event Action Pressed;

        public NextButton()
        {
            InitializeComponent();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ellipsePreviousStyle = borderEllipse.Style;
            _pathPreviousStyle = arrowPoly.Style;
            _linePreviousStyle = barLine.Style;

            var ellipseStyle = Application.Current.FindResource(MOUSE_DOWN_ELLIPSE_STYLE);
            borderEllipse.Style = (Style)ellipseStyle;

            var pathStyle = Application.Current.FindResource(MOUSE_DOWN_POLY_STYLE);
            arrowPoly.Style = (Style)pathStyle;

            var lineStyle = Application.Current.FindResource(MOUSE_DOWN_LINE_STYLE);
            barLine.Style = (Style)lineStyle;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ResetStyle();

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetStyle();

        }

        private void ResetStyle()
        {
            if (_ellipsePreviousStyle != null)
                borderEllipse.Style = _ellipsePreviousStyle;
            if (_pathPreviousStyle != null)
                arrowPoly.Style = _pathPreviousStyle;
            if (_linePreviousStyle != null)
                barLine.Style = _linePreviousStyle;

        }
    }
}
