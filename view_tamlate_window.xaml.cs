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
using System.IO;

// Notes:
//  - The Canvas must have a non-null background to make it generate mouse events.

namespace es_theme_editor
{
    public partial class view_tamlate_window : Window
    {
        Es_theme_editor _parent;

        public bool isOpened = false;
        //On/off transparency for Rectangle
        private bool transperentRectangle = false;

        //Initialize window
        public view_tamlate_window()
        {
            InitializeComponent();            
        }

        public view_tamlate_window(Es_theme_editor parent, string name, View view)
        {
            InitializeComponent();
            isOpened = true;

            toolbox.setCurrentView(view);
            toolbox.onElementCreate += this.CreateRect;
            toolbox.onElementRemove += this.RemoveRect;
            toolbox.onElementSelect += this.SelectRect;
            //флаг по которому будем определять какие поля показывать в редакторе форм для 
            //задания обоев, фоновой музыки, лого.
            if (name == "system")
                propertiesbox.isSystem = true;
            propertiesbox.setViewProperties(toolbox.currentView.bgsound, toolbox.currentView.background, toolbox.currentView.logo);
            propertiesbox.onPropertyChanget += this.elementPropertySave;
            propertiesbox.onPropertyViewChanget += viewPropertiesSave;
            this.Title = name;
            fillItem();
            _parent = parent;
            SetCanvasResolution(((App)Application.Current).Width, ((App)Application.Current).Height);
        }

        //Closing window
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selected_rectangle != null)
            {
                selected_rectangle.Unselect();
            }

