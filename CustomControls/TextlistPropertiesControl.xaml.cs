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
    public partial class TextlistPropertiesControl : Grid
    {
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyElementChanget;
        //На случай если мы не захотим чтобы программа создавала событие при изменении полей
        private bool manualClear = false;
        public TextlistPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            manualClear = true;
            cbx_alignment.SelectedValue = "center";
            tb_fontPath.Text = "./../SomeArt/font.ttf";
            tb_scrollSound.Text = "./../SomeArt/scrollsound.ogg";
            tb_fontSize.Text = "0.03";
            cb_forceUppercase.IsChecked = false;
            btn_secondaryColor.Foreground = SomeUtilities.GetBrushFromHex("#FF777777");
            btn_selectedColor.Foreground = SomeUtilities.GetBrushFromHex("#FFe2ead9");
            btn_selectorColor.Foreground = SomeUtilities.GetBrushFromHex("#77ffa500");
            btn_primaryColor.Foreground = SomeUtilities.GetBrushFromHex("#FF440040");
            tb_lineSpacing.Text = "1.5";
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
                _properties.Add(btn_secondaryColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_secondaryColor.Foreground));
                _properties.Add(btn_selectedColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_selectedColor.Foreground));
                _properties.Add(btn_selectorColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_selectorColor.Foreground));
                _properties.Add(btn_primaryColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_primaryColor.Foreground));
                _properties.Add(tb_fontPath.Name.Replace("tb_", ""), tb_fontPath.Text.ToString());
                _properties.Add(tb_fontSize.Name.Replace("tb_", ""), tb_fontSize.Text.ToString());
                _properties.Add(tb_lineSpacing.Name.Replace("tb_", ""), tb_lineSpacing.Text.ToString());
                _properties.Add(tb_scrollSound.Name.Replace("tb_", ""), tb_scrollSound.Text.ToString());
                if (cbx_alignment.SelectedValue!=null)
                _properties.Add(cbx_alignment.Name.Replace("cbx_", ""), cbx_alignment.SelectedValue.ToString());
                if (cb_forceUppercase.IsChecked == true)
                    _properties.Add(cb_forceUppercase.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_forceUppercase.Name.Replace("cb_", ""), "0");

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
                    val = value.FirstOrDefault(x => x.Key == btn_secondaryColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_secondaryColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == btn_selectedColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_selectedColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == btn_selectorColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_selectorColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == btn_primaryColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_primaryColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == tb_fontPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_fontSize.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontSize.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_lineSpacing.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_lineSpacing.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_scrollSound.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_scrollSound.Text = val;
                    val = value.FirstOrDefault(x => x.Key == cbx_alignment.Name.Replace("cbx_", "")).Value;
                    if (val != null)
                    {
                        cbx_alignment.SelectedValue = val;
                    }
                        //cbx_alignment.SelectedValue = val;
                    val = value.FirstOrDefault(x => x.Key == cb_forceUppercase.Name.Replace("cb_", "")).Value;
                    if (val != null)
                        if (val == "1")
                            cb_forceUppercase.IsChecked = true;
                        else
                            cb_forceUppercase.IsChecked = false;
                }
            }
        }


        private void textColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            cpd.ColorPicker.SelectedColor = ((System.Windows.Media.SolidColorBrush)(((Button)sender).Foreground)).Color;
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Foreground = cpd.SelectedColor;
                onPropertyChanged(((Button)sender).Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(((Button)sender).Foreground));
            }
        }

        private void btn_fontPath_Click(object sender, RoutedEventArgs e)
        {
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(this, toollName);
            string filename = SomeUtilities.openFileDialog("Font files(*.ttf)|*.ttf" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
            {
                foundTextBox.Text = filename;
            }
        }

        private void btn_scrollSound_Click(object sender, RoutedEventArgs e)
        {
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = ((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(this, toollName);
            string filename = SomeUtilities.openFileDialog("Sound files(*.wav;*.ogg)|*.wav;*.ogg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
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

            if (cbx_alignment.SelectedValue != null)
            {
                string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;
                onPropertyChanged("alignment", text);
            }
        }

        private void cb_forceUppercase_Checked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged("forceUppercase", "1");
        }

        private void cb_forceUppercase_Unchecked(object sender, RoutedEventArgs e)
        {
            onPropertyChanged("forceUppercase", "0");
        }
    }
}
