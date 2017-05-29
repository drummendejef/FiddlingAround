using System;
using System.Collections.Generic;
using System.IO;
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
            MainAsync(args).Wait();//Alles downloaden

            Console.WriteLine("Alles is gedownload");
            Console.ReadKey();
        }

        //Alles downloaden
        static async Task MainAsync(string[] args)
        {
            //EVE Radio heeft leuke setjes die je tot 2 weken terug kan herbeluisteren. 
            //Nu wil ik die setjes langer kunnen beluisteren en daarom download ik die.
            //Aangezien ik te lui ben om dit elke keer handmatig te doen schrijven we hier ene programmaatje voor.
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

                        //Proberen met een proxy dingen te omzeilen
                        WebProxy wp = new WebProxy("35.161.2.200", 80);
                        client.Proxy = wp;

                        //Nakijken of dat proxy werkt?

                        //Make session-unique name
                        string filename = downloadUrl.Split('/').Last();
                        string path = System.Reflection.Assembly.GetEntryAssembly().Location;

                        //Check if we didn't already download this file.
                        if(!File.Exists(filename))
                        {
                            //client.DownloadFile(downloadUrl, "test.mp3");//Sync
                            //client.DownloadFileAsync(new Uri(downloadUrl), "test.mp3");//Async slecht
                            await client.DownloadFileTaskAsync(downloadUrl, filename);//Async maar 1 per 1
                                                                                      //await DownloadFileAsync(downloadUrl, client, filename);//Async
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static private async Task DownloadFileAsync(string url, WebClient client, string naam)
        {
            try
            {
                using(WebClient wc = new WebClient())
                {
                    WebProxy wp = new WebProxy("35.161.2.200", 80);
                    wc.Proxy = wp;
                    //    await wc.DownloadFileTaskAsync(new Uri(url), "test.mp3");
                    await client.DownloadFileTaskAsync(new Uri(url), naam);
                }
                //await client.DownloadFileTaskAsync(new Uri(url), naam);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //Het verloop van de download tonen
        static void Client_DownLoadProcessChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("Download " + e.ProgressPercentage + "%");
        }
    }
}
