﻿using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestWPF
{
    /// <summary>
    /// MainWindow.xaml 
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            JObject x = new JObject(Get("https://xqgy0d8a3l.execute-api.us-east-1.amazonaws.com/Prod/places/" + ZipCodeTextBox.Text));
            IList<JToken> results = x["results"].Children().ToList();

            IList<Place> searchResults = new List<Place>();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Place searchResult = result.ToObject<Place>();
                searchResults.Add(searchResult);
            }

            Console.WriteLine(searchResults);
            foreach(Place searchResult in searchResults)
            {
                ListBoxItem placeListBoxItem = new ListBoxItem();
                placeListBoxItem.Content = searchResult.Name;
                PlacesListBox.Items.Add(placeListBoxItem);
            }
            
        }
        public JObject Get(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                JObject o = JObject.Parse(reader.ReadToEnd());
                return o;

            }
        }
    }
}
