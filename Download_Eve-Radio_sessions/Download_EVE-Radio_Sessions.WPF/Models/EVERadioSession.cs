using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Download_EVE_Radio_Sessions.WPF.Models
{
    public class EVERadioSession : ViewModelBase
    {
        public string FileName { get; set; }
        //public int Progress { get; set; } = 0;

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

        //public string Color { get; set; } = "Orange";
    }
}
