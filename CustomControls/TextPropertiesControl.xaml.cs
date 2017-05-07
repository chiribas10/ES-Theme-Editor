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
    public partial class TextPropertiesControl : Grid
    {
        public TextPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            tb_fontPath.Text = "./../SomeArt/font.ttf";
            tb_fontSize.Text = "0.03";
            cb_forceUppercase.IsChecked = false;
            btn_color.Foreground = SomeUtilities.GetBrushFromHex("#777777FF");
            tb_lineSpacing.Text = "1.5";
            tb_text.Text = "";
            cbx_alignment.SelectedValue = "center";
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {

            get
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(btn_color.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_color.Foreground));
                _properties.Add(tb_fontPath.Name.Replace("tb_", ""), tb_fontPath.Text.ToString());
                _properties.Add(tb_fontSize.Name.Replace("tb_", ""), tb_fontSize.Text.ToString());
                _properties.Add(tb_lineSpacing.Name.Replace("tb_", ""), tb_lineSpacing.Text.ToString());
                _properties.Add(tb_text.Name.Replace("tb_", ""), tb_text.Text.ToString());
                _properties.Add(this.cbx_alignment.Name.Replace("cbx_", ""), SomeUtilities.getComboBoxValue(cbx_alignment));
                if (cb_forceUppercase.IsChecked == true)
                    _properties.Add(cb_forceUppercase.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_forceUppercase.Name.Replace("cb_", ""), "0");

                return _properties;
            }
            set
            {
                string val;
                if (value.Count > 0)
                {
                    Clear();

                    val = value.FirstOrDefault(x => x.Key == btn_color.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_color.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == tb_fontPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_fontSize.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontSize.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_lineSpacing.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_lineSpacing.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_text.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_text.Text = val;
                    val = value.FirstOrDefault(x => x.Key == cbx_alignment.Name.Replace("cbx_", "")).Value;
                    if (val != null)
                        cbx_alignment.SelectedValue = val;
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
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Foreground = cpd.SelectedColor;
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
                foundTextBox.Text = filename;
        }
    }
}
