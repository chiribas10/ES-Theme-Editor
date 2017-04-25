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
    /// Interaction logic for RectangleNamed.xaml
    /// </summary>
    public partial class RectangleNamed : Canvas
    {

        private bool _isSelected = false;

        #region Getters and Setters
        public bool isSelected
        {
            get { return _isSelected; }
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

        public Brush Fill
        {
            get { return rectangle.Fill; }
            set
            {
                if (rectangle.Fill == value)
                    return;
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

        public RectangleNamed()
        {
            InitializeComponent();
        }

        public void Unselect()
        {
            _isSelected = false;
            rectangle.Stroke = new SolidColorBrush(Colors.Black);
            rectangle.StrokeThickness = 1;
        }

        public void Select() 
        {
            _isSelected = true;
            rectangle.Stroke = new SolidColorBrush(Colors.Red);
            rectangle.StrokeThickness = 3;
        }
    }
}
