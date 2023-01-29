using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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

namespace Async_RestApi
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonSync_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LoadDataSync();
            sw.Stop();
            var time = sw.ElapsedMilliseconds;

            textBlockInfo.Text += $"\n\nTotal time: {time}\n============================";
        }

        private async void ButtonASync_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await LoadDataASyncParallel();
            sw.Stop();
            var time = sw.ElapsedMilliseconds;

            textBlockInfo.Text += $"\n\nTotal time: {time}\n============================";
        }
        public void LoadDataSync()
        {
            List<string> sites = PrepareLoadSize();
            foreach (var item in sites)
            {
                DataModel dm = LoadSite(item);
                PrintInfo(dm);
            }
        }
        public async Task LoadDataASync()
        {
            List<string> sites = PrepareLoadSize();
            foreach (var item in sites)
            {
                DataModel dm = await Task.Run(() => LoadSite(item));
                PrintInfo(dm);
            }
           
        }
        public async Task LoadDataASyncParallel()
        {
            List<string> sites = PrepareLoadSize();
            List<Task<DataModel>> tasks = new List<Task<DataModel>>();
            foreach (var item in sites)
            {
                tasks.Add(Task.Run(() => LoadSite(item)));
            }
            DataModel[] dm = await Task.WhenAll(tasks);
            foreach (var item in dm)
            {
                PrintInfo(item);
            }
           
        }
        private void PrintInfo(DataModel dataModel)
        {
            textBlockInfo.Text += $"\nUrl: {dataModel.Url}, Length: {dataModel.Data.Length}";
        }
        private List<string> PrepareLoadSize()
        {
            List<string> sites = new List<string>()
            {
                "https://google.com",
                "https://my.progtime.net",

            };
            return sites;
        }
        private DataModel LoadSite(string site)
        {
            DataModel dm = new DataModel();
            dm.Url = site;
            WebClient wc = new WebClient();
            dm.Data = wc.DownloadString(site);
            Dispatcher.BeginInvoke((Action)(() => { textBlockInfo.Text = ""; }));
            
            return dm;
        }
    }
}
