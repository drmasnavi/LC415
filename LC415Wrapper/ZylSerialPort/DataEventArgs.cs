using System;

namespace YasnaSoftwareGroup.ZylSerialPort
{
    public class DataEventArgs : EventArgs
    {
        private readonly byte[] _mBuffer;

        public DataEventArgs(byte[] bufferArray)
        {
            this._mBuffer = bufferArray;
        }

        public byte[] Buffer
        {
            get
            {
                return this._mBuffer;
            }
        }
    }
}

