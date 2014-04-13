using System.Collections.Generic;
using System.Xml;

namespace DataManagement
{
    public class NasaDataKey
    {
        public string Id = string.Empty;
        public string Slug = string.Empty; 
    }

    public class CharacteristicOrders
    {
        public string MassOrder = string.Empty;
        public string AreaOrder = string.Empty;
        public string DistanceOrder = string.Empty;
    }

    public class CelestialBodyCharacteristics
    {
        public string Radius = string.Empty;
        public string Distance = string.Empty; //is empty for the sun, is relative to sun for planets and to planet for it's satellites
        public string Area = string.Empty;
        public string Mass = string.Empty;
        public string Link = string.Empty;
        public Dictionary<string, CelestialBodyCharacteristics> Subobjects = new Dictionary<string,CelestialBodyCharacteristics>();
    }

    public class SolarSystemDataManager
    {
        public SolarSystemDataManager()
        {
            LoadFromXml("Recources\\SolarSystemData.xml");
        }

        public Dictionary<string, NasaDataKey> GetNasaKeys()
        {
            return _keys;
        }

        public KeyValuePair<string, CelestialBodyCharacteristics> GetCelestialBodyCharacteristics()
        {
            return _containingObject;
        }

        public CharacteristicOrders GetCharacteristicOrders()
        {
            return _orders;
        }

        private void LoadFromXml(string fileName)
        {
            var doc = new XmlDocument {PreserveWhitespace = false};
            doc.Load(fileName);

            _orders = new CharacteristicOrders();
            LoadOrdersFromXml(doc.DocumentElement, ref _orders);

            var Object = new CelestialBodyCharacteristics();
            LoadObjectFromXml(doc.DocumentElement, ref Object);
            _containingObject = new KeyValuePair<string,CelestialBodyCharacteristics>(doc.DocumentElement.Name, Object);

            _keys = new Dictionary<string, NasaDataKey>();
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
                            var characteristics = new CelestialBodyCharacteristics();
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
            var key = new NasaDataKey();
            if (element.HasAttribute("id"))
                key.Id = element.Attributes["id"].Value;

            if (element.HasAttribute("slug"))
                key.Slug = element.Attributes["slug"].Value;

            _keys.Add(element.Name, key);

            if (null != element.SelectSingleNode("subobjects"))
                foreach (XmlElement child in element.SelectSingleNode("subobjects").ChildNodes)
                    LoadKeysFromXml(child);
        }

        private CharacteristicOrders _orders;
        private Dictionary<string, NasaDataKey> _keys;
        private KeyValuePair<string, CelestialBodyCharacteristics> _containingObject;
    }
}
