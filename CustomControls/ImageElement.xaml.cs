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
    public partial class ImageElement : Grid
    {
        public ImageElement()
        {
            InitializeComponent();
            Clear("");
        }


        public void Clear(string defaultimage)
        {
            
            cb_tile.IsChecked = false;
            btn_color.Foreground = SomeUtilities.GetHexFromBrush("#000000FF");
            tb_path.Text = defaultimage;
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {

            get
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(btn_color.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_color.Foreground));
                _properties.Add(tb_path.Name.Replace("tb_", ""), tb_path.Text.ToString());
                if (cb_tile.IsChecked == true)
                    _properties.Add(cb_tile.Name.Replace("cb_", ""), "1");
                else
                    _properties.Add(cb_tile.Name.Replace("cb_", ""), "0");
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
                        btn_color.Foreground = SomeUtilities.GetHexFromBrush(val);
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
                ((Button)sender).Background = cpd.SelectedColor;
                Color myColor = ((System.Windows.Media.SolidColorBrush)(cpd.SelectedColor)).Color;
                myColor.B = 255;
                myColor.G = 255;
                myColor.R = 255;
                System.Windows.Media.SolidColorBrush scb = new SolidColorBrush(myColor);

                //string theHexColor = "#" + myColor.R.ToString("X2") + myColor.G.ToString("X2") + myColor.B.ToString("X2") + myColor.A.ToString("X2");

            }
        }

        private void btn_path_Click(object sender, RoutedEventArgs e)
        {
            Es_theme_editor mainWindow = (Es_theme_editor)Application.Current.MainWindow;
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = mainWindow.tb_themefolder.Text + mainWindow.cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(mainWindow.grid1, toollName);
            string filename = SomeUtilities.openFileDialog("Image files(*.png;*.jpg;*.svg)|*.png;*.jpg;*.svg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
        }
    }
}
