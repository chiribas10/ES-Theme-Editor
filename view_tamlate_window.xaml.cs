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

// Notes:
//  - The Canvas must have a non-null background to make it generate mouse events.

namespace es_theme_editor
{
    public partial class view_tamlate_window : Window
    {
        Es_theme_editor _parent;

        public bool isOpened = false;

        //To tell the parent window that the selected object has changed
        //public event NotifySelectRectangleChanging OnSelectinChanged;
        //public event NotifyPositionRectangleChanging OnPositionChanged;
        public bool RectangleNamedMouseLeave = true;
        //public View view;
        //On/off transparency for Rectangle
        private bool transperentRectangle = false;

        //Inicialize window
        //public view_tamlate_window()
        //{
        //    InitializeComponent();            
        //}

        public view_tamlate_window(Es_theme_editor parent, string name, View view)
        {
            InitializeComponent();
            isOpened = true;
            //this.view = view;

            toolbox.setCurrentView(view);
            toolbox.onElementCreate += this.CreateRect;
            toolbox.onElementRemove += this.RemoveRect;
            toolbox.onElementSelect += this.SelectRect;
            //toolbox.onCurrentElementChanget += propertiesbox.setElementProperties;
            this.Title = name;
            fillItem();
            _parent = parent;
            SetCanvasResolution(view.width, view.height);
        }

        //private void RemoveRect(string name)
        //{
        //    RemoveRect(name);
        //}

        //private void SelectRect(string name)
        //{
        //    SelectRect(name);
        //}

        //We pass the view_tamlate_window task to create a new rectengl
        private void CreateRect(string name, Element.types typeOfElement, Brush fill, double opacity)
        {
            if (!transperentRectangle)
                opacity = 1;

            //if (_viewtmplatewindow != null)
            //{
            //For rating stars, we set the template size smaller than the rest
            if (name != "md_rating" && name != "help")
                CreateRect(name, typeOfElement, fill, opacity, 200, 200);
            else if (name == "help")
                CreateRect(name, typeOfElement, fill, opacity, canvas1.ActualWidth, 75, 0, canvas1.ActualHeight - 75);
            else
                CreateRect(name, typeOfElement, fill, opacity, 30, 30);

            toolbox.addCreatedelement(name);
            if (toolbox.currentView.elements.ContainsKey(name))
            switch (toolbox.currentView.elements[name].typeOfElement.ToString())
            {
                case "text":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.text.Properties;
                    else
                        propertiesbox.text.Properties = toolbox.currentView.elements[name].Properties;
                    break;
                case "image":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.image.Properties;
                    else
                        propertiesbox.image.Properties = toolbox.currentView.elements[name].Properties;
                    break;
                case "textlist":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.textlist.Properties;
                    else
                        propertiesbox.textlist.Properties = toolbox.currentView.elements[name].Properties;
                    break;
                case "rating":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.rating.Properties;
                    else
                        propertiesbox.rating.Properties = toolbox.currentView.elements[name].Properties;
                    break;
                case "datetime":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.datetime.Properties;
                    else
                        propertiesbox.datetime.Properties = toolbox.currentView.elements[name].Properties;
                    break;
                case "helpsystem":
                    if (toolbox.currentView.elements[name].Properties.Count == 0)
                        toolbox.currentView.elements[name].Properties = propertiesbox.helpsystem.Properties;
                    else
                        propertiesbox.helpsystem.Properties = toolbox.currentView.elements[name].Properties;
                    break;
            }
            //}
            //else
            //{
            //    //Turn on the manual switch mode CheckBox so that the program does not react to changing their states
            //    //Enable or disable CheckBox if they have already been marked previously for the selected view
            //    toolBox.cbManualChangeState = true;

            //    uncheckAllCheckboxes();

            //    toolBox.cbManualChangeState = false;
            //}
        }

        //Closing window
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selected_rectangle != null)
            {
                selected_rectangle.Unselect();
                NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
            }

