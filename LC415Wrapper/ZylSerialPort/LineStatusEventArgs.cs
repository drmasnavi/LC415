using System;

namespace YasnaSoftwareGroup.ZylSerialPort
{
    public class LineStatusEventArgs : EventArgs
    {
        private readonly ulong _mLineStatus;

        public LineStatusEventArgs(ulong lineStatus)
        {
            this._mLineStatus = lineStatus;
        }

        public ulong LineStatus
        {
            get
            {
                return this._mLineStatus;
            }
        }
    }
}

