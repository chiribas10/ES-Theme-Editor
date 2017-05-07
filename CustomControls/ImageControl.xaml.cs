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
using System.Xml;
using System.Windows.Markup;

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for DatetimeElement.xaml
    /// </summary>
    public partial class ImageControl : Grid
    {

        public ImageControl()
        {
            InitializeComponent();
        }

        public void setProperty(string name, string value) 
        {
            if (name == "")
                return;

            switch (name)
            {
                case "path":
                    if (name == "path")
                    {
                        if (value != "")
                        value = SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", value);
                        if (File.Exists(value))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(value);
                            bitmap.EndInit();
                            image.Source = bitmap;
                            this.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                            this.Visibility = System.Windows.Visibility.Hidden;
                    }
                    break;
                case "color":
                    image.Opacity = SomeUtilities.getOpacityFromHex(value);
                    break;
            }
        }
    }
}
