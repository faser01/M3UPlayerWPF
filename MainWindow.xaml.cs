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
            this.Loaded += new RoutedEventHandler(Window_Loaded);
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string fileName = "webtv_usr.xml";
            playlist = ParseXMLFile(fileName);

            if (playlist.Count > 0)
            {
                foreach (Channel channel in playlist)
                {
                    listBoxChannels.Items.Add(channel);
                }
            }
            else
            {
                MessageBox.Show("Не удалось загрузить список каналов из файла xml");
            }
        }

        private List<Channel> ParseXMLFile(string fileName)
        {
            List<Channel> channels = new List<Channel>();

            if (!File.Exists(fileName))
            {
                MessageBox.Show("Файл не найден: " + fileName);
                return channels;
            }

            try
            {
                XDocument xdoc = XDocument.Load(fileName);
                foreach (XElement channelElement in xdoc.Element("webtvs").Elements("webtv"))
                {
                    Channel channel = new Channel();
                    XAttribute nameAttribute = channelElement.Attribute("title");
                    XAttribute urlAttribute = channelElement.Attribute("url");

                    if (nameAttribute != null)
                    {
                        channel.Name = nameAttribute.Value.Trim();
                    }

                    if (urlAttribute != null)
                    {
                        channel.Url = urlAttribute.Value.Trim();
                    }

                    channels.Add(channel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении файла xml: " + ex.Message);
            }

            if (channels.Count == 0)
            {
                MessageBox.Show("Нет каналов в этом файле или ошибка чтения файла");
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