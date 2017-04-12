using Newtonsoft.Json;
using NSpeedTest;
using NSpeedTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TizenSpeedTest
{

    public class App : Application
    {
        private static SpeedTestClient client;
        private static Settings settings;
        private const string DefaultCountry = "Belarus";
        private static string clientCountry = null;
        StackLayout parent = null;
        private struct PrintableSpeed
        {
            public string label;
            public double speed;
            public PrintableSpeed(double speed)
            {
                if (speed > 1024)
                {
                    speed =Math.Round(speed / 1024, 2);
                    this.speed = speed;
                    this.label = "Mbps";
                }
                else
                {
                    speed = Math.Round(speed, 2);
                    this.speed = speed;
                    this.label = "Kbps";
                }

            }
            public string getPrintableSpeed()
            {
                return label + ": " + speed.ToString();
            }
        }

        private Label testLabel;
        private Label dnLabel;

        public App()
        {
            // The root page of your application
            parent = new StackLayout();

            Button add = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Xamarin.Forms.Color.White,
                Text = "Start Test",
                TextColor = Xamarin.Forms.Color.Maroon,
            };

            Button added = new Button
            {
                HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = Xamarin.Forms.Color.White,
                Text = "Test",
                TextColor = Xamarin.Forms.Color.Black,
            };

            add.Clicked += OnButtonClicked;

            Label firstLabel = new Label
            {
                Text = "Biatch",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Xamarin.Forms.Color.FromHex("#000000")
            };
            parent.Children.Add(add);
            parent.Children.Add(added);
            parent.Children.Add(firstLabel);
            MainPage = new ContentPage
            {
                Content = parent

            };



        }

        private void UpdateUi(double dnSpeed, double upSpeed)
        {
            var printableDownloadSpeed = new PrintableSpeed(dnSpeed);
            var printableUploadSpeed = new PrintableSpeed(upSpeed);
            var tparent = new StackLayout();
            var dntext = "Download " + printableDownloadSpeed.getPrintableSpeed();
            var uptext = "Upload " + printableUploadSpeed.getPrintableSpeed();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {

            Label dnLabel = new Label
            {
                Text = dntext,
                FontSize = 30,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Xamarin.Forms.Color.FromHex("#000000")
            };
            Label upLabel = new Label
            {
                Text = uptext,
                FontSize = 30,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Xamarin.Forms.Color.FromHex("#000000")
            };
            tparent.Children.Add(dnLabel);
            tparent.Children.Add(upLabel);
            MainPage = new ContentPage
            {
                Content = tparent

            };
            });


        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                StartSpeedTestAsync();
            }).ConfigureAwait(false);

            parent = new StackLayout();

            Button add = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Xamarin.Forms.Color.White,
                Text = "Start lapoest",
                TextColor = Xamarin.Forms.Color.Maroon,
            };

            add.Clicked += OnButtonClickedTest;

            testLabel = new Label
            {
                Text = "LOLLLLL",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Xamarin.Forms.Color.FromHex("#000000")
            };
            dnLabel = new Label
            {
                Text = "LOLLLLL",
                HorizontalOptions = LayoutOptions.StartAndExpand,
                TextColor = Xamarin.Forms.Color.FromHex("#000000")
            };
            parent.Children.Add(add);
            parent.Children.Add(testLabel);
            parent.Children.Add(dnLabel);
            MainPage = new ContentPage
            {
                Content = parent

            };
        }

        private void OnButtonClickedTest(object sender, EventArgs e)
        {
            Random rnd = new Random();
            testLabel.Text = rnd.Next(1, 3) == 1? "heyo stop it!": "Oy nnyeaaaaaaaaaaas";
            dnLabel.Text = rnd.Next(1, 3) == 3? "heyo stop it!": "Oy nnyeaaaaaaaaaaas";
        }
        protected override void OnStart()
        {
            // Handle when your app starts
            //var xxx = StartSpeedTest();

        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes

        }

        private static Server SelectBestServer(IEnumerable<Server> servers)
        {
            Debug.WriteLine("__");
            Debug.WriteLine("Best server by latency:");
            var bestServer = servers.OrderBy(x => x.Latency).First();
            PrintServerDetails(bestServer);
            Debug.WriteLine("__");
            return bestServer;
        }

        private static IEnumerable<Server> SelectServers()
        {
            Debug.WriteLine("__");
            Debug.WriteLine("Selecting best server by distance...");
            List<Server> servers;
            if (clientCountry != null)
            {
                servers = settings.Servers.Where(s => s.Country.Equals(clientCountry)).Take(10).ToList();
            }
            else
            {
                servers = settings.Servers.Where(s => s.Country.Equals(DefaultCountry)).Take(10).ToList();
            }

            foreach (var server in servers)
            {
                server.Latency = client.TestServerLatencyAsync(server).GetAwaiter().GetResult();
                PrintServerDetails(server);
            }
            return servers;
        }

        private static void PrintServerDetails(Server server)
        {
            Debug.WriteLine("Hosted by {0} ({1}/{2}), distance: {3}km, latency: {4}ms", server.Sponsor, server.Name,
                server.Country, (int)server.Distance / 1000, server.Latency);
        }

        private static void PrintSpeed(string type, double speed)
        {
            if (speed > 1024)
            {
                Debug.WriteLine("{0} speed: {1} Mbps", type, Math.Round(speed / 1024, 2));
            }
            else
            {
                Debug.WriteLine("{0} speed: {1} Kbps", type, Math.Round(speed, 2));
            }
        }

        private static async System.Threading.Tasks.Task<string> GetClienCountryAsync()
        {
            string url = "http://freegeoip.net/json/";
            var serverResponse = await SpeedTestWebClient.client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            string jsonstring = await serverResponse.Content.ReadAsStringAsync();
            dynamic dynObj = JsonConvert.DeserializeObject(jsonstring);
            return dynObj.country_name;
        }

        public async System.Threading.Tasks.Task<string> StartSpeedTestAsync()
        {

            client = new SpeedTestClient();
            settings = await client.GetSettingsAsync();
            clientCountry = await GetClienCountryAsync();


            var servers = SelectServers();
            var bestServer = SelectBestServer(servers);

            var downloadSpeed = client.TestDownloadSpeed(bestServer, settings.Download.ThreadsPerUrl);
            //PrintSpeed("Download", downloadSpeed);
            var uploadSpeed = client.TestUploadSpeed(bestServer, settings.Upload.ThreadsPerUrl);
            //PrintSpeed("Upload", uploadSpeed);
            UpdateUi(downloadSpeed, uploadSpeed);
            return "Down: " + Math.Round(downloadSpeed / 1024, 2).ToString() + "Up: " + Math.Round(uploadSpeed / 1024, 2).ToString();
            //Console.WriteLine("Press a key to exit.");
            //Console.ReadKey();

        }
    }
}
