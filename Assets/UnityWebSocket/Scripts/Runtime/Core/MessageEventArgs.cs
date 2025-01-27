using System;
using System.IO;
using System.Text;

namespace UnityWebSocket
{
    public class MessageEventArgs : EventArgs
    {
        private byte[] _rawData;
        private string _data;
        public int RawDataCount;

        public static MessageEventArgs GetObject() => _pool.Rent();
        private static ObjectPool<MessageEventArgs> _pool = new ObjectPool<MessageEventArgs>(32, 256);

        public MessageEventArgs() { }

        internal static void ReturnObject(MessageEventArgs obj, bool forceReturn = true)
        {
            _pool.Return(obj);
            if (forceReturn)
            {
                System.Buffers.ArrayPool<byte>.Shared.Return(obj._rawData);
            }

            obj._rawData = null;
            obj.RawDataCount = 0;
        }

        internal void SetCode(Opcode opcode)
        {
            Opcode = opcode;
        }

        internal void SetData(Opcode opcode, byte[] data)
        {
            Opcode = opcode;
            _rawData = data;
            RawDataCount = data.Length;
        }

        internal void SetData(Opcode opcode, string data)
        {
            Opcode = opcode;
            _data = data;
        }

        public void SetBytesFromMemoryStream(MemoryStream ms)
        {
            if (ms == null) return;
            if (ms.Length == 0) return;
            _rawData = System.Buffers.ArrayPool<byte>.Shared.Rent((int)ms.Length);
            bool result = ms.TryGetBuffer(out ArraySegment<byte> buffer);
            if (result)
            {
                Buffer.BlockCopy(buffer.Array, buffer.Offset, _rawData, 0, (int)ms.Length);
            }
            RawDataCount = (int)ms.Length;
        }


        /// <summary>
        /// Gets the opcode for the message.
        /// </summary>
        /// <value>
        /// <see cref="Opcode.Text"/>, <see cref="Opcode.Binary"/>.
        /// </value>
        internal Opcode Opcode { get; private set; }

        /// <summary>
        /// Gets the message data as a <see cref="string"/>.
        /// </summary>
        /// <value>
        /// A <see cref="string"/> that represents the message data if its type is
        /// text and if decoding it to a string has successfully done;
        /// otherwise, <see langword="null"/>.
        /// </value>
        public string Data => _data ?? (RawDataCount > 0 ? Encoding.UTF8.GetString(_rawData, 0, RawDataCount) : null);

        /// <summary>
        /// Gets the message data as an array of <see cref="byte"/>.
        /// </summary>
        /// <value>
        /// An array of <see cref="byte"/> that represents the message data.
        /// </value>
        public ArraySegment<byte> RawData => _rawData != null ? new ArraySegment<byte>(_rawData, 0, RawDataCount) : (_data != null ? new ArraySegment<byte>(Encoding.UTF8.GetBytes(_data)) : default);


        /// <summary>
        /// Gets a value indicating whether the message type is binary.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message type is binary; otherwise, <c>false</c>.
        /// </value>
        public bool IsBinary => Opcode == Opcode.Binary;

        /// <summary>
        /// Gets a value indicating whether the message type is text.
        /// </summary>
        /// <value>
        /// <c>true</c> if the message type is text; otherwise, <c>false</c>.
        /// </value>
        public bool IsText => Opcode == Opcode.Text;

    }
}