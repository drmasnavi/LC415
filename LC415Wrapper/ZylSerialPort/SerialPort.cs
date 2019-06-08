using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace YasnaSoftwareGroup.ZylSerialPort
{
    public class SerialPort : Component
    {
        private const uint CLRBREAK = 9;
        private const uint CLRDTR = 6;
        private const uint CLRRTS = 4;
        private IntPtr comDevice;
        private Container components;
        private Thread comThread;
        private ConnectionEventHandler _Connected;
        private const uint dcb_AbortOnError = 0x4000;
        private const uint dcb_Binary = 1;
        private const uint dcb_DsrSensivity = 0x40;
        private const uint dcb_DtrControlDisable = 0;
        private const uint dcb_DtrControlEnable = 0x10;
        private const uint dcb_DtrControlHandshake = 0x20;
        private const uint dcb_DtrControlMask = 0x30;
        private const uint dcb_ErrorChar = 0x400;
        private const uint dcb_InX = 0x200;
        private const uint dcb_NullStrip = 0x800;
        private const uint dcb_OutX = 0x100;
        private const uint dcb_OutxCtsFlow = 4;
        private const uint dcb_OutxDsrFlow = 8;
        private const uint dcb_ParityCheck = 2;
        private const uint dcb_Reserveds = 0xffff8000;
        private const uint dcb_RtsControlDisable = 0;
        private const uint dcb_RtsControlEnable = 0x1000;
        private const uint dcb_RtsControlHandshake = 0x2000;
        private const uint dcb_RtsControlMask = 0x3000;
        private const uint dcb_RtsControlToggle = 0x3000;
        private const uint dcb_TXContinueOnXoff = 0x80;
        private ConnectionEventHandler _Disconnected;
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private LineStatusEventHandler _LineStatusChanged;
        private SerialBaudRate m_BaudRate;
        private SerialCommPort m_ConnectedTo;
        private int m_CustomBaudRate;
        private SerialDataWidth m_DataWidth;
        private int m_Delay;
        private bool m_EnableDTROnOpen;
        private bool m_EnableRTSOnOpen;
        private SerialHardwareFlowControl m_HwFlowControl;
        private int m_InputBuffer;
        private bool m_IsReceiving;
        private bool m_IsSending;
        private ulong m_LineStatus;
        private bool m_NeedSynchronization;
        private int m_OutputBuffer;
        private SerialParityBits m_ParityBits;
        private SerialCommPort m_Port;
        private ThreadPriority m_Priority;
        private int m_ReadIntervalTimeout;
        private int m_ReadTotalTimeoutConstant;
        private int m_ReadTotalTimeoutMultiplier;
        private bool m_Registered;
        private SerialStopBits m_StopBits;
        private SerialSoftwareFlowControl m_SwFlowControl;
        private int m_WriteTotalTimeoutConstant;
        private int m_WriteTotalTimeoutMultiplier;
        private const ulong MS_CTS_ON = 0x10L;
        private const ulong MS_DSR_ON = 0x20L;
        private const ulong MS_RING_ON = 0x40L;
        private const ulong MS_RLSD_ON = 0x80L;
        private const uint OPEN_EXISTING = 3;
        private const int PURGE_RXABORT = 2;
        private const int PURGE_RXCLEAR = 8;
        private const int PURGE_TXABORT = 1;
        private const int PURGE_TXCLEAR = 4;
        private DataEventHandler _Received;
        private DataEventHandler _Sent;
        private const uint SETBREAK = 8;
        private const uint SETDTR = 5;
        private const uint SETRTS = 3;
        private const uint SETXOFF = 1;
        private const uint SETXON = 2;
        private static object synchronizeVariable = "locking variable for threads";

        [Description("Occures after the serial port is connected.")]
        public event ConnectionEventHandler Connected
        {
            add
            {
                ConnectionEventHandler handler2;
                ConnectionEventHandler connected = _Connected;
                do
                {
                    handler2 = connected;
                    var handler3 = (ConnectionEventHandler) Delegate.Combine(handler2, value);
                    connected = Interlocked.CompareExchange<ConnectionEventHandler>(ref _Connected, handler3, handler2);
                }
                while (connected != handler2);
            }
            remove
            {
                ConnectionEventHandler handler2;
                ConnectionEventHandler connected = _Connected;
                do
                {
                    handler2 = connected;
                    ConnectionEventHandler handler3 = (ConnectionEventHandler) Delegate.Remove(handler2, value);
                    connected = Interlocked.CompareExchange<ConnectionEventHandler>(ref _Connected, handler3, handler2);
                }
                while (connected != handler2);
            }
        }

        [Description("Occures when the serial port is disconnected.")]
        public event ConnectionEventHandler Disconnected
        {
            add
            {
                ConnectionEventHandler handler2;
                ConnectionEventHandler disconnected = _Disconnected;
                do
                {
                    handler2 = disconnected;
                    ConnectionEventHandler handler3 = (ConnectionEventHandler) Delegate.Combine(handler2, value);
                    disconnected = Interlocked.CompareExchange<ConnectionEventHandler>(ref _Disconnected, handler3, handler2);
                }
                while (disconnected != handler2);
            }
            remove
            {
                ConnectionEventHandler handler2;
                ConnectionEventHandler disconnected = _Disconnected;
                do
                {
                    handler2 = disconnected;
                    var handler3 = (ConnectionEventHandler) Delegate.Remove(handler2, value);
                    disconnected = Interlocked.CompareExchange<ConnectionEventHandler>(ref _Disconnected, handler3, handler2);
                }
                while (disconnected != handler2);
            }
        }

        [Description("Occures when the line status of the serial port is changed.")]
        public event LineStatusEventHandler LineStatusChanged
        {
            add
            {
                LineStatusEventHandler handler2;
                LineStatusEventHandler lineStatusChanged = _LineStatusChanged;
                do
                {
                    handler2 = lineStatusChanged;
                    var handler3 = (LineStatusEventHandler) Delegate.Combine(handler2, value);
                    lineStatusChanged = Interlocked.CompareExchange<LineStatusEventHandler>(ref _LineStatusChanged, handler3, handler2);
                }
                while (lineStatusChanged != handler2);
            }
            remove
            {
                LineStatusEventHandler handler2;
                LineStatusEventHandler lineStatusChanged = _LineStatusChanged;
                do
                {
                    handler2 = lineStatusChanged;
                    var handler3 = (LineStatusEventHandler) Delegate.Remove(handler2, value);
                    lineStatusChanged = Interlocked.CompareExchange<LineStatusEventHandler>(ref _LineStatusChanged, handler3, handler2);
                }
                while (lineStatusChanged != handler2);
            }
        }

        [Description("Occures when data was received.")]
        public event DataEventHandler Received
        {
            add
            {
                DataEventHandler handler2;
                DataEventHandler received = _Received;
                do
                {
                    handler2 = received;
                    DataEventHandler handler3 = (DataEventHandler) Delegate.Combine(handler2, value);
                    received = Interlocked.CompareExchange<DataEventHandler>(ref _Received, handler3, handler2);
                }
                while (received != handler2);
            }
            remove
            {
                DataEventHandler handler2;
                DataEventHandler received = _Received;
                do
                {
                    handler2 = received;
                    DataEventHandler handler3 = (DataEventHandler) Delegate.Remove(handler2, value);
                    received = Interlocked.CompareExchange<DataEventHandler>(ref _Received, handler3, handler2);
                }
                while (received != handler2);
            }
        }

        [Description("Occures when data was sent.")]
        public event DataEventHandler Sent
        {
            add
            {
                DataEventHandler handler2;
                DataEventHandler sent = _Sent;
                do
                {
                    handler2 = sent;
                    var handler3 = (DataEventHandler) Delegate.Combine(handler2, value);
                    sent = Interlocked.CompareExchange<DataEventHandler>(ref _Sent, handler3, handler2);
                }
                while (sent != handler2);
            }
            remove
            {
                DataEventHandler handler2;
                DataEventHandler sent = _Sent;
                do
                {
                    handler2 = sent;
                    DataEventHandler handler3 = (DataEventHandler) Delegate.Remove(handler2, value);
                    sent = Interlocked.CompareExchange<DataEventHandler>(ref _Sent, handler3, handler2);
                }
                while (sent != handler2);
            }
        }

        public SerialPort()
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            InitializeComponent();
            timeBeginPeriod(1);
        }

        public SerialPort(IContainer container)
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            container.Add(this);
            InitializeComponent();
        }

        public SerialPort(SerialCommPort port)
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            InitializeComponent();
            timeBeginPeriod(1);
            m_Port = port;
        }

        public SerialPort(SerialCommPort port, SerialDataWidth dataWidth, SerialStopBits stopBits, SerialParityBits parityBits)
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            InitializeComponent();
            timeBeginPeriod(1);
            m_Port = port;
            m_DataWidth = dataWidth;
            m_StopBits = stopBits;
            m_ParityBits = parityBits;
        }

        public SerialPort(SerialCommPort port, SerialDataWidth dataWidth, SerialStopBits stopBits, SerialParityBits parityBits, SerialHardwareFlowControl hwFlowControl, SerialSoftwareFlowControl swFlowControl)
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            InitializeComponent();
            timeBeginPeriod(1);
            m_Port = port;
            m_DataWidth = dataWidth;
            m_StopBits = stopBits;
            m_ParityBits = parityBits;
            m_HwFlowControl = hwFlowControl;
            m_SwFlowControl = swFlowControl;
        }

        public SerialPort(SerialCommPort port, SerialDataWidth dataWidth, SerialStopBits stopBits, SerialParityBits parityBits, SerialHardwareFlowControl hwFlowControl, SerialSoftwareFlowControl swFlowControl, bool enableDTROnOpen, bool enableRTSOnOpen)
        {
            comDevice = IntPtr.Zero;
            m_Port = SerialCommPort.COM02;
            m_BaudRate = SerialBaudRate.br004800;
            m_DataWidth = SerialDataWidth.dw8Bits;
            m_EnableDTROnOpen = true;
            m_EnableRTSOnOpen = true;
            m_Delay = 100;
            m_Priority = ThreadPriority.Normal;
            m_InputBuffer = 0x1000;
            m_OutputBuffer = 0x1000;
            m_ReadIntervalTimeout = -1;
            m_WriteTotalTimeoutMultiplier = 100;
            m_WriteTotalTimeoutConstant = 0x3e8;
            InitializeComponent();
            timeBeginPeriod(1);
            m_Port = port;
            m_DataWidth = dataWidth;
            m_StopBits = stopBits;
            m_ParityBits = parityBits;
            m_HwFlowControl = hwFlowControl;
            m_SwFlowControl = swFlowControl;
            m_EnableDTROnOpen = enableDTROnOpen;
            m_EnableRTSOnOpen = enableRTSOnOpen;
        }

        public static string ASCIIByteArrayToString(byte[] characters)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(characters);
        }

        public static string ByteArrayToDecimalString(byte[] data)
        {
            string str = string.Empty;
            for (int i = 0; i < (data.Length - 1); i++)
            {
                str = str + data[i];
            }
            return str;
        }

        public static string ByteArrayToHexaString(byte[] data)
        {
            string str = string.Empty;
            for (int i = 0; i < (data.Length - 1); i++)
            {
                str = str + data[i].ToString("x");
            }
            return str;
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int ClearCommError(IntPtr hFile, out int lpErrors, ref COMSTAT lpComStat);
        public void ClearInputBuffer()
        {
            PurgeComm(comDevice, 8);
        }

        public void ClearOutputBuffer()
        {
            PurgeComm(comDevice, 4);
        }

        [Description("Is Port Currently Open?")]
        public bool IsOpen
            {
            get
                {
                return m_ConnectedTo != SerialCommPort.None;
                }


            }

        public void Close()
        {
            try
            {
                if (((int) comDevice) <= 0)
                {
                    return;
                }
                if (m_NeedSynchronization)
                {
                    lock (synchronizeVariable)
                    {
                        OnDisconnected(new ConnectionEventArgs(m_ConnectedTo));
                        goto Label_0050;
                    }
                }
                OnDisconnected(new ConnectionEventArgs(m_ConnectedTo));
            Label_0050:
                if (((int) comDevice) > 0)
                {
                    PurgeComm(comDevice, 15);
                    CloseHandle(comDevice);
                    comDevice = IntPtr.Zero;
                }
                m_ConnectedTo = SerialCommPort.None;
                if ((comThread != null) && comThread.IsAlive)
                {
                    comThread.Abort();
                }
            }
            catch
            {
                comDevice = IntPtr.Zero;
                m_ConnectedTo = SerialCommPort.None;
            }
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool CloseHandle(IntPtr hObject);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, int lpSecurityAttributes, uint dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            try
            {
                if (((int) comDevice) > 0)
                {
                    CloseHandle(comDevice);
                    comDevice = IntPtr.Zero;
                }
            }
            catch
            {
            }
            if ((comThread != null) && comThread.IsAlive)
            {
                comThread.Abort();
            }
            timeEndPeriod(1);
            base.Dispose(disposing);
        }

       /* private void DoRegistrationCheck()
        {
            if (!m_Registered)
            {
                MessageBox.Show("ZylSerialPort.NET - Demo version by Zyl Soft - www.zylsoft.com");
            }
        }*/

        [DllImport("kernel32.dll", SetLastError=true)]
        internal static extern bool EscapeCommFunction(IntPtr hFile, uint dwFunc);
        ~SerialPort()
        {
            Dispose(false);
        }

        public bool GetCD()
        {
            return ((GetLineStatus(comDevice) & ((ulong) 8L)) != 0L);
        }

        public static bool GetCD(ulong lineStatus)
        {
            return ((lineStatus & ((ulong) 8L)) != 0L);
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool GetCommModemStatus(IntPtr hObject, ref ulong lpModemStat);
        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int GetCommState(IntPtr hCommDev, ref DCB lpDCB);
        public bool GetCTS()
        {
            return ((GetLineStatus(comDevice) & ((ulong) 1L)) != 0L);
        }

        public static bool GetCTS(ulong lineStatus)
        {
            return ((lineStatus & ((ulong) 1L)) != 0L);
        }

        public bool GetDSR()
        {
            return ((GetLineStatus(comDevice) & ((ulong) 2L)) != 0L);
        }

        public static bool GetDSR(ulong lineStatus)
        {
            return ((lineStatus & ((ulong) 2L)) != 0L);
        }

        public static SerialCommPort[] GetExistingCommPorts()
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");
                string[] valueNames = key.GetValueNames();
                SerialCommPort[] portArray = new SerialCommPort[valueNames.Length];
                for (int i = 0; i < valueNames.Length; i++)
                {
                    portArray[i] = StringToSerialCommPort(key.GetValue(valueNames[i]).ToString());
                }
                return portArray;
            }
            catch
            {
                return null;
            }
        }

        private ulong GetLineStatus(IntPtr hComDevice)
        {
            ulong num = 0L;
            if (((int) hComDevice) <= 0)
            {
                return 0L;
            }
            ulong lpModemStat = 0L;
            if (!GetCommModemStatus(hComDevice, ref lpModemStat))
            {
                return 0L;
            }
            if ((lpModemStat & ((ulong) 0x10L)) != 0L)
            {
                num |= (ulong) 1L;
            }
            if ((lpModemStat & ((ulong) 0x20L)) != 0L)
            {
                num |= (ulong) 2L;
            }
            if ((lpModemStat & ((ulong) 0x40L)) != 0L)
            {
                num |= (ulong) 4L;
            }
            if ((lpModemStat & ((ulong) 0x80L)) != 0L)
            {
                num |= (ulong) 8L;
            }
            return num;
        }

        public bool GetRING()
        {
            return ((GetLineStatus(comDevice) & ((ulong) 4L)) != 0L);
        }

        public static bool GetRING(ulong lineStatus)
        {
            return ((lineStatus & ((ulong) 4L)) != 0L);
        }

        private void InitializeComponent()
        {
            components = new Container();
        }

        protected virtual void OnConnected(ConnectionEventArgs e)
        {
            if (_Connected != null)
            {
                _Connected(this, e);
            }
        }

        protected virtual void OnDisconnected(ConnectionEventArgs e)
        {
            if (_Disconnected != null)
            {
                _Disconnected(this, e);
            }
        }

        protected virtual void OnLineStatusChanged(LineStatusEventArgs e)
        {
            if (_LineStatusChanged != null)
            {
                _LineStatusChanged(this, e);
            }
        }

        protected virtual void OnReceived(DataEventArgs e)
        {
            if (_Received != null)
            {
                _Received(this, e);
            }
        }

        protected virtual void OnSent(DataEventArgs e)
        {
            if (_Sent != null)
            {
                _Sent(this, e);
            }
        }

        public bool Open()
        {
            if (m_Port != m_ConnectedTo)
            {
                bool flag;
                //DoRegistrationCheck();
                if (m_ConnectedTo != SerialCommPort.None)
                {
                    Close();
                }
                try
                {
                    comDevice = CreateFile(@"\\.\COM" + ((uint) m_Port).ToString(), 0xc0000000, 0, 0, 3, 0, 0);
                    if (((int) comDevice) <= 0)
                    {
                        return false;
                    }
                    if (!SetDCBState(comDevice))
                    {
                        return false;
                    }
                    if (!SetBuffers(comDevice, m_InputBuffer, m_OutputBuffer))
                    {
                        return false;
                    }
                    SetTimeouts(comDevice);
                    var start = new ThreadStart(ThreadJob);
                    comThread = new Thread(start) {Priority = m_Priority};
                    comThread.Start();
                    m_ConnectedTo = m_Port;
                    if (m_NeedSynchronization)
                    {
                        lock (synchronizeVariable)
                        {
                            OnConnected(new ConnectionEventArgs(m_Port));
                            goto Label_012A;
                        }
                    }
                    OnConnected(new ConnectionEventArgs(m_Port));
                Label_012A:
                    flag = true;
                }
                catch
                {
                    flag = false;
                }
                return flag;
            }
            return false;
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int PurgeComm(IntPtr hFile, int dwFlags);
        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int ReadFile(IntPtr hFile, byte[] Buffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, int Flags);
        public int SendASCIIString(string text)
        {
            return SendByteArray(StringToASCIIByteArray(text));
        }

        public int SendByteArray(byte[] btArray)
        {
            int lpNumberOfBytesWritten = 0;
            if ((btArray == null) || (btArray.Length <= 0))
            {
                return 0;
            }
            if (((int) comDevice) > 0)
            {
                try
                {
                    m_IsSending = true;
                    WriteFile(comDevice, btArray, btArray.Length, ref lpNumberOfBytesWritten, 0);
                    m_IsSending = false;
                    if (lpNumberOfBytesWritten > 0)
                    {
                        OnSent(new DataEventArgs(btArray));
                    }
                    return lpNumberOfBytesWritten;
                }
                catch
                {
                    m_IsSending = false;
                    throw new Exception(m_Port + " is not open.");
                }
            }
            throw new Exception(m_Port + " is not open.");
        }

        public int SendUnicodeString(string text)
        {
            return SendByteArray(StringToUnicodeByteArray(text));
        }

        public int SendUTF7String(string text)
        {
            return SendByteArray(StringToUTF7ByteArray(text));
        }

        public int SendUTF8String(string text)
        {
            return SendByteArray(StringToUTF8ByteArray(text));
        }

        public static string SerialCommPortToString(SerialCommPort port)
        {
            if (port == SerialCommPort.COM01)
            {
                return "COM1";
            }
            if (port == SerialCommPort.COM02)
            {
                return "COM2";
            }
            if (port == SerialCommPort.COM03)
            {
                return "COM3";
            }
            if (port == SerialCommPort.COM04)
            {
                return "COM4";
            }
            if (port == SerialCommPort.COM05)
            {
                return "COM5";
            }
            if (port == SerialCommPort.COM06)
            {
                return "COM6";
            }
            if (port == SerialCommPort.COM07)
            {
                return "COM7";
            }
            if (port == SerialCommPort.COM08)
            {
                return "COM8";
            }
            if (port == SerialCommPort.COM09)
            {
                return "COM9";
            }
            if (port == SerialCommPort.COM10)
            {
                return "COM10";
            }
            if (port == SerialCommPort.COM11)
            {
                return "COM11";
            }
            if (port == SerialCommPort.COM12)
            {
                return "COM12";
            }
            if (port == SerialCommPort.COM13)
            {
                return "COM13";
            }
            if (port == SerialCommPort.COM14)
            {
                return "COM14";
            }
            if (port == SerialCommPort.COM15)
            {
                return "COM15";
            }
            if (port == SerialCommPort.COM16)
            {
                return "COM16";
            }
            if (port == SerialCommPort.COM17)
            {
                return "COM17";
            }
            if (port == SerialCommPort.COM18)
            {
                return "COM18";
            }
            if (port == SerialCommPort.COM19)
            {
                return "COM19";
            }
            if (port == SerialCommPort.COM20)
            {
                return "COM20";
            }
            if (port == SerialCommPort.COM21)
            {
                return "COM21";
            }
            if (port == SerialCommPort.COM22)
            {
                return "COM22";
            }
            if (port == SerialCommPort.COM23)
            {
                return "COM23";
            }
            if (port == SerialCommPort.COM24)
            {
                return "COM24";
            }
            if (port == SerialCommPort.COM25)
            {
                return "COM25";
            }
            if (port == SerialCommPort.COM26)
            {
                return "COM26";
            }
            if (port == SerialCommPort.COM27)
            {
                return "COM27";
            }
            if (port == SerialCommPort.COM28)
            {
                return "COM28";
            }
            if (port == SerialCommPort.COM29)
            {
                return "COM29";
            }
            if (port == SerialCommPort.COM30)
            {
                return "COM30";
            }
            if (port == SerialCommPort.COM31)
            {
                return "COM31";
            }
            if (port == SerialCommPort.COM32)
            {
                return "COM32";
            }
            if (port == SerialCommPort.COM33)
            {
                return "COM33";
            }
            if (port == SerialCommPort.COM34)
            {
                return "COM34";
            }
            if (port == SerialCommPort.COM35)
            {
                return "COM35";
            }
            if (port == SerialCommPort.COM36)
            {
                return "COM36";
            }
            if (port == SerialCommPort.COM37)
            {
                return "COM37";
            }
            if (port == SerialCommPort.COM38)
            {
                return "COM38";
            }
            if (port == SerialCommPort.COM39)
            {
                return "COM39";
            }
            if (port == SerialCommPort.COM40)
            {
                return "COM40";
            }
            if (port == SerialCommPort.COM41)
            {
                return "COM41";
            }
            if (port == SerialCommPort.COM42)
            {
                return "COM42";
            }
            if (port == SerialCommPort.COM43)
            {
                return "COM43";
            }
            if (port == SerialCommPort.COM44)
            {
                return "COM44";
            }
            if (port == SerialCommPort.COM45)
            {
                return "COM45";
            }
            if (port == SerialCommPort.COM46)
            {
                return "COM46";
            }
            if (port == SerialCommPort.COM47)
            {
                return "COM47";
            }
            if (port == SerialCommPort.COM48)
            {
                return "COM48";
            }
            if (port == SerialCommPort.COM49)
            {
                return "COM49";
            }
            if (port == SerialCommPort.COM50)
            {
                return "COM50";
            }
            if (port == SerialCommPort.COM51)
            {
                return "COM51";
            }
            if (port == SerialCommPort.COM52)
            {
                return "COM52";
            }
            if (port == SerialCommPort.COM53)
            {
                return "COM53";
            }
            if (port == SerialCommPort.COM54)
            {
                return "COM54";
            }
            if (port == SerialCommPort.COM55)
            {
                return "COM55";
            }
            if (port == SerialCommPort.COM56)
            {
                return "COM56";
            }
            if (port == SerialCommPort.COM57)
            {
                return "COM57";
            }
            if (port == SerialCommPort.COM58)
            {
                return "COM58";
            }
            if (port == SerialCommPort.COM59)
            {
                return "COM59";
            }
            if (port == SerialCommPort.COM60)
            {
                return "COM60";
            }
            if (port == SerialCommPort.COM61)
            {
                return "COM61";
            }
            if (port == SerialCommPort.COM62)
            {
                return "COM62";
            }
            if (port == SerialCommPort.COM63)
            {
                return "COM63";
            }
            if (port == SerialCommPort.COM64)
            {
                return "COM64";
            }
            if (port == SerialCommPort.COM65)
            {
                return "COM65";
            }
            if (port == SerialCommPort.COM66)
            {
                return "COM66";
            }
            if (port == SerialCommPort.COM67)
            {
                return "COM67";
            }
            if (port == SerialCommPort.COM68)
            {
                return "COM68";
            }
            if (port == SerialCommPort.COM69)
            {
                return "COM69";
            }
            if (port == SerialCommPort.COM70)
            {
                return "COM70";
            }
            if (port == SerialCommPort.COM81)
            {
                return "COM81";
            }
            if (port == SerialCommPort.COM82)
            {
                return "COM82";
            }
            if (port == SerialCommPort.COM83)
            {
                return "COM83";
            }
            if (port == SerialCommPort.COM84)
            {
                return "COM84";
            }
            if (port == SerialCommPort.COM85)
            {
                return "COM85";
            }
            if (port == SerialCommPort.COM86)
            {
                return "COM86";
            }
            if (port == SerialCommPort.COM87)
            {
                return "COM87";
            }
            if (port == SerialCommPort.COM88)
            {
                return "COM88";
            }
            if (port == SerialCommPort.COM89)
            {
                return "COM89";
            }
            if (port == SerialCommPort.COM90)
            {
                return "COM90";
            }
            if (port == SerialCommPort.COM91)
            {
                return "COM91";
            }
            if (port == SerialCommPort.COM92)
            {
                return "COM92";
            }
            if (port == SerialCommPort.COM93)
            {
                return "COM93";
            }
            if (port == SerialCommPort.COM94)
            {
                return "COM94";
            }
            if (port == SerialCommPort.COM95)
            {
                return "COM95";
            }
            if (port == SerialCommPort.COM96)
            {
                return "COM96";
            }
            if (port == SerialCommPort.COM97)
            {
                return "COM97";
            }
            if (port == SerialCommPort.COM98)
            {
                return "COM98";
            }
            if (port == SerialCommPort.COM99)
            {
                return "COM99";
            }
            if (port == SerialCommPort.COM100)
            {
                return "COM100";
            }
            if (port == SerialCommPort.COM101)
            {
                return "COM101";
            }
            if (port == SerialCommPort.COM102)
            {
                return "COM102";
            }
            if (port == SerialCommPort.COM103)
            {
                return "COM103";
            }
            if (port == SerialCommPort.COM104)
            {
                return "COM104";
            }
            if (port == SerialCommPort.COM105)
            {
                return "COM105";
            }
            if (port == SerialCommPort.COM106)
            {
                return "COM106";
            }
            if (port == SerialCommPort.COM107)
            {
                return "COM107";
            }
            if (port == SerialCommPort.COM108)
            {
                return "COM108";
            }
            if (port == SerialCommPort.COM109)
            {
                return "COM109";
            }
            if (port == SerialCommPort.COM110)
            {
                return "COM110";
            }
            if (port == SerialCommPort.COM111)
            {
                return "COM111";
            }
            if (port == SerialCommPort.COM112)
            {
                return "COM112";
            }
            if (port == SerialCommPort.COM113)
            {
                return "COM113";
            }
            if (port == SerialCommPort.COM114)
            {
                return "COM114";
            }
            if (port == SerialCommPort.COM115)
            {
                return "COM115";
            }
            if (port == SerialCommPort.COM116)
            {
                return "COM116";
            }
            if (port == SerialCommPort.COM117)
            {
                return "COM117";
            }
            if (port == SerialCommPort.COM118)
            {
                return "COM118";
            }
            if (port == SerialCommPort.COM119)
            {
                return "COM119";
            }
            if (port == SerialCommPort.COM120)
            {
                return "COM120";
            }
            if (port == SerialCommPort.COM121)
            {
                return "COM121";
            }
            if (port == SerialCommPort.COM122)
            {
                return "COM122";
            }
            if (port == SerialCommPort.COM123)
            {
                return "COM123";
            }
            if (port == SerialCommPort.COM124)
            {
                return "COM124";
            }
            if (port == SerialCommPort.COM125)
            {
                return "COM125";
            }
            if (port == SerialCommPort.COM126)
            {
                return "COM126";
            }
            if (port == SerialCommPort.COM127)
            {
                return "COM127";
            }
            if (port == SerialCommPort.COM128)
            {
                return "COM128";
            }
            if (port == SerialCommPort.COM129)
            {
                return "COM129";
            }
            if (port == SerialCommPort.COM130)
            {
                return "COM130";
            }
            if (port == SerialCommPort.COM131)
            {
                return "COM131";
            }
            if (port == SerialCommPort.COM132)
            {
                return "COM132";
            }
            if (port == SerialCommPort.COM133)
            {
                return "COM133";
            }
            if (port == SerialCommPort.COM134)
            {
                return "COM134";
            }
            if (port == SerialCommPort.COM135)
            {
                return "COM135";
            }
            if (port == SerialCommPort.COM136)
            {
                return "COM136";
            }
            if (port == SerialCommPort.COM137)
            {
                return "COM137";
            }
            if (port == SerialCommPort.COM138)
            {
                return "COM138";
            }
            if (port == SerialCommPort.COM139)
            {
                return "COM139";
            }
            if (port == SerialCommPort.COM140)
            {
                return "COM140";
            }
            if (port == SerialCommPort.COM141)
            {
                return "COM141";
            }
            if (port == SerialCommPort.COM142)
            {
                return "COM142";
            }
            if (port == SerialCommPort.COM143)
            {
                return "COM143";
            }
            if (port == SerialCommPort.COM144)
            {
                return "COM144";
            }
            if (port == SerialCommPort.COM145)
            {
                return "COM145";
            }
            if (port == SerialCommPort.COM146)
            {
                return "COM146";
            }
            if (port == SerialCommPort.COM147)
            {
                return "COM147";
            }
            if (port == SerialCommPort.COM148)
            {
                return "COM148";
            }
            if (port == SerialCommPort.COM149)
            {
                return "COM149";
            }
            if (port == SerialCommPort.COM150)
            {
                return "COM150";
            }
            if (port == SerialCommPort.COM151)
            {
                return "COM151";
            }
            if (port == SerialCommPort.COM152)
            {
                return "COM152";
            }
            if (port == SerialCommPort.COM153)
            {
                return "COM153";
            }
            if (port == SerialCommPort.COM154)
            {
                return "COM154";
            }
            if (port == SerialCommPort.COM155)
            {
                return "COM155";
            }
            if (port == SerialCommPort.COM156)
            {
                return "COM156";
            }
            if (port == SerialCommPort.COM157)
            {
                return "COM157";
            }
            if (port == SerialCommPort.COM158)
            {
                return "COM158";
            }
            if (port == SerialCommPort.COM159)
            {
                return "COM159";
            }
            if (port == SerialCommPort.COM160)
            {
                return "COM160";
            }
            if (port == SerialCommPort.COM161)
            {
                return "COM161";
            }
            if (port == SerialCommPort.COM162)
            {
                return "COM162";
            }
            if (port == SerialCommPort.COM163)
            {
                return "COM163";
            }
            if (port == SerialCommPort.COM164)
            {
                return "COM164";
            }
            if (port == SerialCommPort.COM165)
            {
                return "COM165";
            }
            if (port == SerialCommPort.COM166)
            {
                return "COM166";
            }
            if (port == SerialCommPort.COM167)
            {
                return "COM167";
            }
            if (port == SerialCommPort.COM168)
            {
                return "COM168";
            }
            if (port == SerialCommPort.COM169)
            {
                return "COM169";
            }
            if (port == SerialCommPort.COM170)
            {
                return "COM170";
            }
            if (port == SerialCommPort.COM181)
            {
                return "COM181";
            }
            if (port == SerialCommPort.COM182)
            {
                return "COM182";
            }
            if (port == SerialCommPort.COM183)
            {
                return "COM183";
            }
            if (port == SerialCommPort.COM184)
            {
                return "COM184";
            }
            if (port == SerialCommPort.COM185)
            {
                return "COM185";
            }
            if (port == SerialCommPort.COM186)
            {
                return "COM186";
            }
            if (port == SerialCommPort.COM187)
            {
                return "COM187";
            }
            if (port == SerialCommPort.COM188)
            {
                return "COM188";
            }
            if (port == SerialCommPort.COM189)
            {
                return "COM189";
            }
            if (port == SerialCommPort.COM190)
            {
                return "COM190";
            }
            if (port == SerialCommPort.COM191)
            {
                return "COM191";
            }
            if (port == SerialCommPort.COM192)
            {
                return "COM192";
            }
            if (port == SerialCommPort.COM193)
            {
                return "COM193";
            }
            if (port == SerialCommPort.COM194)
            {
                return "COM194";
            }
            if (port == SerialCommPort.COM95)
            {
                return "COM195";
            }
            if (port == SerialCommPort.COM196)
            {
                return "COM196";
            }
            if (port == SerialCommPort.COM197)
            {
                return "COM197";
            }
            if (port == SerialCommPort.COM198)
            {
                return "COM198";
            }
            if (port == SerialCommPort.COM199)
            {
                return "COM199";
            }
            if (port == SerialCommPort.COM200)
            {
                return "COM200";
            }
            if (port == SerialCommPort.COM201)
            {
                return "COM201";
            }
            if (port == SerialCommPort.COM202)
            {
                return "COM202";
            }
            if (port == SerialCommPort.COM203)
            {
                return "COM203";
            }
            if (port == SerialCommPort.COM204)
            {
                return "COM204";
            }
            if (port == SerialCommPort.COM205)
            {
                return "COM205";
            }
            if (port == SerialCommPort.COM206)
            {
                return "COM206";
            }
            if (port == SerialCommPort.COM207)
            {
                return "COM207";
            }
            if (port == SerialCommPort.COM208)
            {
                return "COM208";
            }
            if (port == SerialCommPort.COM209)
            {
                return "COM209";
            }
            if (port == SerialCommPort.COM210)
            {
                return "COM210";
            }
            if (port == SerialCommPort.COM211)
            {
                return "COM211";
            }
            if (port == SerialCommPort.COM212)
            {
                return "COM212";
            }
            if (port == SerialCommPort.COM213)
            {
                return "COM213";
            }
            if (port == SerialCommPort.COM214)
            {
                return "COM214";
            }
            if (port == SerialCommPort.COM215)
            {
                return "COM215";
            }
            if (port == SerialCommPort.COM216)
            {
                return "COM216";
            }
            if (port == SerialCommPort.COM217)
            {
                return "COM217";
            }
            if (port == SerialCommPort.COM218)
            {
                return "COM218";
            }
            if (port == SerialCommPort.COM219)
            {
                return "COM219";
            }
            if (port == SerialCommPort.COM220)
            {
                return "COM220";
            }
            if (port == SerialCommPort.COM221)
            {
                return "COM221";
            }
            if (port == SerialCommPort.COM222)
            {
                return "COM222";
            }
            if (port == SerialCommPort.COM223)
            {
                return "COM223";
            }
            if (port == SerialCommPort.COM224)
            {
                return "COM224";
            }
            if (port == SerialCommPort.COM225)
            {
                return "COM225";
            }
            if (port == SerialCommPort.COM226)
            {
                return "COM226";
            }
            if (port == SerialCommPort.COM227)
            {
                return "COM227";
            }
            if (port == SerialCommPort.COM228)
            {
                return "COM228";
            }
            if (port == SerialCommPort.COM229)
            {
                return "COM229";
            }
            if (port == SerialCommPort.COM230)
            {
                return "COM230";
            }
            if (port == SerialCommPort.COM231)
            {
                return "COM231";
            }
            if (port == SerialCommPort.COM232)
            {
                return "COM232";
            }
            if (port == SerialCommPort.COM233)
            {
                return "COM233";
            }
            if (port == SerialCommPort.COM234)
            {
                return "COM234";
            }
            if (port == SerialCommPort.COM235)
            {
                return "COM235";
            }
            if (port == SerialCommPort.COM236)
            {
                return "COM236";
            }
            if (port == SerialCommPort.COM237)
            {
                return "COM237";
            }
            if (port == SerialCommPort.COM238)
            {
                return "COM238";
            }
            if (port == SerialCommPort.COM239)
            {
                return "COM239";
            }
            if (port == SerialCommPort.COM240)
            {
                return "COM240";
            }
            if (port == SerialCommPort.COM241)
            {
                return "COM241";
            }
            if (port == SerialCommPort.COM242)
            {
                return "COM242";
            }
            if (port == SerialCommPort.COM243)
            {
                return "COM243";
            }
            if (port == SerialCommPort.COM244)
            {
                return "COM244";
            }
            if (port == SerialCommPort.COM245)
            {
                return "COM245";
            }
            if (port == SerialCommPort.COM246)
            {
                return "COM246";
            }
            if (port == SerialCommPort.COM247)
            {
                return "COM247";
            }
            if (port == SerialCommPort.COM248)
            {
                return "COM248";
            }
            if (port == SerialCommPort.COM249)
            {
                return "COM249";
            }
            if (port == SerialCommPort.COM250)
            {
                return "COM250";
            }
            if (port == SerialCommPort.COM251)
            {
                return "COM251";
            }
            if (port == SerialCommPort.COM252)
            {
                return "COM252";
            }
            if (port == SerialCommPort.COM253)
            {
                return "COM253";
            }
            if (port == SerialCommPort.COM254)
            {
                return "COM254";
            }
            if (port == SerialCommPort.COM255)
            {
                return "COM255";
            }
            return string.Empty;
        }

        public void SetBreak(bool onOff)
        {
            uint num;
            if (onOff)
            {
                num = 8;
            }
            else
            {
                num = 9;
            }
            EscapeCommFunction(comDevice, num);
        }

        private bool SetBuffers(IntPtr hComDevice, int inputBuffer, int outputBuffer)
        {
            if (((int) hComDevice) <= 0)
            {
                return false;
            }
            try
            {
                return SetupComm(hComDevice, inputBuffer, outputBuffer);
            }
            catch
            {
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int SetCommState(IntPtr hCommDev, ref DCB lpDCB);
        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int SetCommTimeouts(IntPtr hFile, ref COMMTIMEOUTS lpCommTimeouts);
        private bool SetDCBState(IntPtr hComDevice)
        {
            if (((int) hComDevice) <= 0)
            {
                return false;
            }
            try
            {
                DCB lpDCB = new DCB();
                GetCommState(hComDevice, ref lpDCB);
                if (m_BaudRate != SerialBaudRate.brCustom)
                {
                    lpDCB.BaudRate = (int) m_BaudRate;
                }
                else
                {
                    lpDCB.BaudRate = m_CustomBaudRate;
                }
                lpDCB.ByteSize = (byte) m_DataWidth;
                lpDCB.StopBits = (byte) m_StopBits;
                lpDCB.Parity = (byte) m_ParityBits;
                lpDCB.Flags = 1;
                if (m_EnableDTROnOpen)
                {
                    lpDCB.Flags |= 0x10;
                }
                if (m_EnableRTSOnOpen)
                {
                    lpDCB.Flags |= 0x1000;
                }
                switch (m_HwFlowControl)
                {
                    case SerialHardwareFlowControl.hfRTS:
                        lpDCB.Flags |= 0x1000;
                        break;

                    case SerialHardwareFlowControl.hfRTSCTS:
                        lpDCB.Flags = (lpDCB.Flags | 4) | 0x2000;
                        break;
                }
                switch (m_SwFlowControl)
                {
                    case SerialSoftwareFlowControl.sfXONXOFF:
                        lpDCB.Flags = (lpDCB.Flags | 0x100) | 0x200;
                        break;
                }
                lpDCB.XonChar = '\x0011';
                lpDCB.XoffChar = '\x0013';
                return (SetCommState(hComDevice, ref lpDCB) != 0);
            }
            catch
            {
                return false;
            }
        }

        public void SetDTR(bool OnOff)
        {
            uint num;
            if (OnOff)
            {
                num = 5;
            }
            else
            {
                num = 6;
            }
            EscapeCommFunction(comDevice, num);
        }

        public void SetRTS(bool OnOff)
        {
            uint num;
            if (OnOff)
            {
                num = 3;
            }
            else
            {
                num = 4;
            }
            EscapeCommFunction(comDevice, num);
        }

        private bool SetTimeouts(IntPtr hComDevice)
        {
            if (((int) hComDevice) <= 0)
            {
                return false;
            }
            try
            {
                COMMTIMEOUTS lpCommTimeouts = new COMMTIMEOUTS();
                lpCommTimeouts.ReadIntervalTimeout = m_ReadIntervalTimeout;
                lpCommTimeouts.ReadTotalTimeoutMultiplier = m_ReadTotalTimeoutMultiplier;
                lpCommTimeouts.ReadTotalTimeoutConstant = m_ReadTotalTimeoutConstant;
                lpCommTimeouts.WriteTotalTimeoutMultiplier = m_WriteTotalTimeoutMultiplier;
                lpCommTimeouts.WriteTotalTimeoutConstant = m_WriteTotalTimeoutConstant;
                if (SetCommTimeouts(hComDevice, ref lpCommTimeouts) == 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool SetupComm(IntPtr hObject, int InBuffer, int OutBuffer);
        public void SetXonXoff(bool OnOff)
        {
            uint num;
            if (OnOff)
            {
                num = 2;
            }
            else
            {
                num = 1;
            }
            EscapeCommFunction(comDevice, num);
        }

        public static byte[] StringToASCIIByteArray(string text)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        public static SerialCommPort StringToSerialCommPort(string port)
        {
            if ((port.ToUpper() == "COM1") || (port.ToUpper() == "COM01"))
            {
                return SerialCommPort.COM01;
            }
            if ((port.ToUpper() == "COM2") || (port.ToUpper() == "COM02"))
            {
                return SerialCommPort.COM02;
            }
            if ((port.ToUpper() == "COM3") || (port.ToUpper() == "COM03"))
            {
                return SerialCommPort.COM03;
            }
            if ((port.ToUpper() == "COM4") || (port.ToUpper() == "COM04"))
            {
                return SerialCommPort.COM04;
            }
            if ((port.ToUpper() == "COM5") || (port.ToUpper() == "COM05"))
            {
                return SerialCommPort.COM05;
            }
            if ((port.ToUpper() == "COM6") || (port.ToUpper() == "COM06"))
            {
                return SerialCommPort.COM06;
            }
            if ((port.ToUpper() == "COM7") || (port.ToUpper() == "COM07"))
            {
                return SerialCommPort.COM07;
            }
            if ((port.ToUpper() == "COM8") || (port.ToUpper() == "COM08"))
            {
                return SerialCommPort.COM08;
            }
            if ((port.ToUpper() == "COM9") || (port.ToUpper() == "COM09"))
            {
                return SerialCommPort.COM09;
            }
            if (port.ToUpper() == "COM10")
            {
                return SerialCommPort.COM10;
            }
            if (port.ToUpper() == "COM11")
            {
                return SerialCommPort.COM11;
            }
            if (port.ToUpper() == "COM12")
            {
                return SerialCommPort.COM12;
            }
            if (port.ToUpper() == "COM13")
            {
                return SerialCommPort.COM13;
            }
            if (port.ToUpper() == "COM14")
            {
                return SerialCommPort.COM14;
            }
            if (port.ToUpper() == "COM15")
            {
                return SerialCommPort.COM15;
            }
            if (port.ToUpper() == "COM16")
            {
                return SerialCommPort.COM16;
            }
            if (port.ToUpper() == "COM17")
            {
                return SerialCommPort.COM17;
            }
            if (port.ToUpper() == "COM18")
            {
                return SerialCommPort.COM18;
            }
            if (port.ToUpper() == "COM19")
            {
                return SerialCommPort.COM19;
            }
            if (port.ToUpper() == "COM20")
            {
                return SerialCommPort.COM20;
            }
            if (port.ToUpper() == "COM21")
            {
                return SerialCommPort.COM21;
            }
            if (port.ToUpper() == "COM22")
            {
                return SerialCommPort.COM22;
            }
            if (port.ToUpper() == "COM23")
            {
                return SerialCommPort.COM23;
            }
            if (port.ToUpper() == "COM24")
            {
                return SerialCommPort.COM24;
            }
            if (port.ToUpper() == "COM25")
            {
                return SerialCommPort.COM25;
            }
            if (port.ToUpper() == "COM26")
            {
                return SerialCommPort.COM26;
            }
            if (port.ToUpper() == "COM27")
            {
                return SerialCommPort.COM27;
            }
            if (port.ToUpper() == "COM28")
            {
                return SerialCommPort.COM28;
            }
            if (port.ToUpper() == "COM29")
            {
                return SerialCommPort.COM29;
            }
            if (port.ToUpper() == "COM30")
            {
                return SerialCommPort.COM30;
            }
            if (port.ToUpper() == "COM31")
            {
                return SerialCommPort.COM31;
            }
            if (port.ToUpper() == "COM32")
            {
                return SerialCommPort.COM32;
            }
            if (port.ToUpper() == "COM33")
            {
                return SerialCommPort.COM33;
            }
            if (port.ToUpper() == "COM34")
            {
                return SerialCommPort.COM34;
            }
            if (port.ToUpper() == "COM35")
            {
                return SerialCommPort.COM35;
            }
            if (port.ToUpper() == "COM36")
            {
                return SerialCommPort.COM36;
            }
            if (port.ToUpper() == "COM37")
            {
                return SerialCommPort.COM37;
            }
            if (port.ToUpper() == "COM38")
            {
                return SerialCommPort.COM38;
            }
            if (port.ToUpper() == "COM39")
            {
                return SerialCommPort.COM39;
            }
            if (port.ToUpper() == "COM40")
            {
                return SerialCommPort.COM40;
            }
            if (port.ToUpper() == "COM41")
            {
                return SerialCommPort.COM41;
            }
            if (port.ToUpper() == "COM42")
            {
                return SerialCommPort.COM42;
            }
            if (port.ToUpper() == "COM43")
            {
                return SerialCommPort.COM43;
            }
            if (port.ToUpper() == "COM44")
            {
                return SerialCommPort.COM44;
            }
            if (port.ToUpper() == "COM45")
            {
                return SerialCommPort.COM45;
            }
            if (port.ToUpper() == "COM46")
            {
                return SerialCommPort.COM46;
            }
            if (port.ToUpper() == "COM47")
            {
                return SerialCommPort.COM47;
            }
            if (port.ToUpper() == "COM48")
            {
                return SerialCommPort.COM48;
            }
            if (port.ToUpper() == "COM49")
            {
                return SerialCommPort.COM49;
            }
            if (port.ToUpper() == "COM50")
            {
                return SerialCommPort.COM50;
            }
            if (port.ToUpper() == "COM51")
            {
                return SerialCommPort.COM51;
            }
            if (port.ToUpper() == "COM52")
            {
                return SerialCommPort.COM52;
            }
            if (port.ToUpper() == "COM53")
            {
                return SerialCommPort.COM53;
            }
            if (port.ToUpper() == "COM54")
            {
                return SerialCommPort.COM54;
            }
            if (port.ToUpper() == "COM55")
            {
                return SerialCommPort.COM55;
            }
            if (port.ToUpper() == "COM56")
            {
                return SerialCommPort.COM56;
            }
            if (port.ToUpper() == "COM57")
            {
                return SerialCommPort.COM57;
            }
            if (port.ToUpper() == "COM58")
            {
                return SerialCommPort.COM58;
            }
            if (port.ToUpper() == "COM59")
            {
                return SerialCommPort.COM59;
            }
            if (port.ToUpper() == "COM60")
            {
                return SerialCommPort.COM60;
            }
            if (port.ToUpper() == "COM61")
            {
                return SerialCommPort.COM61;
            }
            if (port.ToUpper() == "COM62")
            {
                return SerialCommPort.COM62;
            }
            if (port.ToUpper() == "COM63")
            {
                return SerialCommPort.COM63;
            }
            if (port.ToUpper() == "COM64")
            {
                return SerialCommPort.COM64;
            }
            if (port.ToUpper() == "COM65")
            {
                return SerialCommPort.COM65;
            }
            if (port.ToUpper() == "COM66")
            {
                return SerialCommPort.COM66;
            }
            if (port.ToUpper() == "COM67")
            {
                return SerialCommPort.COM67;
            }
            if (port.ToUpper() == "COM68")
            {
                return SerialCommPort.COM68;
            }
            if (port.ToUpper() == "COM69")
            {
                return SerialCommPort.COM69;
            }
            if (port.ToUpper() == "COM70")
            {
                return SerialCommPort.COM70;
            }
            if (port.ToUpper() == "COM71")
            {
                return SerialCommPort.COM71;
            }
            if (port.ToUpper() == "COM72")
            {
                return SerialCommPort.COM72;
            }
            if (port.ToUpper() == "COM73")
            {
                return SerialCommPort.COM73;
            }
            if (port.ToUpper() == "COM74")
            {
                return SerialCommPort.COM74;
            }
            if (port.ToUpper() == "COM75")
            {
                return SerialCommPort.COM75;
            }
            if (port.ToUpper() == "COM76")
            {
                return SerialCommPort.COM76;
            }
            if (port.ToUpper() == "COM77")
            {
                return SerialCommPort.COM77;
            }
            if (port.ToUpper() == "COM78")
            {
                return SerialCommPort.COM78;
            }
            if (port.ToUpper() == "COM79")
            {
                return SerialCommPort.COM79;
            }
            if (port.ToUpper() == "COM80")
            {
                return SerialCommPort.COM80;
            }
            if (port.ToUpper() == "COM81")
            {
                return SerialCommPort.COM81;
            }
            if (port.ToUpper() == "COM82")
            {
                return SerialCommPort.COM82;
            }
            if (port.ToUpper() == "COM83")
            {
                return SerialCommPort.COM83;
            }
            if (port.ToUpper() == "COM84")
            {
                return SerialCommPort.COM84;
            }
            if (port.ToUpper() == "COM85")
            {
                return SerialCommPort.COM85;
            }
            if (port.ToUpper() == "COM86")
            {
                return SerialCommPort.COM86;
            }
            if (port.ToUpper() == "COM87")
            {
                return SerialCommPort.COM87;
            }
            if (port.ToUpper() == "COM88")
            {
                return SerialCommPort.COM88;
            }
            if (port.ToUpper() == "COM89")
            {
                return SerialCommPort.COM89;
            }
            if (port.ToUpper() == "COM90")
            {
                return SerialCommPort.COM90;
            }
            if (port.ToUpper() == "COM91")
            {
                return SerialCommPort.COM91;
            }
            if (port.ToUpper() == "COM92")
            {
                return SerialCommPort.COM92;
            }
            if (port.ToUpper() == "COM93")
            {
                return SerialCommPort.COM93;
            }
            if (port.ToUpper() == "COM94")
            {
                return SerialCommPort.COM94;
            }
            if (port.ToUpper() == "COM95")
            {
                return SerialCommPort.COM95;
            }
            if (port.ToUpper() == "COM96")
            {
                return SerialCommPort.COM96;
            }
            if (port.ToUpper() == "COM97")
            {
                return SerialCommPort.COM97;
            }
            if (port.ToUpper() == "COM98")
            {
                return SerialCommPort.COM98;
            }
            if (port.ToUpper() == "COM99")
            {
                return SerialCommPort.COM99;
            }
            if (port.ToUpper() == "COM100")
            {
                return SerialCommPort.COM100;
            }
            if (port.ToUpper() == "COM101")
            {
                return SerialCommPort.COM101;
            }
            if (port.ToUpper() == "COM102")
            {
                return SerialCommPort.COM102;
            }
            if (port.ToUpper() == "COM103")
            {
                return SerialCommPort.COM103;
            }
            if (port.ToUpper() == "COM104")
            {
                return SerialCommPort.COM104;
            }
            if (port.ToUpper() == "COM105")
            {
                return SerialCommPort.COM105;
            }
            if (port.ToUpper() == "COM106")
            {
                return SerialCommPort.COM106;
            }
            if (port.ToUpper() == "COM107")
            {
                return SerialCommPort.COM107;
            }
            if (port.ToUpper() == "COM108")
            {
                return SerialCommPort.COM108;
            }
            if (port.ToUpper() == "COM109")
            {
                return SerialCommPort.COM109;
            }
            if (port.ToUpper() == "COM110")
            {
                return SerialCommPort.COM110;
            }
            if (port.ToUpper() == "COM111")
            {
                return SerialCommPort.COM111;
            }
            if (port.ToUpper() == "COM112")
            {
                return SerialCommPort.COM112;
            }
            if (port.ToUpper() == "COM113")
            {
                return SerialCommPort.COM113;
            }
            if (port.ToUpper() == "COM114")
            {
                return SerialCommPort.COM114;
            }
            if (port.ToUpper() == "COM115")
            {
                return SerialCommPort.COM115;
            }
            if (port.ToUpper() == "COM116")
            {
                return SerialCommPort.COM116;
            }
            if (port.ToUpper() == "COM117")
            {
                return SerialCommPort.COM117;
            }
            if (port.ToUpper() == "COM118")
            {
                return SerialCommPort.COM118;
            }
            if (port.ToUpper() == "COM119")
            {
                return SerialCommPort.COM119;
            }
            if (port.ToUpper() == "COM120")
            {
                return SerialCommPort.COM120;
            }
            if (port.ToUpper() == "COM121")
            {
                return SerialCommPort.COM121;
            }
            if (port.ToUpper() == "COM122")
            {
                return SerialCommPort.COM122;
            }
            if (port.ToUpper() == "COM123")
            {
                return SerialCommPort.COM123;
            }
            if (port.ToUpper() == "COM124")
            {
                return SerialCommPort.COM124;
            }
            if (port.ToUpper() == "COM125")
            {
                return SerialCommPort.COM125;
            }
            if (port.ToUpper() == "COM126")
            {
                return SerialCommPort.COM126;
            }
            if (port.ToUpper() == "COM127")
            {
                return SerialCommPort.COM127;
            }
            if (port.ToUpper() == "COM128")
            {
                return SerialCommPort.COM128;
            }
            if (port.ToUpper() == "COM129")
            {
                return SerialCommPort.COM129;
            }
            if (port.ToUpper() == "COM130")
            {
                return SerialCommPort.COM130;
            }
            if (port.ToUpper() == "COM131")
            {
                return SerialCommPort.COM131;
            }
            if (port.ToUpper() == "COM132")
            {
                return SerialCommPort.COM132;
            }
            if (port.ToUpper() == "COM133")
            {
                return SerialCommPort.COM133;
            }
            if (port.ToUpper() == "COM134")
            {
                return SerialCommPort.COM134;
            }
            if (port.ToUpper() == "COM135")
            {
                return SerialCommPort.COM135;
            }
            if (port.ToUpper() == "COM136")
            {
                return SerialCommPort.COM136;
            }
            if (port.ToUpper() == "COM137")
            {
                return SerialCommPort.COM137;
            }
            if (port.ToUpper() == "COM138")
            {
                return SerialCommPort.COM138;
            }
            if (port.ToUpper() == "COM139")
            {
                return SerialCommPort.COM139;
            }
            if (port.ToUpper() == "COM140")
            {
                return SerialCommPort.COM140;
            }
            if (port.ToUpper() == "COM141")
            {
                return SerialCommPort.COM141;
            }
            if (port.ToUpper() == "COM142")
            {
                return SerialCommPort.COM142;
            }
            if (port.ToUpper() == "COM143")
            {
                return SerialCommPort.COM143;
            }
            if (port.ToUpper() == "COM144")
            {
                return SerialCommPort.COM144;
            }
            if (port.ToUpper() == "COM145")
            {
                return SerialCommPort.COM145;
            }
            if (port.ToUpper() == "COM146")
            {
                return SerialCommPort.COM146;
            }
            if (port.ToUpper() == "COM147")
            {
                return SerialCommPort.COM147;
            }
            if (port.ToUpper() == "COM148")
            {
                return SerialCommPort.COM148;
            }
            if (port.ToUpper() == "COM149")
            {
                return SerialCommPort.COM149;
            }
            if (port.ToUpper() == "COM150")
            {
                return SerialCommPort.COM150;
            }
            if (port.ToUpper() == "COM151")
            {
                return SerialCommPort.COM151;
            }
            if (port.ToUpper() == "COM152")
            {
                return SerialCommPort.COM152;
            }
            if (port.ToUpper() == "COM153")
            {
                return SerialCommPort.COM153;
            }
            if (port.ToUpper() == "COM154")
            {
                return SerialCommPort.COM154;
            }
            if (port.ToUpper() == "COM155")
            {
                return SerialCommPort.COM155;
            }
            if (port.ToUpper() == "COM156")
            {
                return SerialCommPort.COM156;
            }
            if (port.ToUpper() == "COM157")
            {
                return SerialCommPort.COM157;
            }
            if (port.ToUpper() == "COM158")
            {
                return SerialCommPort.COM158;
            }
            if (port.ToUpper() == "COM159")
            {
                return SerialCommPort.COM159;
            }
            if (port.ToUpper() == "COM160")
            {
                return SerialCommPort.COM160;
            }
            if (port.ToUpper() == "COM161")
            {
                return SerialCommPort.COM161;
            }
            if (port.ToUpper() == "COM162")
            {
                return SerialCommPort.COM162;
            }
            if (port.ToUpper() == "COM163")
            {
                return SerialCommPort.COM163;
            }
            if (port.ToUpper() == "COM164")
            {
                return SerialCommPort.COM164;
            }
            if (port.ToUpper() == "COM165")
            {
                return SerialCommPort.COM165;
            }
            if (port.ToUpper() == "COM166")
            {
                return SerialCommPort.COM166;
            }
            if (port.ToUpper() == "COM167")
            {
                return SerialCommPort.COM167;
            }
            if (port.ToUpper() == "COM168")
            {
                return SerialCommPort.COM168;
            }
            if (port.ToUpper() == "COM169")
            {
                return SerialCommPort.COM169;
            }
            if (port.ToUpper() == "COM170")
            {
                return SerialCommPort.COM170;
            }
            if (port.ToUpper() == "COM171")
            {
                return SerialCommPort.COM171;
            }
            if (port.ToUpper() == "COM172")
            {
                return SerialCommPort.COM172;
            }
            if (port.ToUpper() == "COM173")
            {
                return SerialCommPort.COM173;
            }
            if (port.ToUpper() == "COM174")
            {
                return SerialCommPort.COM174;
            }
            if (port.ToUpper() == "COM175")
            {
                return SerialCommPort.COM175;
            }
            if (port.ToUpper() == "COM176")
            {
                return SerialCommPort.COM176;
            }
            if (port.ToUpper() == "COM177")
            {
                return SerialCommPort.COM177;
            }
            if (port.ToUpper() == "COM178")
            {
                return SerialCommPort.COM178;
            }
            if (port.ToUpper() == "COM179")
            {
                return SerialCommPort.COM179;
            }
            if (port.ToUpper() == "COM180")
            {
                return SerialCommPort.COM180;
            }
            if (port.ToUpper() == "COM181")
            {
                return SerialCommPort.COM181;
            }
            if (port.ToUpper() == "COM182")
            {
                return SerialCommPort.COM182;
            }
            if (port.ToUpper() == "COM183")
            {
                return SerialCommPort.COM183;
            }
            if (port.ToUpper() == "COM184")
            {
                return SerialCommPort.COM184;
            }
            if (port.ToUpper() == "COM185")
            {
                return SerialCommPort.COM185;
            }
            if (port.ToUpper() == "COM186")
            {
                return SerialCommPort.COM186;
            }
            if (port.ToUpper() == "COM187")
            {
                return SerialCommPort.COM187;
            }
            if (port.ToUpper() == "COM188")
            {
                return SerialCommPort.COM188;
            }
            if (port.ToUpper() == "COM189")
            {
                return SerialCommPort.COM189;
            }
            if (port.ToUpper() == "COM190")
            {
                return SerialCommPort.COM190;
            }
            if (port.ToUpper() == "COM191")
            {
                return SerialCommPort.COM191;
            }
            if (port.ToUpper() == "COM192")
            {
                return SerialCommPort.COM192;
            }
            if (port.ToUpper() == "COM193")
            {
                return SerialCommPort.COM193;
            }
            if (port.ToUpper() == "COM194")
            {
                return SerialCommPort.COM194;
            }
            if (port.ToUpper() == "COM195")
            {
                return SerialCommPort.COM195;
            }
            if (port.ToUpper() == "COM196")
            {
                return SerialCommPort.COM196;
            }
            if (port.ToUpper() == "COM197")
            {
                return SerialCommPort.COM197;
            }
            if (port.ToUpper() == "COM198")
            {
                return SerialCommPort.COM198;
            }
            if (port.ToUpper() == "COM199")
            {
                return SerialCommPort.COM199;
            }
            if (port.ToUpper() == "COM200")
            {
                return SerialCommPort.COM200;
            }
            if (port.ToUpper() == "COM201")
            {
                return SerialCommPort.COM201;
            }
            if (port.ToUpper() == "COM202")
            {
                return SerialCommPort.COM202;
            }
            if (port.ToUpper() == "COM203")
            {
                return SerialCommPort.COM203;
            }
            if (port.ToUpper() == "COM204")
            {
                return SerialCommPort.COM204;
            }
            if (port.ToUpper() == "COM205")
            {
                return SerialCommPort.COM205;
            }
            if (port.ToUpper() == "COM206")
            {
                return SerialCommPort.COM206;
            }
            if (port.ToUpper() == "COM207")
            {
                return SerialCommPort.COM207;
            }
            if (port.ToUpper() == "COM208")
            {
                return SerialCommPort.COM208;
            }
            if (port.ToUpper() == "COM209")
            {
                return SerialCommPort.COM209;
            }
            if (port.ToUpper() == "COM210")
            {
                return SerialCommPort.COM210;
            }
            if (port.ToUpper() == "COM211")
            {
                return SerialCommPort.COM211;
            }
            if (port.ToUpper() == "COM212")
            {
                return SerialCommPort.COM212;
            }
            if (port.ToUpper() == "COM213")
            {
                return SerialCommPort.COM213;
            }
            if (port.ToUpper() == "COM214")
            {
                return SerialCommPort.COM214;
            }
            if (port.ToUpper() == "COM215")
            {
                return SerialCommPort.COM215;
            }
            if (port.ToUpper() == "COM216")
            {
                return SerialCommPort.COM216;
            }
            if (port.ToUpper() == "COM217")
            {
                return SerialCommPort.COM217;
            }
            if (port.ToUpper() == "COM218")
            {
                return SerialCommPort.COM218;
            }
            if (port.ToUpper() == "COM219")
            {
                return SerialCommPort.COM219;
            }
            if (port.ToUpper() == "COM220")
            {
                return SerialCommPort.COM220;
            }
            if (port.ToUpper() == "COM221")
            {
                return SerialCommPort.COM221;
            }
            if (port.ToUpper() == "COM222")
            {
                return SerialCommPort.COM222;
            }
            if (port.ToUpper() == "COM223")
            {
                return SerialCommPort.COM223;
            }
            if (port.ToUpper() == "COM224")
            {
                return SerialCommPort.COM224;
            }
            if (port.ToUpper() == "COM225")
            {
                return SerialCommPort.COM225;
            }
            if (port.ToUpper() == "COM226")
            {
                return SerialCommPort.COM226;
            }
            if (port.ToUpper() == "COM227")
            {
                return SerialCommPort.COM227;
            }
            if (port.ToUpper() == "COM228")
            {
                return SerialCommPort.COM228;
            }
            if (port.ToUpper() == "COM229")
            {
                return SerialCommPort.COM229;
            }
            if (port.ToUpper() == "COM230")
            {
                return SerialCommPort.COM230;
            }
            if (port.ToUpper() == "COM231")
            {
                return SerialCommPort.COM231;
            }
            if (port.ToUpper() == "COM232")
            {
                return SerialCommPort.COM232;
            }
            if (port.ToUpper() == "COM233")
            {
                return SerialCommPort.COM233;
            }
            if (port.ToUpper() == "COM234")
            {
                return SerialCommPort.COM234;
            }
            if (port.ToUpper() == "COM235")
            {
                return SerialCommPort.COM235;
            }
            if (port.ToUpper() == "COM236")
            {
                return SerialCommPort.COM236;
            }
            if (port.ToUpper() == "COM237")
            {
                return SerialCommPort.COM237;
            }
            if (port.ToUpper() == "COM238")
            {
                return SerialCommPort.COM238;
            }
            if (port.ToUpper() == "COM239")
            {
                return SerialCommPort.COM239;
            }
            if (port.ToUpper() == "COM240")
            {
                return SerialCommPort.COM240;
            }
            if (port.ToUpper() == "COM241")
            {
                return SerialCommPort.COM241;
            }
            if (port.ToUpper() == "COM242")
            {
                return SerialCommPort.COM242;
            }
            if (port.ToUpper() == "COM243")
            {
                return SerialCommPort.COM243;
            }
            if (port.ToUpper() == "COM244")
            {
                return SerialCommPort.COM244;
            }
            if (port.ToUpper() == "COM245")
            {
                return SerialCommPort.COM245;
            }
            if (port.ToUpper() == "COM246")
            {
                return SerialCommPort.COM246;
            }
            if (port.ToUpper() == "COM247")
            {
                return SerialCommPort.COM247;
            }
            if (port.ToUpper() == "COM248")
            {
                return SerialCommPort.COM248;
            }
            if (port.ToUpper() == "COM249")
            {
                return SerialCommPort.COM249;
            }
            if (port.ToUpper() == "COM250")
            {
                return SerialCommPort.COM250;
            }
            if (port.ToUpper() == "COM251")
            {
                return SerialCommPort.COM251;
            }
            if (port.ToUpper() == "COM252")
            {
                return SerialCommPort.COM252;
            }
            if (port.ToUpper() == "COM253")
            {
                return SerialCommPort.COM253;
            }
            if (port.ToUpper() == "COM254")
            {
                return SerialCommPort.COM254;
            }
            if (port.ToUpper() == "COM255")
            {
                return SerialCommPort.COM255;
            }
            return SerialCommPort.None;
        }

        public static byte[] StringToUnicodeByteArray(string text)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetBytes(text);
        }

        public static byte[] StringToUTF7ByteArray(string text)
        {
            UTF7Encoding encoding = new UTF7Encoding();
            return encoding.GetBytes(text);
        }

        public static byte[] StringToUTF8ByteArray(string text)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        private void ThreadJob()
        {
            int lpNumberOfBytesRead = 0;
            while ((((int) comDevice) > 0) && comThread.IsAlive)
            {
                try
                {
                    int num2;
                    COMSTAT comstat;
                    ulong lineStatus = GetLineStatus(comDevice);
                    if (m_LineStatus != lineStatus)
                    {
                        m_LineStatus = lineStatus;
                        if (m_NeedSynchronization)
                        {
                            lock (synchronizeVariable)
                            {
                                OnLineStatusChanged(new LineStatusEventArgs(lineStatus));
                                goto Label_0060;
                            }
                        }
                        OnLineStatusChanged(new LineStatusEventArgs(lineStatus));
                    }
                Label_0060:
                    comstat = new COMSTAT();
                    ClearCommError(comDevice, out num2, ref comstat);
                    int cbInQue = comstat.cbInQue;
                    if (((cbInQue > 0) && (((int) comDevice) > 0)) && comThread.IsAlive)
                    {
                        var buffer = new byte[cbInQue];
                        m_IsReceiving = true;
                        ReadFile(comDevice, buffer, cbInQue, ref lpNumberOfBytesRead, 0);
                        m_IsReceiving = false;
                        if (lpNumberOfBytesRead > 0)
                        {
                            if (m_NeedSynchronization)
                            {
                                lock (synchronizeVariable)
                                {
                                    OnReceived(new DataEventArgs(buffer));
                                    goto Label_0113;
                                }
                            }
                            OnReceived(new DataEventArgs(buffer));
                        }
                    }
                }
                catch
                {
                    m_IsReceiving = false;
                }
            Label_0113:
                if ((((int) comDevice) > 0) && comThread.IsAlive)
                {
                    Thread.Sleep(m_Delay);
                }
            }
        }

        [DllImport("winmm.dll")]
        internal static extern uint timeBeginPeriod(uint period);
        [DllImport("winmm.dll")]
        internal static extern uint timeEndPeriod(uint period);
        public static string UnicodeByteArrayToString(byte[] characters)
        {
            UnicodeEncoding encoding = new UnicodeEncoding();
            return encoding.GetString(characters);
        }

        public static string UTF7ByteArrayToString(byte[] characters)
        {
            UTF7Encoding encoding = new UTF7Encoding();
            return encoding.GetString(characters);
        }

        public static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(characters);
        }

        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern int WriteFile(IntPtr hFile, byte[] Buffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, int Flags);

        [Description("Baud rate at which the communications device operates.")]
        public SerialBaudRate BaudRate
        {
            get
            {
                return m_BaudRate;
            }
            set
            {
                if (m_BaudRate != value)
                {
                    m_BaudRate = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Returns the physical port where the component is connected to."), Browsable(false)]
        public SerialCommPort ConnectedTo
        {
            get
            {
                return m_ConnectedTo;
            }
        }

        [Description("Custom baud rate value, used when BaudRate is set to brCustom.")]
        public int CustomBaudRate
        {
            get
            {
                return m_CustomBaudRate;
            }
            set
            {
                if (m_CustomBaudRate != value)
                {
                    m_CustomBaudRate = value;
                    if (m_BaudRate == SerialBaudRate.brCustom)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Number of bits in the bytes transmitted and received.")]
        public SerialDataWidth DataWidth
        {
            get
            {
                return m_DataWidth;
            }
            set
            {
                if (m_DataWidth != value)
                {
                    if ((m_StopBits == SerialStopBits.sb1_5Bits) && (((value == SerialDataWidth.dw6Bits) || (value == SerialDataWidth.dw7Bits)) || (value == SerialDataWidth.dw8Bits)))
                    {
                        throw new Exception("Invalid data-width and stop-bits combination.");
                    }
                    if ((m_StopBits == SerialStopBits.sb2Bits) && (value == SerialDataWidth.dw5Bits))
                    {
                        throw new Exception("Invalid data-width and stop-bits combination.");
                    }
                    m_DataWidth = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Time interval between two receivings in milliseconds (frequency).")]
        public int Delay
        {
            get
            {
                return m_Delay;
            }
            set
            {
                m_Delay = value;
            }
        }

        [Description("Enable/disable DTR when the port is open.")]
        public bool EnableDTROnOpen
        {
            get
            {
                return m_EnableDTROnOpen;
            }
            set
            {
                if (m_EnableDTROnOpen != value)
                {
                    m_EnableDTROnOpen = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Enable/disable RTS when the port is open.")]
        public bool EnableRTSOnOpen
        {
            get
            {
                return m_EnableRTSOnOpen;
            }
            set
            {
                if (m_EnableRTSOnOpen != value)
                {
                    m_EnableRTSOnOpen = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Hardware flow control of the serial port.")]
        public SerialHardwareFlowControl HardwareFlowControl
        {
            get
            {
                return m_HwFlowControl;
            }
            set
            {
                if (m_HwFlowControl != value)
                {
                    m_HwFlowControl = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Recommended size of the device's internal input buffer, in bytes.")]
        public int InputBuffer
        {
            get
            {
                return m_InputBuffer;
            }
            set
            {
                if (m_InputBuffer != value)
                {
                    m_InputBuffer = value;
                    if (((int) comDevice) > 0)
                    {
                        SetBuffers(comDevice, m_InputBuffer, m_OutputBuffer);
                    }
                }
            }
        }

        [Description("This property is false when the component is receiving data from the port, otherwise is false. Use this property to check if the component is inside a receiving process."), Browsable(false)]
        public bool IsReceiving
        {
            get
            {
                return m_IsReceiving;
            }
        }

        [Browsable(false), Description("This property is true when the component is sending data to the port, otherwise is false. Use this property to check if the component is inside a sending process.")]
        public bool IsSending
        {
            get
            {
                return m_IsSending;
            }
        }

        [Browsable(false), Description("Line status of the serial port.")]
        public ulong LineStatus
        {
            get
            {
                return m_LineStatus;
            }
        }

        public bool NeedSynchronization
        {
            get
            {
                return m_NeedSynchronization;
            }
            set
            {
                m_NeedSynchronization = value;
            }
        }

        [Description("Recommended size of the device's internal output buffer, in bytes.")]
        public int OutputBuffer
        {
            get
            {
                return m_OutputBuffer;
            }
            set
            {
                if (m_OutputBuffer != value)
                {
                    m_OutputBuffer = value;
                    if (((int) comDevice) > 0)
                    {
                        SetBuffers(comDevice, m_InputBuffer, m_OutputBuffer);
                    }
                }
            }
        }

        [Description("Parity scheme to be used.")]
        public SerialParityBits ParityBits
        {
            get
            {
                return m_ParityBits;
            }
            set
            {
                if (m_ParityBits != value)
                {
                    m_ParityBits = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Physical name of the serial port.")]
        public SerialCommPort Port
        {
            get
            {
                return m_Port;
            }
            set
            {
                if (m_Port != value)
                {
                    m_Port = value;
                }
            }
        }

        [Description("Priority of the receiver thread.")]
        public ThreadPriority Priority
        {
            get
            {
                return m_Priority;
            }
            set
            {
                if (m_Priority != value)
                {
                    m_Priority = value;
                    if (comThread != null)
                    {
                        comThread.Priority = m_Priority;
                    }
                }
            }
        }

        [Description("Maximum time allowed to elapse between the arrival of two characters on the communications line, in milliseconds. During a ReadFile operation, the time period begins when the first character is received. If the interval between the arrival of any two characters exceeds this amount, the ReadFile operation is completed and any buffered data is returned. A value of zero indicates that interval time-outs are not used. A value of MAXDWORD, combined with zero values for both the ReadTotalTimeoutConstant and ReadTotalTimeoutMultiplier members, specifies that the read operation is to return immediately with the characters that have already been received, even if no characters have been received.")]
        public int ReadIntervalTimeout
        {
            get
            {
                return m_ReadIntervalTimeout;
            }
            set
            {
                if (m_ReadIntervalTimeout != value)
                {
                    m_ReadIntervalTimeout = value;
                    if (((int) comDevice) > 0)
                    {
                        SetTimeouts(comDevice);
                    }
                }
            }
        }

        [Description("Constant used to calculate the total time-out period for read operations, in milliseconds. For each read operation, this value is added to the product of the ReadTotalTimeoutMultiplier member and the requested number of bytes. A value of zero for both the ReadTotalTimeoutMultiplier and ReadTotalTimeoutConstant members indicates that total time-outs are not used for read operations.")]
        public int ReadTotalTimeoutConstant
        {
            get
            {
                return m_ReadTotalTimeoutConstant;
            }
            set
            {
                if (m_ReadTotalTimeoutConstant != value)
                {
                    m_ReadTotalTimeoutConstant = value;
                    if (((int) comDevice) > 0)
                    {
                        SetTimeouts(comDevice);
                    }
                }
            }
        }

        [Description("Multiplier used to calculate the total time-out period for read operations, in milliseconds. For each read operation, this value is multiplied by the requested number of bytes to be read.")]
        public int ReadTotalTimeoutMultiplier
        {
            get
            {
                return m_ReadTotalTimeoutMultiplier;
            }
            set
            {
                if (m_ReadTotalTimeoutMultiplier != value)
                {
                    m_ReadTotalTimeoutMultiplier = value;
                    if (((int) comDevice) > 0)
                    {
                        SetTimeouts(comDevice);
                    }
                }
            }
        }

        [Description("Software flow control of the serial port.")]
        public SerialSoftwareFlowControl SoftwareFlowControl
        {
            get
            {
                return m_SwFlowControl;
            }
            set
            {
                if (m_SwFlowControl != value)
                {
                    m_SwFlowControl = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Number of stop bits to be used.")]
        public SerialStopBits StopBits
        {
            get
            {
                return m_StopBits;
            }
            set
            {
                if (m_StopBits != value)
                {
                    if ((value == SerialStopBits.sb1_5Bits) && (((m_DataWidth == SerialDataWidth.dw6Bits) || (m_DataWidth == SerialDataWidth.dw7Bits)) || (m_DataWidth == SerialDataWidth.dw8Bits)))
                    {
                        throw new Exception("Invalid data-width and stop-bits combination.");
                    }
                    if ((value == SerialStopBits.sb2Bits) && (m_DataWidth == SerialDataWidth.dw5Bits))
                    {
                        throw new Exception("Invalid data-width and stop-bits combination.");
                    }
                    m_StopBits = value;
                    if (((int) comDevice) > 0)
                    {
                        SetDCBState(comDevice);
                    }
                }
            }
        }

        [Description("Constant used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is added to the product of the WriteTotalTimeoutMultiplier member and the number of bytes to be written. A value of zero for both the WriteTotalTimeoutMultiplier and WriteTotalTimeoutConstant members indicates that total time-outs are not used for write operations.")]
        public int WriteTotalTimeoutConstant
        {
            get
            {
                return m_WriteTotalTimeoutConstant;
            }
            set
            {
                if (m_WriteTotalTimeoutConstant != value)
                {
                    m_WriteTotalTimeoutConstant = value;
                    if (((int) comDevice) > 0)
                    {
                        SetTimeouts(comDevice);
                    }
                }
            }
        }

        [Description("Multiplier used to calculate the total time-out period for write operations, in milliseconds. For each write operation, this value is multiplied by the number of bytes to be written.")]
        public int WriteTotalTimeoutMultiplier
        {
            get
            {
                return m_WriteTotalTimeoutMultiplier;
            }
            set
            {
                if (m_WriteTotalTimeoutMultiplier != value)
                {
                    m_WriteTotalTimeoutMultiplier = value;
                    if (((int) comDevice) > 0)
                    {
                        SetTimeouts(comDevice);
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        private struct COMMTIMEOUTS
        {
            public int ReadIntervalTimeout;
            public int ReadTotalTimeoutMultiplier;
            public int ReadTotalTimeoutConstant;
            public int WriteTotalTimeoutMultiplier;
            public int WriteTotalTimeoutConstant;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        private struct COMSTAT
        {
            public int fBitFields;
            public int cbInQue;
            public int cbOutQue;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        private struct DCB
        {
            public int DCBLength;
            public int BaudRate;
            public uint Flags;
            public short wReserved1;
            public short XonLim;
            public short XoffLim;
            public byte ByteSize;
            public byte Parity;
            public byte StopBits;
            public char XonChar;
            public char XoffChar;
            public char ErrorChar;
            public char EofChar;
            public char EvtChar;
            public short wReserved2;
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        private struct OVERLAPPED
        {
            public int Internal;
            public int InternalHigh;
            public int Offset;
            public int OffsetHigh;
            public IntPtr hEvent;
        }

        public enum SerialBaudRate
        {
            br000075 = 0x4b,
            br000110 = 110,
            br000134 = 0x86,
            br000150 = 150,
            br000300 = 300,
            br000600 = 600,
            br001200 = 0x4b0,
            br001800 = 0x708,
            br002400 = 0x960,
            br004800 = 0x12c0,
            br007200 = 0x1c20,
            br009600 = 0x2580,
            br014400 = 0x3840,
            br019200 = 0x4b00,
            br038400 = 0x9600,
            br057600 = 0xe100,
            br115200 = 0x1c200,
            br128000 = 0x1f400,
            br230400 = 0x38400,
            br256000 = 0x3e800,
            br460800 = 0x70800,
            br921600 = 0xe1000,
            brCustom = 0
        }

        public enum SerialCommPort
        {
            None,
            COM01,
            COM02,
            COM03,
            COM04,
            COM05,
            COM06,
            COM07,
            COM08,
            COM09,
            COM10,
            COM11,
            COM12,
            COM13,
            COM14,
            COM15,
            COM16,
            COM17,
            COM18,
            COM19,
            COM20,
            COM21,
            COM22,
            COM23,
            COM24,
            COM25,
            COM26,
            COM27,
            COM28,
            COM29,
            COM30,
            COM31,
            COM32,
            COM33,
            COM34,
            COM35,
            COM36,
            COM37,
            COM38,
            COM39,
            COM40,
            COM41,
            COM42,
            COM43,
            COM44,
            COM45,
            COM46,
            COM47,
            COM48,
            COM49,
            COM50,
            COM51,
            COM52,
            COM53,
            COM54,
            COM55,
            COM56,
            COM57,
            COM58,
            COM59,
            COM60,
            COM61,
            COM62,
            COM63,
            COM64,
            COM65,
            COM66,
            COM67,
            COM68,
            COM69,
            COM70,
            COM71,
            COM72,
            COM73,
            COM74,
            COM75,
            COM76,
            COM77,
            COM78,
            COM79,
            COM80,
            COM81,
            COM82,
            COM83,
            COM84,
            COM85,
            COM86,
            COM87,
            COM88,
            COM89,
            COM90,
            COM91,
            COM92,
            COM93,
            COM94,
            COM95,
            COM96,
            COM97,
            COM98,
            COM99,
            COM100,
            COM101,
            COM102,
            COM103,
            COM104,
            COM105,
            COM106,
            COM107,
            COM108,
            COM109,
            COM110,
            COM111,
            COM112,
            COM113,
            COM114,
            COM115,
            COM116,
            COM117,
            COM118,
            COM119,
            COM120,
            COM121,
            COM122,
            COM123,
            COM124,
            COM125,
            COM126,
            COM127,
            COM128,
            COM129,
            COM130,
            COM131,
            COM132,
            COM133,
            COM134,
            COM135,
            COM136,
            COM137,
            COM138,
            COM139,
            COM140,
            COM141,
            COM142,
            COM143,
            COM144,
            COM145,
            COM146,
            COM147,
            COM148,
            COM149,
            COM150,
            COM151,
            COM152,
            COM153,
            COM154,
            COM155,
            COM156,
            COM157,
            COM158,
            COM159,
            COM160,
            COM161,
            COM162,
            COM163,
            COM164,
            COM165,
            COM166,
            COM167,
            COM168,
            COM169,
            COM170,
            COM171,
            COM172,
            COM173,
            COM174,
            COM175,
            COM176,
            COM177,
            COM178,
            COM179,
            COM180,
            COM181,
            COM182,
            COM183,
            COM184,
            COM185,
            COM186,
            COM187,
            COM188,
            COM189,
            COM190,
            COM191,
            COM192,
            COM193,
            COM194,
            COM195,
            COM196,
            COM197,
            COM198,
            COM199,
            COM200,
            COM201,
            COM202,
            COM203,
            COM204,
            COM205,
            COM206,
            COM207,
            COM208,
            COM209,
            COM210,
            COM211,
            COM212,
            COM213,
            COM214,
            COM215,
            COM216,
            COM217,
            COM218,
            COM219,
            COM220,
            COM221,
            COM222,
            COM223,
            COM224,
            COM225,
            COM226,
            COM227,
            COM228,
            COM229,
            COM230,
            COM231,
            COM232,
            COM233,
            COM234,
            COM235,
            COM236,
            COM237,
            COM238,
            COM239,
            COM240,
            COM241,
            COM242,
            COM243,
            COM244,
            COM245,
            COM246,
            COM247,
            COM248,
            COM249,
            COM250,
            COM251,
            COM252,
            COM253,
            COM254,
            COM255
        }

        public enum SerialDataWidth
        {
            dw5Bits = 5,
            dw6Bits = 6,
            dw7Bits = 7,
            dw8Bits = 8
        }

        public enum SerialHardwareFlowControl
        {
            hfNone,
            hfRTS,
            hfRTSCTS
        }

        public enum SerialParityBits
        {
            pbNone,
            pbOdd,
            pbEven,
            pbMark,
            pbSpace
        }

        public enum SerialSoftwareFlowControl
        {
            sfNone,
            sfXONXOFF
        }

        public enum SerialStopBits
        {
            sb1Bit,
            sb1_5Bits,
            sb2Bits
        }
    }
}

