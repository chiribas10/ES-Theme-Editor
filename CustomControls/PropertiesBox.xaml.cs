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
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyChanget;
        public delegate void PropertyViewChanget(string propertyName, string propertyValue);
        public event PropertyViewChanget onPropertyViewChanget;
        private bool manualClear = false;

        public PropertiesBox()
        {
            InitializeComponent();
            //saveSystemView();
            image.onPropertyElementChanget += onPropertyElementChanget;
            textlist.onPropertyElementChanget += onPropertyElementChanget;
            rating.onPropertyElementChanget += onPropertyElementChanget;
            datetime.onPropertyElementChanget += onPropertyElementChanget;
            text.onPropertyElementChanget += onPropertyElementChanget;
            carousel.onPropertyElementChanget += onPropertyElementChanget;
        }

        public bool isSystem
        {
            set 
            {
                if (value)
                {
                    lbl_bgsound.Visibility = System.Windows.Visibility.Visible;
                    tb_bgsound.Visibility = System.Windows.Visibility.Visible;
                    btn_bgsound.Visibility = System.Windows.Visibility.Visible;
                    lbl_logo.Visibility = System.Windows.Visibility.Visible;
                    tb_logo.Visibility = System.Windows.Visibility.Visible;
                    btn_logo.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }
        public void propertiesTabVisibilityChange(string gb_name) 
        {
            GroupBox foundGrid = SomeUtilities.FindChild<GroupBox>(this, "gb_" + gb_name);
            if (foundGrid != null)
                foundGrid.Visibility = System.Windows.Visibility.Visible;
            if (gb_name == "base")
                cb_extra_on_off.IsEnabled = false;
            else
                cb_extra_on_off.IsEnabled = true;
        }

        private void onPropertyElementChanget(string propertyName, string propertyValue) 
        {
            if (!manualClear)
                onPropertyChanget(propertyName, propertyValue);
        }

        public void setPosition(double x, double y, Element.types typeOfElement)
        {
            switch (typeOfElement)
            {
                case Element.types.text:
                    text.tb_pos_h.Text = y.ToString();
                    text.tb_pos_w.Text = x.ToString();
                    break;
                case Element.types.image:
                    image.setPosition(x, y);
                    break;
                case Element.types.video:
                    video.tb_pos_h.Text = y.ToString();
                    video.tb_pos_w.Text = x.ToString();
                    break;
                case Element.types.textlist:
                    textlist.tb_pos_h.Text = y.ToString();
                    textlist.tb_pos_w.Text = x.ToString();
                    break;
                case Element.types.rating:
                    image.setPosition(x, y);
                    break;
                case Element.types.datetime:
                    datetime.tb_pos_h.Text = y.ToString();
                    datetime.tb_pos_w.Text = x.ToString();
                    break;
                case Element.types.helpsystem:
                    helpsystem.tb_pos_h.Text = y.ToString();
                    helpsystem.tb_pos_w.Text = x.ToString();
                    break;
                case Element.types.carousel:
                    carousel.tb_pos_h.Text = y.ToString();
                    carousel.tb_pos_w.Text = x.ToString();
                    break;
            }
        }

        public void setSize(double width, double height, Element.types typeOfElement)
        {
            switch (typeOfElement)
            {
                case Element.types.text:
                    text.tb_size_w.Text = width.ToString();
                    text.tb_size_h.Text = height.ToString();
                    break;
                case Element.types.textlist:
                    textlist.tb_size_w.Text = width.ToString();
                    textlist.tb_size_h.Text = height.ToString();
                    break;
                case Element.types.rating:
                    rating.setSize(0, height);
                    break;
                case Element.types.carousel:
                    carousel.tb_size_w.Text = width.ToString();
                    carousel.tb_size_h.Text = height.ToString();
                    break;
            }
        }

        public void setMaxSize(double width, double height, Element.types typeOfElement)
        {
            switch (typeOfElement)
            {
                case Element.types.image:
                    image.setMaxSize(width, height);
                    break;
                case Element.types.video:
                    video.tb_maxSize_w.Text = width.ToString();
                    video.tb_maxSize_h.Text = height.ToString();
                    break;
            }
        }

        public void setOrigin(double x, double y, Element.types typeOfElement)
        {
            switch (typeOfElement)
            {
                case Element.types.image:
                    image.setOrigin(x, y);
                    break;
                case Element.types.video:
                    video.tb_origin_h.Text = y.ToString();
                    video.tb_origin_w.Text = x.ToString();
                    break;
            }
        }

        public SortedList<string, string> getPropertiesByType(Element.types typeOfElement) 
        {
            switch (typeOfElement)
            {
                case Element.types.text:
                    return text.Properties;
                case Element.types.image:
                    return image.Properties;
                case Element.types.textlist:
                    return textlist.Properties;
                case Element.types.rating:
                    return rating.Properties;
                case Element.types.datetime:
                    return datetime.Properties;
                case Element.types.helpsystem:
                    return helpsystem.Properties;
                case Element.types.carousel:
                    return carousel.Properties;
                case Element.types.video:
                    return video.Properties;
                default:
                    return null;
            }
        }

        public bool setPropertiesByType(Element.types typeOfElement, SortedList<string, string> Properties, string extra)
        {
            if (Properties == null)
                Properties = new SortedList<string, string>();
            if (Properties.Count <= 0)
                return false;
            switch (extra)
            {
                case "":
                    cb_extra_on_off.IsChecked = false;
                    break;
                case "true":
                    cb_extra_on_off.IsChecked = true;
                    cb_extra.IsChecked = true;
                    break;
                case "false":
                    cb_extra_on_off.IsChecked = true;
                    cb_extra.IsChecked = false;
                    break;
            }

            switch (typeOfElement)
            {
                case Element.types.text:
                    text.Properties = Properties;
                    return true;
                case Element.types.image:
                    image.Properties = Properties;
                    return true;
                case Element.types.textlist:
                    textlist.Properties = Properties;
                    return true;
                case Element.types.rating:
                    rating.Properties = Properties;
                    return true;
                case Element.types.datetime:
                    datetime.Properties = Properties;
                    return true;
                case Element.types.helpsystem:
                    helpsystem.Properties = Properties;
                    return true;
                case Element.types.carousel:
                    carousel.Properties = Properties;
                    return true;
                case Element.types.video:
                    video.Properties = Properties;
                    return true;
                default:
                    return false;
            }
        }

        public void Clear(Element.types typeOfElement)
        {
            switch (typeOfElement)
            {
                case Element.types.text:
                    text.Clear();
                    break;
                case Element.types.image:
                    image.Clear("");
                    break;
                case Element.types.textlist:
                    textlist.Clear();
                    break;
                case Element.types.rating:
                    rating.Clear();
                    break;
                case Element.types.datetime:
                    datetime.Clear();
                    break;
                case Element.types.helpsystem:
                    helpsystem.Clear();
                    break;
                case Element.types.carousel:
                    carousel.Clear();
                    break;
            }

            manualClear = true;
            cb_extra_on_off.IsChecked = false;
            //cb_extra.IsChecked = false;
            manualClear = false;
        }

        //private void saveSystemView() 
        //{
        //    ((App)Application.Current).backgroundImagePath = tb_background_system.Text;
        //    ((App)Application.Current).backgroundSoundPath = tb_bgsound_system.Text;
        //    ((App)Application.Current).backgroundSystemImagePath = tb_background_system_for_window_system.Text;
        //    ((App)Application.Current).logoPath = tb_logo_system.Text;
        //    ((App)Application.Current).helpIconColor = btn_iconColor_system.Background;
        //    ((App)Application.Current).helpTextColor = btn_textColor_system.Background;
        //}

        //private void setSystemView()
        //{
        //    if (!string.IsNullOrEmpty(((App)Application.Current).backgroundImagePath))
        //        tb_background_system.Text = ((App)Application.Current).backgroundImagePath;
        //    if (!string.IsNullOrEmpty(((App)Application.Current).backgroundSoundPath))
        //        tb_bgsound_system.Text = ((App)Application.Current).backgroundSoundPath;
        //    if (!string.IsNullOrEmpty(((App)Application.Current).backgroundSystemImagePath))
        //        tb_background_system_for_window_system.Text = ((App)Application.Current).backgroundSystemImagePath;
        //    if (!string.IsNullOrEmpty(((App)Application.Current).logoPath))
        //        tb_logo_system.Text = ((App)Application.Current).logoPath;
        //    if (((App)Application.Current).helpIconColor != null)
        //        btn_iconColor_system.Background = ((App)Application.Current).helpIconColor;
        //    if (((App)Application.Current).helpTextColor != null)
        //        btn_textColor_system.Background = ((App)Application.Current).helpTextColor;
        //}

        public void setViewProperties(string bgsound, string background, string logo)
        {
            tb_bgsound.Text = bgsound;
            tb_background.Text = background;
            tb_logo.Text = logo;
        }

        private void btn_bgsound_system_Click(object sender, RoutedEventArgs e)
        {
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            string filename = SomeUtilities.openFileDialog("Sound files(*.wav;*.ogg)|*.wav;*.ogg" + "|Все файлы (*.*)|*.* ", this.tb_bgsound.Text, themeFilename);
            if (filename != null)
                this.tb_bgsound.Text = filename;
            //saveSystemView();
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
            //saveSystemView();
        }

        private void btn_image_color_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            cpd.ColorPicker.SelectedColor = ((System.Windows.Media.SolidColorBrush)(((Button)sender).Background)).Color;
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Background = cpd.SelectedColor;
            }
            //saveSystemView();
        }

        private void cb_extra_Checked(object sender, RoutedEventArgs e)
        {
            onPropertyElementChanget("extra", "true");
        }

        private void cb_extra_Unchecked(object sender, RoutedEventArgs e)
        {
            onPropertyElementChanget("extra", "false");
        }

        private void cb_extra_on_off_Checked(object sender, RoutedEventArgs e)
        {
            cb_extra.IsEnabled = true;
            onPropertyElementChanget("extra", cb_extra.IsChecked.ToString().ToLower());
        }

        private void cb_extra_on_off_Unchecked(object sender, RoutedEventArgs e)
        {
            cb_extra.IsEnabled = false;
            cb_extra.IsChecked = false;
            onPropertyElementChanget("extra", "");
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                onPropertyViewChanget(((TextBox)sender).Name.Replace("tb_", ""), ((TextBox)sender).Text);
            }
            catch (Exception) { }            
        }
    }
}
