using System;

namespace YasnaSoftwareGroup.ZylSerialPort
{
    public class ConnectionEventArgs : EventArgs
    {
        private readonly SerialPort.SerialCommPort _mPort;

        public ConnectionEventArgs(SerialPort.SerialCommPort port)
        {
            this._mPort = port;
        }

        public SerialPort.SerialCommPort Port
        {
            get
            {
                return this._mPort;
            }
        }
    }
}

