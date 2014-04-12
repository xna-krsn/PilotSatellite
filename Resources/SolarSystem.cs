using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace TricubicalExtrapolation
{
    class SolarSystem
    {
        public void LoadFromXml(string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.Load(FileName);
                    
            Id = doc.DocumentElement.Attributes["id"].Value;
            Slug = doc.DocumentElement.Attributes["slug"].Value;
            MassOrder = doc.DocumentElement.Attributes["mass_order"].Value;
            AreaOrder = doc.DocumentElement.Attributes["area_order"].Value;
            DistanceOrder = doc.DocumentElement.Attributes["distance_order"].Value;
            
            Planets = new List<CelestialBody>();
            foreach (XmlElement node in doc.DocumentElement.ChildNodes)
            {
                if ("link" == node.Name)
                    Link = node.InnerText.Trim();
                else
                {
                    CelestialBody Planet = new CelestialBody();
                    Planet.SetFromXml(node);
                    Planets.Add(Planet);
                }
            }        
        }

        public string MassOrder;
        public string AreaOrder;
        public string DistanceOrder;
	    public string Id;
        public string Slug;
		public string Link;
        public List<CelestialBody> Planets;
    }

    class CelestialBody
    {
        public void SetFromXml(XmlElement node)
        {
            Name = node.Name;

            if (node.HasAttribute("id"))
                Id = node.Attributes["id"].Value;

            if (node.HasAttribute("slug"))
                Slug = node.Attributes["slug"].Value;

            Mass = node.SelectSingleNode("mass").InnerText;
            Radius = node.SelectSingleNode("radius").InnerText;
            Area = node.SelectSingleNode("surface_area").InnerText;

            if (null != node.SelectSingleNode("distance"))
                Distance = node.SelectSingleNode("distance").InnerText;

            Link = node.SelectSingleNode("link").InnerText.Trim(); 

            Satellites = new List<CelestialBody>();
            if (null != node.SelectSingleNode("satellite"))
                foreach (XmlElement satellite in node.SelectSingleNode("satellite").ChildNodes)
                {
                    CelestialBody Satellite = new CelestialBody();
                    Satellite.SetFromXml(satellite);
                    Satellites.Add(Satellite);
                }
        }

        public string Name;
        public string Id;   //can be null
        public string Slug; //can be null
        public string Radius;
        public string Distance; //is null for the sun, is relative to sun for planets and to planet for her satellites
		public string Area;
		public string Mass;
		public string Link;
        public List<CelestialBody> Satellites;  //can be null
    }


}
