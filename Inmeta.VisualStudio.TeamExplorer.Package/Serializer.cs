using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Inmeta.VisualStudio.TeamExplorer
{
    static public class Serializer
    {
        public static T DeSerialize<T>(string localPath)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var rdr = new XmlTextReader(localPath))
            {
                rdr.WhitespaceHandling = WhitespaceHandling.All;
                return (T)serializer.Deserialize(rdr);
            }

        }

        public static void Serialize<T>(string localPath, T settings)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var sw = new StreamWriter(localPath, false))
            {
                serializer.Serialize(sw, settings);
            }
        }
    }
}
