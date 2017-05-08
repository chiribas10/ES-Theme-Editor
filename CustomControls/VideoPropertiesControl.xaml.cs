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
        public VideoPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            //cb_tile.IsChecked = true;
            //btn_color.Foreground = SomeUtilities.GetHexFromBrush("#000000FF");

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
                }
            }
        }

        //For element image, the color property sets only the alpha channel, and the other parameters 255
        //private void btn_image_color_Click(object sender, RoutedEventArgs e)
        //{
        //    ColorPickerDialog cpd = new ColorPickerDialog();
        //    if (cpd.ShowDialog() == true)
        //    {
        //        ((Button)sender).Background = cpd.SelectedColor;
        //        Color myColor = ((System.Windows.Media.SolidColorBrush)(cpd.SelectedColor)).Color;
        //        myColor.B = 255;
        //        myColor.G = 255;
        //        myColor.R = 255;
        //        System.Windows.Media.SolidColorBrush scb = new SolidColorBrush(myColor);

        //        //string theHexColor = "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2") + myColor.A.ToString("X2");

        //    }
        //}
    }
}
