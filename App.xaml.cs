using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace es_theme_editor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //будем хранить путь к выбранной теме и выбранную платорму в глобальных переменных
        public string themefolder;
        public string gameplatformtheme;
        public string backgroundSoundPath;
        public string backgroundSystemImagePath;
        public string backgroundImagePath;
        public string logoPath;
        public Brush helpIconColor;
        public Brush helpTextColor;
    }
}
