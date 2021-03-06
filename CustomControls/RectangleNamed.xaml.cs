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
using System.IO;

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for RectangleNamed.xaml
    /// </summary>
    public partial class RectangleNamed : Canvas
    {
        public delegate void RectangleNamedSelected(string name, bool isSelected);
        public event RectangleNamedSelected onRectangleNamedSelectedChange;
        private bool _isSelected = false;
        private bool _imageSetted;
        public Element element;
        public bool horizontalResizing = true;
        public bool verticallResizing = true;

        #region Getters and Setters
        public bool isSelected
        {
            get { return _isSelected; }
        }

        public string getExtra 
        {
            get { return element.extra; }
        }

        public Brush Stroke
        {
            get { return rectangle.Stroke; }
            set
            {
                if (rectangle.Stroke == value)
                    return;
                rectangle.Stroke = value;
            }
        }

        public string description
        {
            get { return canvas.Name; }
            set
            {
                if (canvas.Name == value && desc.Text == value)
                    return;
                canvas.Name = value;
                desc.Text = value;
            }
        }

        private void showVisual(bool setted)
        {
            if (setted)
            {
                rectangle.Fill = Brushes.Transparent;

                //if (!isSelected)
                //    rectangle.Stroke = new SolidColorBrush(Colors.Transparent);
                _imageSetted = true;
                this.desc.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                rectangle.Fill = rectangleFill;
                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.StrokeThickness = 1;
                _imageSetted = false;
            }
        }
        private Brush rectangleFill;
        public Brush Fill
        {
            get { return rectangleFill; }
            set
            {
                rectangleFill = value;
                rectangle.Fill = value;
            }
        }

        public double StrokeThickness
        {
            get { return rectangle.StrokeThickness; }
            set
            {
                if (rectangle.StrokeThickness == value)
                    return;
                rectangle.StrokeThickness = value;
            }
        }
        #endregion Getters and Setters


        //public RectangleNamed()
        //{
        //    InitializeComponent();
        //}

        public RectangleNamed(Element originalElement)
        {
            if (originalElement == null)
                return;

            InitializeComponent();

            element = originalElement;
            this.description = element.name;// +element.item_fill.ToString().Replace("#", "_");
            this.Stroke = new SolidColorBrush(Colors.Black);
            this.Fill = element.item_fill;
            this.Width = element.size_width;
            this.Height = element.size_height;
            this.Opacity = element.opacity;
            Canvas.SetLeft(this, element.pos_x);
            Canvas.SetTop(this, element.pos_y);
            Panel.SetZIndex(this, 1);

            fillProperties(element.Properties);

        }

        public void fillProperties(SortedList<string, string> Properties) 
        {
            if (Properties == null)
                return;
            for (int i = 0; i < Properties.Count; i++)
            {
                addPropertie(Properties.Keys[i], Properties.Values[i]);
            }
        }

        public RectangleNamed(string rectname, Element.types typeOfElement, Brush rectcolor, double rectopacity, double width, double height, double x, double y)
        {
            InitializeComponent();

            this.description = rectname;
            this.Stroke = new SolidColorBrush(Colors.Black);
            this.Fill = rectcolor;
            this.Width = width;
            this.Height = height;
            this.Opacity = rectopacity;
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            Panel.SetZIndex(this, 1);

            if (element == null)
                element = new Element(this.description, typeOfElement, width, height, rectcolor);
            else
                fillElement();
        }

        private void fillElement() 
        {
            element.opacity = this.Opacity;
            element.item_fill = this.Fill;
            element.size_width = this.Width;
            element.size_height = this.Height;
            element.pos_x = Canvas.GetLeft(this);
            element.pos_y = Canvas.GetTop(this);
            element.pos_x_NORMALIZED = Canvas.GetLeft(this) / ((App)Application.Current).Width;
            element.pos_y_NORMALIZED = Canvas.GetTop(this) / ((App)Application.Current).Height;
            element.size_height_NORMALIZED = this.Height / ((App)Application.Current).Height;
            element.size_width_NORMALIZED = this.Width / ((App)Application.Current).Width;
        }
        
        public void addPropertie(string name, string value)
        {

            if (name == "")
                return;
            element.addPropertie(name, value);
            if (name == "extra")
                element.extra = value;
            else
            switch (element.typeOfElement.ToString())
            {
                case "text":
                    if (string.IsNullOrEmpty(textControl.somestring))
                        textControl.somestring = description;
                    textControl.setProperty(name, value);
                    if (textControl.Visibility == System.Windows.Visibility.Visible)
                        showVisual(true);
                    break;
                case "image":
                case "rating":
                    imageControl.setProperty(name, value);
                    if (imageControl.Visibility == System.Windows.Visibility.Visible)
                        showVisual(true);
                    break;
                case "textlist":
                    textListControl.setProperty(name, value);
                    if (textListControl.Visibility == System.Windows.Visibility.Visible)
                        showVisual(true);
                    break;
                case "datetime":
                    if (string.IsNullOrEmpty(textControl.somestring))
                    {
                        DateTime dt = DateTime.Now;
                        textControl.somestring = dt.ToString();
                    }
                    textControl.setProperty(name, value);
                    if (textControl.Visibility == System.Windows.Visibility.Visible)
                        showVisual(true);
                    break;
                case "carousel":
                    if (name == "type")
                    {
                        if (value == "vertical")
                        {
                            this.Width = ((App)Application.Current).Width * 0.2291666666666667;
                            this.Height = ((App)Application.Current).Height;
                            Canvas.SetLeft(this, 0);
                            Canvas.SetTop(this, 0);
                            horizontalResizing = true;
                            verticallResizing = false;
                        }
                        else
                        {
                            this.Width = ((App)Application.Current).Width;
                            this.Height = ((App)Application.Current).Height * 0.2291666666666667;
                            Canvas.SetLeft(this, 0);
                            Canvas.SetTop(this, ((App)Application.Current).Height / 2 - (((App)Application.Current).Height * 0.2291666666666667) / 2);
                            horizontalResizing = false;
                            verticallResizing = true;
                        }
                    }
                    if (name == "color")
                    {
                        rectangle.Fill = SomeUtilities.GetBrushFromHex(value);
                    }
                    break;
                case "helpsystem":
                    break;
            }
        }

        public void Unselect()
        {
            if (Name != "canvas")
            {
                _isSelected = false;
                //if (!_imageSetted)
                //    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                //else
                //    rectangle.Stroke = new SolidColorBrush(Colors.Transparent);
                rectangle.Stroke = new SolidColorBrush(Colors.Black);
                rectangle.StrokeThickness = 1;
                onRectangleNamedSelectedChange(this.description, false);
            }
        }

        public void Select()
        {
            if (Name != "canvas")
            {
                _isSelected = true;
                rectangle.Stroke = new SolidColorBrush(Colors.Red);
                rectangle.StrokeThickness = 3;
                onRectangleNamedSelectedChange(this.description, true);
            }
        }
    }
}
