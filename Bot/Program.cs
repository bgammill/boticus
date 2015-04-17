using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S22.Xmpp;
using S22.Xmpp.Client;
using System.Net;
using Newtonsoft.Json;

namespace Bot
{
    class Program
    {
        public static XmppClient client;

        static void Main(string[] args)
        {
            var creds = new Credentials()
            {
                Server = "",
                Username = "",
                Password = ""
            };

            client = new XmppClient(creds.Server, creds.Username, creds.Password);

            client.Message += client_Message;
            client.Connect();

            Console.WriteLine("Type 'quit' to exit.");
            while (Console.ReadLine() != "quit") ;
        }

        static void client_Message(object sender, S22.Xmpp.Im.MessageEventArgs e)
        {
            if (e.Message.Body.Contains("!") && e.Message.Body.IndexOf('!') == 0)
            {
                var pieces = e.Message.Body.Split('!');

                if (pieces[1].Contains("weather"))
                {
                    var weatherPieces = e.Message.Body.Split(' ');

                    var webClient = new WebClient();
                    var res = webClient.DownloadString("http://api.openweathermap.org/data/2.5/weather?zip=" + weatherPieces[1] + ",us");

                    var output = JsonConvert.DeserializeObject<WeatherObject>(res);

                    var temperature = (output.main.temp - 273.15) * 1.8000 + 32;

                    client.SendMessage(e.Jid, output.weather[0].description + ". Temperature is " + temperature + " F" + " and winds at " + output.wind.speed + " MPH");
                }
                else if (pieces[1] == "hello")
                {
                    client.SendMessage(e.Jid, "http://ohn1.slausworks.netdna-cdn.com/newohnblog/wp-content/uploads/2012/04/SULU.jpg");
                }

                Console.WriteLine("Command: " + pieces[1] + " was called by: " + e.Jid);
            }
            Console.WriteLine(e.Message.Body);
        }
    }
}
