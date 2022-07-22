using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TapeRecordWizard.Models;

namespace TapeRecordWizard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void miNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            Models.Model.ModelInstance.CurrentPlaylist = new Models.PlayList();
            var newPlayList = new Views.PlayListCreator(this);
            newPlayList.DataContext = Models.Model.ModelInstance;
            newPlayList.ShowDialog();
        }

        private void miSavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "TRW Playlists|*.json";
            if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    IgnoreReadOnlyProperties = true
                };
                string json = System.Text.Json.JsonSerializer.Serialize<PlayList>(Model.ModelInstance.CurrentPlaylist, jsonOptions);
                var file = File.CreateText(sfd.FileName);
                file.Write(json);
                file.Close();
            }
            System.Windows.MessageBox.Show("Playlista zapisana");
        }
    }
}
