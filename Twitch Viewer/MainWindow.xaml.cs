using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TwitchLib;
using TwitchLib.Models.API;

namespace Twitch_Viewer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            TwitchAPI.Settings.ClientId = "08fsgpsoy1yb2frvnqkqbe0mrgyz5r"; 
            InitializeComponent();
            this.Title = this.Title + " -";
            GetListGames();
            if(Properties.Settings.Default.hlsDelay=="")
            {
                Properties.Settings.Default.hlsDelay = "4";
            }

            if (Properties.Settings.Default.hlsSegments  == "")
            {
                Properties.Settings.Default.hlsSegments = "3";
            }

            if (Properties.Settings.Default.username == "" || Properties.Settings.Default.token =="")
            {
                Login loginwindow = new Login();
                loginwindow.Show();
                loginwindow.Activate();
                loginwindow.Topmost = true;
                //test git
            }
            else
            {
                this.Title = this.Title + " " + Properties.Settings.Default.username;
                CheckChatOption();
                GetFollowedStreams();
            }

        }
        
        private void listgames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetListStream(listgames.SelectedItem.ToString());
        }

        private void liststreams_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(liststreams.SelectedItems.Count>0)
            {
                StreamList channel = (StreamList)liststreams.Items[liststreams.SelectedIndex];
                LaunchStream(channel.Url);
            }       
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        { 
            if(listgames.Text!="")
            {
                GetListStream(listgames.SelectedItem.ToString());
            }
        }

        private void follow_Click(object sender, RoutedEventArgs e)
        {
            FollowStream();
        }

        private void unfollow_Click(object sender, RoutedEventArgs e)
        {
            UnfollowStream();
        }

        private void listfollow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listfollow.SelectedItems.Count>0)
            {
                StreamList channel = (StreamList)listfollow.Items[listfollow.SelectedIndex];
                LaunchStream(channel.Url);
            }
        }

        private void refreshfollows_Click(object sender, RoutedEventArgs e)
        {
            GetFollowedStreams();
        }

        private void loginmenu_Click(object sender, RoutedEventArgs e)
        {
            Login loginwindow = new Login();
            loginwindow.Show();
        }


        private async void GetListGames()
        {
            TwitchLib.Models.API.v5.Games.TopGames topgames = await TwitchAPI.Games.v5.GetTopGames(100);
            List<String> gameslist = new List<string>();
            foreach (TwitchLib.Models.API.v5.Games.TopGame game in topgames.Top)
            {
                gameslist.Add(game.Game.Name);
            }
            listgames.ItemsSource = gameslist;
        }

        public async void GetListStream(String game)
        {
            liststreams.Items.Clear();
            TwitchLib.Models.API.v5.Streams.LiveStreams liststream = await TwitchAPI.Streams.v5.GetLiveStreams(null, game, null, null, 100);

            foreach (TwitchLib.Models.API.v5.Streams.Stream stream in liststream.Streams)
            {
                this.liststreams.Items.Add(new StreamList { Name = stream.Channel.DisplayName, Status = stream.Channel.Status, Viewers = stream.Viewers.ToString(), Id = stream.Channel.Id.ToString(), Url = stream.Channel.Url });

            }
        }

        public async void GetFollowedStreams()
        {
            listfollow.Items.Clear();
            if(Properties.Settings.Default.username!="")
            {
                TwitchLib.Models.API.v5.Users.Users userid = await TwitchAPI.Users.v5.GetUserByName(Properties.Settings.Default.username);
                if (userid.Total != 0)
                {
                    TwitchLib.Models.API.v5.Users.UserFollows follows = await TwitchAPI.Users.v5.GetUserFollows(userid.Matches[0].Id.ToString(),100,null,"desc","last_broadcast"); 
                    if (follows.Follows != null)
                    {   
                        

                        foreach (TwitchLib.Models.API.v5.Users.UserFollow follow in follows.Follows)
                        {
                            string viewerstatus;
                            string gamestatus;
                            string streamstatus;
                            TwitchLib.Models.API.v5.Streams.StreamByUser stream = await TwitchAPI.Streams.v5.GetStreamByUser(follow.Channel.Id.ToString());
                            if(stream.Stream==null)
                            {
                                viewerstatus = "0";
                                gamestatus = "";
                                streamstatus = "OFFLINE";
                            }
                            else
                            {       
                                viewerstatus = stream.Stream.Viewers.ToString();
                                gamestatus = stream.Stream.Game;
                                streamstatus = stream.Stream.Channel.Status;
                            }
                            this.listfollow.Items.Add(new StreamList { Name = follow.Channel.DisplayName, Game = gamestatus, Status = streamstatus, Id = follow.Channel.Id.ToString(), Url = follow.Channel.Url, Viewers = viewerstatus });

                        }
                    }
                    
                }   
            }
            else
            {
                MessageBox.Show("Please enter username");
            }            
        }       
        

        private async void FollowStream()
        {
            if (Properties.Settings.Default.username == "")
            {
                MessageBox.Show("Veuillez enregistrer un username");
            }
            else
            {
                if (liststreams.SelectedItems.Count > 0)
                {
                    StreamList channel = (StreamList)liststreams.Items[liststreams.SelectedIndex];
                    TwitchLib.Models.API.v5.Users.Users userid = await TwitchAPI.Users.v5.GetUserByName(Properties.Settings.Default.username);
                    if (userid.Total != 0)
                    {
                        TwitchLib.Models.API.v5.Users.UserFollow follow = await TwitchAPI.Users.v5.FollowChannel(userid.Matches[0].Id.ToString(), channel.Id, null, Properties.Settings.Default.token);
                        GetFollowedStreams();
                    }
                }
            }
        }


        private async void UnfollowStream()
        {
            if (Properties.Settings.Default.username == "" || Properties.Settings.Default.token == "")
            {
                MessageBox.Show("Veuillez enregistrer un username et un token");
            }
            else
            {
                if (listfollow.SelectedItems.Count > 0)
                {
                    StreamList channel = (StreamList)listfollow.Items[listfollow.SelectedIndex];
                    TwitchLib.Models.API.v5.Users.Users userid = await TwitchAPI.Users.v5.GetUserByName(Properties.Settings.Default.username);
                    if (userid.Total != 0)
                    {
                        await TwitchAPI.Users.v5.UnfollowChannel(userid.Matches[0].Id.ToString(), channel.Id, Properties.Settings.Default.token);
                        GetFollowedStreams();
                    }
                }
            }
        }

        private void LaunchStream(string url)
        {
            var sb = new StringBuilder();
            
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C streamlink " + url + " best --hls-live-edge " + Properties.Settings.Default.hlsDelay + " --hls-segment-threads " + Properties.Settings.Default.hlsSegments;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            if (checkChat.IsChecked == true)
            {
                StreamChat test = new StreamChat(url + "/chat?popout=");
                test.Show();
            }
        }



        private void CheckChatOption()
        {
            if (Properties.Settings.Default.chatActive == true)
            {
                checkChat.IsChecked = true;
            }
            else
            {
                checkChat.IsChecked = false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(checkChat.IsChecked==true)
            {
                Properties.Settings.Default.chatActive = true;
            }
            else
            {
                Properties.Settings.Default.chatActive = false;
            }
            Properties.Settings.Default.Save();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            string[] tokens = this.Title.Split(' ');
            if ( tokens[tokens.Length-1] != Properties.Settings.Default.username)
            {
                this.Title = this.Title + " " + Properties.Settings.Default.username;

            }
            
        }

        private void streamOptions_Click(object sender, RoutedEventArgs e)
        {
            StreamOptions sowindow = new StreamOptions();
            sowindow.Show();

        }

        
    }


}
