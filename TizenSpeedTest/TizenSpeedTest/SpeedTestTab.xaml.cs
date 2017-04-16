using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TizenSpeedTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpeedTestTab : ContentPage
    {
        private double width = 0;
        private double height = 0;
        public SpeedTestTab()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width != this.width || height != this.height)
            {
                this.width = width;
                this.height = height;


                if (width > height) //on Landscape
                {
                    if (height < 500) //sd screens
                    {
                        speedDisplayLayout.HeightRequest = 270;
                        TestBtn.FontSize = 22;
                        InAppLogo.VerticalOptions = LayoutOptions.StartAndExpand;
                        InAppLogo.Scale = 0.5;
                        layout.Spacing = 0;
                        History.Scale = 0.7;
                        About.Scale = 0.7;
                    }
                    else // hd screens
                    {
                        TestBtn.FontSize = 30;
                        InAppLogo.Scale = 0.7;
                    }

                }


                if (width < height) // on Portrait sd
                {
                    if (width < 500) //sd screens
                    {
                        speedDisplayLayout.HeightRequest = 270;
                        InAppLogo.VerticalOptions = LayoutOptions.StartAndExpand;
                        TestBtn.FontSize = 26;
                        InAppLogo.Scale = 0.5;
                        History.Scale = 0.7;
                        About.Scale = 0.7;
                    }
                    else // hd screens
                    {
                        TestBtn.FontSize = 36;
                        InAppLogo.Scale = 1;
                    }


                }


            }
        }

        public static Animation CreateFlashingAnimation()
        {
            //    Random rand = new Random(); //Timer Code: 
            //    int A = rand.Next(0, 255); int R = rand.Next(0, 255);
            //    int G = rand.Next(0, 255); int B = rand.Next(0, 255);

            //    label1.ForeColor = Color.FromRgb(A, R, G, B);

            //    timer1.Start(); timer1.Enabled = true; //Close Button Code: 
            //
            return new Animation();
        }
    }
}
