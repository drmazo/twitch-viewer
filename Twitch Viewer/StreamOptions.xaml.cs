using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Twitch_Viewer
{
    /// <summary>
    /// Logique d'interaction pour StreamOptions.xaml
    /// </summary>
    public partial class StreamOptions : Window
    {
        public StreamOptions()
        {
            InitializeComponent();

            for (int i=0;i<10;i++)
            {
                eosdelay.Items.Add(i);
            }
            for (int i = 1; i < 6; i++)
            {
                dsegments.Items.Add(i);
            }

            eosdelay.Text = Properties.Settings.Default.hlsDelay;
            dsegments.Text = Properties.Settings.Default.hlsSegments;
        }

        private void streamoptionsave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.hlsDelay = eosdelay.Text;
            Properties.Settings.Default.hlsSegments = dsegments.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}