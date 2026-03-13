using System.Collections.Generic;

namespace KszUtil.MyStorage
{
    public interface IMyStorage
    {
        void SaveBytes(byte[] data, DatName dataName);
        byte[] LoadBytes(DatName dataName);
        void Serialize<T>(T data, DatName dataName);
        T Deserialize<T>(DatName dataName);
        bool Exists(DatName datName);
        void DeleteData(DatName datName);
        IEnumerable<string> GetFiles();
    }
}
