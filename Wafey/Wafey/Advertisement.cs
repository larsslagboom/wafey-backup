using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Wafey
{
    class Advertisement
    {
        DispatcherTimer dtimer = new DispatcherTimer();
        MainWindow window;
        int lastAd;
        public Advertisement(MainWindow window)
        {
            if (window.currentUser.UserSubscription.Equals(0))
            {
                this.window = window;

                dtimer.Tick += showAd;
                dtimer.Interval = TimeSpan.FromSeconds(10);
                dtimer.Start();
            }

        }

        public void showAd(Object sender, EventArgs agrs)
        {
            if (window.currentUser.UserSubscription.Equals(0))
            {
                Random rand = new Random();
                int advert = rand.Next(166, 181);
                while (lastAd == advert)
                {
                    advert = rand.Next(166, 181);
                }
                lastAd = advert;
                string adpath = "pack://application:,,,/Images/Screenshot_" + lastAd + ".png";
                window.Advertisement.Source = new BitmapImage(new Uri(adpath));
                window.Advertisement.Visibility = Visibility.Visible;
            }
            else
            {
                window.Advertisement.Visibility = Visibility.Hidden;
            }
        }
    }
}