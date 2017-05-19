using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace es_theme_editor
{
    class XMLWorker
    {
        public static XmlDocument XMLOptimazer(XmlDocument XMLOriginDoc, bool sameType = false)
        {
            XmlDocument resDoc = new XmlDocument();
            XmlNode tmpNode;
            XmlNode themeNode = null, resDocthemeNode, elementNode_created, viewNode_created;
            string elementNodeName = "", elementName = "";
            //XmlNode propertiesNodeToBeOptimized = null;
            string nameofview = "";
            bool elementNotExist;
            //List<string> worked = new List<string>();
            //int findIndx = -1;
            SortedList<string, XmlNode> optimizedNodes = new SortedList<string,XmlNode>();
            //получаем Theme node и удаляем ее из документа
            foreach (XmlNode currentNode in XMLOriginDoc)
            {
                if (currentNode.Name == "theme")
                {
                    themeNode = currentNode;
                    tmpNode = themeNode;
                    XMLOriginDoc.RemoveChild(themeNode);
                }
            }

            //Create the main element for the theme
            resDocthemeNode = resDoc.CreateElement("theme");
            resDoc.AppendChild(resDocthemeNode);
            resDocthemeNode.AppendChild(XMLWorker.createXMLNode(resDoc, "formatVersion", "", "", "4"));

            if (themeNode == null)
                return null;

            //если уже есть объединенные view мы их переносим в результирующую ноду.
            for (int i = 0; i < themeNode.ChildNodes.Count; i++)
            {
                if (themeNode.ChildNodes[i].Name == "view" && themeNode.ChildNodes[i].Attributes["name"] != null && themeNode.ChildNodes[i].Attributes["name"].InnerText.Contains(","))
                {
                    XmlNode importNode = resDoc.ImportNode(themeNode.ChildNodes[i], true);

                    resDocthemeNode.AppendChild(importNode);
                    optimizedNodes.Add(themeNode.ChildNodes[i].Attributes["name"].InnerText, importNode);
                    themeNode.RemoveChild(themeNode.ChildNodes[i]);
                    i--;
                }
            }
            //проходимся по view элементы которых будем сравнивать.
            foreach (XmlNode viewNode in themeNode.ChildNodes)
            {
                //если имя Node view
                if (viewNode.Name == "view")
                {
                    //проходимся по всем элемента текущего view.
                    foreach (XmlNode elementNode in viewNode)
                    {
                        //Нужно пройти по всем properties childNode(для нашей проги это Properties) и найти их копии
                        foreach (XmlNode propertiesNode in elementNode)
                        {
                            nameofview = "";
                            elementNodeName = "";
                            foreach (XmlNode viewnodeWhereFind in themeNode.ChildNodes)
                            {
                                if (viewnodeWhereFind.Name == "view" && !viewNode.Equals(viewnodeWhereFind))
                                foreach (XmlNode elementnodeWhereFind in viewnodeWhereFind)
                                {
                                    if ((sameType && elementNode.Name == elementnodeWhereFind.Name) || (!sameType && elementNode.Name == elementnodeWhereFind.Name && elementNode.Attributes["name"].InnerText == elementnodeWhereFind.Attributes["name"].InnerText))
                                    {
                                        foreach (XmlNode propertiesNodeWhereFind in elementnodeWhereFind)
                                        {
                                            if (propertiesNodeWhereFind.Name == propertiesNode.Name && propertiesNode.InnerText == propertiesNodeWhereFind.InnerText)
                                            {
                                                if (nameofview == "")
                                                    nameofview = viewNode.Attributes["name"].InnerText;
                                                if (viewnodeWhereFind.Attributes["name"].InnerText != null && !nameofview.Contains(viewnodeWhereFind.Attributes["name"].InnerText))
                                                    nameofview += ", " + viewnodeWhereFind.Attributes["name"].InnerText;
                                                XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNodeWhereFind, "remove", "true");
                                                if (sameType)
                                                {
                                                    if (elementNodeName == "")
                                                        elementNodeName = elementNode.Name;
                                                    else
                                                        if (!elementNodeName.Contains(elementNode.Name))
                                                            elementNodeName += ", " + elementNode.Name;
                                                }
                                                else
                                                    elementNodeName = elementNode.Name;
                                                elementName = elementNode.Attributes["name"].InnerText;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            //После того как прошли по всем нодам сохраняем
                            elementNotExist = true;
                            string sortednameofview = sortString(nameofview);
                            if (sortednameofview != "")// && sortednameofview.Contains(", ")
                            {
                                if (optimizedNodes.IndexOfKey(sortednameofview) >= 0)
                                {
                                    viewNode_created = optimizedNodes.Values[optimizedNodes.IndexOfKey(sortednameofview)];
                                    foreach (XmlNode elementNodeCreated in viewNode_created)
                                    {
                                        if (elementNodeCreated.Name == elementNodeName && elementNodeCreated.Attributes["name"].InnerText == elementName)
                                        {
                                            elementNotExist = false;
                                            if (getIndexNode(elementNodeCreated, propertiesNode) < 0)
                                            {
                                                elementNodeCreated.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                                XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                            }
                                        }
                                    }
                                    if (elementNotExist)
                                    {
                                        elementNode_created = XMLWorker.createXMLNode(resDoc, elementNodeName, "name", elementName, "");
                                        elementNode_created.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                        viewNode_created.AppendChild(elementNode_created);
                                        XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                    }
                                }
                                else
                                {
                                    viewNode_created = XMLWorker.createXMLNode(resDoc, "view", "name", sortednameofview, "");
                                    elementNode_created = XMLWorker.createXMLNode(resDoc, elementNodeName, "name", elementName, "");
                                    elementNode_created.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                    viewNode_created.AppendChild(elementNode_created);
                                    optimizedNodes.Add(sortednameofview, viewNode_created);
                                    XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                }
                                
                                //elementNode.RemoveChild(propertiesNode);
                            }
                        }                         
                    }
                }                
            }

            for (int i = 0; i < optimizedNodes.Count; i++)
            {
                //resDocthemeNode.RemoveChild(optimizedNodes.Values[i]);
                try
                {
                    resDocthemeNode.AppendChild(optimizedNodes.Values[i]);
                }
                catch (Exception)
                {
                    resDocthemeNode.AppendChild(resDoc.ImportNode(optimizedNodes.Values[i], true));
                }
            }
            themeNode = clearXmlNode(themeNode);
            foreach (XmlNode viewNode in themeNode.ChildNodes)
                resDocthemeNode.AppendChild(resDoc.ImportNode(viewNode, true));
            resDoc.AppendChild(resDocthemeNode);
            return resDoc;
        }

        public static XmlDocument XMLOptimazerByType(XmlDocument XMLOriginDoc)
        {
            XmlDocument resDoc = new XmlDocument();
            XmlNode tmpNode;
            XmlNode themeNode = null, resDocthemeNode, elementNode_created, viewNode_created;
            string elementNodeName = "", elementName = "";
            //XmlNode propertiesNodeToBeOptimized = null;
            string nameofview = "";
            bool elementNotExist;
            //List<string> worked = new List<string>();
            //int findIndx = -1;
            SortedList<string, XmlNode> optimizedNodes = new SortedList<string, XmlNode>();
            //получаем Theme node и удаляем ее из документа
            foreach (XmlNode currentNode in XMLOriginDoc)
            {
                if (currentNode.Name == "theme")
                {
                    themeNode = currentNode;
                    tmpNode = themeNode;
                    XMLOriginDoc.RemoveChild(themeNode);
                }
            }

            //Create the main element for the theme
            resDocthemeNode = resDoc.CreateElement("theme");
            resDoc.AppendChild(resDocthemeNode);
            resDocthemeNode.AppendChild(XMLWorker.createXMLNode(resDoc, "formatVersion", "", "", "4"));

            if (themeNode == null)
                return null;

            //если уже есть объединенные view мы их переносим в результирующую ноду.
            for (int i = 0; i < themeNode.ChildNodes.Count; i++)
            {
                if (themeNode.ChildNodes[i].Name == "view" && themeNode.ChildNodes[i].Attributes["name"] != null && themeNode.ChildNodes[i].Attributes["name"].InnerText.Contains(","))
                {
                    XmlNode importNode = resDoc.ImportNode(themeNode.ChildNodes[i], true);

                    resDocthemeNode.AppendChild(importNode);
                    optimizedNodes.Add(themeNode.ChildNodes[i].Attributes["name"].InnerText, importNode);
                    themeNode.RemoveChild(themeNode.ChildNodes[i]);
                    i--;
                }
            }
            //проходимся по view элементы которых будем сравнивать.
            for (int i = 0; i < (int)Element.types.notexistsname; i++ )
                foreach (XmlNode viewNode in themeNode.ChildNodes)
                {
                    //если имя Node view
                    if (viewNode.Name == "view")
                    {
                        //проходимся по всем элемента текущего view.
                        foreach (XmlNode elementNode in viewNode)
                        {
                            if ((((Element.types)1).ToString()) == elementNode.Name)
                            //Нужно пройти по всем properties childNode(для нашей проги это Properties) и найти их копии
                            foreach (XmlNode propertiesNode in elementNode)
                            {
                                nameofview = "";
                                elementNodeName = "";
                                foreach (XmlNode viewnodeWhereFind in themeNode.ChildNodes)
                                {
                                    if (viewnodeWhereFind.Name == "view" && !viewNode.Equals(viewnodeWhereFind))
                                        foreach (XmlNode elementnodeWhereFind in viewnodeWhereFind)
                                        {
                                            if (elementNode.Name == elementnodeWhereFind.Name)
                                            {
                                                foreach (XmlNode propertiesNodeWhereFind in elementnodeWhereFind)
                                                {
                                                    if (propertiesNodeWhereFind.Name == propertiesNode.Name)
                                                    {
                                                        if (nameofview == "")
                                                            nameofview = viewNode.Attributes["name"].InnerText;
                                                        if (viewnodeWhereFind.Attributes["name"].InnerText != null && !nameofview.Contains(viewnodeWhereFind.Attributes["name"].InnerText))
                                                            nameofview += ", " + viewnodeWhereFind.Attributes["name"].InnerText;
                                                        elementNodeName = elementNode.Name;
                                                        if (elementName == "")
                                                            elementName = elementNode.Attributes["name"].InnerText;
                                                        if (!elementName.Contains(elementnodeWhereFind.Attributes["name"].InnerText))
                                                            elementName += ", " + elementnodeWhereFind.Attributes["name"].InnerText;
                                                        XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNodeWhereFind, "remove", "true");

                                                    }
                                                }
                                            }
                                        }
                                }
                                //После того как прошли по всем нодам сохраняем
                                elementNotExist = true;
                                string sortednameofview = sortString(nameofview);
                                if (sortednameofview != "")// && sortednameofview.Contains(", ")
                                {
                                    if (optimizedNodes.IndexOfKey(sortednameofview) >= 0)
                                    {
                                        viewNode_created = optimizedNodes.Values[optimizedNodes.IndexOfKey(sortednameofview)];
                                        foreach (XmlNode elementNodeCreated in viewNode_created)
                                        {
                                            if (elementNodeCreated.Name == elementNodeName)
                                            {
                                                elementNotExist = false;
                                                if (getIndexNode(elementNodeCreated, propertiesNode) < 0)
                                                {
                                                    elementNodeCreated.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                                    XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                                }
                                            }
                                        }
                                        if (elementNotExist)
                                        {
                                            elementNode_created = XMLWorker.createXMLNode(resDoc, elementNodeName, "name", elementName, "");
                                            elementNode_created.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                            viewNode_created.AppendChild(elementNode_created);
                                            XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                        }
                                    }
                                    else
                                    {
                                        viewNode_created = XMLWorker.createXMLNode(resDoc, "view", "name", sortednameofview, "");
                                        elementNode_created = XMLWorker.createXMLNode(resDoc, elementNodeName, "name", elementName, "");
                                        elementNode_created.AppendChild(XMLWorker.createXMLNode(resDoc, propertiesNode.Name, "", "", propertiesNode.InnerText));
                                        viewNode_created.AppendChild(elementNode_created);
                                        optimizedNodes.Add(sortednameofview, viewNode_created);
                                        XMLWorker.createXmlAttribute(XMLOriginDoc, propertiesNode, "remove", "true");
                                    }

                                    //elementNode.RemoveChild(propertiesNode);
                                }
                            }
                        }
                    }

                }

            for (int i = 0; i < optimizedNodes.Count; i++)
            {
                //resDocthemeNode.RemoveChild(optimizedNodes.Values[i]);
                try
                {
                    resDocthemeNode.AppendChild(optimizedNodes.Values[i]);
                }
                catch (Exception)
                {
                    resDocthemeNode.AppendChild(resDoc.ImportNode(optimizedNodes.Values[i], true));
                }
            }
            themeNode = clearXmlNode(themeNode);
            foreach (XmlNode viewNode in themeNode.ChildNodes)
                resDocthemeNode.AppendChild(resDoc.ImportNode(viewNode, true));
            resDoc.AppendChild(resDocthemeNode);
            return resDoc;
        }

        private static XmlNode optimazeXMLNode(XmlNode toOptimization) 
        {

            return toOptimization;
        }

        private static XmlNode clearXmlNode(XmlNode xmlNode) 
        {
            XmlNode viewNode, elementNode, propertiesNode;
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++ )
            {
                viewNode = xmlNode.ChildNodes[i];
                if (viewNode.Name == "view")
                    for (int j = 0; j < viewNode.ChildNodes.Count; j++)
                    {
                        elementNode = viewNode.ChildNodes[j];
                        for (int y = 0; y < elementNode.ChildNodes.Count; y++)
                        {
                            propertiesNode = elementNode.ChildNodes[y];
                            if (propertiesNode.Attributes["remove"] != null && propertiesNode.Attributes["remove"].InnerText == "true")
                            {
                                elementNode.RemoveChild(propertiesNode);
                                y--;
                            }

                        }
                        if (elementNode.ChildNodes.Count == 0)
                        {
                            viewNode.RemoveChild(elementNode);
                            j--;
                        }

                    }
                else
                {
                    xmlNode.RemoveChild(viewNode);
                    i--;
                }
            }
            return xmlNode;
        }

        private static string sortString(string original) 
        {
            string res = "";
            Char delimiter = ',';
            String[] substrings = original.Replace(" ", "").Split(delimiter);
            IEnumerable<string> orderedNumbers = from i in substrings
                                 orderby i ascending
                                 select i;
            foreach (string str in orderedNumbers)
            {
                if (res == "")
                    res = str;
                else
                    res += ", " + str;
            }
            return res;
        }

        private static int getIndexNode(XmlNode nodeWhereFind, XmlNode nodeToFind) 
        {
            int indx = -1;
            XmlNode childNode = null;
            for (int i = 0; i < nodeWhereFind.ChildNodes.Count; i++)
            {
                childNode = nodeWhereFind.ChildNodes[i];
                if (childNode.Name == nodeToFind.Name && childNode.InnerText == nodeToFind.InnerText)
                    return i;
            }

            return indx;
        }

        public static XmlNode createXMLNode(XmlDocument doc, string nodename, string attributename, string attributevalue, string childvalue)
        {
            XmlNode node;

            node = doc.CreateElement(nodename.Replace(" ", ""));

            createXmlAttribute(doc, node, attributename, attributevalue);

            if (childvalue != "")
                node.AppendChild(doc.CreateTextNode(childvalue));

            return node;

        }

        public static void createXmlAttribute(XmlDocument doc, XmlNode node, string attributename, string attributevalue)
        {
            XmlAttribute nodeAttribute;

            if (attributevalue != "")
            {
                nodeAttribute = doc.CreateAttribute(attributename);
                nodeAttribute.Value = attributevalue;
                node.Attributes.Append(nodeAttribute);
            }
        }

    }
}
