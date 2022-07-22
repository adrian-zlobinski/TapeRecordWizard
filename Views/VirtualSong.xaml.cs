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
using TapeRecordWizard.Models;

namespace TapeRecordWizard.Views
{
    /// <summary>
    /// Interaction logic for VirtualSong.xaml
    /// </summary>
    public partial class VirtualSong : Window
    {
        private Song _virtualSong;
        public VirtualSong()
        {
            InitializeComponent();
        }

        public VirtualSong(Window owner) : this()
        {
            this.Owner = owner;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this._virtualSong = new Song(tbTitle.Text, TimeSpan.Parse(tbDuration.Text));
            this.DialogResult = true;
            this.Close();
        }

        public Song Song
        {
            get
            {
                return this._virtualSong;
            }
        }
    }
}