            _parent.currentItem = toolbox.currentView;
            isOpened = false;
        }

        //Return selected rectengl
        public string GetSelectedRectengleName 
        {
            get 
            {
                return selected_rectangle.Name;
            }
        }

        //We notify the parent window about changing the selected Rectangle
        public void NotifySelectinChanged(string name, bool isSelected)
        {
            rectangleSelectChanget(name, isSelected);
            //if (OnSelectinChanged != null)
            //{               
            //    //Raise Event. All the listeners of this event will get a call.               
            //    OnSelectinChanged(name, isSelected);
            //}
        }

        //We notify the parent window about changing the location of the selected Rectangle
        public void NotifyPositionChanged(string name)
        {
            rectanglePositionChanget(name);
            //if (OnPositionChanged != null)
            //{
            //    //Raise Event. All the listeners of this event will get a call.               
            //    OnPositionChanged(name);
            //}
        }

        //When changing the position of the current Renctengl (it's our Element)
        void rectanglePositionChanget(string name)
        {
            //Canvas.SetLeft(rectangle, new_x);
            //Canvas.SetTop(rectangle, new_y);
            try
            {
                switch (toolbox.currentView.elements[name].typeOfElement.ToString())
                {
                    case "text":
                        propertiesbox.text.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();
                        propertiesbox.text.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();
                        if (name == "md_description")
                        {
                            propertiesbox.text.tb_size_h.Text = selected_rectangle.ActualHeight.ToString();
                            propertiesbox.text.tb_size_w.Text = selected_rectangle.ActualWidth.ToString();
                        }
                        break;
                    case "image":
                        propertiesbox.image.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();
                        propertiesbox.image.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();
                        propertiesbox.image.tb_size_h.Text = selected_rectangle.ActualHeight.ToString();
                        propertiesbox.image.tb_size_w.Text = selected_rectangle.ActualWidth.ToString();
                        propertiesbox.image.tb_origin_w.Text = "0";
                        propertiesbox.image.tb_origin_h.Text = "0";
                        break;
                    case "textlist":
                        propertiesbox.textlist.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();
                        propertiesbox.textlist.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();
                        propertiesbox.textlist.tb_size_h.Text = selected_rectangle.ActualHeight.ToString();
                        propertiesbox.textlist.tb_size_w.Text = selected_rectangle.ActualWidth.ToString();
                        break;
                    case "video":
                        propertiesbox.video.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();
                        propertiesbox.video.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();
                        propertiesbox.video.tb_size_h.Text = selected_rectangle.ActualHeight.ToString();
                        propertiesbox.video.tb_size_w.Text = selected_rectangle.ActualWidth.ToString();
                        propertiesbox.video.tb_origin_w.Text = "0";
                        propertiesbox.video.tb_origin_h.Text = "0";
                        break;
                    case "rating":
                        propertiesbox.rating.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();
                        propertiesbox.rating.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();
                        propertiesbox.rating.tb_size_h.Text = selected_rectangle.ActualHeight.ToString();
                        break;
                    case "datetime":
                        propertiesbox.datetime.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();;
                        propertiesbox.datetime.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();;
                        break;
                    case "helpsystem":
                        propertiesbox.helpsystem.tb_pos_h.Text = Canvas.GetTop(selected_rectangle).ToString();;
                        propertiesbox.helpsystem.tb_pos_w.Text = Canvas.GetLeft(selected_rectangle).ToString();;
                        break;
                }
            }
            catch (Exception err)
            {
                Logger.Write(err);
            }
        }

        private void hiddenAllGroupBox()
        {
            List<GroupBox> groupboxes = SomeUtilities.GetLogicalChildCollection<GroupBox>(propertiesbox);

            for (int i = 0; i < groupboxes.Count; i++)
            {
                groupboxes[i].Visibility = System.Windows.Visibility.Hidden;
            }
        }

        //When the selection in Rectengl changes
        void rectangleSelectChanget(string name, bool isSelected)
        {
            //Before changing the rectengl, we process the fields if they are filled.
            //string comboBoxItemvalue;
            //comboBoxItemvalue = getComboBoxValue((ComboBoxItem)cbViewSelector.SelectedItem);
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

                GroupBox foundGroupBox = SomeUtilities.FindChild<GroupBox>(propertiesbox, "gb_base");
                foundGroupBox.Visibility = System.Windows.Visibility.Visible;
                //tb_log.Text = name.ToString();
                return;
            }

            //Save the values in the fields from the current view opened in the viewtmplatewindow window
            if (isSelected)
            {
                //Save view to global variable
                string elementType = toolbox.currentView.elements[name].typeOfElement.ToString();
                if (isSelected)
                {
                    GroupBox foundGrid = SomeUtilities.FindChild<GroupBox>(propertiesbox, "gb_" + elementType);
                    foundGrid.Visibility = System.Windows.Visibility.Visible;
                    //tb_log.Text = name.ToString();
                }
                getPropertiesFromElement(name);
            }
        }

        public void save() 
        {
            if (isOpened)
            {
                addPropertiesToElement(GetSelectedRectengleName);
                _parent.currentItem = toolbox.currentView;
            }
        }

        //Get Properties at the modified Rectangle
        private void getPropertiesFromElement(string name)
        {
            if ((toolbox.currentView != null) && (toolbox.currentView.elements != null) && (toolbox.currentView.elements.Keys.Contains(name)))
            {
                string elementType = toolbox.currentView.elements[name].typeOfElement.ToString();

                if (elementType != Element.types.notexistsname.ToString())
                    switch (elementType)
                    {
                        case "text":
                            propertiesbox.text.Properties = toolbox.currentView.elements[name].Properties;
                            break;
                        case "image":
                            propertiesbox.image.Properties = toolbox.currentView.elements[name].Properties;
                            //if (name == "logo")
                            //    propertiesbox.image.Clear(propertiesbox.tb_logo_system.Text);
                            //else
                                //propertiesbox.image.Clear("");
                            break;
                        case "textlist":
                            propertiesbox.textlist.Properties = toolbox.currentView.elements[name].Properties;
                            break;
                        case "rating":
                            propertiesbox.rating.Properties = toolbox.currentView.elements[name].Properties;
                            break;
                        case "datetime":
                            propertiesbox.datetime.Properties = toolbox.currentView.elements[name].Properties;
                            break;
                        case "helpsystem":
                            propertiesbox.helpsystem.Properties = toolbox.currentView.elements[name].Properties;
                            break;
                    }
            }
        }

        //Transfer Protectis Rectangle
        private void addPropertiesToElement(string name)
        {
            if ((toolbox.currentView != null) && (toolbox.currentView.elements != null) && (toolbox.currentView.elements.Keys.Contains(name)))
                switch (toolbox.currentView.elements[name].typeOfElement.ToString())
                {
                    case "text":
                        toolbox.currentView.elements[name].Properties = propertiesbox.text.Properties;
                        propertiesbox.text.Clear();
                        break;
                    case "image":
                        toolbox.currentView.elements[name].Properties = propertiesbox.image.Properties;
                        propertiesbox.image.Clear("");
                        break;
                    case "textlist":
                        toolbox.currentView.elements[name].Properties = propertiesbox.textlist.Properties;
                        propertiesbox.textlist.Clear();
                        break;
                    case "rating":
                        toolbox.currentView.elements[name].Properties = propertiesbox.rating.Properties;
                        propertiesbox.rating.Clear();
                        break;
                    case "datetime":
                        toolbox.currentView.elements[name].Properties = propertiesbox.datetime.Properties;
                        propertiesbox.datetime.Clear();
                        break;
                    case "helpsystem":
                        toolbox.currentView.elements[name].Properties = propertiesbox.helpsystem.Properties;
                        propertiesbox.helpsystem.Clear();
                        break;
                }
        }

        private void fillItem() 
        {
            for (int i = 0; i < toolbox.currentView.elements.Count; i++)
                CreateRect(toolbox.currentView.elements.Values[i]);
        }

        public void SetCanvasResolution(int width, int height) 
        {
            
            double left = (canvas.ActualWidth - canvas1.ActualWidth) / 2;
            Canvas.SetLeft(canvas1, left);

            double top = (canvas.ActualHeight - canvas1.ActualHeight) / 2;
            Canvas.SetTop(canvas1, top);
            canvas1.Width = width;
            canvas1.Height = height;
            canvas.UpdateLayout();
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && Keyboard.IsKeyDown(Key.Enter))
            {
                if (this.WindowState != WindowState.Maximized && this.WindowStyle != WindowStyle.None)
                {
                    this.WindowState = WindowState.Maximized;
                    this.WindowStyle = WindowStyle.None;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
            }
        }

        #region drag region
        // The part of the rectangle the mouse is over.
        private enum HitType
        {
            None, Body, UL, UR, LR, LL, L, R, T, B
        };

        // True if a drag is in progress.
        private bool _dragInProgress = false;
        public bool DragInProgress
        {
            get { return _dragInProgress; }
            set
            {
                if (_dragInProgress == value)
                    return;
                _dragInProgress = value;
                if (!value)
                {

                    SaveItem(rectangle);
                }
            }
        }
        // The drag's last point.
        private Point LastPoint;
        private RectangleNamed rectangle = new RectangleNamed();
        private RectangleNamed selected_rectangle = new RectangleNamed();
        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(RectangleNamed rect, Point point)
        {
            double left = Canvas.GetLeft(rect);
            double top = Canvas.GetTop(rect);
            double right = left + rect.Width;
            double bottom = top + rect.Height;
            if (double.IsNaN(left) || double.IsNaN(top) || double.IsNaN(right) || double.IsNaN(bottom))
                return HitType.None;
            if (point.X < left) return HitType.None;
            if (point.X > right) return HitType.None;
            if (point.Y < top) return HitType.None;
            if (point.Y > bottom) return HitType.None;

            const double GAP = 10;
            if (point.X - left < GAP)
            {
                // Left edge.
                if (point.Y - top < GAP) return HitType.UL;
                if (bottom - point.Y < GAP) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP)
            {
                // Right edge.
                if (point.Y - top < GAP) return HitType.UR;
                if (bottom - point.Y < GAP) return HitType.LR;
                return HitType.R;
            }
            if (point.Y - top < GAP) return HitType.T;
            if (bottom - point.Y < GAP) return HitType.B;
            return HitType.Body;
        }

        // Set a mouse cursor appropriate for the current hit type.
        private void SetMouseCursor()
        {
            // See what cursor we should display.
            Cursor desired_cursor = Cursors.Arrow;
            switch (MouseHitType)
            {
                case HitType.None:
                    desired_cursor = Cursors.Arrow;
                    break;
                case HitType.Body:
                    desired_cursor = Cursors.ScrollAll;
                    break;
                case HitType.UL:
                case HitType.LR:
                    desired_cursor = Cursors.SizeNWSE;
                    break;
                case HitType.LL:
                case HitType.UR:
                    desired_cursor = Cursors.SizeNESW;
                    break;
                case HitType.T:
                case HitType.B:
                    desired_cursor = Cursors.SizeNS;
                    break;
                case HitType.L:
                case HitType.R:
                    desired_cursor = Cursors.SizeWE;
                    break;
            }

            // Display the desired cursor.
            if (Cursor != desired_cursor) Cursor = desired_cursor;
        }

        // Start dragging.
        public void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas1));
            SetMouseCursor();
            if (selected_rectangle != null)
            if (e.OriginalSource is Canvas)
            {
                selected_rectangle.Unselect();
                NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
                selected_rectangle = null;
                return;
            }
            if (MouseHitType == HitType.None)
            {
                if (selected_rectangle != null)
                {
                    selected_rectangle.Unselect();
                    NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
                }
                return;
            }

            LastPoint = Mouse.GetPosition(canvas1);
            DragInProgress = true;


            try
            {
                if (selected_rectangle != null)
                {
                    selected_rectangle.Unselect();
                    NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
                }
                //We check that one of the elements on RectangleNamed was pressed
                if (e.OriginalSource is Rectangle)
                    selected_rectangle = (RectangleNamed)((Rectangle)e.OriginalSource).Parent;
                if (e.OriginalSource is TextBlock)
                    selected_rectangle = (RectangleNamed)((TextBlock)e.OriginalSource).Parent;
                selected_rectangle.Select();
                toolbox.cb_SelectCreatedelement.SelectedItem = selected_rectangle.Name;
                NotifySelectinChanged(selected_rectangle.Name, selected_rectangle.isSelected);
            }
            catch (Exception err) 
            {
                Logger.Write(err);
            }

        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas1));
                SetMouseCursor();
                try
                {
                    rectangle = (RectangleNamed)((Rectangle)e.OriginalSource).Parent;
                }
                catch (Exception err) 
                {
                    Logger.Write(err);
                }
            }
            else
            {
                // See how much the mouse has moved.
                Point point = Mouse.GetPosition(canvas1);
                double offset_x = point.X - LastPoint.X;
                double offset_y = point.Y - LastPoint.Y;

                // Get the rectangle's current position.
                double new_x = Canvas.GetLeft(rectangle);
                double new_y = Canvas.GetTop(rectangle);
                double new_width = rectangle.Width;
                double new_height = rectangle.Height;

                // Update the rectangle.
                switch (MouseHitType)
                {
                    case HitType.Body:
                        new_x += offset_x;
                        new_y += offset_y;
                        break;
                    case HitType.UL:
                        new_x += offset_x;
                        new_y += offset_y;
                        new_width -= offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.UR:
                        new_y += offset_y;
                        new_width += offset_x;
                        new_height -= offset_y;
                        break;
                    case HitType.LR:
                        new_width += offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.LL:
                        new_x += offset_x;
                        new_width -= offset_x;
                        new_height += offset_y;
                        break;
                    case HitType.L:
                        new_x += offset_x;
                        new_width -= offset_x;
                        break;
                    case HitType.R:
                        new_width += offset_x;
                        break;
                    case HitType.B:
                        new_height += offset_y;
                        break;
                    case HitType.T:
                        new_y += offset_y;
                        new_height -= offset_y;
                        break;
                }

                // Don't use negative width or height.
                if ((new_width > 0) && (new_height > 0))
                {
                    // Update the rectangle.

                    //Check that the new coordinates do not go beyond our Canvas.
                    double xc = Canvas.GetLeft(canvas1);
                    //if (double.IsNaN(Canvas.GetLeft(canvas1)))
                    //{
                        xc = 0;
                    //}
                    //else
                    //    xc = Canvas.GetLeft(canvas1);

                    double yc = Canvas.GetTop(canvas1);
                    //if (double.IsNaN(Canvas.GetTop(canvas1)))
                    //{
                        yc = 0;
                    //}
                    //else
                    //    yc = Canvas.GetTop(canvas1);

                    if (new_x < xc)
                        new_x = xc;
                    if (new_y < yc)
                        new_y = yc;
                    if (new_x + new_width > canvas1.ActualWidth)
                        new_x = canvas1.ActualWidth - new_width;
                    if (new_y + new_height > canvas1.ActualHeight)
                        new_y = canvas1.ActualHeight - new_height;

                    Canvas.SetLeft(rectangle, new_x);
                    Canvas.SetTop(rectangle, new_y);
                    NotifyPositionChanged(selected_rectangle.description);
                    if (new_width > canvas1.ActualWidth)
                        rectangle.Width = canvas1.ActualWidth;
                    else
                        rectangle.Width = new_width;
                    if (new_height > canvas1.ActualHeight)
                        rectangle.Height = canvas1.ActualHeight;
                    else
                        rectangle.Height = new_height;

                    // Save the mouse's new location.
                    LastPoint = point;
                }
            }
        }

        // Stop dragging.
        private void canvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = false;

            if (!DragInProgress && RectangleNamedMouseLeave)
            if ((e.OriginalSource is RectangleNamed) || (e.OriginalSource is Rectangle) || (e.OriginalSource is TextBlock))
            {
                NotifyPositionChanged(selected_rectangle.description);
                if (selected_rectangle != null)
                {
                    if (e.OriginalSource is Rectangle)
                        selected_rectangle = (RectangleNamed)((Rectangle)e.OriginalSource).Parent;
                    if (e.OriginalSource is TextBlock)
                        selected_rectangle = (RectangleNamed)((TextBlock)e.OriginalSource).Parent;
                    selected_rectangle.Select();
                    toolbox.cb_SelectCreatedelement.SelectedItem = selected_rectangle.Name;
                    NotifySelectinChanged(selected_rectangle.Name, selected_rectangle.isSelected);
                }
                else 
                {
                    NotifySelectinChanged("None", false);
                }
            }
        }

        // Stop dragging.
        private void canvas1_MouseLeave(object sender, MouseEventArgs e)
        {
            DragInProgress = false;
            if (selected_rectangle!=null)
                NotifyPositionChanged(selected_rectangle.description);
            MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas1));
            SetMouseCursor();
        }

        //We track that the mouse when going abroad our rectengla did not activate the rectengle over which it may be at this moment
        //отслеживаем чтобы мышь когда выходит за границу нашего ректенгла не активировала ректенгл над которым может оказаться в этот момент
        private void RectangleNamed_MouseLeave(object sender, MouseEventArgs e)
        {
            RectangleNamedMouseLeave = true;
            //SaveItem(rectangle);
        }
        //We track that the mouse when going abroad our rectengla did not activate the rectengle over which it may be at this moment
        //отслеживаем чтобы мышь когда выходит за границу нашего ректенгла не активировала ректенгл над которым может оказаться в этот момент
        private void RectangleNamed_MouseMove(object sender, MouseEventArgs e)
        {
            RectangleNamedMouseLeave = false;
        }
        #endregion drag region

        //static public void BringToFront(Canvas pParent, string pToMoveName)
        //{
        //    try
        //    {
        //        RectangleNamed pToMove = SomeUtilities.FindChild<RectangleNamed>(pParent, pToMoveName);
        //        pParent.Children.Remove(pToMove);
        //        pParent.Children.Add(pToMove);
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        #region Rectangle work
        public void CreateRect(Element item)
        {
            CreateRect(item.name, item.typeOfElement, item.item_fill, item.opacity, item.size_width, item.size_height, item.pos_x, item.pos_y);
        }

        public void CreateRect(string rectname, Element.types typeOfElement, Brush rectcolor, double rectopacity, double width, double height)
        {
            CreateRect(rectname, typeOfElement, rectcolor, rectopacity, width, height, 0, 0);
        }

        public void CreateRect(string rectname, Element.types typeOfElement, Brush rectcolor, double rectopacity, double width, double height, double x, double y) 
        {
            List<RectangleNamed> rctngl = SomeUtilities.GetLogicalChildCollection<RectangleNamed>(canvas1);
            for (int i = 0; i < rctngl.Count; i++)
            {
                if (rctngl[i].Name == rectname)
                    return;
            }
            RectangleNamed rect;
            rect = new RectangleNamed();
            rect.description = rectname;
            rect.MouseMove += RectangleNamed_MouseMove;
            rect.MouseLeave += RectangleNamed_MouseLeave;
            rect.Stroke = new SolidColorBrush(Colors.Black);
            rect.Fill = rectcolor;
            rect.Width = width;
            rect.Height = height;
            rect.Opacity = rectopacity;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            Panel.SetZIndex(rect, 1);
            canvas1.Children.Add(rect);
            SaveItem(rect, typeOfElement);
        }

        //public static void BringToFront(FrameworkElement element)
        //{
        //    if (element == null) return;

        //    Canvas parent = element.Parent as Canvas;
        //    if (parent == null) return;

        //    var maxZ = parent.Children.OfType<UIElement>()
        //      .Where(x => x != element)
        //      .Select(x => Panel.GetZIndex(x))
        //      .Max();
        //    Panel.SetZIndex(element, maxZ + 1);
        //}

        //private int GetMaxZindex() 
        //{
        //    return  canvas1.Children.OfType<UIElement>().Select(x => Panel.GetZIndex(x)).Max();
        //}

         private void SaveItem(RectangleNamed rect) 
        {
            if (toolbox.currentView.elements.IndexOfKey(rect.description) >= 0)
                SaveItem(rect, toolbox.currentView.elements[rect.description].typeOfElement);
        }
        private void SaveItem(RectangleNamed rect, Element.types typeOfElement) 
        {
            Element item = toolbox.currentView.getItem(rect.description);

            if (selected_rectangle != null)
            {
                if (item == null)
                    item = new Element(rect.description, typeOfElement, null, rect.Width, rect.Height, rect.Fill);
                NotifyPositionChanged(selected_rectangle.description);
                {
                    item.opacity = rect.Opacity;
                    item.item_fill = rect.Fill;
                    item.size_width = rect.Width;
                    item.size_height = rect.Height;
                    item.pos_x = Canvas.GetLeft(rect);
                    item.pos_y = Canvas.GetTop(rect);
                    item.pos_x_NORMALIZED = Canvas.GetLeft(rect) / canvas1.Width;
                    item.pos_y_NORMALIZED = Canvas.GetTop(rect) / canvas1.Height;
                    item.size_height_NORMALIZED = rect.Height / canvas1.Height;
                    item.size_width_NORMALIZED = rect.Width / canvas1.Width;
                }
            }
            toolbox.currentView.addItem = item;
        }

        public void RemoveRect(string rectname)
        {
            List<RectangleNamed> rctngl = SomeUtilities.GetLogicalChildCollection<RectangleNamed>(canvas1);
            for (int i = 0; i < rctngl.Count; i++)
            {
                if (rctngl[i].Name == rectname)
                {
                    if (rctngl[i].isSelected)
                    {
                        rctngl[i].Unselect();
                        NotifySelectinChanged(rctngl[i].Name, rctngl[i].isSelected);
                    }
                    canvas1.Children.Remove(rctngl[i]);
                    toolbox.currentView.removeItem(rectname);

                }
            }
        }

        public void SelectRect(string rectname)
        {
            if (selected_rectangle != null)
            {
                selected_rectangle.Unselect();
                NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
            }
            selected_rectangle = SomeUtilities.FindChild<RectangleNamed>(canvas1, rectname);
            selected_rectangle.Select();
            toolbox.cb_SelectCreatedelement.SelectedItem = selected_rectangle.Name;
            NotifySelectinChanged(selected_rectangle.Name, selected_rectangle.isSelected);
        }
        #endregion Rectangle work

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (dp_toolbox.Visibility != System.Windows.Visibility.Visible)
                dp_toolbox.Visibility = System.Windows.Visibility.Visible;
            else
                dp_toolbox.Visibility = System.Windows.Visibility.Hidden;
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (dp_propertiesbox.Visibility != System.Windows.Visibility.Visible)
                dp_propertiesbox.Visibility = System.Windows.Visibility.Visible;
            else
                dp_propertiesbox.Visibility = System.Windows.Visibility.Hidden;
        }
        private void Grid_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            double left = (canvas.ActualWidth - canvas1.ActualWidth) / 2;
            Canvas.SetLeft(canvas1, left);

            double top = (canvas.ActualHeight - canvas1.ActualHeight) / 2;
            Canvas.SetTop(canvas1, top);
        }

        private void btn_BringToFront_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selected_rectangle != null)
                {
                    RectangleNamed pToMove = SomeUtilities.FindChild<RectangleNamed>(canvas1, selected_rectangle.Name);
                    canvas1.Children.Remove(pToMove);
                    canvas1.Children.Add(pToMove);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
