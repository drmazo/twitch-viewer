using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Twitch_Viewer.Properties;
using TwitchLib;

namespace Twitch_Viewer
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
            usernameBox.Text = Properties.Settings.Default.username;
            tokenBox.Text = Properties.Settings.Default.token;
        }



        private void logintry_Click(object sender, RoutedEventArgs e)
        {       
            if(usernameBox.Text!="" && tokenBox.Text !="")
            {
                Properties.Settings.Default.username = usernameBox.Text;
                Properties.Settings.Default.token = tokenBox.Text;
                this.Close();
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Please enter a username and a token");
            }

            
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
