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
    /// Interaction logic for PlaceholderTextbox.xaml
    /// </summary>
    public partial class PlaceholderTextbox : TextBox
    {
        private Style _baseStyle;
        private Style _placeholderStyle;
        private bool _textSetInternally = false;

        public string Placeholder { get; set; } 

        public PlaceholderTextbox()
        {
            InitializeComponent();

            _baseStyle = (Style)Resources["BasePlaceholderStyle"];
            _placeholderStyle = (Style)Resources["PlaceholderStyle"];
            this.Style = _placeholderStyle;

        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if(this.Text == Placeholder)
            {
                this.Style = _baseStyle;
                _textSetInternally = true;
                this.Text = "";

            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(this.Text))
            {
                this.Style = _placeholderStyle;
                _textSetInternally = true;
                this.Text = Placeholder;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textSetInternally == true)
            {
                e.Handled = true;
                _textSetInternally = false;
            }
            else
                e.Handled = false;
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.Text = Placeholder;

        }
    }
}
