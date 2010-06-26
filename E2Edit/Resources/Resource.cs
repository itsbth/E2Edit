using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace E2Edit.Resources
{
    internal static class Resource
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof (ResourceNode[]),
                                                                             new XmlRootAttribute("Resources"));

        private static readonly IEnumerable<ResourceNode> Nodes = LoadNodes();
        private static readonly Dictionary<string, BitmapImage> ImageCache = new Dictionary<string, BitmapImage>();

        private static IEnumerable<ResourceNode> LoadNodes()
        {
            Stream resourceManifest =
                Assembly.GetExecutingAssembly().GetManifestResourceStream("E2Edit.Resources.Resources.xml");
            if (resourceManifest == null) throw new FileNotFoundException("Resource manifest not found.");
            return
                (IEnumerable<ResourceNode>)
                Serializer.Deserialize(
                    resourceManifest);
        }

        public static Stream GetResource(string name)
        {
            ResourceNode node = Nodes.FirstOrDefault(n => n.ID == name);
            if (node == null) return null;
            return File.Exists(node.File)
                       ? new FileStream(node.File, FileMode.Open)
                       : Assembly.GetExecutingAssembly().GetManifestResourceStream(node.Name);
        }

        public static BitmapImage GetBitmapImage(string name)
        {
            if (ImageCache.ContainsKey(name)) return ImageCache[name];
            Stream source = GetResource(name);
            if (source == null) return null;
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = source;
            bmp.EndInit();
            ImageCache[name] = bmp;
            return bmp;
        }

        public static void Export()
        {
            foreach (ResourceNode node in Nodes.Where(node => !File.Exists(node.File)))
            {
                if (!Directory.Exists(new FileInfo(node.File).DirectoryName))
                    Directory.CreateDirectory(new FileInfo(node.File).DirectoryName);
                using (FileStream fs = File.OpenWrite(node.File))
                using (Stream from = Assembly.GetExecutingAssembly().GetManifestResourceStream(node.Name))
                {
                    if (from == null) continue;
                    var buff = new byte[8192];
                    int len;
                    while ((len = from.Read(buff, 0, buff.Length)) > 0)
                        fs.Write(buff, 0, len);
                }
            }
        }
    }

    public class ResourceNode
    {
        [XmlElement] public string File;
        [XmlAttribute] public string ID;
        [XmlElement] public string Name;
    }
}