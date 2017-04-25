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
        public event NotifySelectRectangleChanging OnSelectinChanged;
        public event NotifyPositionRectangleChanging OnPositionChanged;
        public bool RectangleNamedMouseLeave = true;
        public View view;

        //Inicialize window
        //public view_tamlate_window()
        //{
        //    InitializeComponent();            
        //}

        public view_tamlate_window(Es_theme_editor parent, string name, View view)
        {
            InitializeComponent();
            isOpened = true;
            this.view = view;
            this.Title = name;
            fillItem();
            _parent = parent;
            SetCanvasResolution(view.width, view.height);
        }

        //Closing window
        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (selected_rectangle != null)
            {
                selected_rectangle.Unselect();
                NotifySelectinChanged(selected_rectangle.description, selected_rectangle.isSelected);
            }

            _parent.currentItem = view;
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
            if (OnSelectinChanged != null)
            {               
                //Raise Event. All the listeners of this event will get a call.               
                OnSelectinChanged(name, isSelected);
            }
        }

        //We notify the parent window about changing the location of the selected Rectangle
        public void NotifyPositionChanged(string name)
        {
            if (OnPositionChanged != null)
            {
                //Raise Event. All the listeners of this event will get a call.               
                OnPositionChanged(name);
            }
        }

        private void fillItem() 
        {
            for (int i = 0; i < view.elements.Count; i++)
                CreateRect(view.elements.Values[i]);
        }

        public void SetCanvasResolution(int width, int height) 
        {
            canvas1.Width = width;
            canvas1.Height = height;
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
        private void canvas1_MouseDown(object sender, MouseButtonEventArgs e)
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
            if (MouseHitType == HitType.None) return;

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
                    if (double.IsNaN(Canvas.GetLeft(canvas1)))
                    {
                        xc = 0;
                    }
                    else
                        xc = Canvas.GetLeft(canvas1);

                    double yc = Canvas.GetTop(canvas1);
                    if (double.IsNaN(Canvas.GetTop(canvas1)))
                    {
                        yc = 0;
                    }
                    else
                        yc = Canvas.GetTop(canvas1);

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
            canvas1.Children.Add(rect);
            SaveItem(rect, typeOfElement);
        }        

         private void SaveItem(RectangleNamed rect) 
        {
            if (view.elements.IndexOfKey(rect.description) >= 0)
                SaveItem(rect, view.elements[rect.description].typeOfElement);
        }
        private void SaveItem(RectangleNamed rect, Element.types typeOfElement) 
        {
            Element item = view.getItem(rect.description);

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
            view.addItem = item;
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
                    view.removeItem(rectname);

                }
            }
        }
        #endregion Rectangle work
    }
}
