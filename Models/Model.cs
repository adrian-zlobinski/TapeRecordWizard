using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace TapeRecordWizard.Models
{
    public class Model : INotifyPropertyChanged
    {

        #region Private Variables

        #endregion

        #region Public properties
        public PlayList CurrentPlaylist { get; set; }
        public Dictionary<string, CassetteType> CassetteTypes { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Events
        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion

        #region Constructor and Initialization
        private static Model _model;

        public static Model ModelInstance
        {
            get
            {
                if (_model is null)
                {
                    _model = new Model();
                }
                return _model;
            }
        }
        private Model()
        {
            InitializeCassetteTypes();
        }

        private void InitializeCassetteTypes()
        {
            CassetteTypes = new Dictionary<string, CassetteType>()
            {
                {"C10", new CassetteType() {Name="C10", TotalLength=new TimeSpan(0,10,0)} },
                {"C12", new CassetteType() {Name="C12", TotalLength=new TimeSpan(0,12,0)} },
                {"C15", new CassetteType() {Name="C15", TotalLength=new TimeSpan(0,15,0)} },
                {"C30", new CassetteType() {Name="C30", TotalLength=new TimeSpan(0,30,0)} },
                {"C40", new CassetteType() {Name="C40", TotalLength=new TimeSpan(0,40,0)} },
                {"C50", new CassetteType() {Name="C50", TotalLength=new TimeSpan(0,50,0)} },
                {"C54", new CassetteType() {Name="C54", TotalLength=new TimeSpan(0,54,0)} },
                {"C60", new CassetteType() {Name="C60", TotalLength=new TimeSpan(1,0,0)} },
                {"C64", new CassetteType() {Name="C64", TotalLength=new TimeSpan(1,4,0)} },
                {"C70", new CassetteType() {Name="C70", TotalLength=new TimeSpan(1,10,0)} },
                {"C74", new CassetteType() {Name="C74", TotalLength=new TimeSpan(1,14,0)} },
                {"C80", new CassetteType() {Name="C80", TotalLength=new TimeSpan(1,20,0)} },
                {"C84", new CassetteType() {Name="C84", TotalLength=new TimeSpan(1,24,0)} },
                {"C90", new CassetteType() {Name="C90", TotalLength=new TimeSpan(1,30,0)} },
                {"C94", new CassetteType() {Name="C94", TotalLength=new TimeSpan(1,34,0)} },
                {"C100", new CassetteType() {Name="C100", TotalLength=new TimeSpan(1,40,0)} },
                {"C105", new CassetteType() {Name="C105", TotalLength=new TimeSpan(1,45,0)} },
                {"C110", new CassetteType() {Name="C110", TotalLength=new TimeSpan(1,50,0)} },
                {"C120", new CassetteType() {Name="C120", TotalLength=new TimeSpan(2,0,0)} }
            };
        }
        #endregion

        #region Functional properties
        public int DgSongsSelectedIndex { get; set; }

        public int DgSongsSelectedItemsCount { get; set; }

        public Song DgSelectedItem { get; set; }

        public bool CanMoveUp
        {
            get
            {
                if (CurrentPlaylist.Songs.Count > 0)
                {
                    if (DgSongsSelectedItemsCount == 1)
                    {
                        if (DgSongsSelectedIndex > 0)
                        {
                            if (DgSelectedItem.OrderNo > 1)
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool CanMoveDown
        {
            get
            {
                if (CurrentPlaylist.Songs.Count > 0)
                {
                    if (DgSongsSelectedItemsCount == 1)
                    {
                        if (DgSongsSelectedIndex < CurrentPlaylist.Songs.Count - 1)
                            if (DgSelectedItem.OrderNo < CurrentPlaylist.Songs.Count)
                                return true;
                    }
                }
                return false;
            }
        }

        public bool CanAddSongs
        {
            get
            {
                return CurrentPlaylist.CassetteType != null;
            }
        }

        public Visibility NoFitOnTape
        {
            get
            {
                if (CurrentPlaylist.SongsFitOnTape)
                {
                    return Visibility.Collapsed;
                }
                else
                    return Visibility.Visible;
            }
        }

        public Visibility NoFitOnSideA
        {
            get
            {
                if (CurrentPlaylist.SongsFitOnSideA)
                {
                    return Visibility.Collapsed;
                }
                else
                    return Visibility.Visible;
            }
        }

        public Visibility NoFitOnSideB
        {
            get
            {
                if (CurrentPlaylist.SongsFitOnSideB)
                {
                    return Visibility.Collapsed;
                }
                else
                    return Visibility.Visible;
            }
        }

        #endregion
        internal void UpdateDgSelection(int selectionCount)
        {
            this.DgSongsSelectedItemsCount = selectionCount;
            OnPropertyChanged(nameof(DgSelectedItem));
            OnPropertyChanged(nameof(DgSongsSelectedIndex));
            OnPropertyChanged(nameof(DgSongsSelectedItemsCount));
        }
        internal void UpdateCanMoveUpDown()
        {
            OnPropertyChanged(nameof(CanMoveUp));
            OnPropertyChanged(nameof(CanMoveDown));
        }

        internal void CassetteTypeChanged()
        {
            OnPropertyChanged(nameof(CurrentPlaylist.CassetteType));
            OnPropertyChanged(nameof(CanAddSongs));
            OnPropertyChanged(nameof(NoFitOnTape));
            OnPropertyChanged(nameof(NoFitOnSideA));
            OnPropertyChanged(nameof(NoFitOnSideB));
        }

        internal void NotFitOnTapeChanged()
        {
            OnPropertyChanged(nameof(NoFitOnTape));
            OnPropertyChanged(nameof(NoFitOnSideA));
            OnPropertyChanged(nameof(NoFitOnSideB));
        }
    }
}
