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
    public partial class TextListControl : Grid
    {
        string primaryColor = "#FF440040";
        string selectedColor = "#e2ead9";
        string selectorColor = "#ffa500";
        string alignment = "Left";//Center, Right//"Stretch";

        public TextListControl()
        {
            InitializeComponent();
            fillCBX();

            setStyle();
        }

        private void fillCBX(bool forceUppercase = false)
        {
            string somenameofsomegame = "Some name of Some game №";
            
            if (forceUppercase)
                somenameofsomegame = somenameofsomegame.ToUpper();
            lb_gamelist.Items.Clear();
            for (int i = 0; i < 100; i++)
            {
                lb_gamelist.Items.Add(somenameofsomegame + (i + 1).ToString());
            }
            ScrollToLastItem();
        }

        public void ScrollToLastItem()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, lb_gamelist.Items.Count);
            lb_gamelist.SelectedIndex = randomNumber;
            lb_gamelist.SelectedItem = lb_gamelist.Items.GetItemAt(randomNumber);
            lb_gamelist.ScrollIntoView(lb_gamelist.SelectedItem);
            ListViewItem item = lb_gamelist.ItemContainerGenerator.ContainerFromItem(lb_gamelist.SelectedItem) as ListViewItem;
            lb_gamelist.ScrollIntoView(lb_gamelist.SelectedItem);
        }

        public void setProperty(string name, string value)
        {
            if (name == "")
                return;
            switch (name)
            {
                case "alignment":
                    if (value.ToLower() == "center")
                        alignment = "Center";
                    if (value.ToLower() == "right")
                        alignment = "Right";
                    if (value.ToLower() == "left")
                        alignment = "Left";
                    setStyle();
                    break;
                case "primaryColor":
                    if (primaryColor != value)
                    {
                        primaryColor = value;
                        setStyle();
                    }
                    break;
                case "selectedColor":
                    if (selectedColor != value)
                    {
                        selectedColor = value;
                        setStyle();
                    }
                    break;
                case "selectorColor":
                    if (selectorColor != value)
                    {
                        selectorColor = value;
                        setStyle();
                    }
                    break;
                case "fontSize":
                    double fontsize;
                    if (!double.TryParse(value.Replace(".", ","), out fontsize))
                        return;
                    if (fontsize > 0)
                    {
                        fontsize = ((App)Application.Current).Height * fontsize;
                        lb_gamelist.FontSize = fontsize;
                        ScrollToLastItem();
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
                        lb_gamelist.FontFamily = new FontFamily("file:///" + value);
                        this.Visibility = System.Windows.Visibility.Visible;
                        lb_gamelist.UpdateLayout();
                    }
                    else
                        this.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case "forceUppercase":
                    if (value == "1")
                        fillCBX(true);
                    if (value == "0")
                        fillCBX();
                    break;
            }
        }

        private void setStyle() 
        {
            //Проверяем чтобы строка цвета была норм
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(selectorColor);
            }
            catch (Exception)
            {
                selectorColor = "#f0f8ff";
            }
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(selectedColor);
            }
            catch (Exception)
            {
                selectedColor = "#e2ead9";
            }

            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(primaryColor);
            }
            catch (Exception)
            {
                primaryColor = "#440040";
            }

            if (alignment == "")
                alignment = "Stretch";

            Style style = new Style();

            string xaml = @"<Style TargetType=""ListBoxItem""  xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                <Setter Property=""Template"">
                                    <Setter.Value>
                                        <ControlTemplate TargetType=""ListBoxItem"">
                                            <Border Name=""Bd""
                                                BorderBrush=""{TemplateBinding BorderBrush}""
                                                BorderThickness=""{TemplateBinding BorderThickness}""
                                                Background=""{TemplateBinding Background}""
                                                Padding=""{TemplateBinding Padding}""
                                                SnapsToDevicePixels=""true"">
                                                <ContentPresenter HorizontalAlignment=""" + alignment + @"""
                                                    SnapsToDevicePixels=""{TemplateBinding SnapsToDevicePixels}""
                                                    VerticalAlignment=""{TemplateBinding VerticalContentAlignment}"" />
                                            </Border>
                                            <ControlTemplate.Triggers>
                                                <MultiTrigger>
                                                    <MultiTrigger.Conditions>
                                                        <Condition Property=""Selector.IsSelectionActive""
                                                                    Value=""False"" />
                                                        <Condition Property=""IsSelected""
                                                                    Value=""True"" />
                                                    </MultiTrigger.Conditions>
                                                    <Setter Property=""Background""
                                                            TargetName=""Bd""
                                                            Value=""" + selectorColor + @""" />
                                                </MultiTrigger>
                                                <MultiTrigger>
                                                    <MultiTrigger.Conditions>
                                                        <Condition Property=""Selector.IsSelectionActive""
                                                                    Value=""True"" />
                                                        <Condition Property=""IsSelected""
                                                                    Value=""True"" />
                                                    </MultiTrigger.Conditions>
                                                    <Setter Property=""Background""
                                                            TargetName=""Bd""
                                                            Value=""" + selectorColor + @""" />
                                                </MultiTrigger>
                                                <Trigger Property=""IsSelected"" Value=""true"">
                                                    <Setter Property=""Foreground"" Value=""" + selectedColor + @""" />
                                                </Trigger>
                                                <Trigger Property=""IsSelected"" Value=""false"">
                                                    <Setter Property=""Foreground"" Value=""" + primaryColor + @""" />
                                                </Trigger>
                                            </ControlTemplate.Triggers>

                                        </ControlTemplate>
                                    </Setter.Value>
                                </Setter>
                            </Style>";
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            lb_gamelist.ItemContainerStyle = XamlReader.Load(xmlReader) as Style;
        }
    }
}