using COM_Schnittstelle.Scripts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace COM_Schnittstelle.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private char[] testDigit;
        public char[] TestDigit
        {
            get { return testDigit; }
            set { testDigit = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> portOptions;
        public ObservableCollection<string> PortOptions
        {
            get { return portOptions; }
            set { portOptions = value;
                OnPropertyChanged();
            }
        }

        private string portSelected;
        public string PortSelected
        {
            get { return portSelected; }
            set { portSelected = value;
                OnPropertyChanged();
            }
        }

        private int tbBaudrate;
        public int Baudrate
        {
            get { return tbBaudrate; }
            set { tbBaudrate = value;
                OnPropertyChanged();
            }
        }

        private StopBits stopBits;
        public StopBits StopBitsSelected
        {
            get { return stopBits; }
            set { stopBits = value;
                OnPropertyChanged();
            }
        }
        private Parity parity;
        public Parity ParitySelected
        {
            get { return parity; }
            set { parity = value;
                OnPropertyChanged();
            }
        }

        private Handshake handshake;

        public Handshake HandshakeSelected
        {
            get { return handshake; }
            set { handshake = value;
                OnPropertyChanged();
            }
        }

        private bool rtsEnabled;

        public bool RTSEnabledSelected
        {
            get { return rtsEnabled; }
            set { rtsEnabled = value;
                OnPropertyChanged();
            }
        }

        private int dataBits;

        public int DataBits
        {
            get { return dataBits; }
            set { dataBits = value;
                OnPropertyChanged();
            }
        }



        public List<StopBits> CBStoppbitChoices { get; set; }
        public List<Parity> CBParityChoices { get; set; }
        public List<Handshake> CBHandshakeChoices { get; set; }
        public List<bool> CBRTSEnabledChoices { get; set; }




        public RelayCommand UpdatePortSelectionCmd { get; set; }
        public RelayCommand SelectPortCmd { get; set; }




        SerialPort port;
        public SerialPort Port
        {
            get { return port; }
            set { port = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            CBParityChoices = new List<Parity>() { Parity.None, Parity.Even, Parity.Odd};
            CBStoppbitChoices = new List<StopBits>() { StopBits.One, StopBits.Two};
            CBHandshakeChoices = new List<Handshake> { Handshake.None, Handshake.XOnXOff };
            CBRTSEnabledChoices = new List<bool> { false, true };
            ParitySelected = CBParityChoices[0];
            StopBitsSelected = CBStoppbitChoices[0];
            HandshakeSelected = CBHandshakeChoices[0];
            RTSEnabledSelected = CBRTSEnabledChoices[0];
            Baudrate = 9600;
            DataBits = 8;

            UpdatePortSelectionCmd = new RelayCommand(o => { UpdatePortOptions(); });
            SelectPortCmd = new RelayCommand(o => { OpenPort(); });

            UpdatePortOptions();
            PortOptions.CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionPropertyChanged);
            
            //only first 4 Digits will be displayed
            //Dot should be a 2.0001 but not displayed
            //string testText = "20001";
            //TestDigit = testText.ToCharArray();
        }

        private void OpenPort()
        {
            if (Port != null && Port.IsOpen)
                Port.Close();
            Port = new SerialPort("COM1");

            Port.BaudRate = Baudrate;
            Port.Parity = ParitySelected;
            Port.StopBits = StopBitsSelected;
            Port.Handshake = HandshakeSelected;
            Port.DataBits = DataBits;
            Port.RtsEnable = RTSEnabledSelected;

            Port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            Port.Open();
            Console.WriteLine(Port);
        }

        private void UpdatePortOptions()
        {
            PortOptions = new ObservableCollection<string>();
            string[] options = SerialPort.GetPortNames();
            foreach (string option in options)
            {
                portOptions.Add(option);
            }
        }

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            Console.WriteLine($"Data Received: {indata}");

            TestDigit = indata.ToCharArray();
        }
    }
}
