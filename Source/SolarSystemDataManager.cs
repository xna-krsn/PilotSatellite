using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace TricubicalExtrapolation
{
    class NasaDataKey
    {
        public string Id = string.Empty;
        public string Slug = string.Empty; 
    }

    class CharacteristicOrders
    {
        public string MassOrder = string.Empty;
        public string AreaOrder = string.Empty;
        public string DistanceOrder = string.Empty;
    }

    class CelestialBodyCharacteristics
    {
        public string Radius = string.Empty;
        public string Distance = string.Empty; //is empty for the sun, is relative to sun for planets and to planet for it's satellites
        public string Area = string.Empty;
        public string Mass = string.Empty;
        public string Link = string.Empty;
        public Dictionary<string, CelestialBodyCharacteristics> Subobjects = new Dictionary<string,CelestialBodyCharacteristics>();
    }

    class SolarSystemDataManager
    {
        public Dictionary<string, NasaDataKey> GetNasaKeys()
        {
            return Keys;
        }

        public KeyValuePair<string, CelestialBodyCharacteristics> GetCelestialBodyCharacteristics()
        {
            return ContainingObject;
        }

        public CharacteristicOrders GetCharacteristicOrders()
        {
            return Orders;
        }

        public void LoadFromXml(string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(FileName);

            Orders = new CharacteristicOrders();
            LoadOrdersFromXml(doc.DocumentElement, ref Orders);

            CelestialBodyCharacteristics Object = new CelestialBodyCharacteristics();
            LoadObjectFromXml(doc.DocumentElement, ref Object);
            ContainingObject = new KeyValuePair<string,CelestialBodyCharacteristics>(doc.DocumentElement.Name, Object);

            Keys = new Dictionary<string, NasaDataKey>();
            LoadKeysFromXml(doc.DocumentElement);       
        }

        private void LoadObjectFromXml(XmlElement element, ref CelestialBodyCharacteristics Body)
        {
            foreach (XmlElement node in element.ChildNodes)
                switch (node.Name)
                {
                    case "mass":
                        Body.Mass = node.InnerText.Trim();
                        break;
                    case "radius":
                        Body.Radius = node.InnerText.Trim();
                        break;
                    case "surface_area":
                        Body.Area = node.InnerText.Trim();
                        break;
                    case "distance":
                        Body.Distance = node.InnerText.Trim();
                        break;
                    case "link":
                        Body.Link = node.InnerText.Trim();
                        break;
                    case "subobjects":
                        foreach (XmlElement subobject in node.ChildNodes)
                        {
                            CelestialBodyCharacteristics characteristics = new CelestialBodyCharacteristics();
                            LoadObjectFromXml(subobject, ref characteristics);
                            Body.Subobjects.Add(subobject.Name, characteristics);
                        }
                        break;
                } 
        }

        private void LoadOrdersFromXml(XmlElement element, ref CharacteristicOrders orders)
        {
            orders.MassOrder = element.Attributes["mass_order"].Value;
            orders.AreaOrder = element.Attributes["area_order"].Value;
            orders.DistanceOrder = element.Attributes["distance_order"].Value;
        }

        private void LoadKeysFromXml(XmlElement element)
        {  
            NasaDataKey key = new NasaDataKey();
            if (element.HasAttribute("id"))
                key.Id = element.Attributes["id"].Value;

            if (element.HasAttribute("slug"))
                key.Slug = element.Attributes["slug"].Value;

            Keys.Add(element.Name, key);

            if (null != element.SelectSingleNode("subobjects"))
                foreach (XmlElement child in element.SelectSingleNode("subobjects").ChildNodes)
                    LoadKeysFromXml(child);
        }

        private CharacteristicOrders Orders;
        private Dictionary<string, NasaDataKey> Keys;
        private KeyValuePair<string, CelestialBodyCharacteristics> ContainingObject;
    }
}
