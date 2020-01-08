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
using System.Windows.Shapes;

namespace SharpJukebox
{
    /// <summary>
    /// Interaction logic for TextInputDialog.xaml
    /// </summary>
    public partial class TextInputDialog : Window
    {
        public string Result { get; private set; }

        public TextInputDialog()
        {
            InitializeComponent();
        }

        public void ShowDialog(string Prompt)
        {
            lblHeader.Content = Prompt;
            this.ShowDialog();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Result = txtInput.Text;
            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtInput.Focus();
        }
    }
}
