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
    public partial class RatingElement : Grid
    {

        public RatingElement()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            tb_filledPath.Text = "./../SomeArt/rating_filled.svg";
            tb_unfilledPath.Text = "./../SomeArt/rating_unfilled.svg";
            tb_pos_h.Text = "";
            tb_pos_w.Text = "";
            tb_size_h.Text = "";
            tb_size_w.Text = "";
        }

        public SortedList<string, string> Properties
        {
            
            get 
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(tb_filledPath.Name.Replace("tb_", ""), this.tb_filledPath.Text.ToString());
                _properties.Add(tb_unfilledPath.Name.Replace("tb_", ""), this.tb_unfilledPath.Text.ToString());
                return _properties; 
            }
            set
            {
                string val;
                if (value.Count > 0)
                {
                    Clear();

                    val = value.FirstOrDefault(x => x.Key == tb_filledPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_filledPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_unfilledPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_unfilledPath.Text = val;
                }
            }
        }

        //private void textColor_Click(object sender, RoutedEventArgs e)
        //{
        //    ColorPickerDialog cpd = new ColorPickerDialog();
        //    if (cpd.ShowDialog() == true)
        //    {
        //        ((Button)sender).Foreground = cpd.SelectedColor;
        //    }
        //}

        private void btn_ImagePath_Click(object sender, RoutedEventArgs e)
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
