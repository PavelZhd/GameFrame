using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace GameFrame.Savable
{
    public interface IBinarySerializable
    {
        void depositData(BinaryWriter binaryWriter);
        void extractData(BinaryReader binaryReader);
    }

    public static class BinarySerializationHelpers
    {
        public static void DepositSafe(this BinaryWriter binaryWriter, Action<BinaryWriter> depositor)
        {
            using (MemoryStream ms = new MemoryStream()) {
                using (BinaryWriter bw = new BinaryWriter(ms)) {
                    depositor(bw);
                }
                var bytes = ms.ToArray();
                binaryWriter.Write(bytes.Length);
                binaryWriter.Write(bytes);
            }
        }
        public static T ExtractSafe<T>(this BinaryReader binaryReader, Func<BinaryReader, T> extractor)
        {
            var len = binaryReader.ReadInt32();
            using (var ms = new MemoryStream(binaryReader.ReadBytes(len))) {
                using (BinaryReader br = new BinaryReader(ms)) {
                    return extractor(br);
                }
            }
        }

        public static void DepositSafe(this BinaryWriter binaryWriter, IBinarySerializable source)
        {
            DepositSafe(binaryWriter, source.depositData);
        }
        public static T ExtractSafe<T>(this BinaryReader binaryReader)
            where T: IBinarySerializable, new()
        {
            return ExtractSafe(binaryReader, (reader) =>
            {
                T item = new T();
                item.extractData(reader);
                return item;
            });
        }

        public static void Deposit<T>(this IEnumerable<T> source, BinaryWriter binaryWriter)
            where T : IBinarySerializable
        {

            binaryWriter.Write(source.Count());
            foreach (IBinarySerializable item in source)
            {
                item.depositData(binaryWriter);
            }
        }
        public static IEnumerable<T> ExtractEnumerable<T>(this BinaryReader binaryReader, Func<T> itemConstructor)
             where T : IBinarySerializable
        {

            List<T> result = new List<T>();
            int len = binaryReader.ReadInt32();
            for (int i = 0; i < len; i++)
            {
                T item = itemConstructor();
                item.extractData(binaryReader);
                result.Add(item);
            }
            return result;
        }

        public static IEnumerable<T> ExtractEnumerable<T>(this BinaryReader binaryReader)
            where T : IBinarySerializable, new()
        {
            return ExtractEnumerable(binaryReader, () => new T());
        }
        public static void Deposit<K, T>(this IDictionary<K, T> source, BinaryWriter binaryWriter, Action<K, BinaryWriter> keyEncoder)
            where T : IBinarySerializable
        {
            binaryWriter.Write(source.Count());
            foreach (var kvp in source)
            {
                keyEncoder(kvp.Key, binaryWriter);
                kvp.Value.depositData(binaryWriter);
            }
        }
        public static void Deposit<T>(this IDictionary<string, T> source, BinaryWriter binaryWriter)
            where T: IBinarySerializable
        {
            Deposit(source, binaryWriter, (key, writer) => writer.Write(key));
        }
        public static void Deposit<T>(this IDictionary<int, T> source, BinaryWriter binaryWriter)
            where T : IBinarySerializable
        {
            Deposit(source, binaryWriter, (key, writer) => writer.Write(key));
        }
        public static void Deposit<K,T>(this IDictionary<K, T> source, BinaryWriter binaryWriter)
            where K : IBinarySerializable
            where T : IBinarySerializable
        {
            Deposit(source, binaryWriter, (key, writer) => key.depositData(writer));
        }
        public static IDictionary<K,T> ExtractDictionary<K,T>(this BinaryReader binaryReader, Func<BinaryReader, K> keyExtractor, Func<T> itemConstructor)
            where T: IBinarySerializable
        {
            Dictionary<K, T> result = new();
            int len = binaryReader.ReadInt32();
            for (int i = 0; i < len; i++)
            {
                K key = keyExtractor(binaryReader);
                T item = itemConstructor();
                item.extractData(binaryReader);
                result.Add(key, item);
            }
            return result;
        }
        public static IDictionary<K,T> ExtractDictionary<K,T>(this BinaryReader binaryReader, Func<K> keyConstructor, Func<T> itemConstructor)
            where K: IBinarySerializable
            where T: IBinarySerializable
        {
            return ExtractDictionary(binaryReader, (reader) =>
            {
                K key = keyConstructor();
                key.extractData(reader);
                return key;
            }, itemConstructor);
        }
        public static IDictionary<K, T> ExtractDictionary<K, T>(this BinaryReader binaryReader, Func<T> itemConstructor)
            where K : IBinarySerializable, new()
            where T : IBinarySerializable
        {
            return ExtractDictionary(binaryReader, () => new K(), itemConstructor);
        }
        public static IDictionary<K, T> ExtractDictionary<K, T>(this BinaryReader binaryReader, Func<K> keyConstructor)
            where K : IBinarySerializable
            where T : IBinarySerializable, new()
        {
            return ExtractDictionary(binaryReader, keyConstructor , () => new T());
        }
        public static IDictionary<K, T> ExtractDictionary<K, T>(this BinaryReader binaryReader)
            where K : IBinarySerializable, new()
            where T : IBinarySerializable, new()
        {
            return ExtractDictionary(binaryReader, ()=>new K(), () => new T());
        }
        public static IDictionary<string, T> ExtractStringDictionary<T>(this BinaryReader binaryReader, Func<T> itemConstructor)
            where T : IBinarySerializable
        {
            return ExtractDictionary(binaryReader, reader=>reader.ReadString(), itemConstructor);
        }
        public static IDictionary<string, T> ExtractStringDictionary<T>(this BinaryReader binaryReader)
            where T : IBinarySerializable, new()
        {
            return ExtractStringDictionary(binaryReader, () => new T());
        }
        public static IDictionary<int, T> ExtractIntDictionary<T>(this BinaryReader binaryReader, Func<T> itemConstructor)
            where T : IBinarySerializable
        {
            return ExtractDictionary(binaryReader, reader => reader.ReadInt32(), itemConstructor);
        }
        public static IDictionary<int, T> ExtractIntDictionary<T>(this BinaryReader binaryReader)
            where T : IBinarySerializable, new()
        {
            return ExtractIntDictionary(binaryReader, () => new T());
        }
    }
}