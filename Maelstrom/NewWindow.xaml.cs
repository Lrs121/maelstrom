﻿using System;
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
using MahApps.Metro.Controls;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Globalization;

namespace FF8Mod.Maelstrom
{
    /// <summary>
    /// Interaction logic for NewWindow.xaml
    /// </summary>
    public partial class NewWindow : MetroWindow
    {
        public NewWindow()
        {
            State.Load(App.Path + @"\settings.json");
            InitializeComponent();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            State.Save(App.Path + @"\settings.json");
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Multiselect = false,
                Filter = "Final Fantasy VIII Executable|ff8_en.exe;ffviii.exe",
                Title = "Select game location"
            };

            if (dialog.ShowDialog() == true)
            {
                GameLocation.Text = dialog.FileName;
                GameLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
    }

    public class TitleConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value[0].ToString();
            var lang = value[1].ToString();

            if (!File.Exists(path) || !Randomizer.DetectVersion(path)) return "No Game Loaded!";
            if (Globals.Remastered) return "Final Fantasy VIII Remastered (" + lang.ToUpper() + " 2019)";
            return "Final Fantasy VIII (" + lang.ToUpper() + " 2013)";
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
