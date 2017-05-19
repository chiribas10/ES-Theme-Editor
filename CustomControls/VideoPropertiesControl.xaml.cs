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

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for RectangleNamed.xaml
    /// </summary>
   public partial class VideoPropertiesControl : Grid
    {
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyElementChanget;
        //На случай если мы не захотим чтобы программа создавала событие при изменении полей
        private bool manualClear = false;
       public VideoPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            manualClear = true;
            cb_showSnapshotDelay.IsChecked = true;
            cb_showSnapshotNoVideo.IsChecked = true;
            tb_default.Text = "./../SomeArt/default.mp4";
            tb_delay.Text = "0";
            manualClear = false;
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {

            get
            {
                SortedList<string, string> _properties = new SortedList<string, string>();
                double Width, Height;
                if (double.TryParse(tb_pos_w.Text, out Width) && double.TryParse(tb_pos_h.Text, out Height))
                    _properties.Add("pos", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
                if (double.TryParse(tb_size_w.Text, out Width) && double.TryParse(tb_size_h.Text, out Height))
                    _properties.Add("size", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
                if (double.TryParse(tb_origin_w.Text, out Width) && double.TryParse(tb_origin_h.Text, out Height))
                    _properties.Add("origin", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
                if (double.TryParse(tb_maxSize_w.Text, out Width) && double.TryParse(tb_maxSize_h.Text, out Height))
                    _properties.Add("maxSize", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
                if (cb_showSnapshotDelay.IsChecked == true)
                    _properties.Add(cb_showSnapshotDelay.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_showSnapshotDelay.Name.Replace("cb_", ""), "0");
                if (cb_showSnapshotNoVideo.IsChecked == true)
                    _properties.Add(cb_showSnapshotNoVideo.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_showSnapshotNoVideo.Name.Replace("cb_", ""), "0");
                _properties.Add(tb_default.Name.Replace("tb_", ""), tb_default.Text.ToString());
                _properties.Add(tb_delay.Name.Replace("tb_", ""), tb_delay.Text.ToString());
                return _properties;
            }
            set
            {
                string val;
                Char delimiter = ' ';
                String[] substrings;
                if (value.Count > 0)
                {
                    //Clear("");

                    val = value.FirstOrDefault(x => x.Key == "pos").Value;
                    if (val != null)
                    {
                        substrings = val.Split(delimiter);
                        tb_pos_w.Text = (double.Parse(substrings[0].Trim().Replace(".", ",")) * ((App)Application.Current).Width).ToString();
                        tb_pos_h.Text = (double.Parse(substrings[1].Trim().Replace(".", ",")) * ((App)Application.Current).Height).ToString();
                    }
                    val = value.FirstOrDefault(x => x.Key == "size").Value;
                    if (val != null)
                    {
                        substrings = val.Split(delimiter);
                        tb_size_w.Text = (double.Parse(substrings[0].Trim().Replace(".", ",")) * ((App)Application.Current).Width).ToString();
                        tb_size_h.Text = (double.Parse(substrings[1].Trim().Replace(".", ",")) * ((App)Application.Current).Height).ToString();
                    }
                    val = value.FirstOrDefault(x => x.Key == "origin").Value;
                    if (val != null)
                    {
                        substrings = val.Split(delimiter);
                        tb_origin_w.Text = (double.Parse(substrings[0].Trim().Replace(".", ","))).ToString();
                        tb_origin_h.Text = (double.Parse(substrings[1].Trim().Replace(".", ","))).ToString();
                    }
                    else
                    {
                        tb_origin_w.Text = "0";
                        tb_origin_h.Text = "0";
                    }
                    val = value.FirstOrDefault(x => x.Key == "maxSize").Value;
                    if (val != null)
                    {
                        substrings = val.Split(delimiter);
                        tb_maxSize_w.Text = (double.Parse(substrings[0].Trim().Replace(".", ",")) * ((App)Application.Current).Width).ToString();
                        tb_maxSize_h.Text = (double.Parse(substrings[1].Trim().Replace(".", ",")) * ((App)Application.Current).Height).ToString();
                    }
                    val = value.FirstOrDefault(x => x.Key == cb_showSnapshotDelay.Name.Replace("cb_", "")).Value;
                    if (val != null)
                        if (val == "1")
                            cb_showSnapshotDelay.IsChecked = true;
                        else
                            cb_showSnapshotDelay.IsChecked = false;
                    val = value.FirstOrDefault(x => x.Key == cb_showSnapshotNoVideo.Name.Replace("cb_", "")).Value;
                    if (val != null)
                        if (val == "1")
                            cb_showSnapshotNoVideo.IsChecked = true;
                        else
                            cb_showSnapshotNoVideo.IsChecked = false;
                    val = value.FirstOrDefault(x => x.Key == tb_default.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_default.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_delay.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_delay.Text = val;
                }
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
                manualClear = true;
                foundTextBox.Text = filename;
                manualClear = false;
                onPropertyChanged(((Button)sender).Name.Replace("btn_", ""), filename);
            }
        }

        private void cb_Checked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged(((CheckBox)sender).Name.Replace("cb_", ""), "1");
        }

        private void cb_Unchecked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged(((CheckBox)sender).Name.Replace("cb_", ""), "0");
        }

        private void tb_TextChanget(object sender, TextChangedEventArgs e)
        {
            onPropertyChanged(((TextBox)sender).Name.Replace("tb_", ""), ((TextBox)sender).Text);
        }

        private void onPropertyChanged(string propertyName, string propertyValue)
        {
            if (!manualClear)
                onPropertyElementChanget(propertyName, propertyValue);
        }
    }
}
