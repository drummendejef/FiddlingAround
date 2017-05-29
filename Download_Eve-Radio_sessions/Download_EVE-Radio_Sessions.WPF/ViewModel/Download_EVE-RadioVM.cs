using Download_EVE_Radio_Sessions.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Download_EVE_Radio_Sessions.WPF.ViewModel
{
    public class Download_EVE_RadioVM : MainViewModel
    {
        #region Properties
        private List<EVERadioSession> _everadiosessions;
        public List<EVERadioSession> EVERadioSessions
        {
            get { return _everadiosessions; }
            set { _everadiosessions = value; RaisePropertyChanged("FoundItems"); }
        }

        public int Client_DownLoadProcessChanged { get; private set; }




        #endregion

        #region Commands

        #endregion

        #region Methods
        //Constructor
        public Download_EVE_RadioVM()
        {
            //Ophalen van HTML bestand


        }

        private GetAllSessions()
        {
            try
            {
                using(WebClient client = new WebClient())
                {
                    //We downloaden de EVE-Radio pagina
                    string htmlCode = client.DownloadString("http://eve-radio.com/radio/rewind");

                    //We splitsen de herbeluister divs op basis van hun ID
                    string[] stringSeperators = new string[] { "<div id='erRW' style='float: left;'>" };
                    string[] rewinds = htmlCode.Split(stringSeperators, StringSplitOptions.None).Skip(1).ToArray();//Het eerste dat we eruit halen is rommel.

                    //Nu gaan we voor elk van de gevonden rewinds de starturl opzoeken
                    foreach(string rewind in rewinds)
                    {
                        client.DownloadProgressChanged += Client_DownLoadProcessChanged;

                        //We vinden de startpositie van de tekst die we willen, de eindpositie, en halen daar de lengte uit.
                        int startPos = rewind.IndexOf("Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'") + "Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'".Length;
                        int length = rewind.IndexOf("'}); return false;\">Start") - startPos;

                        string downloadUrl = rewind.Substring(startPos, length);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //public void Client_DownLoadProcessChanged
        //{

        //}
        #endregion

    }
}
