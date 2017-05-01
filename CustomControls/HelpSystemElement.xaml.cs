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
    public partial class HelpSystemElement : Grid
    {

        public HelpSystemElement()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            tb_fontPath.Text = "./../SomeArt/font.ttf";
            tb_fontSize.Text = "0.003";
            btn_iconColor.Foreground = SomeUtilities.GetBrushFromHex("#777777FF");
            btn_textColor.Foreground = SomeUtilities.GetBrushFromHex("#777777FF");
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {
            
            get 
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(btn_iconColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_iconColor.Foreground));
                _properties.Add(btn_textColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_textColor.Foreground));
                _properties.Add(tb_fontPath.Name.Replace("tb_", ""), tb_fontPath.Text.ToString());
                _properties.Add(tb_fontSize.Name.Replace("tb_", ""), tb_fontSize.Text.ToString());
                
                return _properties; 
            }
            set
            {
                string val;
                if (value.Count>0)
                {
                    Clear();
                    val = value.FirstOrDefault(x => x.Key == btn_iconColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_iconColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == btn_textColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_textColor.Foreground = SomeUtilities.GetBrushFromHex(val);
                    val = value.FirstOrDefault(x => x.Key == tb_fontPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_fontSize.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontSize.Text = val;
                }
            }
        }

        private void textColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog();
            if (cpd.ShowDialog() == true)
            {
                ((Button)sender).Foreground = cpd.SelectedColor;
                //((Button)sender).Content = SomeUtilities.GetHexFromBrush(cpd.SelectedColor);
            }
        }

        private void btn_fontPath_Click(object sender, RoutedEventArgs e)
        {
            Es_theme_editor mainWindow = (Es_theme_editor)Application.Current.MainWindow;
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = mainWindow.tb_themefolder.Text + mainWindow.cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(mainWindow.grid1, toollName);
            string filename = SomeUtilities.openFileDialog("Font files(*.ttf)|*.ttf" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
        }
    }
}
