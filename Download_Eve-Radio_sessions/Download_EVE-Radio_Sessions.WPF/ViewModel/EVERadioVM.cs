using Download_EVE_Radio_Sessions.WPF.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.Remoting.Contexts;

namespace Download_EVE_Radio_Sessions.WPF.ViewModel
{
    public class EVERadioVM : MainViewModel
    {
        #region Properties
        private List<EVERadioSession> _everadiosessions;
        public List<EVERadioSession> EVERadioSessions
        {
            get { return _everadiosessions; }
            set { _everadiosessions = value; RaisePropertyChanged("EVERadioSessions"); }
        }

        public Dictionary<WebClient, int> DownloadStatusDictionary { get; set; }

        //Delegate
        public delegate void UpdateDownloadProgressFromSession(EVERadioSession everadiosession);

        #endregion

        #region Commands
        //Button "Download All" in main screen
        public ICommand DownloadAllCommand
        {
            get { return new RelayCommand(DownloadAll); }
        }
        #endregion

        #region Methods
        public EVERadioVM()
        {
            EVERadioSessions = new List<EVERadioSession>();

            //Alle sessies ophalen
            GetAllSessions();

            DownloadStatusDictionary = new Dictionary<WebClient, int>();
        }

        //Download all the sessions
        private void DownloadAll()
        {
            try
            {
                //We overlopen alle EVE Radio sessies die we willen downloaden
                foreach(EVERadioSession sessie in EVERadioSessions)
                {

                    string localpath = "C:\\Users\\Admin\\Music\\" + sessie.FileName;

                    //Kijken of dat het bestand niet al bestaat
                    //En geen mislukte download is
                    FileInfo fi = new FileInfo(localpath);
                    if(!File.Exists(localpath) || fi.Length < 100000000)// || fi.Length < 330000000)//Sommige bestanden zijn in slechtere kwaliteit opgenomen
                    {
                        sessie.Achtergrondkleur = "Orange";
                        DownloadFileAsync(sessie.FilePath, localpath);
                    }
                    else
                    {
                        sessie.Achtergrondkleur = "Green";
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Foutboodschap: " + ex.Message);
            }
        }

        //Het downloaden van een bestand
        private async Task DownloadFileAsync(string url, string naam)
        {
            try
            {
                using(WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Client_DownLoadProcessChanged;//Om tussentijdse feedback te kunnen geven.
                    wc.DownloadFileCompleted += Client_DownloadProcessComplete;//Om feedback te kunnen geven bij voltooing

                    WebProxy wp = new WebProxy("35.167.77.175", 80);
                    wc.Proxy = wp;

                    DownloadStatusDictionary.Add(wc, 0);//Toevoegen aan dictionary om progress bij te houden per onderdeel.

                    await wc.DownloadFileTaskAsync(new Uri(url), naam);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //When the download completed
        private void Client_DownloadProcessComplete(object sender, AsyncCompletedEventArgs e)
        {
            //De meegezonden sessie ophalen
            string sessiontolookfor = ((Uri)((TaskCompletionSource<object>)e.UserState).Task.AsyncState).Segments[4].ToString();

            //Opzoeken van de bestandsnaam in de sessielijst
            EVERadioSession evers = EVERadioSessions.Find(f => f.FileName == sessiontolookfor);

            //Als we iets gevonden hebben vullen we een percentage in.
            if(evers != null)
            { 
                evers.Progress = 100;
                evers.Achtergrondkleur = "Green";
            }


        }

        //When the download is progressing.
        private void Client_DownLoadProcessChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Oude versie van de lange lijn om een meegezonden sessie op te halen
            //TaskCompletionSource<object> test = (TaskCompletionSource<object>)e.UserState;
            //Uri asyncstate = (Uri)test.Task.AsyncState;
            //string sessiontolookfor = asyncstate.Segments[4].ToString();//Het 4de fragment is de naam

            //De meegezonden sessie ophalen
            string sessiontolookfor = ((Uri)((TaskCompletionSource<object>)e.UserState).Task.AsyncState).Segments[4].ToString();

            //Opzoeken van de bestandsnaam in de sessielijst
            EVERadioSession evers = EVERadioSessions.Find(f => f.FileName == sessiontolookfor);

            //Als we iets gevonden hebben vullen we een percentage in.
            if(evers != null)
                evers.Progress = e.ProgressPercentage;
        }

        //Get all the sessions and show them
        private void GetAllSessions()
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
                        //client.DownloadProgressChanged += Client_DownLoadProcessChanged;

                        //We vinden de startpositie van de tekst die we willen, de eindpositie, en halen daar de lengte uit.
                        int startPos = rewind.IndexOf("Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'") + "Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'".Length;
                        int length = rewind.IndexOf("'}); return false;\">Start") - startPos;

                        string downloadUrl = rewind.Substring(startPos, length);
                        string bestandsnaam = downloadUrl.Split('/').Last();

                        EVERadioSessions.Add(new EVERadioSession() { FilePath = downloadUrl, FileName = bestandsnaam });
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
