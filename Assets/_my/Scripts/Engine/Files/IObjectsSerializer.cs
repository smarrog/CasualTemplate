using System;

namespace Smr.Files {
    public interface IObjectSerializer {
        T Deserialize<T>(byte[] data);
        object Deserialize(Type type, byte[] data);
        byte[] Serialize<T>(T token);
        byte[] Serialize(object token);
        string DecodeToJson(byte[] data);
        byte[] EncodeFromJson(string json);
    }
}