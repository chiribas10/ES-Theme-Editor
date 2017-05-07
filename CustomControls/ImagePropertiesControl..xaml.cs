﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for RectangleNamed.xaml
    /// </summary>
    public partial class ImagePropertiesControl : Grid
    {
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyElementChanget;
        //На случай если мы не захотим чтобы программа создавала событие при изменении полей
        private bool manualClear = false;

        public ImagePropertiesControl()
        {
            InitializeComponent();
            Clear("");
        }

        public void setPosition(double x, double y)
        {
            tb_pos_h.Text = y.ToString();
            tb_pos_w.Text = x.ToString();
        }

        public void setSize(double width, double height)
        {
            tb_size_h.Text = height.ToString();
            tb_size_w.Text = width.ToString();
        }

        public void setMaxSize(double width, double height)
        {
            tb_maxSize_h.Text = height.ToString();
            tb_maxSize_w.Text = width.ToString();
        }

        public void setOrigin(double x, double y)
        {
            tb_origin_h.Text = y.ToString();
            tb_origin_w.Text = x.ToString();
        }

        //public string getImagePath()
        //{
        //    if (tb_path.Text == "")
        //        return "";
        //    return SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", tb_path.Text);
        //}

        public void Clear(string defaultimage)
        {
            manualClear = true;
            cb_tile.IsChecked = false;
            btn_color.Background = SomeUtilities.GetBrushFromHex("#FFFFFFFF");
            tb_path.Text = defaultimage;
            manualClear = false;
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {
            get
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(btn_color.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_color.Background));
                _properties.Add(tb_path.Name.Replace("tb_", ""), tb_path.Text.ToString());
                if (cb_tile.IsChecked == true)
                    _properties.Add(cb_tile.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_tile.Name.Replace("cb_", ""), "0");
                //_properties.Add(btn_color.Name.Replace("btn_", ""), SomeUtilities.getOpacityFromBrush(btn_color.Background).ToString());
                return _properties;
            }
            set
            {
                string val;
                if (value.Count > 0)
                {
                    Clear("");

                    val = value.FirstOrDefault(x => x.Key == btn_color.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_color.Background = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == tb_path.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_path.Text = val;
                    val = value.FirstOrDefault(x => x.Key == cb_tile.Name.Replace("cb_", "")).Value;
                    if (val != null)
                        if (val == "1")
                            cb_tile.IsChecked = true;
                        else
                            cb_tile.IsChecked = false;
                }
            }
        }

        //For element image, the color property sets only the alpha channel, and the other parameters 255
        private void btn_image_color_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            if (cpd.ShowDialog() == true)
            {
                //((Button)sender).Background = cpd.SelectedColor;
                Color myColor = ((System.Windows.Media.SolidColorBrush)(cpd.SelectedColor)).Color;
                myColor.B = 255;
                myColor.G = 255;
                myColor.R = 255;
                System.Windows.Media.SolidColorBrush scb = new SolidColorBrush(myColor);
                ((Button)sender).Background = scb;
                //string theHexColor = "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2") + myColor.A.ToString("X2");
                onPropertyChanged(((Button)sender).Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(((Button)sender).Background));
            }
        }

        private void btn_path_Click(object sender, RoutedEventArgs e)
        {
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(this, toollName);
            string filename = SomeUtilities.openFileDialog("Image files(*.png;*.jpg;*.svg)|*.png;*.jpg;*.svg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
            {
                foundTextBox.Text = filename;
                onPropertyChanged(((Button)sender).Name.Replace("btn_", ""), filename);
            }
        }

        private void cb_tile_Checked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged("tile", "1");
        }

        private void cb_tile_Unchecked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged("tile", "0");
        }

        private void tb_path_TargetUpdated(object sender, TextChangedEventArgs e)
        {
            //string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            onPropertyChanged(((TextBox)sender).Name.Replace("tb_", ""), ((TextBox)sender).Text);
        }
        private void onPropertyChanged(string propertyName, string propertyValue) 
        {
            if (!manualClear)
                onPropertyElementChanget(propertyName, propertyValue);
        }
    }
}
