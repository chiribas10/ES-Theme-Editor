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
        //SortedList<string, string> viewsToElements = new SortedList<string, string>();
        //On/off transparency for Rectangle
        private bool transperentRectangle = false;
        view_tamlate_window _viewtmplatewindow;
        //Flag to ensure that the program does not react to changing the checkboxes when we do not need it
        bool cbManualChangeState = false;

        public Es_theme_editor()
        {
            try
            {
                InitializeComponent();
                String path = AppDomain.CurrentDomain.BaseDirectory;
                if (Directory.Exists(path + "themes\\SomeTemlateTheme\\"))
                    tb_themefolder.Text = path + "themes\\SomeTemlateTheme\\";
                if (Directory.Exists(path + "themes\\SomeTemlateTheme\\" + "SomeTemplateSystem"))
                    cbx_gameplatformtheme.SelectedValue = "SomeTemplateSystem";
                Logger.WriteMessage("Es_theme_editor InitializeComponent completed");
            }
            catch (Exception err)
            {
                Logger.Write(err);
            }
        }

        //Closing the window
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Do you want close application?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //There is something you can do if the user really decided to close the application
            }
            else e.Cancel = true;
        }

        //Close all child windows
        private void Window_Closed_1(object sender, EventArgs e)
        {
            foreach (Window w in App.Current.Windows)
                w.Close();
        }

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

        public View currentItem
        {
            set
            {
                views.Remove(value.name);
                views.Add(value.name, value);
            }
        }
        #endregion getters and setters

        #region StandartItemWorking
        //Universal metod for item add/delete checkboxes
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            Element.types typeOfElement;
            string str = ((CheckBox)sender).Name;
            Rectangle foundRectangle = SomeUtilities.FindChild<Rectangle>(grid1, str.Replace("cb", "rctngl"));
            if (!cbManualChangeState)
            {
                openViewEditor();
                typeOfElement = Element.GetType(str.Replace("cb_", ""));
                CreateRect(str.Replace("cb_", ""), typeOfElement, foundRectangle.Fill, 0.5);
            }
        }

        //Universal metod for item add/delete checkboxes 
        private void cbLogo_Unchecked(object sender, RoutedEventArgs e)
        {
            string str = ((CheckBox)sender).Name;
            if (!cbManualChangeState && _viewtmplatewindow != null)
                if (MessageBox.Show("Do you want delete this item?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _viewtmplatewindow.RemoveRect(str.Replace("cb_", ""));
                    cb_SelectCreatedelement.Items.Remove(str.Replace("cb_", ""));
                }
        }
        #endregion StandartItemWorking

        #region CustomItemWorking
        private void cb_CustomeItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_CustomeItem.SelectedItem != null)
            {
                //string comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cb_CustomeItem.SelectedItem);
                View currview = views.Values[views.IndexOfKey(getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem))];
                Element currelement = currview.elements.Values[currview.elements.IndexOfKey(cb_CustomeItem.SelectedValue.ToString())];
                tb_customelementName.Text = currelement.name;
                cb_customelementType.SelectedValue = currelement.typeOfElement.ToString();
            }
            //tb_customelementName
        }

        private void btn_addCustomelement_Click(object sender, RoutedEventArgs e)
        {
            if (getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem) != "")
            {
                openViewEditor();
                CreateRect(tb_customelementName.Text, (Element.types)System.Enum.Parse(typeof(Element.types), cb_customelementType.SelectedValue.ToString()), Element.GetRandomColor(), 1);
                cb_CustomeItem.Items.Add(tb_customelementName.Text);
            }
        }

        private void delCustomelement_Click(object sender, RoutedEventArgs e)
        {
            string comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
            if (_viewtmplatewindow != null && _viewtmplatewindow.isOpened && _viewtmplatewindow.Title == comboBoxItemvalue)
                if (MessageBox.Show("Do you want delete this item?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _viewtmplatewindow.RemoveRect(tb_customelementName.Text);
                    cb_CustomeItem.Items.Remove(tb_customelementName.Text);
                    cb_SelectCreatedelement.Items.Remove(tb_customelementName.Text);
                }
        }
        #endregion CustomItemWorking

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
                readXML();
                //tb_log.Text = MakeRelativePath(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml", tb_themefolder.Text + "\\SomeTemplateSystem\\art\\art_blur_whiten.png");
                //var uri = new Uri(new Uri(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml"), tb_log.Text.ToString());
                //tb_log.Text += "\n\n\n" + MakeAbsolutePath(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml", tb_log.Text.ToString());
            }
        }

        private void readXML()
        {
            XmlDocument doc = new XmlDocument();
            //int x;
            XmlNode viewNode, itemNode, themeNode;
            
            
            String[] elementNames;
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
                                tb_bgsound_system.Text = path;
                                break;
                            case "background":
                                tb_background_system.Text = path;
                                break;
                            case "logo":
                                tb_logo_system.Text = path;
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
                                        tb_background_system_for_window_system.Text = childNodes.InnerText;
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
                                Rectangle foundRectangle = SomeUtilities.FindChild<Rectangle>(grid1, "rctngl_" + elementName.Trim());
                                Brush brush;
                                if (foundRectangle != null)
                                    brush = foundRectangle.Fill;
                                else
                                {
                                    brush = Element.GetRandomColor();
                                }
                                if (!curentWorkingView.elements.ContainsKey(elementName.Trim()))
                                    curentWorkingView.elements.Add(elementName.Trim(), new Element(elementName.Trim(), type, Properties, parentCanvasWidth, parentCanvasHeight, brush));
                                else
                                    curentWorkingView.elements[elementName.Trim()].filligFromProperties(Properties, parentCanvasWidth, parentCanvasHeight, brush);
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
            if (_viewtmplatewindow != null)
                addPropertiesToElement(_viewtmplatewindow.GetSelectedRectengleName); //saveLastChangedProp();
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
                if (tb_bgsound_system.Text != "")
                {
                    itemNode = createXMLNode(doc, "sound", "name", "bgsound", "");
                    subItemNode = createXMLNode(doc, "path", "", "", tb_bgsound_system.Text);
                    itemNode.AppendChild(subItemNode);
                    viewNode.AppendChild(itemNode);
                }
                if (tb_background_system.Text != "")
                {
                    itemNode = createXMLNode(doc, "image", "name", "background", "");
                    createXmlAttribute(doc, itemNode, "extra", "true");
                    subItemNode = createXMLNode(doc, "path", "", "", tb_background_system.Text);
                    itemNode.AppendChild(subItemNode);
                    viewNode.AppendChild(itemNode);
                }
                if (tb_logo_system.Text != "")
                {
                    itemNode = createXMLNode(doc, "image", "name", "logo", "");
                    subItemNode = createXMLNode(doc, "path", "", "", tb_logo_system.Text);
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

                    if (tb_background_system_for_window_system.Text != "")
                    {
                        itemNode = createXMLNode(doc, "image", "name", "background", "");
                        createXmlAttribute(doc, itemNode, "extra", "false");
                        subItemNode = createXMLNode(doc, "path", "", "", tb_background_system_for_window_system.Text);
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

                        itemNode = createXMLNode(doc, type, "name", views.Values[i].elements.Values[j].name.ToLower(), "");
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
            //tb_log.Text = MakeRelativePath(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml", tb_themefolder.Text + "\\SomeTemplateSystem\\art\\art_blur_whiten.png");
            //var uri = new Uri(new Uri(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml"), tb_log.Text.ToString());
            //tb_log.Text += "\n\n\n" + MakeAbsolutePath(tb_themefolder.Text + "\\" + "SomeTemplateSystem" + "\\theme.xml", tb_log.Text.ToString());
            fillgameplatformtheme();
        }
        #endregion selectTheme

        private string getComboBoxValue(ComboBoxItem cbxi)
        {
            //Get the value from the ComboBox and check if such view exists
            ComboBoxItem typeItem = (ComboBoxItem)cbxi;
            string comboBoxItemvalue = "";
            if (typeItem != null)
                comboBoxItemvalue = typeItem.Content.ToString();
            return comboBoxItemvalue;
        }

        //We pass the view_tamlate_window task to create a new rectengl
        private void CreateRect(string name, Element.types typeOfElement, Brush fill, double opacity)
        {
            if (!transperentRectangle)
                opacity = 1;

            if (_viewtmplatewindow != null)
            {
                //For rating stars, we set the template size smaller than the rest
                if (name != "md_rating" && name != "help")
                    _viewtmplatewindow.CreateRect(name, typeOfElement, fill, opacity, 200, 200);
                else if (name == "help")
                    _viewtmplatewindow.CreateRect(name, typeOfElement, fill, opacity, parentCanvasWidth, 75, 0, parentCanvasHeight - 75);
                else
                    _viewtmplatewindow.CreateRect(name, typeOfElement, fill, opacity, 30, 30);

                cb_SelectCreatedelement.Items.Add(name);

                switch (_viewtmplatewindow.view.elements[name].typeOfElement.ToString())
                {
                    case "text":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = text.Properties;
                        else
                            text.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                    case "image":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = image.Properties;
                        else
                            image.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                    case "textlist":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = textlist.Properties;
                        else
                            textlist.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                    case "rating":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = rating.Properties;
                        else
                            rating.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                    case "datetime":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = datetime.Properties;
                        else
                            datetime.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                    case "helpsystem":
                        if (_viewtmplatewindow.view.elements[name].Properties.Count == 0)
                            _viewtmplatewindow.view.elements[name].Properties = helpsystem.Properties;
                        else
                            helpsystem.Properties = _viewtmplatewindow.view.elements[name].Properties;
                        break;
                }
            }
            else
            {
                //Turn on the manual switch mode CheckBox so that the program does not react to changing their states
                //Enable or disable CheckBox if they have already been marked previously for the selected view
                cbManualChangeState = true;

                uncheckAllCheckboxes();

                cbManualChangeState = false;
            }
        }

        //Checking the input parameters for compliance with the necessary conditions
        //Used in XML for our TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //All CheckBox set the state of uncheck
        private void uncheckAllCheckboxes() 
        {
            List<CheckBox> checkboxes = SomeUtilities.GetLogicalChildCollection<CheckBox>(grid1);

            for (int i = 0; i < checkboxes.Count; i++)
            {
                if (checkboxes[i].Content.ToString() == "")
                    checkboxes[i].IsChecked = false;
            }
        }

        //All GroupBox set the state of Hidden
        private void hiddenAllGroupBox()
        {
            List<GroupBox> groupboxes = SomeUtilities.GetLogicalChildCollection<GroupBox>(grid1);

            for (int i = 0; i < groupboxes.Count; i++)
            {
                groupboxes[i].Visibility = System.Windows.Visibility.Hidden;
            }
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
            //Get all items for the selected view
            View currview = views.Values[views.IndexOfKey(comboBoxItemvalue)];

            //Turn on the manual switch mode CheckBox so that the program does not react to changing their states
            //Enable or disable CheckBox if they have already been marked previously for the selected view
            cbManualChangeState = true;

            uncheckAllCheckboxes();

            CheckBox foundCheckBox;
            for (int i = 0; i < currview.elements.Count; i++)
            {
                foundCheckBox = SomeUtilities.FindChild<CheckBox>(grid1, "cb_" + currview.elements.Values[i].name);
                if (foundCheckBox != null)
                    foundCheckBox.IsChecked = true;
                else
                {
                    if (cb_CustomeItem.Items.IndexOf(currview.elements.Values[i].name) < 0)
                        cb_CustomeItem.Items.Add(currview.elements.Values[i].name);
                }
            }
            //Turn off the manual switch mode CheckBox
            cbManualChangeState = false;
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

                    for (int i = 0; i < currview.elements.Count; i++ )
                        cb_SelectCreatedelement.Items.Add(currview.elements.Values[i].name);
                    
                    //It is unforgettable to subscribe to the change of the selected rectengl and resizing
                    _viewtmplatewindow.OnSelectinChanged += new NotifySelectRectangleChanging(rectangleSelectChanget);
                    _viewtmplatewindow.OnPositionChanged += new NotifyPositionRectangleChanging(rectanglePositionChanget);
                    _viewtmplatewindow.Show();
                    return true;
                }
                _viewtmplatewindow.Show();
                return false;
            }
            return false;
        }

        //When changing the position of the current Renctengl (it's our Element)
        void rectanglePositionChanget(string name) 
        {
            try
            {
                switch (_viewtmplatewindow.view.elements[name].typeOfElement.ToString())
                {
                    case "text":
                        text.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        text.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        if (name == "md_description")
                        {
                            text.tb_size_h.Text = _viewtmplatewindow.view.elements[name].size_height.ToString();
                            text.tb_size_w.Text = _viewtmplatewindow.view.elements[name].size_width.ToString();
                        }
                        break;
                    case "image":
                        image.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        image.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        image.tb_size_h.Text = _viewtmplatewindow.view.elements[name].size_height.ToString();
                        image.tb_size_w.Text = _viewtmplatewindow.view.elements[name].size_width.ToString();
                        image.tb_origin_w.Text = "0.5";
                        image.tb_origin_h.Text = "0.5";
                        break;
                    case "textlist":
                        textlist.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        textlist.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        textlist.tb_size_h.Text = _viewtmplatewindow.view.elements[name].size_height.ToString();
                        textlist.tb_size_w.Text = _viewtmplatewindow.view.elements[name].size_width.ToString();
                        break;
                    case "video":
                        video.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        video.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        video.tb_size_h.Text = _viewtmplatewindow.view.elements[name].size_height.ToString();
                        video.tb_size_w.Text = _viewtmplatewindow.view.elements[name].size_width.ToString();
                        video.tb_origin_w.Text = "0.5";
                        video.tb_origin_h.Text = "0.5";
                        break;
                    case "rating":
                        rating.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        rating.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        rating.tb_size_h.Text = _viewtmplatewindow.view.elements[name].size_height.ToString();
                        break;
                    case "datetime":
                        datetime.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        datetime.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        break;
                    case "helpsystem":
                        helpsystem.tb_pos_h.Text = _viewtmplatewindow.view.elements[name].pos_y.ToString();
                        helpsystem.tb_pos_w.Text = _viewtmplatewindow.view.elements[name].pos_x.ToString();
                        break;
                }
            }
            catch(Exception err)
            {
                Logger.Write(err);
            }
        }

        //When the selection in Rectengl changes
        void rectangleSelectChanget(string name, bool isSelected)
        {
            //Before changing the rectengl, we process the fields if they are filled.
            string comboBoxItemvalue;
            comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
            hiddenAllGroupBox();
            //Save the values in the fields to the current view opened in the viewtmplatewindow window
            if (!isSelected)
            {
                if (name != "None")
                    try
                    {
                        addPropertiesToElement(name);
                    }
                    catch (Exception err) 
                    {
                        Logger.Write(err);
                    }

                GroupBox foundGroupBox = SomeUtilities.FindChild<GroupBox>(grid1, "gb_base");
                foundGroupBox.Visibility = System.Windows.Visibility.Visible;
                tb_log.Text = name.ToString();
                return;
            }

            //Save the values in the fields from the current view opened in the viewtmplatewindow window
            if (isSelected)
            {
                //Save view to global variable
                string elementType = _viewtmplatewindow.view.elements[name].typeOfElement.ToString();
                if (isSelected)
                {
                    GroupBox foundGrid = SomeUtilities.FindChild<GroupBox>(grid1, "gb_" + elementType);
                    foundGrid.Visibility = System.Windows.Visibility.Visible;
                    tb_log.Text = name.ToString();
                }
                getPropertiesFromElement(name);
            }
        }

        //Get Properties at the modified Rectangle
        private void getPropertiesFromElement(string name) 
        {
            if ((_viewtmplatewindow.view != null) && (_viewtmplatewindow.view.elements != null) && (_viewtmplatewindow.view.elements.Keys.Contains(name)))
            {
                string elementType = _viewtmplatewindow.view.elements[name].typeOfElement.ToString();

                if (elementType != Element.types.notexistsname.ToString())
                    switch (elementType)
                    {
                        case "text":
                            text.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                        case "image":
                            image.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                        case "textlist":
                            textlist.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                        case "rating":
                            rating.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                        case "datetime":
                            datetime.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                        case "helpsystem":
                            helpsystem.Properties = _viewtmplatewindow.view.elements[name].Properties;
                            break;
                    }
            }
        }

        //Transfer Protectis Rectangle
        private void addPropertiesToElement(string name) 
        {
            if ((_viewtmplatewindow.view != null) && (_viewtmplatewindow.view.elements != null) && (_viewtmplatewindow.view.elements.Keys.Contains(name)))
                switch (_viewtmplatewindow.view.elements[name].typeOfElement.ToString())
            {
                case "text":
                    _viewtmplatewindow.view.elements[name].Properties = text.Properties;
                    text.Clear();
                    break;
                case "image":
                    _viewtmplatewindow.view.elements[name].Properties = image.Properties;
                    if (name == "logo")
                        image.Clear(tb_logo_system.Text);
                    else
                        image.Clear("");
                    break;
                case "textlist":
                    _viewtmplatewindow.view.elements[name].Properties = textlist.Properties;
                    textlist.Clear();
                    break;
                case "rating":
                    _viewtmplatewindow.view.elements[name].Properties = rating.Properties;
                    rating.Clear();
                    break;
                case "datetime":
                    _viewtmplatewindow.view.elements[name].Properties = datetime.Properties;
                    datetime.Clear();
                    break;
                case "helpsystem":
                    _viewtmplatewindow.view.elements[name].Properties = helpsystem.Properties;
                    helpsystem.Clear();
                    break;
            }
        }

        //Set the size of the workspace in the _viewtmplatewindow
        private void btn_ApplyResolution_Click(object sender, RoutedEventArgs e)
        {
            _viewtmplatewindow.SetCanvasResolution(parentCanvasWidth, parentCanvasHeight);
        }

        //Open view_tamlate_window
        private void btnOpenViewEditor_Click(object sender, RoutedEventArgs e)
        {
            openViewEditor();
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

        //Some hidden button, some not realised function
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string filename = SomeUtilities.MakeAbsolutePath(tb_themefolder.Text + "\\" + cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml", tb_background_system.Text);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(filename);
            bitmap.EndInit();

            Image image = new Image();
            image.Source = bitmap;
            image.Width = _viewtmplatewindow.canvas1.Width;
            image.Height = _viewtmplatewindow.canvas1.Height;
            _viewtmplatewindow.canvas1.Children.Add(image);
        }

        private void btn_bgsound_system_Click(object sender, RoutedEventArgs e)
        {
            string themeFilename = tb_themefolder.Text + cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            string filename = SomeUtilities.openFileDialog("Sound files(*.wav;*.ogg)|*.wav;*.ogg" + "|Все файлы (*.*)|*.* ", this.tb_bgsound_system.Text, themeFilename);
            if (filename != null)
                this.tb_bgsound_system.Text = filename;
        }

        private void btn_getimagefile_Click(object sender, RoutedEventArgs e)
        {
            string toollName = ((Button)sender).Name;
            toollName = toollName.Replace("btn", "tb");
            string themeFilename = tb_themefolder.Text + cbx_gameplatformtheme.SelectedItem.ToString() + "\\theme.xml";
            TextBox foundTextBox = SomeUtilities.FindChild<TextBox>(grid1, toollName);
            string filename = SomeUtilities.openFileDialog("Image files(*.png;*.jpg;*.svg)|*.png;*.jpg;*.svg" + "|Все файлы (*.*)|*.* ", foundTextBox.Text, themeFilename);
            if (filename != null)
                foundTextBox.Text = filename;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            view_tamlate_window.BringToFront(_viewtmplatewindow.canvas1, _viewtmplatewindow.GetSelectedRectengleName);
        }

        private void cb_SelectCreatedelement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             //sender
            string si = ((ComboBox)sender).SelectedItem.ToString();
            if (((ComboBox)sender).SelectedItem != null && ((ComboBox)sender).SelectedItem.ToString() != "")
                _viewtmplatewindow.SelectRect(((ComboBox)sender).SelectedItem.ToString());
        }
    }
}
