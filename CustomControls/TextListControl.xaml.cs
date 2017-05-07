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
        public TextListControl()
        {
            InitializeComponent();
            for (int i = 0; i < 120; i++)
                lb_gamelist.Items.Add("Some name of Some game №" + (i+1).ToString());

            setStyle("#ffa500", "Stretch");
        }

        public void setProperty(string name, string value)
        {
            if (name == "")
                return;

            switch (name)
            {
                case "fontSize":

                   double fontsize;
                   if (!double.TryParse(value.Replace(".", ","), out fontsize))
                       return;
                   if (fontsize > 0)
                   {
                       Random random = new Random();
                       int randomNumber = random.Next(1, lb_gamelist.Items.Count);
                        fontsize = ((App)Application.Current).Height * fontsize;
                        //lb_gamelist.SelectedItems.Clear();
                        lb_gamelist.FontSize = fontsize;
                        lb_gamelist.SelectedIndex = randomNumber;
                        ScrollToLastItem(randomNumber);
                        //if (lb_gamelist.SelectedItems.Count==0)
                        //lb_gamelist.SelectedItems.Add(lb_gamelist.Items[randomNumber]);
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
            }
        }

        public void ScrollToLastItem(int index)
        {
            lb_gamelist.SelectedItem = lb_gamelist.Items.GetItemAt(index);
            lb_gamelist.ScrollIntoView(lb_gamelist.SelectedItem);
            ListViewItem item = lb_gamelist.ItemContainerGenerator.ContainerFromItem(lb_gamelist.SelectedItem) as ListViewItem;
            lb_gamelist.ScrollIntoView(lb_gamelist.SelectedItem);
        }

        private void setStyle(string HexColor, string HorizontalAlignment) 
        {
            //Проверяем чтобы строка цвета была норм
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(HexColor);
            }
            catch (Exception)
            {
                HexColor = "#f0f8ff";
            }
            if (HorizontalAlignment == "")
                HorizontalAlignment = "Stretch";

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
                                                <ContentPresenter HorizontalAlignment=""" + HorizontalAlignment + @"""
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
                                            Value=""" + HexColor + @""" />
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
                                            Value=""" + HexColor + @""" />
                                                </MultiTrigger>
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
