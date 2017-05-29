using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Download_Eve_Radio_sessions
{
    class Program
    {
        static void Main(string[] args)
        {
            //EVE Radio heeft leuke setjes die je tot 2 weken terug kan herbeluisteren. 
            //Nu wil ik die setjes langer kunnen beluisteren en daarom download ik die.
            //Aangezien ik te lui ben om dit elke keer handmatig te doen schrijven we hier ene programmaatje voor.

            using(WebClient client = new WebClient())
            {
                //We downloaden de EVE-Radio pagina
                string htmlCode = client.DownloadString("http://eve-radio.com/radio/rewind");

                //We splitsen de herbeluister divs op basis van hun ID
                string[] stringSeperators = new string[] { "<div id='erRW' style='float: left;'>" };
                string[] rewinds = htmlCode.Split(stringSeperators, StringSplitOptions.None).Skip(1).ToArray();
                //herbeluisters.Skip(1).ToArray();//Het eerste element eruit halen.

                //Nu gaan we voor elk van de gevonden rewinds de starturl opzoeken
                foreach(string rewind in rewinds)
                {
                    //client.DownloadProgressChanged += Client_DownLoadProcessChanged;

                    //We vinden de startpositie van de tekst die we willen, de eindpositie, en halen daar de lengte uit.
                    int startPos = rewind.IndexOf("Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'") + "Listen from: <a href='#' onclick=\"javascript:doCmd({rewind:'".Length;
                    int length = rewind.IndexOf("'}); return false;\">Start") - startPos;

                    string downloadUrl = rewind.Substring(startPos, length);

                    client.DownloadFile(downloadUrl, "test.mp3");
                    //client.DownloadFileAsync(new Uri(downloadUrl), "test.mp3");
                }
            }
        }

        //private async Task DownloadFileAsync(DocumentObject)

        //Het verloop van de download tonen
        //static void Client_DownLoadProcessChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    int value = e.ProgressPercentage;
        //}
    }
}
