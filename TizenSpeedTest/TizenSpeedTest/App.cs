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
        private SpeedTestTab speedTestTab;
        private AboutTab aboutTab;
        private HistoryTab historyTab;
        private Button testBtn;
        private Button aboutBtn;
        private Button historyBtn;
        private Label serverInfo;
        private Image downloadSpeedIcon;
        private Label downloadSpeedValue;
        private Label downloadSpeedLabel;
        private Image uploadSpeedIcon;
        private Label uploadSpeedValue;
        private Label uploadSpeedLabel;
        private TestState myTestState;


        enum TestState
        {
            Idle,
            Testing,
            OneDone,
            AllDone
        }

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


        public App()
        {
            //Initializing the tabs
            speedTestTab = new SpeedTestTab();
            historyTab = new HistoryTab();
            aboutTab = new AboutTab();

            //setting references to the UI views
            testBtn = speedTestTab.FindByName<Button>("TestBtn");

            downloadSpeedIcon = speedTestTab.FindByName<Image>("DownloadSpeedIcon");
            downloadSpeedValue = speedTestTab.FindByName<Label>("DownloadSpeedValue");
            downloadSpeedLabel = speedTestTab.FindByName<Label>("DownloadSpeedLabel");

            uploadSpeedIcon = speedTestTab.FindByName<Image>("UploadSpeedIcon");
            uploadSpeedValue = speedTestTab.FindByName<Label>("UploadSpeedValue");
            uploadSpeedLabel = speedTestTab.FindByName<Label>("UploadSpeedLabel");

            historyBtn = speedTestTab.FindByName<Button>("History");
            aboutBtn = speedTestTab.FindByName<Button>("About");
            serverInfo = speedTestTab.FindByName<Label>("ServerInfo");

            //setting the click handelers
            testBtn.Clicked += OnTestBtnClicked;
            historyBtn.Clicked += OnHistoryBtnClicked;
            aboutBtn.Clicked += OnAboutBtnClicked;

            //State zero
            myTestState = TestState.Idle;

            //Using a NavigationPage to handle multiple tabs(pages)
            MainPage = new NavigationPage(speedTestTab)
            { BarBackgroundColor = Color.FromHex("#343C46"),
              BarTextColor = Color.FromHex("#F0F8E6")
              };




        }

        private void OnTestBtnClicked(object sender, EventArgs e)
        {
            testBtn.IsEnabled = false;
            HideResults();
            myTestState = TestState.Testing;
            Task.Run(async () =>
            {
                StartSpeedTestAsync();
            }).ConfigureAwait(false);
        }

        private void OnAboutBtnClicked(object sender, EventArgs e)
        {
            MainPage.Navigation.PushAsync(aboutTab);
        }

        private void OnHistoryBtnClicked(object sender, EventArgs e)
        {
            MainPage.Navigation.PushAsync(historyTab);
        }

        private void UpdateServerInfo(Server info)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                serverInfo.Text = "Hosted by " + info.Sponsor + " (" + info.Name + "/" + info.Country + ")";

            });
        }

        private void UpdateDownloadUi(double dnSpeed)
        {
            var printableDownloadSpeed = new PrintableSpeed(dnSpeed);
            UpdateTestState();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                downloadSpeedValue.Text = printableDownloadSpeed.speed.ToString();
                downloadSpeedLabel.Text = printableDownloadSpeed.label;
                ShowDownloadResults();


            });


        }

        private void UpdateUploadUi(double upSpeed)
        {
            var printableUploadSpeed = new PrintableSpeed(upSpeed);
            UpdateTestState();

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                uploadSpeedValue.Text = printableUploadSpeed.speed.ToString();
                uploadSpeedLabel.Text = printableUploadSpeed.label;
                ShowUploadResults();



            });


        }

        private void UpdateTestState()
        {
            if (myTestState == TestState.Testing)
            {
                myTestState = TestState.OneDone;
            }
            else
            {
                myTestState = TestState.AllDone;
            }
        }

        private void HideResults()
        {
            serverInfo.Text = "Finding the Best Server";
            downloadSpeedLabel.IsVisible = false;
            downloadSpeedValue.IsVisible = false;
            uploadSpeedLabel.IsVisible = false;
            uploadSpeedValue.IsVisible = false;
        }

        private void ShowDownloadResults()
        {
            downloadSpeedLabel.IsVisible = true;
            downloadSpeedValue.IsVisible = true;
            if (myTestState == TestState.AllDone)
            {
                testBtn.IsEnabled = true;
            }
        }

        private void ShowUploadResults()
        {
            uploadSpeedLabel.IsVisible = true;
            uploadSpeedValue.IsVisible = true;
            if (myTestState == TestState.AllDone)
            {
                testBtn.IsEnabled = true;
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            HideResults();

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
            UpdateServerInfo(bestServer);

            var downloadSpeed = client.TestDownloadSpeed(bestServer, settings.Download.ThreadsPerUrl);
            UpdateDownloadUi(downloadSpeed);
            //PrintSpeed("Download", downloadSpeed);
            var uploadSpeed = client.TestUploadSpeed(bestServer, settings.Upload.ThreadsPerUrl);
            //PrintSpeed("Upload", uploadSpeed);
            UpdateUploadUi(uploadSpeed);
            return "Down: " + Math.Round(downloadSpeed / 1024, 2).ToString() + "Up: " + Math.Round(uploadSpeed / 1024, 2).ToString();
            //Console.WriteLine("Press a key to exit.");
            //Console.ReadKey();

        }
    }
}
