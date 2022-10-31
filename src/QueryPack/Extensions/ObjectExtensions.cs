using System.Data.HashFunction.xxHash;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QueryPack.Extensions
{
    public static class ObjectExtensions
    {
        public static string Hash<T>(this T @object)
        {
            var binFormatter = new BinaryFormatter();
            using (var mStream = new MemoryStream())
            {
                binFormatter.Serialize(mStream, @object);
                var hash = xxHashFactory.Instance.Create();

                return hash.ComputeHash(mStream.ToArray()).AsHexString();
            }
        }
    }
}
