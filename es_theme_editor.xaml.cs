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
using System.Collections;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Web;
using Microsoft.Win32;

namespace es_theme_editor
{
    //Delegates to communicate with the child window
    public delegate void NotifySelectRectangleChanging(string rectName, bool isSelected);
    public delegate void NotifyPositionRectangleChanging(string name);

    public partial class Es_theme_editor : Window
    {
        //This is a collection of all our views, which we will pass as a parameter to the opened theme editor
        SortedList<string, View> views = new SortedList<string, View>();
        view_tamlate_window _viewtmplatewindow;

        #region getters and setters
        //We check that the width parameter are entered correct digits
        public int parentCanvasWidth
        {
            get
            {
                int res;
                bool isInt = Int32.TryParse(tbwidth.Text, out res);
                if (isInt)
                    return res;
                else
                    return 800;
            }
        }

        //We check that the height parameter are entered correct digits
        public int parentCanvasHeight
        {
            get
            {
                int res;
                bool isInt = Int32.TryParse(tbheight.Text, out res);
                if (isInt)
                    return res;
                else
                    return 600;
            }
        }

        public void addView(View view)
        {
            if (view != null)
                {
                    if (views.IndexOfKey(view.name) >= 0)
                        views.Remove(view.name);
                    views.Add(view.name, view);
                }
        }
        #endregion getters and setters

        public Es_theme_editor()
        {
                InitializeComponent();

                ((App)Application.Current).Width = parentCanvasWidth;
                ((App)Application.Current).Height = parentCanvasHeight;
                String path = AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(path + "themes\\SomeTemlateTheme\\"))
                    tb_themefolder.Text = path + "themes\\SomeTemlateTheme\\";
                if (Directory.Exists(path + "themes\\SomeTemlateTheme\\" + "SomeTemplateSystem"))
                    cbx_gameplatformtheme.SelectedValue = "SomeTemplateSystem";
                Logger.WriteMessage("Es_theme_editor InitializeComponent completed");
        }

