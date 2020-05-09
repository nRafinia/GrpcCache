using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FASTER.core;

namespace Test
{
    public class MyKey : IFasterEqualityComparer<MyKey>
    {
        public string Key;

        public long GetHashCode64(ref MyKey k)
        {
            //return Utility.GetHashCode(k.key);

            return GetInt64HashCode(k.Key);
        }

        public bool Equals(ref MyKey key1, ref MyKey key2)
        {
            //return key1.key == key2.key;
            return GetInt64HashCode(key1.Key) == GetInt64HashCode(key2.Key);
        }

        private static long GetInt64HashCode(string strText)
        {
            long hashCode = 0;
            if (string.IsNullOrEmpty(strText))
                return hashCode;

            //Unicode Encode Covering all character set
            var byteContents = Encoding.Unicode.GetBytes(strText);
            System.Security.Cryptography.SHA256 hash =
                new System.Security.Cryptography.SHA256CryptoServiceProvider();
            var hashText = hash.ComputeHash(byteContents);
            //32Byte hashText separate
            //hashCodeStart = 0~7  8Byte
            //hashCodeMedium = 8~23  8Byte
            //hashCodeEnd = 24~31  8Byte
            //and Fold
            var hashCodeStart = BitConverter.ToInt64(hashText, 0);
            var hashCodeMedium = BitConverter.ToInt64(hashText, 8);
            var hashCodeEnd = BitConverter.ToInt64(hashText, 24);
            hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            return hashCode;
        }
    }

    public class MyKeySerializer : BinaryObjectSerializer<MyKey>
    {
        public override void Serialize(ref MyKey key)
        {
            writer.Write(key.Key);
        }

        public override void Deserialize(ref MyKey key)
        {
            key.Key = reader.ReadString();
        }
    }


    public class MyValue
    {
        public int Value;
        public DateTime ExpireDate;
        //public object Item { get; set; }
    }

    public class MyValueSerializer : BinaryObjectSerializer<MyValue>
    {
        public override void Serialize(ref MyValue value)
        {
            writer.Write(value.ExpireDate.Ticks);
            writer.Write(value.Value);
        }

        public override void Deserialize(ref MyValue value)
        {
            value.ExpireDate = new DateTime(reader.ReadInt64());
            value.Value = reader.ReadInt32();
        }
    }

    public class MyInput
    {
        public int Value;
        public DateTime ExpireDate;
    }

    public class MyOutput
    {
        public MyValue Value;
    }

    public class MyContext { }

    public class MyFunctions : IFunctions<MyKey, MyValue, MyInput, MyOutput, MyContext>
    {
        private long _count = 0;
        public long Count => _count;

        public void InitialUpdater(ref MyKey key, ref MyInput input, ref MyValue value)
        {
            if (value == null)
                value = new MyValue();
            value.ExpireDate = input.ExpireDate;
            value.Value = input.Value;
        }

        public void CopyUpdater(ref MyKey key, ref MyInput input, ref MyValue oldValue, ref MyValue newValue)
        {
            newValue = oldValue;
        }

        public bool InPlaceUpdater(ref MyKey key, ref MyInput input, ref MyValue value)
        {
            value.Value += input.Value;
            return true;
        }

        public void SingleReader(ref MyKey key, ref MyInput input, ref MyValue value, ref MyOutput dst) => dst.Value = value;

        public void SingleWriter(ref MyKey key, ref MyValue src, ref MyValue dst)
        {
            dst = src;
            Interlocked.Increment(ref _count);
        }

        public void ConcurrentReader(ref MyKey key, ref MyInput input, ref MyValue value, ref MyOutput dst) => dst.Value = value;

        public bool ConcurrentWriter(ref MyKey key, ref MyValue src, ref MyValue dst)
        {
            dst = src;

            if (src != null)
                return true;

            Interlocked.Decrement(ref _count);
            return true;

        }

        public void ReadCompletionCallback(ref MyKey key, ref MyInput input, ref MyOutput output, MyContext ctx, Status status) { }

        public void UpsertCompletionCallback(ref MyKey key, ref MyValue value, MyContext ctx)
        {
            Interlocked.Increment(ref _count);
        }

        public void RMWCompletionCallback(ref MyKey key, ref MyInput input, MyContext ctx, Status status) { }

        public void DeleteCompletionCallback(ref MyKey key, MyContext ctx)
        {
            Interlocked.Decrement(ref _count);
        }

        public void CheckpointCompletionCallback(string sessionId, CommitPoint commitPoint) { }

    }

}