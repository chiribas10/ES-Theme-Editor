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
    /// Interaction logic for ToolBox.xaml
    /// </summary>
    public partial class ToolBox : Grid
    {
        public delegate void ElementCreated(string elementName, Element.types typeOfElement, Brush Fill, double opacity);
        public event ElementCreated onElementCreate;
        public delegate void ElementRemoved(string elementName);
        public event ElementRemoved onElementRemove;
        public delegate void ElementSelected(string elementName);
        public event ElementSelected onElementSelect;
        //public delegate void CurrentElementChanget(string elementName, SortedList<string, string> Properties, Element.types typeOfElement);
        //public event CurrentElementChanget onCurrentElementChanget;
        //Flag to ensure that the program does not react to changing the checkboxes when we do not need it
        public bool cbManualChangeState = false;
        //Collection of created not standart element
        SortedList<string, string> customElements = new SortedList<string, string>();
        public View currentView;

        public ToolBox()
        {
            InitializeComponent();
        }

        public void setCurrentView(View view) 
        {
            currentView = view;
            //Turn on the manual switch mode CheckBox so that the program does not react to changing their states
            //Enable or disable CheckBox if they have already been marked previously for the selected view
            cbManualChangeState = true;

            uncheckAllCheckboxes();

            CheckBox foundCheckBox;
            for (int i = 0; i < currentView.elements.Count; i++)
            {
                addCreatedelement(currentView.elements.Values[i].name);
                foundCheckBox = SomeUtilities.FindChild<CheckBox>(grid1, "cb_" + currentView.elements.Values[i].name);
                if (foundCheckBox != null)
                    foundCheckBox.IsChecked = true;
                else
                {
                    if (cb_CustomeItem.Items.IndexOf(currentView.elements.Values[i].name) < 0)
                        cb_CustomeItem.Items.Add(currentView.elements.Values[i].name);
                }
            }
            //Turn off the manual switch mode CheckBox
            cbManualChangeState = false;

            switch (View.GetType(currentView.name))
            {
                case View.types.basic:
                    c_basic.Visibility = System.Windows.Visibility.Visible;
                    if (!checCheckedCheckboxes(c_detailed))
                        c_detailed.Visibility = System.Windows.Visibility.Hidden;
                    if (!checCheckedCheckboxes(c_video))
                        c_video.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case View.types.detailed:
                    c_basic.Visibility = System.Windows.Visibility.Visible;
                    c_detailed.Visibility = System.Windows.Visibility.Visible;
                    if (!checCheckedCheckboxes(c_video))
                        c_video.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case View.types.video:
                    c_basic.Visibility = System.Windows.Visibility.Visible;
                    c_detailed.Visibility = System.Windows.Visibility.Visible;
                    c_video.Visibility = System.Windows.Visibility.Visible;
                    break;
            }

        }

        private bool checCheckedCheckboxes(DependencyObject parent) 
        {
            List<CheckBox> checkboxes = SomeUtilities.GetLogicalChildCollection<CheckBox>(parent);

            for (int i = 0; i < checkboxes.Count; i++)
            {
                if (checkboxes[i].IsChecked == true)
                    return true;

            }

            return false;
        }

        private void uncheckAllCheckboxes()
        {
            List<CheckBox> checkboxes = SomeUtilities.GetLogicalChildCollection<CheckBox>(grid1);

            for (int i = 0; i < checkboxes.Count; i++)
            {
                if (checkboxes[i].Content.ToString() == "")
                    checkboxes[i].IsChecked = false;
            }
        }

        public void addCreatedelement(string name) 
        {
            string cbvalue;
            for (int i = 0; i < cb_SelectCreatedelement.Items.Count; i++)
            {
                cbvalue = cb_SelectCreatedelement.Items[i].ToString();
                if (cbvalue == name)
                    return;
            }
            cb_SelectCreatedelement.Items.Add(name);
        }


        #region StandartItemWorking
        //Universal metod for item add/delete checkboxes
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            Element.types typeOfElement;
            string str = ((CheckBox)sender).Name;
            Rectangle foundRectangle = SomeUtilities.FindChild<Rectangle>(grid1, str.Replace("cb", "rctngl"));
            if (!cbManualChangeState)
            {
                typeOfElement = Element.GetType(str.Replace("cb_", ""));
                onElementCreate(str.Replace("cb_", ""), typeOfElement, foundRectangle.Fill, 0.5);
                //cb_SelectCreatedelement.Items.Add(str.Replace("cb_", ""));
            }
        }

        //Universal metod for item add/delete checkboxes 
        private void cbLogo_Unchecked(object sender, RoutedEventArgs e)
        {
            string str = ((CheckBox)sender).Name;
            if (!cbManualChangeState)
                if (MessageBox.Show("Do you want delete this item?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    onElementRemove(str.Replace("cb_", ""));
                    cb_SelectCreatedelement.Items.Remove(str.Replace("cb_", ""));
                }
        }
        #endregion StandartItemWorking

        #region CustomItemWorking
        private void cb_CustomeItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_CustomeItem.SelectedItem != null)
            {
                tb_customelementName.Text = cb_CustomeItem.SelectedItem.ToString();
                cb_customelementType.SelectedItem = customElements[cb_CustomeItem.SelectedItem.ToString()];
            }
        }

        private void btn_addCustomelement_Click(object sender, RoutedEventArgs e)
        {
            //if (getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem) != "")
            //{
                customElements.Add(tb_customelementName.Text, cb_customelementType.SelectedValue.ToString());
                onElementCreate(tb_customelementName.Text, (Element.types)System.Enum.Parse(typeof(Element.types), cb_customelementType.SelectedValue.ToString()), Element.GetRandomColor(), 1);
                cb_CustomeItem.Items.Add(tb_customelementName.Text);
                cb_SelectCreatedelement.Items.Add(tb_customelementName.Text);
            //}
        }

        private void delCustomelement_Click(object sender, RoutedEventArgs e)
        {
            //string comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
                if (MessageBox.Show("Do you want delete this item?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    onElementRemove(tb_customelementName.Text);
                    customElements.Remove(tb_customelementName.Text);
                    cb_CustomeItem.Items.Remove(tb_customelementName.Text);
                    cb_SelectCreatedelement.Items.Remove(tb_customelementName.Text);
                }
        }
        #endregion CustomItemWorking

        private void cb_SelectCreatedelement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //sender
            if (((ComboBox)sender).SelectedItem != null)
            {
                string si = ((ComboBox)sender).SelectedItem.ToString();
                if (((ComboBox)sender).SelectedItem != null && ((ComboBox)sender).SelectedItem.ToString() != "")
                {
                    onElementSelect(((ComboBox)sender).SelectedItem.ToString());
                    //onCurrentElementChanget(currentView.elements[((ComboBox)sender).SelectedItem.ToString()].name,
                    //                        currentView.elements[((ComboBox)sender).SelectedItem.ToString()].Properties,
                    //                        currentView.elements[((ComboBox)sender).SelectedItem.ToString()].typeOfElement);
                }
            }
        }

    }
}
