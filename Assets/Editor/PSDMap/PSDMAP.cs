using System.Collections.Generic;
using System.Xml.Serialization;
namespace psdmap
{

    [XmlRoot]
    public class PSDMAP
    {
        [XmlAttribute]
        public int width { get; set; }

        [XmlAttribute]
        public int height { get; set; }

        [XmlAttribute]
        public int maxIndex { get; set; }

        [XmlElement]
        public Items items { get; set; }

        [XmlElement]
        public Layers layers { get; set; }

    }
    public class Items
    {
        [XmlElement("item")]
        public List<Item> itemList { get; set; }
    }
    public class Item
    {
        [XmlAttribute]
        public string name { get; set; }

        //[XmlAttribute]
        //public int width { get; set; }

        //[XmlAttribute]
        //public int height { get; set; }
    }

    public class Layers
    {
        [XmlElement]
        public MapTop mapTop { get; set; }

        [XmlElement]
        public Active active { get; set; }

        [XmlElement]
        public Map map { get; set; }

        [XmlElement]
        public Top top { get; set; }
    }

    public class Layer
    {
        [XmlAttribute]
        public string item { get; set; }

        [XmlAttribute]
        public string x { get; set; }

        [XmlAttribute]
        public string y { get; set; }

        [XmlAttribute]
        public string width { get; set; }

        [XmlAttribute]
        public string height { get; set; }

        [XmlAttribute]
        public int index { get; set; }

        [XmlAttribute]
        public string flip { get; set; }
    }

    public class MapTop
    {
        [XmlElement("layer")]
        public List<Layer> layerList { get; set; }
    }

    public class Active : MapTop
    {

    }
    public class Map : MapTop
    {

    }
    public class Top : MapTop
    {

    }
}