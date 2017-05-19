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
    public partial class RatingPropertiesControl : Grid
    {
        public delegate void PropertyElementChanget(string propertyName, string propertyValue);
        public event PropertyElementChanget onPropertyElementChanget = delegate { };
        //На случай если мы не захотим чтобы программа создавала событие при изменении полей
        private bool manualClear = false;

        public RatingPropertiesControl()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            manualClear = true;
            tb_filledPath.Text = "./../SomeArt/rating_filled.png";
            tb_unfilledPath.Text = "./../SomeArt/rating_unfilled.png";
            tb_pos_h.Text = "";
            tb_pos_w.Text = "";
            tb_size_h.Text = "";
            tb_size_w.Text = "";
            manualClear = false;
        }

        public void setPosition(double x, double y)
        {
            tb_pos_h.Text = y.ToString();
            tb_pos_w.Text = x.ToString();
        }

        public void setSize(double width, double height)
        {
            tb_size_h.Text = height.ToString();
            tb_size_w.Text = width.ToString();
        }

        public string getImagePath()
        {
            if (tb_filledPath.Text == "")
                return "";
            return SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", tb_filledPath.Text);
        }

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
                _properties.Add(tb_filledPath.Name.Replace("tb_", ""), this.tb_filledPath.Text.ToString());
                _properties.Add(tb_unfilledPath.Name.Replace("tb_", ""), this.tb_unfilledPath.Text.ToString());
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
                    val = value.FirstOrDefault(x => x.Key == tb_filledPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_filledPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_unfilledPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_unfilledPath.Text = val;
                }
            }
        }

        private void btn_ImagePath_Click(object sender, RoutedEventArgs e)
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
                onPropertyChanged(foundTextBox.Name.Replace("tb_", ""), foundTextBox.Text);
                
            }
        }

        private void tb_TextChanged(object sender, TextChangedEventArgs e)
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
