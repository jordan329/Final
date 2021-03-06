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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestWPF
{
    public partial class MainWindow : Window
    {
        IList<Place> searchResults = new List<Place>();
        IList<Details> detailResults = new List<Details>();
        string currentFilePath = "";
        public MainWindow()
        {
            InitializeComponent();
        }
       
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            JObject response = new JObject(Get("https://xqgy0d8a3l.execute-api.us-east-1.amazonaws.com/Prod/places/" + ZipCodeTextBox.Text));
            IList<JToken> results = response["results"].Children().ToList();
            foreach (JToken result in results)
            {
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                Place searchResult = result.ToObject<Place>();
                searchResults.Add(searchResult);
            }
            foreach(Place searchResult in searchResults)
            {
                ListBoxItem placeListBoxItem = new ListBoxItem();
                placeListBoxItem.Content = searchResult.Name;
                PlacesListBox.Items.Add(placeListBoxItem);
            }            
        }
        
        private void PlacesListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if(PlacesListBox.Items.Count == 0)
            {
                return;
            }
            var place_id = searchResults.ElementAt(PlacesListBox.SelectedIndex).Place_Id;
            JObject response = new JObject(Get("https://xqgy0d8a3l.execute-api.us-east-1.amazonaws.com/Prod/details/" + place_id));
            var name = response["result"]["name"];
            var phone = response["result"]["international_phone_number"];
            var address = response["result"]["formatted_address"];
            var price = response["result"]["price_level"];
            var rating = response["result"]["rating"];

            if (name != null) {
                NameTextBox.Text = name.ToString();
            }
            if (phone != null) {
                PhoneNumberTextBox.Text = phone.ToString();
            }
            if (address != null) {
                AddressTextBox.Text = address.ToString();
            }
            if (price != null) {
                PriceTextBox.Text = price.ToString();
            }
            if(rating != null) {
                RatingTextBox.Text = rating.ToString();
            }
            var hours = response["result"]["opening_hours"]["weekday_text"].Children().ToList();
            HoursTextBox.Text = "";
                
            if(hours != null) {
                hours.ForEach((hour) => {
                    HoursTextBox.AppendText(hour + Environment.NewLine);
                });
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
                JObject obj = JObject.Parse(reader.ReadToEnd());
                return obj;
            }
        }
        private void Clear()
        {
            PlacesListBox.Items.Clear();
            searchResults.Clear();
            NameTextBox.Text = "";
            AddressTextBox.Text = "";
            PhoneNumberTextBox.Text = "";
            RatingTextBox.Text = "";
            HoursTextBox.Text = "";
            PriceTextBox.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // http://msdn.microsoft.com/en-us/library/system.io.path.getdirectoryname.aspx

            if (currentFilePath != "") {
                saveFileDialog.FileName = System.IO.Path.GetFileName(currentFilePath);
                saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(currentFilePath);
            }
            saveFileDialog.DefaultExt = ".txt"; // Default file extension
            saveFileDialog.Filter = "Text Documents (.txt)|*.txt"; // Filter files by extension
            // Show save file dialog box
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                saveDocument(saveFileDialog.FileName);
            }
        }
        private void saveDocument(String filePath) {            
                TextWriter txt = new StreamWriter(filePath);
                try {
                    txt.Write(
                        "Name: " + NameTextBox.Text + Environment.NewLine +
                        "Phone: " + PhoneNumberTextBox.Text + Environment.NewLine +
                        "Address: " + AddressTextBox.Text + Environment.NewLine +
                        "Price: " + PriceTextBox.Text + Environment.NewLine +
                        "Rating: " + RatingTextBox.Text + Environment.NewLine + Environment.NewLine +
                        "Hours: " + Environment.NewLine + HoursTextBox.Text + Environment.NewLine
                        );
                }
                catch {
                    System.Windows.MessageBox.Show("An error occured");
                }
                txt.Close();           
        }
    }
}
