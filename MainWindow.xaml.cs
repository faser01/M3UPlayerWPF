using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace M3UPlayerWPF
{
    public partial class MainWindow : Window
    {
        private List<Channel> playlist;


        public MainWindow()
        {
            InitializeComponent();
            this.MediaElementPlayer.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(MediaFailed);
        }

        void MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Ошибка воспроизведения: " + e.ErrorException.Message);
        }

        class Channel
        {
            public string Name { get; set; }
            public string Url { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string fileName = "webtv_usr.xml";
            playlist = await ParseXMLFile(fileName);

            if (playlist.Count > 0)
            {
                foreach (Channel channel in playlist)
                {
                    listBoxChannels.Items.Add(channel);
                }
            }
            else
            {
                MessageBox.Show("Не удалось загрузить список каналов из файла m3u");
            }
        }

        private async Task<List<Channel>> ParseXMLFile(string fileName)
        {
            List<Channel> channels = new List<Channel>();

            try
            {
                XDocument xdoc = XDocument.Load(fileName);
                foreach (XElement channelElement in xdoc.Element("channels").Elements("channel"))
                {
                    Channel channel = new Channel();
                    XElement nameElement = channelElement.Element("name");
                    XElement urlElement = channelElement.Element("url");

                    if (nameElement != null)
                    {
                        channel.Name = nameElement.Value.Trim();
                    }

                    if (urlElement != null)
                    {
                        channel.Url = urlElement.Value.Trim();
                    }

                    channels.Add(channel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении файла xml: " + ex.Message);
            }

            return channels;
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxChannels.SelectedItem != null)
            {
                Channel selectedChannel = (Channel)listBoxChannels.SelectedItem; // нужно привести selectedItem к типу Channel

                MediaElementPlayer.Source = new Uri(selectedChannel.Url); // использую свойство Url выбранного канала
                MediaElementPlayer.Play();
            }
            else
            {
                MessageBox.Show("Выберите канал для проигрывания");
            }
        }
    }
}