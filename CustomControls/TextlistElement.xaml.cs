﻿using System;
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
    public partial class TextlistElement : Grid
    {

        public TextlistElement()
        {
            InitializeComponent();
            Clear();
        }

        public void Clear()
        {
            tb_fontPath.Text = "./../SomeArt/font.ttf";
            tb_scrollSound.Text = "./../SomeArt/scrollsound.ogg";
            tb_fontSize.Text = "0.03";
            cb_forceUppercase.IsChecked = false;
            btn_secondaryColor.Foreground = SomeUtilities.GetHexFromBrush("#777777FF");
            btn_selectedColor.Foreground = SomeUtilities.GetHexFromBrush("#777777FF");
            btn_selectorColor.Foreground = SomeUtilities.GetHexFromBrush("#777777FF");
            btn_primaryColor.Foreground = SomeUtilities.GetHexFromBrush("#777777FF");
            tb_lineSpacing.Text = "1.5";
            cbx_alignment.SelectedValue = "center";
        }

        //We fill in the properties indicated here. Then they will be assigned to the element for which they were filled.
        public SortedList<string, string> Properties
        {

            get
            {
                SortedList<string, string> _properties = new SortedList<string, string>();

                _properties.Add(btn_secondaryColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_secondaryColor.Foreground));
                _properties.Add(btn_selectedColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_selectedColor.Foreground));
                _properties.Add(btn_selectorColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_selectorColor.Foreground));
                _properties.Add(btn_primaryColor.Name.Replace("btn_", ""), SomeUtilities.GetHexFromBrush(btn_primaryColor.Foreground));
                _properties.Add(tb_fontPath.Name.Replace("tb_", ""), tb_fontPath.Text.ToString());
                _properties.Add(tb_fontSize.Name.Replace("tb_", ""), tb_fontSize.Text.ToString());
                _properties.Add(tb_lineSpacing.Name.Replace("tb_", ""), tb_lineSpacing.Text.ToString());
                _properties.Add(tb_scrollSound.Name.Replace("tb_", ""), tb_scrollSound.Text.ToString());
                _properties.Add(cbx_alignment.Name.Replace("cbx_", ""), SomeUtilities.getComboBoxValue(cbx_alignment));
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

                    val = value.FirstOrDefault(x => x.Key == btn_secondaryColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_secondaryColor.Foreground = SomeUtilities.GetHexFromBrush(val);
                    val = value.FirstOrDefault(x => x.Key == btn_selectedColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_selectedColor.Foreground = SomeUtilities.GetHexFromBrush(val);
                    val = value.FirstOrDefault(x => x.Key == btn_selectorColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_selectorColor.Foreground = SomeUtilities.GetHexFromBrush(val);
                    val = value.FirstOrDefault(x => x.Key == btn_primaryColor.Name.Replace("btn_", "")).Value;
                    if (val != null)
                        btn_primaryColor.Foreground = SomeUtilities.GetHexFromBrush(val);
                    val = value.FirstOrDefault(x => x.Key == tb_fontPath.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontPath.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_fontSize.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_fontSize.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_lineSpacing.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_lineSpacing.Text = val;
                    val = value.FirstOrDefault(x => x.Key == tb_scrollSound.Name.Replace("tb_", "")).Value;
                    if (val != null)
                        tb_scrollSound.Text = val;
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
            Es_theme_editor mainWindow = (Es_theme_editor)Application.Current.MainWindow;
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = mainWindow.tb_themefolder.Text + mainWindow.cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(mainWindow.grid1, toollName);
            string filename = SomeUtilities.openFileDialog("Font files(*.ttf)|*.ttf" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
        }

        private void btn_scrollSound_Click(object sender, RoutedEventArgs e)
        {
            Es_theme_editor mainWindow = (Es_theme_editor)Application.Current.MainWindow;
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = mainWindow.tb_themefolder.Text + mainWindow.cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(mainWindow.grid1, toollName);
            string filename = SomeUtilities.openFileDialog("Sound files(*.wav;*.ogg)|*.wav;*.ogg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
        }
    }
}
