using System;
using System.Collections.Generic;
using System.Xml;
using ComplexEditor.DataLinker.RoutedOperation;

namespace ComplexEditor.DataStructure.XmlProvider
{
    public class XmlProvider
    {
        private readonly List<NamedOperation> _xmlResponses = new();

        public IEnumerable<NamedOperation> GetAllXmlElements(string uri)
        {
            try
            {
                XmlDocument xDoc = new()
                {
                    PreserveWhitespace = false
                };
                xDoc.Load(uri);

                var xmlNodeList = xDoc.SelectNodes("view/rows");
                if (xmlNodeList == null) return null;
                foreach (XmlNode xmlNode in xmlNodeList)
                foreach (XmlNode r in xmlNode.ChildNodes)
                {
                    if (r.InnerXml.Trim() == "") continue;
                    var name = "";
                    var id = 0;
                    var remainingElements = new List<string>();

                    foreach (XmlElement xmlElement in r)
                    {
                        switch (xmlElement.Name)
                        {
                            case "APPONAME":
                                name = xmlElement.InnerText;
                                continue;
                            case "APPOID":
                                id = int.Parse(xmlElement.InnerText);
                                continue;
                        }

                        remainingElements.Add(xmlElement.Name + ":" + xmlElement.InnerText);
                    }

                    _xmlResponses.Add(new NamedOperation(id, name, remainingElements));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return _xmlResponses;
        }
    }
}