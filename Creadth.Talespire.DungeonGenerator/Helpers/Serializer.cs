using System;

namespace Creadth.Talespire.DungeonGenerator.Helpers
{
    public static class Serializer
    {
        public static unsafe byte[] Serialize<T>(T value) where T : unmanaged
        {
            var buffer = new byte[sizeof(T)];

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(&value, bufferPtr, sizeof(T), sizeof(T));
            }

            return buffer;
        }

        public static unsafe T Deserialize<T>(byte[] buffer) where T : unmanaged
        {
            var result = new T();

            fixed (byte* bufferPtr = buffer)
            {
                Buffer.MemoryCopy(bufferPtr, &result, sizeof(T), sizeof(T));
            }

            return result;
        }
    }

}