            _parent.addView(toolbox.currentView);
            isOpened = false;
        }

        //When changing the position of the current Renctengl (it's our Element)
        void rectanglePositionChanget(string name)
        {
            try
            {
                propertiesbox.setPosition(Canvas.GetLeft(selected_rectangle), Canvas.GetTop(selected_rectangle), toolbox.currentView.elements[name].typeOfElement);
                propertiesbox.setSize(selected_rectangle.ActualWidth, selected_rectangle.ActualHeight, toolbox.currentView.elements[name].typeOfElement);
                propertiesbox.setMaxSize(selected_rectangle.ActualWidth, selected_rectangle.ActualHeight, toolbox.currentView.elements[name].typeOfElement);
                propertiesbox.setOrigin(0, 0, toolbox.currentView.elements[name].typeOfElement);
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
            hiddenAllGroupBox();
            //Save the values in the fields to the current view opened in the viewtmplatewindow window
            if (!isSelected)
            {
                if (name != "None")
                    try
                    {
                        addPropertiesToElement(name);
                        SaveItem(selected_rectangle);
                    }
                    catch (Exception err)
                    {
                        Logger.Write(err);
                    }
                propertiesbox.propertiesTabVisibilityChange("base");
                selected_rectangle = null;
                return;
            }

            //Save the values in the fields from the current view opened in the viewtmplatewindow window
            if (isSelected)
            {
                propertiesbox.propertiesTabVisibilityChange(toolbox.currentView.elements[name].typeOfElement.ToString());
                getPropertiesFromElement(name);
                toolbox.cb_SelectCreatedelement.SelectedItem = selected_rectangle.Name;
            }
        }

        public void save() 
        {
            if (isOpened)
            {
                if (selected_rectangle != null)
                    selected_rectangle.Unselect();
                _parent.addView(toolbox.currentView);
            }
        }

        private void elementPropertySave(string name, string value) 
        {
            if (propertiesbox.Visibility != System.Windows.Visibility.Hidden)
                if ((toolbox.currentView != null) && (toolbox.currentView.elements != null) && (toolbox.currentView.elements.Keys.Contains(selected_rectangle.description)))
                    selected_rectangle.addPropertie(name, value);
        }

        private void viewPropertiesSave(string name, string value)
        {
            if ((toolbox.currentView != null))
            {
                if (name != "")
                switch (name)
                {
                    case "bgsound":
                        toolbox.currentView.bgsound = value;
                        break;
                    case "background":
                        toolbox.currentView.background = value;
                        break;
                    case "logo":
                        toolbox.currentView.logo = value;
                        break;
                }
            }
        }

        //Get Properties at the modified Rectangle
        private void getPropertiesFromElement(string name)
        {
            if ((toolbox.currentView != null) && (toolbox.currentView.elements != null) && (toolbox.currentView.elements.Keys.Contains(name)))
            {
                Element.types typeOfElement = toolbox.currentView.elements[name].typeOfElement;

                if (typeOfElement != Element.types.notexistsname)
                    propertiesbox.setPropertiesByType(typeOfElement, toolbox.currentView.elements[name].Properties, toolbox.currentView.elements[name].extra);
            }
        }

        //Transfer Propertis Rectangle
        private void addPropertiesToElement(string name)
        {
            if (propertiesbox.Visibility != System.Windows.Visibility.Hidden)
                if ((toolbox.currentView != null) && (toolbox.currentView.elements != null) && (toolbox.currentView.elements.Keys.Contains(name)))
                {
                    toolbox.currentView.elements[name].Properties = propertiesbox.getPropertiesByType(toolbox.currentView.elements[name].typeOfElement);
                    propertiesbox.Clear(toolbox.currentView.elements[name].typeOfElement);
                }
        }

        private void fillItem() 
        {
            for (int i = 0; i < toolbox.currentView.elements.Count; i++)
                CreateRect(toolbox.currentView.elements.Values[i]);
        }

        public void SetCanvasResolution(double width, double height) 
        {
            //размещаем наш канвас по центру окна
            double left = (canvas.ActualWidth - canvas1.ActualWidth) / 2;
            Canvas.SetLeft(canvas1, left);
            double top = (canvas.ActualHeight - canvas1.ActualHeight) / 2;
            Canvas.SetTop(canvas1, top);

            //Устанавливаем нужный нам размер
            canvas1.Width = width;
            canvas1.Height = height;

            //Обновляем родителя содержащего наш канвас
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

        private void btn_ToolBox_Click(object sender, RoutedEventArgs e)
        {
            if (dp_toolbox.Visibility != System.Windows.Visibility.Visible)
                dp_toolbox.Visibility = System.Windows.Visibility.Visible;
            else
                dp_toolbox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void dp_PropertiesBox_Click(object sender, RoutedEventArgs e)
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

        private void btn_setBackground_Click(object sender, RoutedEventArgs e)
        {
            string filename;
            filename = toolbox.currentView.background;
            if (filename == "")
                return;
            //if (this.Title.ToLower() == "system")
            //    filename = ((App)Application.Current).backgroundImagePath;
            //else
            //    filename = ((App)Application.Current).backgroundSystemImagePath;
            filename = SomeUtilities.MakeAbsolutePath(((App)Application.Current).themefolder + ((App)Application.Current).gameplatformtheme + "\\theme.xml", filename);
            if (!File.Exists(filename))
                return;
            Image image = SomeUtilities.FindChild<Image>(canvas, "Backgroundimage");
            if (image != null)
            {
                if (image.Visibility == System.Windows.Visibility.Visible)
                    image.Visibility = System.Windows.Visibility.Hidden;
                else
                {
                    image.Visibility = System.Windows.Visibility.Visible;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filename);
                    bitmap.EndInit();

                    //image = new Image();
                    image.Name = "Backgroundimage";
                    image.Source = bitmap;
                    image.Width = canvas1.Width;
                    image.Height = canvas1.Height;
                    image.MouseDown += canvas1_MouseDown;
                    //Panel.SetZIndex(image, 0);
                    //canvas1.Children.Add(image);
                }
            }
            else
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filename);
                bitmap.EndInit();

                image = new Image();
                image.Name = "Backgroundimage";
                image.Source = bitmap;
                image.Width = canvas1.Width;
                image.Height = canvas1.Height;
                image.MouseDown += canvas1_MouseDown;
                Panel.SetZIndex(image, 0);
                canvas1.Children.Add(image);
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
        private RectangleNamed rectangle;
        private RectangleNamed selected_rectangle;
        // The part of the rectangle under the mouse.
        HitType MouseHitType = HitType.None;

        // Return a HitType value to indicate what is at the point.
        private HitType SetHitType(RectangleNamed rect, Point point)
        {
            if (rect == null)
                return HitType.None;
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
            if (point.X - left < GAP && rectangle.horizontalResizing)
            {
                // Left edge.
                if (point.Y - top < GAP && rectangle.verticallResizing) return HitType.UL;
                if (bottom - point.Y < GAP && rectangle.verticallResizing) return HitType.LL;
                return HitType.L;
            }
            if (right - point.X < GAP && rectangle.horizontalResizing)
            {
                // Right edge.
                if (point.Y - top < GAP && rectangle.verticallResizing) return HitType.UR;
                if (bottom - point.Y < GAP && rectangle.verticallResizing) return HitType.LR;
                return HitType.R;
            }
            //if (rectangle.verticallResizing)
            //{
            if (point.Y - top < GAP && rectangle.verticallResizing) return HitType.T;
            if (bottom - point.Y < GAP && rectangle.verticallResizing) return HitType.B;
            return HitType.Body;
            //}

            //return HitType.None;
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
            if (MouseHitType == HitType.None)
            {
                if (selected_rectangle != null)
                {
                    selected_rectangle.Unselect();
                    selected_rectangle = null;
                }
                return;
            }

            LastPoint = Mouse.GetPosition(canvas1);
            DragInProgress = true;
        }

        // If a drag is in progress, continue the drag.
        // Otherwise display the correct cursor.
        private void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (MouseHitType == HitType.None)
            //{
            //    DragInProgress = false;
            //    return;
            //}
            if (rectangle == null)
                return;
            if (!DragInProgress)
            {
                MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas1));
                SetMouseCursor();
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
                    //double xc = Canvas.GetLeft(canvas1);
                    double xc = 0;
                    //double yc = Canvas.GetTop(canvas1);
                    double yc = 0;

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
                    if (selected_rectangle!=null)
                        rectanglePositionChanget(selected_rectangle.description);
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
        }

        // Stop dragging.
        private void canvas1_MouseLeave(object sender, MouseEventArgs e)
        {
            DragInProgress = false;
            MouseHitType = SetHitType(rectangle, Mouse.GetPosition(canvas1));
            SetMouseCursor();
        }

        //We track that the mouse when going abroad our rectengla did not activate the rectengle over which it may be at this moment
        //отслеживаем чтобы мышь когда выходит за границу нашего ректенгла не активировала ректенгл над которым может оказаться в этот момент
        private void RectangleNamed_MouseLeave(object sender, MouseEventArgs e)
        {
            SetMouseCursor();
        }

        //We track that the mouse when going abroad our rectengla did not activate the rectengle over which it may be at this moment
        //отслеживаем чтобы мышь когда выходит за границу нашего ректенгла не активировала ректенгл над которым может оказаться в этот момент
        private void RectangleNamed_MouseMove(object sender, MouseEventArgs e)
        {
            if (!DragInProgress)
                if (sender is RectangleNamed)
                {
                    rectangle = (RectangleNamed)sender;
                }
        }

        public void RectangleNamed_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragInProgress = true;
            if (sender is RectangleNamed)
            {
                if (selected_rectangle != null)
                {
                    SaveItem(rectangle);
                    selected_rectangle.Unselect();
                    selected_rectangle = null;
                }
                selected_rectangle = (RectangleNamed)sender;
                selected_rectangle.Select();
            }
        }

        #endregion drag region

        #region Rectangle work

        //        static int GCD(int a, int b)
        //{
        //    int Remainder;

        //    while( b != 0 )
        //    {
        //        Remainder = a % b;
        //        a = b;
        //        b = Remainder;
        //    }

        //    return a;
        //}

        //return string.Format("{0}:{1}",x/GCD(x,y), y/GCD(x,y));

        //We pass the view_tamlate_window task to create a new rectengl
        private void CreateRect(string name, Element.types typeOfElement, Brush fill, double opacity)
        {
            if (!transperentRectangle)
                opacity = 1;
            //0,2291666666666667
            //For rating stars, we set the template size smaller than the rest
            if (typeOfElement == Element.types.text || typeOfElement == Element.types.datetime)
                CreateRect(name, typeOfElement, fill, opacity, 200, 40); //200, 40
            else
            switch (name)
            {
                case "md_rating":
                    CreateRect(name, typeOfElement, fill, opacity, 30, 30);
                    break;
                case "help":
                    CreateRect(name, typeOfElement, fill, opacity, canvas1.ActualWidth, canvas1.ActualHeight * 0.0648148148148148, 0, canvas1.ActualHeight - (canvas1.ActualHeight * 0.0648148148148148), false, false);//для 1920х1080 - Height * 0.0648148148148148 = 70
                    break;
                case "systemcarousel":
                    CreateRect(name, typeOfElement, fill, opacity, canvas1.ActualWidth, canvas1.ActualHeight * 0.2291666666666667, 0, canvas1.ActualHeight/2 - (canvas1.ActualHeight * 0.2291666666666667)/2, false);
                    break;
                default:
                    CreateRect(name, typeOfElement, fill, opacity, 200, 200);
                    break;
            }

            toolbox.addCreatedelement(name);
            if (toolbox.currentView.elements.ContainsKey(name))
            {
                //if (toolbox.currentView.elements[name].Properties.Count == 0)
                propertiesbox.setPropertiesByType(typeOfElement, toolbox.currentView.elements[name].Properties, toolbox.currentView.elements[name].extra);
                toolbox.currentView.elements[name].Properties = propertiesbox.getPropertiesByType(typeOfElement);
                //else
                //    propertiesbox.setPropertiesByType(typeOfElement, toolbox.currentView.elements[name].Properties, toolbox.currentView.elements[name].extra);
            }
        }

        public void CreateRect(Element element)
        {
            RectangleNamed rect = new RectangleNamed(element);
            rect.MouseMove += RectangleNamed_MouseMove;
            rect.MouseLeave += RectangleNamed_MouseLeave;
            rect.MouseDown += RectangleNamed_MouseDown;
            rect.onRectangleNamedSelectedChange += rectangleSelectChanget;
            canvas1.Children.Add(rect);
            SaveItem(rect, element.typeOfElement);
        }

        public void CreateRect(string rectname, Element.types typeOfElement, Brush rectcolor, double rectopacity, double width, double height)
        {
            CreateRect(rectname, typeOfElement, rectcolor, rectopacity, width, height, 0, 0);
        }

        public void CreateRect(string rectname, Element.types typeOfElement, Brush rectcolor, double rectopacity, double width, double height, double x, double y, bool horizontalResizing = true, bool verticallResizing = true) 
        {
            List<RectangleNamed> rctngl = SomeUtilities.GetLogicalChildCollection<RectangleNamed>(canvas1);
            for (int i = 0; i < rctngl.Count; i++)
            {
                if (rctngl[i].Name == rectname)
                    return;
            }
            RectangleNamed rect;
            rect = new RectangleNamed(rectname, typeOfElement, rectcolor, rectopacity, width, height, x, y);
            rect.MouseMove += RectangleNamed_MouseMove;
            rect.MouseLeave += RectangleNamed_MouseLeave;
            rect.MouseDown += RectangleNamed_MouseDown;
            rect.onRectangleNamedSelectedChange += rectangleSelectChanget;
            rect.horizontalResizing = horizontalResizing;
            rect.verticallResizing = verticallResizing;
            SaveItem(rect, typeOfElement);
            rect.fillProperties(propertiesbox.getPropertiesByType(typeOfElement));
            canvas1.Children.Add(rect);
            
        }

         private void SaveItem(RectangleNamed rect) 
        {
            if (rect == null)
                return;
            if (toolbox.currentView.elements.IndexOfKey(rect.description) >= 0)
                SaveItem(rect, toolbox.currentView.elements[rect.description].typeOfElement);
        }
        private void SaveItem(RectangleNamed rect, Element.types typeOfElement) 
        {
            Element item = toolbox.currentView.getItem(rect.description);
            if (item == null)
                item = new Element(rect.description, typeOfElement, null, rect.Width, rect.Height, rect.Fill);
            item.applyChanges(rect.Opacity, rect.Fill, rect.Width, rect.Height, Canvas.GetLeft(rect), Canvas.GetTop(rect), rect.getExtra);
            
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
            }
            selected_rectangle = SomeUtilities.FindChild<RectangleNamed>(canvas1, rectname);
            selected_rectangle.Select();
        }
        #endregion Rectangle work
        
    }
}
