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
using System.Drawing.Text;

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for DatetimeElement.xaml
    /// </summary>
    public partial class TextControl : Grid
    {
        string color = "#FF440040";
        public string somestring;
        bool forceUppercase = false;
        public TextControl()
        {
            InitializeComponent();
            fill(forceUppercase);
        }

        private void fill(bool forceUppercase = false)
        {
            if (forceUppercase)
                date.Text = somestring.ToUpper();
            else
                date.Text = somestring;
        }

        public void setProperty(string name, string value)
        {
            if (name == "")
                return;
            switch (name)
            {
                case "text":
                    if (value != "")
                        somestring = value;
                    fill(forceUppercase);
                    break;
                case "alignment":
                    if (value.ToLower() == "center")
                        date.TextAlignment = TextAlignment.Center;
                    if (value.ToLower() == "right")
                        date.TextAlignment = TextAlignment.Right;
                    if (value.ToLower() == "left")
                        date.TextAlignment = TextAlignment.Left;
                    break;
                case "color":
                    if (color != value)
                    {
                        color = value;
                        date.Foreground = SomeUtilities.GetBrushFromHex(value);
                    }
                    break;
                case "fontSize":
                    double fontsize;
                    if (!double.TryParse(value.Replace(".", ","), out fontsize))
                        return;
                    if (fontsize > 0)
                    {
                        fontsize = ((App)Application.Current).Height * fontsize;
                        date.FontSize = fontsize;
                        this.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                        this.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "fontPath":
                    value = SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", value);
                    if (File.Exists(value))
                    {
                        PrivateFontCollection fileFonts = new PrivateFontCollection();
                        fileFonts.AddFontFile(value);
                        value = value.Replace(System.IO.Path.GetFileName(value), "#" + fileFonts.Families[0].Name).Replace("\\", "/");
                        date.FontFamily = new FontFamily("file:///" + value);
                        this.Visibility = System.Windows.Visibility.Visible;
                        date.UpdateLayout();
                    }
                    else
                        this.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "forceUppercase":
                    if (value == "1")
                        forceUppercase = true;
                    if (value == "0")
                        forceUppercase = false;
                    fill(forceUppercase);
                    break;
            }
            fill(forceUppercase);
        }
    }
}