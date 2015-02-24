using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace TTG.NavalWar.NWComms
{
    public class Serializer<T>
    {
        public void SaveToXML(T obj, string fileName)
        {
            SaveToXML(obj, fileName, SerializationHelper.GetDataFolder());
        }

        public void SaveToXML(T obj, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fs = new StreamWriter(Path.Combine(path, fileName), false);
            var xls = new XmlSerializer(typeof(T));
            xls.Serialize(fs, obj);
            fs.Close();
        }

        public void SaveListToXML(List<T> list, string fileName)
        {
            SaveListToXML(list, fileName, SerializationHelper.GetDataFolder());
        }

        public void SaveListToXML(List<T> list, string fileName, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fs = new StreamWriter(Path.Combine(path, fileName), false);
            var xls = new XmlSerializer(typeof(List<T>));
            xls.Serialize(fs, list);
            fs.Close();
        }

        public List<T> LoadListFromXML(FileInfo[] files)
        {
            List<T> list = new List<T>();
            foreach (var file in files)
            {
                var aList = LoadListFromXML(file.FullName);
                list.AddRange(aList);
            }
            return list;
        }

        public List<T> LoadListFromXML(string file)
        {
            StreamReader fs = new StreamReader(file);
            XmlSerializer xls = new XmlSerializer(typeof(List<T>));
            List<T> list = (List<T>)xls.Deserialize(fs);
            fs.Close();
            return list;
        }

        public List<T> LoadListFromXML(string fileName, string path)
        {
            return LoadListFromXML(Path.Combine(path, fileName));
        }

        public List<T> LoadFromXML(FileInfo[] files)
        {
            return files.Select(LoadFromXML).ToList();
        }

        public T LoadFromXML(FileInfo file)
        {
            return LoadFromXML(file.FullName);
        }

        public T LoadFromXML(string file)
        {
            var fs = new StreamReader(file);
            var xls = new XmlSerializer(typeof(T));
            var list = (T)xls.Deserialize(fs);
            fs.Close();
            return list;
        }

        public T LoadFromXML(string fileName, string path)
        {
            return LoadFromXML(Path.Combine(path, fileName));
        }
    }
}
