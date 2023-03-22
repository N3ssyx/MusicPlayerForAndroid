/*
 * ETML
 * Author : Youness Takache
 * Date : 19.11.2021
 * Description : Main Page of the Music Player 
 */

using Android.Media;
using System;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Music
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Attribute of the path that will be scanned & instance of the mediaplayer
        /// </summary>
        string _path = "storage/emulated/0/Download";
        protected MediaPlayer _mediaPlayer = new MediaPlayer();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            ChargeListOfFiles();
        }

        /// <summary>
        /// Method for checking permissions, granting them if needed and getting all mp3 files in the repository indicated in the string "path"
        /// </summary>
        private async void ChargeListOfFiles()
        {
            // Asks permission for reading in storage
            var readPermission = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if(readPermission != PermissionStatus.Granted)
            {
                readPermission = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            // Asks permission for writing in storage
            var writePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (writePermission != PermissionStatus.Granted)
            {
                writePermission = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            // Asks permission to get access to media
            var accessMediaPermission = await Permissions.CheckStatusAsync<Permissions.Media>();
            if (accessMediaPermission != PermissionStatus.Granted)
            {
                accessMediaPermission = await Permissions.RequestAsync<Permissions.Media>();
            }

            // Tries to get path and reads all files in it, else return an error message
            DirectoryInfo directory = new DirectoryInfo(_path);
            try
            {
                FileInfo[] files = directory.GetFiles();
                List<string> listOfFiles = new List<string>();

                foreach(FileInfo item in files)
                {
                    listOfFiles.Add(item.Name);
                }

                listOfFiles.Sort();

                // Create a button for each audio file present in the directory
                foreach (string item in listOfFiles)
                {
                    if (item.Contains(".mp3") || item.Contains(".m4a") || item.Contains(".wav") || item.Contains(".ogg"))
                    {
                        Button btn = new Button();
                        btn.Text = item;
                        musicsToDisplay.Children.Add(btn);
                        btn.Clicked += OnButtonClicked;
                    }
                }
            }
            catch(Exception e)
            {
                Label label = new Label();
                label.Text = e.Message;
                musicsToDisplay.Children.Add(label);
            }
        }

        /// <summary>
        /// Method if a button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClicked(object sender, EventArgs e)
        {
            string songName = (sender as Button).Text;
            _mediaPlayer.Reset();
            _mediaPlayer.SetDataSource(_path + "/"+ songName);
            _mediaPlayer.Prepare();
            _mediaPlayer.Start();
            pausePlayButton.Source = "pause.png";
            if (songName.Length <= 54)
            {
                nowPlaying.Text = songName;
            }
            else
            {
                nowPlaying.Text = songName.Substring(0, 50) + "...";
            }
        }

        /// <summary>
        /// Method to pause or resume the song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PauseButton_Clicked(object sender, EventArgs e)
        {
            if (_mediaPlayer.IsPlaying)
            {
                _mediaPlayer.Pause();
                pausePlayButton.Source = "play.png";
            }
            else
            {
                _mediaPlayer.Start();
                pausePlayButton.Source = "pause.png";
            }
        }
    }
}
