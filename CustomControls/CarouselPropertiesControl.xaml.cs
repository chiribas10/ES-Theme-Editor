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
    public partial class CarouselPropertiesControl : Grid
    {
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyElementChanget;
        //На случай если мы не захотим чтобы программа создавала событие при изменении полей
        private bool manualClear = false;
        public CarouselPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            manualClear = true;
            cbx_type.SelectedValue = "horizontal";
            tb_logoScale.Text = "1.2";
            btn_color.Foreground = SomeUtilities.GetBrushFromHex("#FF777777");
            tb_maxLogoCount.Text = "5";
            tb_logoSize_w.Text = "0.125";
            tb_logoSize_h.Text = "0.155";
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
                if (double.TryParse(tb_logoSize_w.Text, out Width) && double.TryParse(tb_logoSize_h.Text, out Height))
                    _properties.Add("logoSize", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
                _properties.Add(btn_color.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_color.Background));
                _properties.Add(tb_logoScale.Name.Replace("tb_", ""), tb_logoScale.Text.ToString());
                _properties.Add(tb_maxLogoCount.Name.Replace("tb_", ""), tb_maxLogoCount.Text.ToString());
                if (cbx_type.SelectedValue != null)
                    _properties.Add(cbx_type.Name.Replace("cbx_", ""), cbx_type.SelectedValue.ToString());

                return _properties;
            }
            set
            {
                string val;
                Char delimiter = ' ';
                String[] substrings;
                if (value.Count > 0)
                {
                    Clear();

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
                    val = value.FirstOrDefault(x => x.Key == "logoSize").Value;
                    if (val != null)
                    {
                        substrings = val.Split(delimiter);
                        tb_logoSize_w.Text = (double.Parse(substrings[0].Trim().Replace(".", ",")) * ((App)Application.Current).Width).ToString();
                        tb_logoSize_h.Text = (double.Parse(substrings[1].Trim().Replace(".", ",")) * ((App)Application.Current).Height).ToString();
                    }
                    val = value.FirstOrDefault(x => x.Key == btn_color.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_color.Background = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == tb_maxLogoCount.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_maxLogoCount.Text = val;
                    val = value.FirstOrDefault(x => x.Key == cbx_type.Name.Replace("cbx_", "")).Value;
                    if (val != null)
                    {
                        cbx_type.SelectedValue = val;
                    }
                }
            }
        }

        private void textColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            cpd.ColorPicker.SelectedColor = ((System.Windows.Media.SolidColorBrush)(((Button)sender).Foreground)).Color;
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Background = cpd.SelectedColor;
                onPropertyChanged(((Button)sender).Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(((Button)sender).Background));
            }
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            onPropertyChanged(((TextBox)sender).Name.Replace("tb_", ""), ((TextBox)sender).Text);
        }

        private void onPropertyChanged(string propertyName, string propertyValue)
        {
            if (!manualClear)
                try
                {
                    onPropertyElementChanget(propertyName, propertyValue);
                }
                catch (Exception) { }
        }

        private void cbx_alignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbx_type.SelectedValue != null)
            {
                string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
                onPropertyChanged("type", text);
            }
        }

        private void tb_logoSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            double Width, Height;
            if (double.TryParse(tb_logoSize_w.Text, out Width) && double.TryParse(tb_logoSize_h.Text, out Height))
                onPropertyChanged("logoSize", (Width / ((App)Application.Current).Width).ToString() + " " + (Height / ((App)Application.Current).Height).ToString());
        }
    }
}
