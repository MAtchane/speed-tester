using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TizenSpeedTest
{
    public struct HistoryEntry
    {
        public string DownloadSpeed { get; set; }
        public string UploadSpeed { get; set; }
        public string DownloadUnit { get; set; }
        public string UploadUnit { get; set; }
        public string Date { get; set; }
        private char delimiter;
        public HistoryEntry(string entry)
        {
            delimiter = ';';
            var attributes = entry.Split(delimiter);
            Date = attributes[0];
            DownloadSpeed = attributes[1];
            DownloadUnit = attributes[2];
            UploadSpeed = attributes[3];
            UploadUnit = attributes[4];
        }

    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryTab : ContentPage
    {
        private int initialnumberOfEntries;

        public HistoryTab()
        {
            var myEntries = CreateGridFromHistoryData();
            initialnumberOfEntries = GetNumberOfHistoryEntries();
            var stack = new StackLayout();
            stack.Children.Add(myEntries);
            var scrollView = new ScrollView
            {
                HorizontalOptions = LayoutOptions.Fill,

                Content = stack
            };


            InitializeComponent();
            Content = scrollView;

        }

        public void UpdateHistoryTable()
        {
            if (initialnumberOfEntries != GetNumberOfHistoryEntries())
            {
                var myEntries = CreateGridFromHistoryData();
                initialnumberOfEntries = GetNumberOfHistoryEntries();
                var stack = new StackLayout();
                stack.Children.Add(myEntries);
                var scrollView = new ScrollView
                {
                    HorizontalOptions = LayoutOptions.Fill,

                    Content = stack
                };
                Content = scrollView;
            }
        }

        private Grid CreateGridFromHistoryData()
        {
            var numberOfEntries = GetNumberOfHistoryEntries();
            RowDefinitionCollection rowDefinitions = new RowDefinitionCollection();
            rowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); //for the titles

            for (int MyCount = 0; MyCount < numberOfEntries; MyCount++)
            {

                rowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            }

            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10,
                RowSpacing = 2,
                ColumnSpacing = 2,
                RowDefinitions = rowDefinitions,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
                }
            };
            //Background for the Columns 
            grid.Children.Add(new BoxView { BackgroundColor = Color.FromHex("#948073") }, 0, 1, 0, numberOfEntries + 1);
            grid.Children.Add(new BoxView { BackgroundColor = Color.FromHex("#B89B8A") }, 1, 2, 0, numberOfEntries + 1);
            grid.Children.Add(new BoxView { BackgroundColor = Color.FromHex("#948073") }, 2, 3, 0, numberOfEntries + 1);           

            //Headers
            grid.Children.Add(new Label { Text = "Date", HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand }, 0, 0);
            grid.Children.Add(new Label { Text = "Download", HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand }, 1, 0);
            grid.Children.Add(new Label { Text = "Upload", HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand},2, 0);

            //Add history entries to the grid
            for (int i = 1; i < numberOfEntries+1; i++)
            {
                var entry = ReadHistoryEntry(i);
                var dataFromEntry = new HistoryEntry(entry);
                grid.Children.Add(new Label { Text = dataFromEntry.Date, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand}, 0, i);
                grid.Children.Add(new Label { Text = dataFromEntry.DownloadSpeed + " " + dataFromEntry.DownloadUnit, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand }, 1, i);
                grid.Children.Add(new Label { Text = dataFromEntry.UploadSpeed + " " + dataFromEntry.UploadUnit, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand }, 2, i);

            }


            return grid;

        }

        public static int GetNumberOfHistoryEntries()
        {
            if (Application.Current.Properties.ContainsKey("currentNumberOfTests"))
            {
                var currentNumberOfTests = Convert.ToInt32(Application.Current.Properties["currentNumberOfTests"]);
                return currentNumberOfTests;
            }
            else
            {
                return 0;

            }
        }

        public static string ReadHistoryEntry(int entryNumber)
        {
            var key = "test#" + entryNumber;
            if (Application.Current.Properties.ContainsKey(key))
            {
                var entry = Convert.ToString(Application.Current.Properties[key]);
                return entry;
            }
            else
            {
                return null;

            }
        }
    }


}
