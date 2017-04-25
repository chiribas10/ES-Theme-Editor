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
using Microsoft.Win32;

namespace es_theme_editor
{
    class SomeUtilities
    {

        public static string getComboBoxValue(ComboBox cb)
        {
            //Get the value from the ComboBox and check if such view exists
            ComboBoxItem typeItem = (ComboBoxItem)cb.SelectedItem;
            string comboBoxItemvalue = "";
            if (typeItem != null)
                comboBoxItemvalue = typeItem.Content.ToString();
            return comboBoxItemvalue;
        }

        //public static IEnumerable<string> GetRelativeFolderContents(string folder, string searchPattern = "*")
        //{
        //    //Проверки на валидность параметров
        //    if (!Directory.Exists(folder))
        //    {
        //        //Нет пути для сканирования содержимого директории
        //        throw new IOException("Путь \"" + folder + "\" не существует.");
        //    }

        //    //Проверяем наличие разделителя папок в конце базового пути, и, если его нет - добавляем.
        //    if (!folder.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())) folder += System.IO.Path.DirectorySeparatorChar;

        //    //Список для результата:
        //    List<string> result = new List<string>();

        //    //Временная переменная:
        //    string t = "";

        //    //Получаем содержимое базового пути, включая все вложенные папки и файлы:
        //    foreach (var item in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
        //    {
        //        t = item.Replace(folder, string.Empty);
        //        //Проверяем, не ссылку ли на папку мы получили?
        //        if (Directory.Exists(item))
        //        {
        //            //Папка пустая?
        //            if (!Directory.GetFiles(item).Any())
        //            {
        //                //Если пустая - добавляем к результату (не пустые папки сами добавятся вместе с 
        //                //путями к файлам, в них содержащимся)
        //                if (!item.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
        //                {
        //                    result.Add(t + System.IO.Path.DirectorySeparatorChar);
        //                }
        //                else
        //                {
        //                    result.Add(t);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            result.Add(t);
        //        }
        //    }
        //    return result;
        //}

        public static string GetHexFromBrush(Brush brush) 
        {
            Color btn_iconColor_mycolor = ((System.Windows.Media.SolidColorBrush)(brush)).Color;
            string theHexColor = "#" + btn_iconColor_mycolor.R.ToString("X2") + btn_iconColor_mycolor.G.ToString("X2") + btn_iconColor_mycolor.B.ToString("X2") + btn_iconColor_mycolor.A.ToString("X2");

            return theHexColor;
        }

        public static Brush GetHexFromBrush(string colorhex)
        {
            if (colorhex != null)
            {
                byte R = Convert.ToByte(colorhex.Substring(1, 2), 16);
                byte G = Convert.ToByte(colorhex.Substring(3, 2), 16);
                byte B = Convert.ToByte(colorhex.Substring(5, 2), 16);
                byte A = Convert.ToByte(colorhex.Substring(7, 2), 16);
                Color color = Color.FromRgb(R, G, B);
                color.A = A;

                SolidColorBrush scb = new SolidColorBrush(color);
                return scb;
            }
            return new SolidColorBrush();
            //applying the brush to the background of the existing Button btn:
            //btn.Background = scb;


            //return (SolidColorBrush)(new BrushConverter().ConvertFrom(theHexColor));
        }

        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static string openFileDialog(string filter, string currentfile, string themeFile)
        {
            if (!File.Exists(themeFile))
            {
                MessageBox.Show("To specify relative paths, select the platform with the theme.xml file", "Attention", MessageBoxButton.OK);
                return null;
            }
            else
            {
                OpenFileDialog myDialog = new OpenFileDialog();
                myDialog.Filter = filter;
                if (currentfile != "")
                    currentfile = SomeUtilities.MakeAbsolutePath(themeFile, currentfile);
                if (File.Exists(currentfile))
                    myDialog.FileName = currentfile;
                myDialog.CheckFileExists = true;
                myDialog.Multiselect = true;
                //myDialog.InitialDirectory = tb_themefolder.Text;
                if (myDialog.ShowDialog() == true)
                {
                    return SomeUtilities.MakeRelativePath(themeFile, myDialog.FileName);
                }
                return null;
            }
        }

        #region work with Absolute and Relative path
        //./file.php (фал лежит в той же папке. такая запись иногда требуется в некоторых юникс системах)
        //images/picture.jpg (файл лежит в капке images, которая находится в текущей)
        //../file.php (файл лежит в папке, которая расположена на один уровень выше от текущей)./../simple.xml
        //../../file.php (файл лежит в папке, которая расположена на два уровня выше от текущей)

        /// <summary>
        /// Get absolute path from relative.
        /// </summary>
        /// <param name="basePath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toRelativePath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeAbsolutePath(String basePath, String relativePath)
        {
            if (String.IsNullOrEmpty(basePath)) throw new ArgumentNullException("basePath");
            if (String.IsNullOrEmpty(relativePath)) throw new ArgumentNullException("relativePath");

            var uri = new Uri(new Uri(basePath), relativePath);
            string fullPath = uri.LocalPath;
            return fullPath;
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="basePath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toRelativePath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String basePath, String toRelativePath)
        {
            if (String.IsNullOrEmpty(basePath)) throw new ArgumentNullException("basePath");
            if (String.IsNullOrEmpty(toRelativePath)) throw new ArgumentNullException("toRelativePath");

            Uri fromUri = new Uri(basePath);
            Uri toUri = new Uri(toRelativePath);

            if (fromUri.Scheme != toUri.Scheme) { return toRelativePath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
        #endregion work with Absolute and Relative path
    }
}
