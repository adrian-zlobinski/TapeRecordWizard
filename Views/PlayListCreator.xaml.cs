﻿using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TapeRecordWizard.Models;
using NAudio.Wave;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using NAudio.Utils;

namespace TapeRecordWizard.Views
{
    /// <summary>
    /// Interaction logic for PlayListCreator.xaml
    /// </summary>
    public partial class PlayListCreator : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        public PlayListCreator()
        {
            InitializeComponent();
            UpdateOrderingButtons();
        }

        public PlayListCreator(Window owner) : this()
        {
            this.Owner = owner;
        }

        #region Events
        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "All Music Files|*.wav;*.aac;*.wma;*.mp2;*.mp3;.WAV;*.AAC;*.WMA;*.MP2;*.MP3";
            ofd.Filter = "All Music Files|*.mp3";
            ofd.Multiselect = true;
            if (ofd.ShowDialog().Value)
            {
                foreach (string filePath in ofd.FileNames)
                {
                    Model.ModelInstance.CurrentPlaylist.Songs.Add(new Models.Song() { FullFilePath = filePath, OrderNo = Model.ModelInstance.CurrentPlaylist.Songs.Count + 1 });
                }
            }
            RefreshSongsGrid();
        }

        private void btnAddVirtualSong_Click(object sender, RoutedEventArgs e)
        {
            var frm = new VirtualSong(this);
            if(frm.ShowDialog() == true)
            {
                Model.ModelInstance.CurrentPlaylist.Songs.Add(frm.Song);
            }
            RefreshSongsGrid();
        }

        private void btnAddFromCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "All Music Files|*.wav;*.aac;*.wma;*.mp2;*.mp3;.WAV;*.AAC;*.WMA;*.MP2;*.MP3";
            ofd.Filter = "CSV Files|*.csv;*.CSV";
            ofd.Multiselect = false;
            if (ofd.ShowDialog().Value)
            {
                Model.ModelInstance.AddVirtualSongsFromCSV(ofd.FileName);
            }
            RefreshSongsGrid();
        }


        private void btnRemoveSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (var row in dgSongs.SelectedItems)
            {
                Model.ModelInstance.CurrentPlaylist.Songs.Remove(row as Song);
            }
            CalculateOrderNo();
            RefreshSongsGrid();
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Model.ModelInstance.CurrentPlaylist.Songs.Clear();
            RefreshSongsGrid();
        }

        private void btnTop_Click(object sender, RoutedEventArgs e)
        {
            var songToMoveToTop = dgSongs.SelectedItem as Song;
            songToMoveToTop.OrderNo = 0;
            int counter = 2;
            foreach (var song in Model.ModelInstance.CurrentPlaylist.Songs)
            {
                if (song.OrderNo != songToMoveToTop.OrderNo)
                {
                    song.OrderNo = counter++;
                }
            }
            songToMoveToTop.OrderNo = 1;
            RefreshSongsGrid();
        }

        private void dgSongs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Model.ModelInstance.UpdateDgSelection(dgSongs.SelectedItems.Count);
            UpdateOrderingButtons();
        }

        private void btnOneUp_Click(object sender, RoutedEventArgs e)
        {
            var songToMoveToUp = dgSongs.SelectedItem as Song;
            foreach (var song in Model.ModelInstance.CurrentPlaylist.Songs)
            {
                if (song.OrderNo == songToMoveToUp.OrderNo - 1)
                {
                    song.OrderNo = song.OrderNo + 1;
                    break;
                }
            }
            songToMoveToUp.OrderNo = songToMoveToUp.OrderNo - 1;
            RefreshSongsGrid();

        }

        private void btnOneDown_Click(object sender, RoutedEventArgs e)
        {
            var songToMoveDown = dgSongs.SelectedItem as Song;
            foreach (var song in Model.ModelInstance.CurrentPlaylist.Songs)
            {
                if (song.OrderNo == songToMoveDown.OrderNo + 1)
                {
                    song.OrderNo = song.OrderNo - 1;
                    break;
                }
            }
            songToMoveDown.OrderNo = songToMoveDown.OrderNo + 1;
            RefreshSongsGrid();
        }

        private void btnBottom_Click(object sender, RoutedEventArgs e)
        {
            var songToMoveToBottom = dgSongs.SelectedItem as Song;
            songToMoveToBottom.OrderNo = int.MaxValue;
            int counter = dgSongs.Items.Count - 1;
            foreach (var song in Model.ModelInstance.CurrentPlaylist.Songs)
            {
                if (song.OrderNo != songToMoveToBottom.OrderNo)
                {
                    song.OrderNo = counter--;
                }
            }
            songToMoveToBottom.OrderNo = dgSongs.Items.Count;
            RefreshSongsGrid();
        }
        private void cmbCassetteType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Model.ModelInstance.CassetteTypeChanged();
        }
        private void btnSideA_Click(object sender, RoutedEventArgs e)
        {
            (dgSongs.SelectedItem as Song).Side = "A";
            RefreshSongsGrid();
        }

        private void btnSideb_Click(object sender, RoutedEventArgs e)
        {
            (dgSongs.SelectedItem as Song).Side = "B";
            RefreshSongsGrid();
        }
        private void btnAutoSideArrange_Click(object sender, RoutedEventArgs e)
        {
            Model.ModelInstance.CurrentPlaylist.AutoArrangeSongs();
            RefreshSongsGrid();
        }


        #endregion


        private void  UpdateOrderingButtons()
        {
            Model.ModelInstance.UpdateCanMoveUpDown();
            Model.ModelInstance.NotFitOnTapeChanged();
            Model.ModelInstance.CanAssignToSideChanged();
            Model.ModelInstance.CurrentPlaylist.SongsChanged();
            Model.ModelInstance.CanPlaySideChanged();
        }

        private void RefreshSongsGrid()
        {
            dgSongs.Items.Refresh();
            dgSongs.Items.SortDescriptions.Clear();
            dgSongs.Items.SortDescriptions.Add(new SortDescription("OrderNo", ListSortDirection.Ascending));
            UpdateOrderingButtons();
        }

        private void CalculateOrderNo()
        {
            int couter = 1;
            foreach(var song in Model.ModelInstance.CurrentPlaylist.Songs.OrderBy(x=>x.OrderNo))
            {
                song.OrderNo = couter++;
            }
        }

        private void PlaySongs(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if(btn.Tag.ToString() == "SideA")
            {
                Model.ModelInstance.Player.PlaySideA();
            }
            if (btn.Tag.ToString() == "SideB")
            {
                Model.ModelInstance.Player.PlaySideB();
            }
            Model.ModelInstance.CanPlaySideChanged();
        }

        private void slSilenceGap_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateOrderingButtons();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Model.ModelInstance.Player.Stop();
            Model.ModelInstance.OnPropertyChanged(nameof(Model.CanPlaySideA));
            Model.ModelInstance.OnPropertyChanged(nameof(Model.CanPlaySideB));
        }

    }
}