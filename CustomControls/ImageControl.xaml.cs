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

        public bool setImage(string filename) 
        {
            if (!File.Exists(filename))
            {
                filename = SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", filename);
                if (!File.Exists(filename))
                    return false;
            }

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename);
            bitmap.EndInit();
            image.Source = bitmap;
            if (bitmap != null)
                return true;

            return false;
        }

        public void setProperty(string name, string value) 
        {
            //if (name == "")
            //    return;

            switch (name)
            {
                case "path":
                case "filledPath"://сюда же будем показывать и рейтинг
                case "unfilledPath"://сюда же будем показывать и рейтинг
                    if (value != "")
                        value = SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", value);
                        if (setImage(value))
                        {
                            this.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                            this.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "color":
                    image.Opacity = SomeUtilities.getOpacityFromHex(value);
                    break;
                case "tile":
                    if (value == "1")
                        image.Stretch = Stretch.Fill;
                    if (value == "0")
                        image.Stretch = Stretch.Uniform;
                    break;
            }
        }
    }
}
