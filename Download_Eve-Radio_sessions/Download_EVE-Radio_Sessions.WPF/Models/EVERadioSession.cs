﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Download_EVE_Radio_Sessions.WPF.Models
{
    public class EVERadioSession : ViewModelBase
    {
        public string FileName { get; set; }

        private int _progress;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; RaisePropertyChanged("Progress"); }
        }

        /// <summary>
        /// The Location of the file online
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// The backgroundcolor of the item in the list
        /// </summary>
        private string _achtergrondkleur = "white";
        public string Achtergrondkleur
        {
            get { return _achtergrondkleur; }
            set { _achtergrondkleur = value; RaisePropertyChanged("Achtergrondkleur"); }
        }

        /// <summary>
        /// Keep the stopwatch per session
        /// </summary>
        private Stopwatch _stopwatch;
        public Stopwatch StopWatch
        {
            get { return _stopwatch; }
            set { _stopwatch = value; RaisePropertyChanged("StopWatch"); }
        }
        /// <summary>
        /// Keep the downloadspeed per downloadsession
        /// </summary>
        private double _downloadspeed;
        public double DownloadSpeed
        {
            get { return _downloadspeed; }
            set { _downloadspeed = value; RaisePropertyChanged("DownloadSpeed"); }
        }

        /// <summary>
        /// Extra information for on the tooltip
        /// </summary>
        private long _filesize;
        public long FileSize 
        {
            get { return _filesize; }
            set { _filesize = value; RaisePropertyChanged("FileSize"); }
        }
        private DateTime _timestarted;//Time the download started
        public DateTime TimeStarted
        {
            get { return _timestarted; }
            set { _timestarted = value; RaisePropertyChanged("TimeStarted"); }
        }




    }
}
