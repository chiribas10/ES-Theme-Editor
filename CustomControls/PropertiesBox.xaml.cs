using System;
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
    public partial class PropertiesBox : Canvas
    {
        public delegate void SystemElementChanget(View view);
        //public event SystemElementChanget onSystemElementChanget;
        //string currentElementName;
        //SortedList<string, string> currentElementPropertie;
        //Element.types currentElementType;

        public PropertiesBox()
        {
            InitializeComponent();
            saveSystemView();
        }

        private void saveSystemView() 
        {
            ((App)Application.Current).backgroundImagePath = tb_background_system.Text;
            ((App)Application.Current).backgroundSoundPath = tb_bgsound_system.Text;
            ((App)Application.Current).backgroundSystemImagePath = tb_background_system_for_window_system.Text;
            ((App)Application.Current).logoPath = tb_logo_system.Text;
            ((App)Application.Current).helpIconColor = btn_iconColor_system.Background;
            ((App)Application.Current).helpTextColor = btn_textColor_system.Background;

        }

        private void setSystemView()
        {
            if (!string.IsNullOrEmpty(((App)Application.Current).backgroundImagePath))
                tb_background_system.Text = ((App)Application.Current).backgroundImagePath;
            if (!string.IsNullOrEmpty(((App)Application.Current).backgroundSoundPath))
                tb_bgsound_system.Text = ((App)Application.Current).backgroundSoundPath;
            if (!string.IsNullOrEmpty(((App)Application.Current).backgroundSystemImagePath))
                tb_background_system_for_window_system.Text = ((App)Application.Current).backgroundSystemImagePath;
            if (!string.IsNullOrEmpty(((App)Application.Current).logoPath))
                tb_logo_system.Text = ((App)Application.Current).logoPath;
            if (((App)Application.Current).helpIconColor != null)
                btn_iconColor_system.Background = ((App)Application.Current).helpIconColor;
            if (((App)Application.Current).helpTextColor != null)
                btn_textColor_system.Background = ((App)Application.Current).helpTextColor;
        }

        private void btn_bgsound_system_Click(object sender, RoutedEventArgs e)
        {
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            string filename = SomeUtilities.openFileDialog("Sound files(*.wav;*.ogg)|*.wav;*.ogg" + "|Все файлы (*.*)|*.* ", this.tb_bgsound_system.Text, themeFilename);
            if (filename != null)
                this.tb_bgsound_system.Text = filename;
            saveSystemView();
        }

        private void btn_getimagefile_Click(object sender, RoutedEventArgs e)
        {
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(canvas, toollName);
            string filename = SomeUtilities.openFileDialog("Image files(*.png;*.jpg;*.svg)|*.png;*.jpg;*.svg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (!string.IsNullOrEmpty(filename))
                foundTextBox.Text = filename;
            saveSystemView();
        }

        private void btn_image_color_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Background = cpd.SelectedColor;
                Color myColor = ((System.Windows.Media.SolidColorBrush)(cpd.SelectedColor)).Color;
                myColor.B = 255;
                myColor.G = 255;
                myColor.R = 255;
                System.Windows.Media.SolidColorBrush scb = new SolidColorBrush(myColor);
            }
            saveSystemView();
        }

        //public void setElementProperties(string elementName, SortedList<string, string> Properties, Element.types typeOfElement) 
        //{
        //    //сначала сохраняем свойства для выбранного перед этим элемента
        //}
    }
}
