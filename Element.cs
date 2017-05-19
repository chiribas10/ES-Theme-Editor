using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace es_theme_editor
{
    public class Element
    {
        public enum types
        {
            notset = 0,
            helpsystem = 1,  // + pos, textColor, iconColor, fontPath, fontSize (Size of the font as a percentage of screen height (e.g. for a value of 0.1, the text's height would be 10% of the screen height).)
            image = 2,       // + pos, size, maxSize, origin, path, tile, color 
            text = 3,        // + pos, size, text, color, fontPath, fontSize (Size of the font as a percentage of screen height (e.g. for a value of 0.1, the text's height would be 10% of the screen height).), alignment, forceUppercase, lineSpacing.
            textlist = 4,    // + pos, size, selectorColor, selectedColor, primaryColor, secondaryColor, fontPath, fontSize, scrollSound, alignment, horizontalMargin, forceUppercase, lineSpacing.
            datetime = 5,    // + pos, size, color, fontPath, fontSize, forceUppercase 
            rating = 6,      // + pos, size, filledPath, unfilledPath 
            sound = 7,       // + path
            video = 8,        // + pos, size, maxSize, origin, tile
            carousel = 9, 
            notexistsname = 10
        };

        public bool feature = false;
        //Elements that should be hidden if the user did not specify specific parameters for them
        public List<string> novisibleelement = new List<string>(new string[]{"help", "md_lbl_rating", 
                    "md_lbl_releasedate", "md_lbl_developer", "md_lbl_publisher", "md_lbl_genre", "md_lbl_players", 
                    "md_lbl_lastplayed", "md_lbl_playcount", "md_developer", "md_publisher", "md_genre", "md_players", 
                    "md_playcount", "md_rating", "md_releasedate", "md_lastplayed"});

        //<feature supported="carousel"></feature>
        //public static List<string> featureelement = new List<string>(new string[]{"video", "carousel"});

        public SortedList<string, string> _properties = new SortedList<string, string>();

        private types _typeOfElement;
        private string _name;
        Brush _item_fill;
        double _opacity;
        public string extra = "";

        //Dimensions and position in pixels
        double _size_width = 0;
        double _size_height = 0;
        double _pos_x = 1;
        double _pos_y = 1;

        //Dimensions and position normalized for emulationstation theme
        double _size_width_NORMALIZED = 1;
        double _size_height_NORMALIZED = 1;
        double _pos_x_NORMALIZED = 1;
        double _pos_y_NORMALIZED = 1;

        private double _origin_h = 0;
        private double _origin_w = 0;

        public Element(string name, types typeOfElement, SortedList<string, string> Properties, double Width, double Height, Brush item_fill)
        {
            _typeOfElement = typeOfElement;
            this.name = name;
            if (Properties!=null)
                filligFromProperties(Properties, Width, Height, item_fill);
        }

        public Element(string name, types typeOfElement, double Width, double Height, Brush item_fill)
        {
            _typeOfElement = typeOfElement;
            this.name = name;
            _size_width = Width;
            _size_height = Height;

            //if (Properties != null)
            //    filligFromProperties(Properties, Width, Height, item_fill);
        }

        public void addPropertie(string name, string value)
        {
            if (name == "")
                return;
            if (_properties.IndexOfKey(name) >= 0)
                _properties.Remove(name);
            _properties.Add(name, value);

        }

        //We will fill values from Properties
        public void filligFromProperties(SortedList<string, string> Properties, double Width, double Height, Brush item_fill)
        {
            string val;
            Char delimiter = ' ';
            String[] substrings;
            _properties.Remove("pos");
            _properties.Remove("size");
            _properties.Remove("maxSize");
            _properties.Remove("origin");

            //In the array of properties we introduce new values and if there have already been some replaced
            for (int i = 0; i < Properties.Count; i++)
            {
                if (this.Properties.IndexOfKey(Properties.Keys[i]) >= 0)
                {
                    _properties.Remove(Properties.Keys[i]);
                }

                _properties.Add(Properties.Keys[i], Properties.Values[i]);
            }
            val = Properties.FirstOrDefault(x => x.Key == "pos").Value;
            if (val != null)
            {
                substrings = val.Split(delimiter);
                _pos_x_NORMALIZED = double.Parse(substrings[0].Trim().Replace(".", ","));
                _pos_y_NORMALIZED = double.Parse(substrings[1].Trim().Replace(".", ","));
                _pos_x = _pos_x_NORMALIZED * ((App)Application.Current).Width;
                _pos_y = _pos_y_NORMALIZED * ((App)Application.Current).Height;
            }
            else 
            {
                switch (name)
                {
                    case "help":
                        _pos_x = 0;
                        _pos_y = ((App)Application.Current).Height - (((App)Application.Current).Height * 0.0648148148148148);
                        break;
                    case "systemcarousel":
                        _pos_x = 0;
                        _pos_y = ((App)Application.Current).Height / 2 - (((App)Application.Current).Height * 0.2291666666666667) / 2;
                        break;
                    default:
                        _pos_x = 0;
                        _pos_y = 0;
                        break;
                }
            }
            val = Properties.FirstOrDefault(x => x.Key == "size").Value;
            if (val != null)
            {
                substrings = val.Split(delimiter);
                size_width_NORMALIZED = double.Parse(substrings[0].Trim().Replace(".", ","));
                size_height_NORMALIZED = double.Parse(substrings[1].Trim().Replace(".", ","));
                _size_width = size_width_NORMALIZED * ((App)Application.Current).Width;
                _size_height = size_height_NORMALIZED * ((App)Application.Current).Height;
            }
            else
            {
                if (typeOfElement == Element.types.text || typeOfElement == Element.types.datetime)
                {
                    _size_width = 200;
                    _size_height = 40; //200, 40
                }
                else
                    switch (name)
                    {
                        case "md_rating":
                            _size_width = 30;
                            _size_height = 30;
                            break;
                        case "help":
                            _size_width = ((App)Application.Current).Width;
                            _size_height = ((App)Application.Current).Height * 0.0648148148148148;
                            break;
                        case "systemcarousel":
                            _size_width = ((App)Application.Current).Width;
                            _size_height = ((App)Application.Current).Height * 0.2291666666666667;
                            break;
                        default:
                            _size_width = 200;
                            _size_height = 200;
                            break;
                    }
            }
            val = Properties.FirstOrDefault(x => x.Key == "origin").Value;
            if (val != null)
            {
                substrings = val.Split(delimiter);
                _origin_w = double.Parse(substrings[0].Trim().Replace(".", ","));
                _origin_h = double.Parse(substrings[1].Trim().Replace(".", ","));
            }
            else 
            {
                _origin_w = 0;
                _origin_h = 0;
            }
            val = Properties.FirstOrDefault(x => x.Key == "maxSize").Value;
            if (val != null)
            {
                substrings = val.Split(delimiter);
                size_width_NORMALIZED = double.Parse(substrings[0].Trim().Replace(".", ","));
                size_height_NORMALIZED = double.Parse(substrings[1].Trim().Replace(".", ","));
                _size_width = size_width_NORMALIZED * ((App)Application.Current).Width;
                _size_height = size_height_NORMALIZED * ((App)Application.Current).Height;
            }
            this._item_fill = item_fill;
            opacity = 1;
        }

        public Brush item_fill
        {
            get 
            {
                if (_item_fill == null)
                {
                    _item_fill = SomeUtilities.GetRandomColor();
                }
                return _item_fill; 
            }
            set
            {
                _item_fill = value;
            }
        }

        public string path
        {
            get 
            {
                string val;
                val = Properties.FirstOrDefault(x => x.Key == "path").Value;
                if (val!=null)
                    return val;
                return ""; 
            }
        }

        public void applyChanges(double opacity, Brush fill, double width, double height, double x, double y, string extra) 
        {
            this.extra = extra;
            this.opacity = opacity;
            item_fill = fill;
            size_width = width;
            size_height = height;
            pos_x = x;
            pos_y = y;
            pos_x_NORMALIZED = x / ((App)Application.Current).Width;
            pos_y_NORMALIZED = y / ((App)Application.Current).Height;
            size_height_NORMALIZED = height / ((App)Application.Current).Height;
            size_width_NORMALIZED = width / ((App)Application.Current).Width;
            saveProperties();
        }

        public void saveProperties() 
        {
            if (_properties == null)
                _properties = new SortedList<string, string>();
            _properties.Remove("pos");
            _properties.Remove("size");
            _properties.Remove("maxSize");
            _properties.Remove("origin");
            ////Add to our array the properties that are set in view_tamlate_window
            switch (_typeOfElement)
            {
                case types.helpsystem:
                    //Не создаем свойство pos для элемента HELP если его не изменяли. Оставим свойства по-умолчанию.
                    if ((int)pos_y != (int)(((App)Application.Current).Height - _size_height))
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                        //_properties.Add("size", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                    break;
                case types.datetime:
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                    //size - You should probably not set this. Leave it to fontSize.
                    break;
                case types.text:
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                    //if (name == "md_description")
                    //{
                            _properties.Add("size", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                    //}
                    //size - type: NORMALIZED_PAIR.
                    //Possible combinations:
                    //0 0 - automatically size so text fits on one line (expanding horizontally).
                    //w 0 - automatically wrap text so it doesn't go beyond w (expanding vertically).
                    //w h - works like a "text box." If h is non-zero and h <= fontSize (implying it should be a single line of text), text that goes beyond w will be truncated with an elipses (...).
                    break;
                case types.textlist:
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                        _properties.Add("size", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                    break;
                case types.image:
                case types.video:
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                        if (size_width_NORMALIZED != 0 && size_height_NORMALIZED != 0)
                            _properties.Add("maxSize", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                        _properties.Add("origin", (_origin_w + " " + _origin_h).Replace(",", "."));
                    break;
                case types.rating:// only one indicator is used to determine the size (in this case we use only the height)
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                        _properties.Add("size", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                    //size - Only one value is actually used. The other value should be zero. (e.g. specify width OR height, but not both. This is done to maintain the aspect ratio.)
                    break;
                case types.carousel:// only one indicator is used to determine the size (in this case we use only the height)
                        _properties.Add("pos", (pos_x_NORMALIZED + " " + pos_y_NORMALIZED).Replace(",", "."));
                        _properties.Add("size", (size_width_NORMALIZED + " " + size_height_NORMALIZED).Replace(",", "."));
                    //size - Only one value is actually used. The other value should be zero. (e.g. specify width OR height, but not both. This is done to maintain the aspect ratio.)
                    break;
            }
        }

        // When assigning a property, we supplement those that are filled on the main form with the size and position that are assigned to this element in dependence
        // from the position and size of the element on the form view_tamlate_window
        public SortedList<string, string> Properties
        {
            get 
            {
                //if (_typeOfElement == types.text && name != "md_description")
                //    _properties.Remove("size");
                return _properties; 
            }
            set
            {
                //Filling the values from the properties
                _properties = value;
            }
        }

        public types typeOfElement
        {
            get 
            {
                if (_typeOfElement != types.notset)
                    return _typeOfElement;
                else
                    return GetType(name);
            }
        }

        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        // for each standard element with a specific name has its own type
        // so we define it in reference to the name.
        public static types GetType(string name) 
        {
            switch (name)
            {
                case "help":
                    return types.helpsystem;
                case "md_description":
                case "logoText":
                case "md_lbl_rating":
                case "md_lbl_releasedate":
                case "md_lbl_developer":
                case "md_lbl_publisher":
                case "md_lbl_genre":
                case "md_lbl_players":                
                case "md_lbl_playcount":
                case "md_developer":
                case "md_publisher":
                case "md_genre":
                case "md_players":
                case "md_playcount":
                case "systemInfo":                    
                    return types.text;
                case "logo":
                case "md_image":
                case "md_marquee":
                    return types.image;
                case "md_rating":
                    return types.rating;
                case "md_releasedate":
                case "md_lastplayed":
                case "md_lbl_lastplayed":
                    return types.datetime;
                case "gamelist":
                    return types.textlist;
                case "md_video":
                    return types.video;
                case "systemcarousel":
                    return types.carousel;                    
                default:
                    return types.notexistsname;
            }
        }

        public double opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity == value)
                    return;
                _opacity = value;
            }
        }

        public double size_width
        {
            get { return _size_width; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _size_width = value;
            }
        }

        public double size_height
        {
            get { return _size_height; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _size_height = value;
            }
        }

        public double pos_x
        {
            get { return _pos_x; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _pos_x = value;
            }
        }

        public double pos_y
        {
            get { return _pos_y; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _pos_y = value;
            }
        }

        public double size_width_NORMALIZED
        {
            get 
            { 
                return _size_width_NORMALIZED; 
            }
            set
            {
                if (double.IsNaN(value))
                    return;
                _size_width_NORMALIZED = value;
            }
        }

        public double size_height_NORMALIZED
        {
            get { return _size_height_NORMALIZED; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _size_height_NORMALIZED = value;
            }
        }

        public double pos_x_NORMALIZED
        {
            get { return _pos_x_NORMALIZED; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _pos_x_NORMALIZED = value;
            }
        }

        public double pos_y_NORMALIZED
        {
            get { return _pos_y_NORMALIZED; }
            set
            {
                if (double.IsNaN(value))
                    return;
                _pos_y_NORMALIZED = value;
            }
        }

        //Для сравнения текущего Element с другим
        public override bool Equals(object obj)
        {
            // Сравнение с null всегда возвращает false
            if (obj == null)
                return false;
            // Если сравниваемые объекты имеют разный тип, равенство не верно
            if (obj.GetType() != this.GetType())
                return false;
            // Вызываем специфический метод сравнения
            return Equals((Element)obj);
        }

        // Реализация интерфейса IEquatable<T>
        public bool Equals(Element other)
        {
            string key;
            bool res;
            res = this._name == other._name;
            if (!res)
                return false;
            res =  this._origin_h == other._origin_h;
            if (!res)
                return false;
            res =  this._origin_w == other._origin_w;
            if (!res)
                return false;
            res =  this._pos_x == other._pos_x;
            if (!res)
                return false;
            res =  this._pos_y == other._pos_y;
            if (!res)
                return false;
            for (int i = 0; i < this._properties.Count; i++)
            {
                key = this._properties.Keys[i];
                if (other._properties.IndexOfKey(key) < 0)
                    return false;
                if (this._properties.Values[i] != other._properties[this._properties.Keys[i]])
                    return false;
            }
            res =  this._size_height == other._size_height;
            if (!res)
                return false;
            res =  this._size_width == other._size_width;
            if (!res)
                return false;
            res = this._typeOfElement == other._typeOfElement;
            if (!res)
                return false;
            res = this.extra == other.extra;
            if (!res)
                return false;
            return res;
        }

    }
}
