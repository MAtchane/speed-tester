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
        public SpeedTestTab()
        {
            InitializeComponent();
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
