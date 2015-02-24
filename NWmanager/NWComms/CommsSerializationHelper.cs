using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace TTG.NavalWar.NWComms
{
    public class CommsSerializationHelper<T>
    {
        public byte[] SerializeToBytes(T objectToSerialize)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectToSerialize);
            byte [] bytes = stream.ToArray();
            stream.Close();
            return bytes;
        }

        public T DeserializeFromBytes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes) {Position = 0};
            BinaryFormatter formatter = new BinaryFormatter();
            T theObject = (T)formatter.Deserialize(stream);
            stream.Close();
            return theObject;
        }
    }
}
