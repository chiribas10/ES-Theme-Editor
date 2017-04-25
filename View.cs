using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace es_theme_editor
{
    public class View
    {
        public enum types
        {
            basic = 1,
            detailed = 2,
            system = 3,
            video = 4,
            none = 5
        };

        private types typeOfView;

        private string _name;
        private int _width;
        private int _height;

        public View(types type, int width, int height)
        {
            this.typeOfView = type;
            _name = Enum.GetName(typeof(View.types), type);
            this.width = width;
            this.height = height;
            _elements = new SortedList<string, Element>();
        }

        // for each standard element with a specific name has its own type
        // so we define it in reference to the name.
        public static types GetType(string name)
        {
            switch (name)
            {
                case "basic":
                    return types.basic;
                case "detailed":
                    return types.detailed;
                case "system":
                    return types.system;
                case "video":
                    return types.video;
                default:
                    return types.none;
            }
        }

        //public View()
        //{
        //    _elements = new SortedList<string, Element>();
        //}

        //public View(string name)
        //{
        //    this._name = name;
        //    _elements = new SortedList<string, Element>();
        //}

        public View(string name, int width, int height)
        {
            typeOfView = GetType(name);
            _name = name;
            this.width = width;
            this.height = height;
            _elements = new SortedList<string, Element>();
        }

        public string name
        {
            get { return _name; }
        }

        public int width
        {
            get { return _width; }
            set
            {
                if (_width == value)
                    return;
                _width = value;
            }
        }

        public int height
        {
            get { return _height; }
            set
            {
                if (_height == value)
                    return;
                _height = value;
            }
        }


        public Element addItem
        {
            set
            {
                if (_elements.IndexOfKey(value.name)>=0)
                    _elements.Remove(value.name);
                _elements.Add(value.name, value);
            }
        }

        private SortedList<string, Element> _elements;
        public SortedList<string, Element> elements
        {
            get { return _elements; }
        }

        public Element getItem(string key)
        {
            if (_elements.IndexOfKey(key) >= 0)
                return _elements.Values[_elements.IndexOfKey(key)];
            else return null;
        }

        public void removeItem(string key)
        {
            _elements.Remove(key);
        }
    }
}
