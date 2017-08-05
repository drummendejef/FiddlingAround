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
using Download_EVE_Radio_Sessions.ClassLibrary;
using Download_EVE_Radio_Sessions.ClassLibrary.Models;
using System.Windows.Forms;

namespace Download_EVE_Radio_Sessions.WPF.ViewModel
{
    public class EVERadioVM : MainViewModel
    {
        #region Properties
        //List of all the found EVE Radio sessions
        private List<EVERadioSession> _everadiosessions;
        public List<EVERadioSession> EVERadioSessions
        {
            get { return _everadiosessions; }
            set { _everadiosessions = value; RaisePropertyChanged("EVERadioSessions"); }
        }

        //Do we want to use the proxy in case of a firewall somewhere?
        private bool _useproxy;
        public bool UseProxy
        {
            get { return _useproxy; }
            set { _useproxy = value; RaisePropertyChanged("UseProxy"); }
        }

        //Feedback and color
        private string _feedback;
        public string Feedback
        {
            get { return _feedback; }
            set { _feedback = value; RaisePropertyChanged("Feedback"); }
        }
        private string _feedbackcolor;
        public string FeedbackColor
        {
            get { return _feedbackcolor; }
            set { _feedbackcolor = value; RaisePropertyChanged("FeedbackColor"); }
        }

        //Location of where the downloads should go to
        private static string _DOWNLOADFOLDER = "C:\\Users\\Admin\\Music\\";

        #endregion

        #region Commands
        //Button "Download All" in main screen
        public ICommand DownloadAllCommand
        {
            get { return new RelayCommand(DownloadAll); }
        }

        //Button "Open Download Folder" in main screen
        public ICommand ChooseDownloadFolderCommand
        {
            get { return new RelayCommand(OpenDownloadFolder); }
        }




        #endregion

        #region Methods
        public EVERadioVM()
        {
            EVERadioSessions = new List<EVERadioSession>();

            //Alle sessies ophalen
            GetAllSessions();

            //Feedback invullen
            Feedback = "Hello, I'm feedback. Please press 'Download All' to start";
            FeedbackColor = "Black";
        }

        //Download all the sessions
        private void DownloadAll()
        {
            try
            {
                //Give feedback to user
                FeedbackColor = "Green";
                Feedback = "We started downloading ALL THE SONGS";

                //We overlopen alle EVE Radio sessies die we willen downloaden
                foreach(EVERadioSession sessie in EVERadioSessions)
                {
                    string localpath = _DOWNLOADFOLDER + sessie.FileName;

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
                        sessie.Achtergrondkleur = "GreenYellow";
                        sessie.Progress = 100;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Foutboodschap: " + ex.Message);
                FeedbackColor = "Red";
                Feedback = "Something went wrong, maybe this helps: " + ex.Message;
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

                    //Make instance off class library
                    DownloadEVERadioSessions downloadEVERadioSessionsClassLib = new DownloadEVERadioSessions();

                    if(UseProxy)//Use a proxy if the user wants to
                    {
                        GetProxyModel proxyinfo = downloadEVERadioSessionsClassLib.GetRandomProxy();//Get a random proxy
                        
                        WebProxy wp = new WebProxy(proxyinfo.Ip, proxyinfo.Port);//Proxy instellen
                        wc.Proxy = wp;
                    }

                    await wc.DownloadFileTaskAsync(new Uri(url), naam);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                if(ex.Message == "Unable to connect to the remote server")
                    Feedback = "Error, try again with Proxy enabled";
                else
                    Feedback = "Something went wrong, maybe this helps: " + ex.Message;

                FeedbackColor = "Red";
                
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
                evers.Achtergrondkleur = "GreenYellow";
            }

        }

        //When the download is progressing.
        private void Client_DownLoadProcessChanged(object sender, DownloadProgressChangedEventArgs e)
        {
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
                FeedbackColor = "Red";
                Feedback = "Something went wrong, maybe this helps: " + ex.Message;
            }
        }

        //Open and choose a folder where the downloads will go to
        private void OpenDownloadFolder()
        {
            try
            {
                using(FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    //Venster openen
                    DialogResult result = fbd.ShowDialog();

                    //Nakijken of dat venster succesvol is geopend en pad is gekozen
                    if(result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        //Pad opslaan
                        _DOWNLOADFOLDER = fbd.SelectedPath;

                        Console.WriteLine("Downloadpad veranderd naar: " + fbd.SelectedPath);
                    }
                }
            }
            catch(Exception ex)
            {
                FeedbackColor = "Red";
                Feedback = "Failed to open folder: " + ex.Message;
            }
        }
        #endregion
    }
}