        //Closing the window
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want close application?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
            }
            else e.Cancel = true;
        }

        //Close all child windows
        private void Window_Closed_1(object sender, EventArgs e)
        {
            try
            {
                foreach (Window w in App.Current.Windows)
                    w.Close();
            }
            catch(Exception err)
            {
                Logger.Write(err);
                return;
            }
        }

        #region xmlWorker
        string include = "";
        private void btn_read_XML_Click(object sender, RoutedEventArgs e)
        {
            include = "";
            views.Clear();
            readXML();
            if (include != "")
            {
                string filename = SomeUtilities.MakeAbsolutePath(tb_themefolder.Text + "\\" + cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml", include);
                if (File.Exists(filename))
                    tb_log.Text = File.ReadAllText(filename);
                try
                {
                    readXML();
                }
                catch(Exception err)
                {
                    Logger.Write(err);
                    return;
                }
            }
        }

        private void readXML()
        {
            XmlDocument doc = new XmlDocument();
            //int x;
            XmlNode viewNode, themeNode;
            
            
            //String[] elementNames;
            try
            {
                doc.LoadXml(tb_log.Text.ToString());
            }
            catch (Exception err)
            {
                MessageBox.Show("Somthing wrong with XML", "Attention", MessageBoxButton.OK);
                Logger.Write(err);
                return;
            }
            //x = doc.ChildNodes.Count;
            //We get the main element. Theme
            if (doc.ChildNodes.Count == 0)
                return;
            else
                themeNode = doc.ChildNodes[0];
            //string themeNodeName = "";
            int docChildNodeCounters = 1;
            while (docChildNodeCounters < doc.ChildNodes.Count)
            {
                themeNode = doc.ChildNodes[docChildNodeCounters];
                if (themeNode.Name == "theme")
                    docChildNodeCounters = doc.ChildNodes.Count;
                else
                    docChildNodeCounters++;
            }
            if (themeNode.Name != "theme")
                return;

            

            for (int i = 0; i < themeNode.ChildNodes.Count; i++)
            {
                viewNode = themeNode.ChildNodes[i];

                if (themeNode.ChildNodes[i].Name == "include")
                    include = themeNode.ChildNodes[i].InnerText;
                // Get all the children of the main element. 
                // Check that the type of this Node was view
                if (themeNode.ChildNodes[i].Name == "view")
                {
                    ReadVeiwFromXML(viewNode);
                }
                if (themeNode.ChildNodes[i].Name == "feature")
                {
                    for (int y = 0; y < viewNode.ChildNodes.Count; y++ )
                        if (viewNode.ChildNodes[y].Name == "view")
                            ReadVeiwFromXML(viewNode.ChildNodes[y]);
                }
            }
        }

        private void ReadVeiwFromXML(XmlNode viewNode)
        {
            SortedList<string, string> Properties = new SortedList<string, string>();
            String[] elementNames;
            XmlNode itemNode;
            View curentWorkingView = null;
            Char delimiter = ',';
            String[] substrings = viewNode.Attributes["name"].InnerText.Split(delimiter);
            string path = "";
            //We go through all the names listed in the Name attribute
            foreach (var substring in substrings)
            {
                //string viewname = substring.Trim();
                // if the name of the received item contains the word "system", then this is the settings for the system selection window
                // Fill out the form fields
                if (substring.Trim().Contains("system"))
                {
                    for (int y = 0; y < viewNode.ChildNodes.Count; y++)
                    {
                        //Find the child element named "path" and take its value
                        foreach (XmlNode childNodes in viewNode.ChildNodes[y].ChildNodes)
                        {
                            if (childNodes.Name == "path")
                                path = childNodes.InnerText;
                        }

                        switch (viewNode.ChildNodes[y].Attributes["name"].InnerText)
                        {
                            case "bgsound":
                                ((App)Application.Current).backgroundSoundPath = path;
                                break;
                            case "background":
                                ((App)Application.Current).backgroundSystemImagePath = path;
                                break;
                            case "logo":
                                ((App)Application.Current).logoPath = path;
                                break;
                        }
                    }
                }
                else
                {
                    //We check whether we have already created a view, if not created, create, if yes, take the existing
                    if (views.Keys.IndexOf(substring.Trim()) > -1)
                        curentWorkingView = views[substring.Trim()];
                    else
                        curentWorkingView = new View(substring.Trim(), parentCanvasWidth, parentCanvasHeight);
                    //We start filling in the elements for the current view
                    for (int y = 0; y < viewNode.ChildNodes.Count; y++)
                    {
                        itemNode = viewNode.ChildNodes[y];
                        //Fill the properties for each element.
                        Properties.Clear();

                        for (int z = 0; z < itemNode.ChildNodes.Count; z++)
                        {
                            Properties.Add(itemNode.ChildNodes[z].Name, itemNode.ChildNodes[z].InnerText);
                        }
                        //Fill the element with the properties obtained from the file or create a new element with the specified properties
                        elementNames = itemNode.Attributes["name"].InnerText.Split(delimiter);
                        foreach (var elementName in elementNames)
                        {
                            if (elementName.Trim() == "background")
                            {
                                foreach (XmlNode childNodes in itemNode.ChildNodes)
                                {
                                    if (childNodes.Name == "path")
                                        ((App)Application.Current).backgroundImagePath = childNodes.InnerText;
                                }
                            }
                            else
                            {
                                Element.types type;
                                try
                                {
                                    type = (Element.types)Enum.Parse(typeof(Element.types), itemNode.Name, true);
                                }
                                catch (Exception err)
                                {
                                    Logger.Write(err);
                                    continue;
                                }
                                //Rectangle foundRectangle = SomeUtilities.FindChild<Rectangle>(grid1, "rctngl_" + elementName.Trim());
                                //Brush brush;
                                //if (foundRectangle != null)
                                //    brush = foundRectangle.Fill;
                                //else
                                //{
                                    //brush = Element.GetRandomColor();
                                //}
                                if (!curentWorkingView.elements.ContainsKey(elementName.Trim()))
                                    curentWorkingView.elements.Add(elementName.Trim(), new Element(elementName.Trim(), type, Properties, parentCanvasWidth, parentCanvasHeight, Element.GetRandomColor()));
                                else
                                    curentWorkingView.elements[elementName.Trim()].filligFromProperties(Properties, parentCanvasWidth, parentCanvasHeight, Element.GetRandomColor());
                                //brush = null;
                            }
                        }
                        //If this is a background image to save in textbox

                    }
                }
                //If the resulting view is not null, we save it
                if (curentWorkingView != null)
                {
                    views.Remove(curentWorkingView.name);
                    views.Add(curentWorkingView.name, curentWorkingView);
                }
            }
        }

        //Generate XML
        private void btn_generate_xml_file_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                generateXML();
            }
            catch (Exception err)
            {
                Logger.Write(err);
            }
        }

        private void generateXML()
        {
            if (_viewtmplatewindow != null && _viewtmplatewindow.isOpened)
                _viewtmplatewindow.save(); //_viewtmplatewindow.addPropertiesToElement(_viewtmplatewindow.GetSelectedRectengleName); //saveLastChangedProp();
            if (views.Values.Count > 0)
            {
                XmlDocument doc;
                XmlNode docNode, viewNode, itemNode, subItemNode, themeNode;
                string type;
                List<string> selectedview = new List<string>();

                //Create xml document
                doc = new XmlDocument();
                docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                //Create the main element for the theme
                themeNode = doc.CreateElement("theme");
                doc.AppendChild(themeNode);
                themeNode.AppendChild(createXMLNode(doc, "formatVersion", "", "", "4"));

                //Fill the section system (system selection)
                viewNode = createXMLNode(doc, "view", "name", "system", "");
                if (((App)Application.Current).backgroundSoundPath != "")
                {
                    itemNode = createXMLNode(doc, "sound", "name", "bgsound", "");
                    subItemNode = createXMLNode(doc, "path", "", "", ((App)Application.Current).backgroundSoundPath);
                    itemNode.AppendChild(subItemNode);
                    viewNode.AppendChild(itemNode);
                }
                if (((App)Application.Current).backgroundSystemImagePath != "")
                {
                    itemNode = createXMLNode(doc, "image", "name", "background", "");
                    createXmlAttribute(doc, itemNode, "extra", "true");
                    subItemNode = createXMLNode(doc, "path", "", "", ((App)Application.Current).backgroundSystemImagePath);
                    itemNode.AppendChild(subItemNode);
                    viewNode.AppendChild(itemNode);
                }
                if (((App)Application.Current).logoPath != "")
                {
                    itemNode = createXMLNode(doc, "image", "name", "logo", "");
                    subItemNode = createXMLNode(doc, "path", "", "", ((App)Application.Current).logoPath);
                    itemNode.AppendChild(subItemNode);
                    viewNode.AppendChild(itemNode);
                }

                themeNode.AppendChild(viewNode);
                selectedview.Clear();
                for (int i = 0; i < views.Values.Count; i++)
                {
                    //Create the main section of type view with the name selected in the ComboBox
                    viewNode = createXMLNode(doc, "view", "name", views.Values[i].name, "");
                    themeNode.AppendChild(viewNode);

                    if (((App)Application.Current).backgroundImagePath != "")
                    {
                        itemNode = createXMLNode(doc, "image", "name", "background", "");
                        createXmlAttribute(doc, itemNode, "extra", "false");
                        subItemNode = createXMLNode(doc, "path", "", "", ((App)Application.Current).backgroundImagePath);
                        itemNode.AppendChild(subItemNode);
                        subItemNode = createXMLNode(doc, "pos", "", "", "0.0 0.0");
                        itemNode.AppendChild(subItemNode);
                        subItemNode = createXMLNode(doc, "origin", "", "", "0.0 0.0");
                        itemNode.AppendChild(subItemNode);
                        subItemNode = createXMLNode(doc, "size", "", "", "1.0 1.0");
                        itemNode.AppendChild(subItemNode);
                        viewNode.AppendChild(itemNode);
                    }

                    for (int j = 0; j < views.Values[i].elements.Count; j++)
                    {
                        selectedview.Add(views.Values[i].elements.Values[j].name);
                        type = views.Values[i].elements.Values[j].typeOfElement.ToString();

                        itemNode = createXMLNode(doc, type, "name", views.Values[i].elements.Values[j].name, "");
                        if (views.Values[i].elements.Values[j].name.ToLower() == "logo" && views.Values[i].elements.Values[j].path == "")
                            continue;
                        else
                            for (int x = 0; x < views.Values[i].elements.Values[j].Properties.Count; x++)
                            {
                                subItemNode = createXMLNode(doc, views.Values[i].elements.Values[j].Properties.Keys[x], "", "", views.Values[i].elements.Values[j].Properties.Values[x].Replace("#", ""));
                                itemNode.AppendChild(subItemNode);
                            }
                        viewNode.AppendChild(itemNode);
                    }

                    for (int y = 0; y < Element.novisibleelement.Count; y++)
                    {
                        //for basic view hide only help view.
                        if (views.Values[i].name == View.types.basic.ToString() && y >= 1)
                            break;
                        if (!selectedview.Contains(Element.novisibleelement[y]))
                        {
                            itemNode = createXMLNode(doc, (Element.GetType(Element.novisibleelement[y])).ToString(), "name", Element.novisibleelement[y], "");
                            subItemNode = createXMLNode(doc, "pos ", "", "", "1.0 1.0");
                            itemNode.AppendChild(subItemNode);
                            viewNode.AppendChild(itemNode);
                        }
                    }
                }

                var stringBuilder = new StringBuilder();
                var xmlWriterSettings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };
                doc.Save(XmlWriter.Create(stringBuilder, xmlWriterSettings));
                tb_log.Text = stringBuilder.ToString();
            }
        }

        private XmlNode createXMLNode(XmlDocument doc, string nodename, string attributename, string attributevalue, string childvalue)
        {

            XmlNode node;

            node = doc.CreateElement(nodename.Replace(" ", ""));

            createXmlAttribute(doc, node, attributename, attributevalue);

            if (childvalue != "")
                node.AppendChild(doc.CreateTextNode(childvalue));

            return node;

        }

        private void createXmlAttribute(XmlDocument doc, XmlNode node, string attributename, string attributevalue)
        {
            XmlAttribute nodeAttribute;

            if (attributevalue != "")
            {
                nodeAttribute = doc.CreateAttribute(attributename);
                nodeAttribute.Value = attributevalue;
                node.Attributes.Append(nodeAttribute);
            }
        }
        #endregion xmlWorker

        #region openTheme
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                tb_themefolder.Text = dialog.SelectedPath;
            }

            fillgameplatformtheme();
        }

        private void cbx_gameplatformtheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbx_gameplatformtheme.SelectedItem != null)
            {
                string filename = tb_themefolder.Text + "\\" + cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
                if (File.Exists(filename))
                    tb_log.Text = File.ReadAllText(filename);
                else
                    MessageBox.Show("The theme.xml file does not exist in the selected theme", "Attention", MessageBoxButton.OK);

                ((App)Application.Current).gameplatformtheme = cbx_gameplatformtheme.SelectedItem.ToString();
            }
        }

        private void fillgameplatformtheme()
        {
            if (Directory.Exists(tb_themefolder.Text))
            {
                DirectoryInfo directory = new DirectoryInfo(tb_themefolder.Text);

                DirectoryInfo[] directories = directory.GetDirectories();
                List<string> list = new List<string>();
                foreach (DirectoryInfo folder in directories)
                {

                    list.Add(folder.Name);
                }
                cbx_gameplatformtheme.ItemsSource = list;
            }
        }

        private void tb_themefolder_TextChanged(object sender, TextChangedEventArgs e)
        {
            fillgameplatformtheme();
            ((App)Application.Current).themefolder = tb_themefolder.Text;
        }
        #endregion openTheme

        private string getComboBoxValue(ComboBoxItem cbxi)
        {
            //Get the value from the ComboBox and check if such view exists
            ComboBoxItem  typeItem = (ComboBoxItem)cbxi;
            string comboBoxItemvalue = "";
            if (typeItem != null)
                comboBoxItemvalue = typeItem.Content.ToString();
            return comboBoxItemvalue;
        }

        //Checking the input parameters for compliance with the necessary conditions
        //Used in XML for our TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Response to state change ComboBox selection view
        private void cbViewSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Get the value from the ComboBox and check if such view exists
            string comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
            if (e.RemovedItems.Count>0 && comboBoxItemvalue == e.RemovedItems[0].ToString())
                return;
            //If there is already editing a new window for editing do not begin.
            if (_viewtmplatewindow != null && _viewtmplatewindow.isOpened && _viewtmplatewindow.Title != comboBoxItemvalue)
            {
                if (e.RemovedItems.Count > 0)
                    ((ComboBox)sender).SelectedValue = e.RemovedItems[0];
                MessageBox.Show("First, finish editing the current view", "Attention", MessageBoxButton.OK);
                return;
            }

            if (views.IndexOfKey(comboBoxItemvalue) < 0)
                views.Add(comboBoxItemvalue, new View((View.types)System.Enum.Parse(typeof(View.types), comboBoxItemvalue), parentCanvasWidth, parentCanvasHeight));
        }

        //Open view_tamlate_window
        private bool openViewEditor()
        {
            string comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
            if (comboBoxItemvalue != "")
            {
                if (views.IndexOfKey(comboBoxItemvalue) < 0)
                    views.Add(comboBoxItemvalue, new View((View.types)System.Enum.Parse(typeof(View.types), comboBoxItemvalue), parentCanvasWidth, parentCanvasHeight));
                View currview = views.Values[views.IndexOfKey(comboBoxItemvalue)];

                if (_viewtmplatewindow == null || !_viewtmplatewindow.isOpened)
                {

                    _viewtmplatewindow = new view_tamlate_window(this, comboBoxItemvalue, currview);

                    _viewtmplatewindow.Show();
                    tbheight.IsEnabled = false;
                    tbwidth.IsEnabled = false;
                    btn_ApplyResolution.IsEnabled = false;
                    return true;
                }
                _viewtmplatewindow.Show();
                tbheight.IsEnabled = false;
                tbwidth.IsEnabled = false;
                btn_ApplyResolution.IsEnabled = false;


                //return false;
            }
            return false;
        }

        //Set the size of the workspace in the _viewtmplatewindow
        private void btn_ApplyResolution_Click(object sender, RoutedEventArgs e)
        {
            if (this.views.Count > 0)
            {
                tbheight.IsEnabled = false;
                tbwidth.IsEnabled = false;
                btn_ApplyResolution.IsEnabled = false;
                return;
            }
            ((App)Application.Current).Width = parentCanvasWidth;
            ((App)Application.Current).Height = parentCanvasHeight;
            //_viewtmplatewindow.SetCanvasResolution(parentCanvasWidth, parentCanvasHeight);
        }

        //Open view_tamlate_window
        private void btnOpenViewEditor_Click(object sender, RoutedEventArgs e)
        {
            openViewEditor();
        }
    }
}
