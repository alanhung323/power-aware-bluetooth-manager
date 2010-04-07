﻿using System;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Microsoft.WindowsMobile.Status;
using Microsoft.Win32;

namespace PowerAwareBluetooth.Model
{
    /// <summary>
    /// Represent the bluetooth device
    /// </summary>
    public class BluetoothAdapter
    {
        #region Constants

        private const string RegistryPath = @"HKEY_LOCAL_MACHINE\System\State\Hardware";
        private const string RegistryValue = "Bluetooth";

        #endregion Constatns

        #region Members

        /// <summary>
        /// InTheHand.Net bluetooth client
        /// </summary>
        BluetoothClient m_client;
        
        /// <summary>
        /// InTheHand.Net bluetooth Radio
        /// </summary>
        BluetoothRadio m_radio;
        
        /// <summary>
        /// Registry entry - the entry where the current bluetooth radio mode is saved
        /// </summary>
        RegistryState m_registryState;

        #endregion Members

        #region Methods
        
        /// <summary>
        /// C'tor
        /// </summary>
        public BluetoothAdapter()
        {
            m_client = new BluetoothClient();
            m_radio = BluetoothRadio.PrimaryRadio;
            
            //register to registry bluetooth mode change event
            m_registryState = new RegistryState(RegistryPath, RegistryValue);
            m_registryState.Changed += new ChangeEventHandler(RegistryBluetoothRadioModeChanged);
        }
        
        /// <summary>
        /// Is bluetooth device picking up another bluetooth signal
        /// </summary>
        /// <returns></returns>
        public bool IsOtherBluetoothExist()
        {             
            BluetoothDeviceInfo[] btInfo = m_client.DiscoverDevices();
            return (btInfo.Length > 0);

            


            //initialize parameters
            //BTSafeNativeMethods.WSAQUERYSET wsQuerySet = new BTSafeNativeMethods.WSAQUERYSET();
            //wsQuerySet.dwSize = System.Runtime.InteropServices.Marshal.SizeOf(wsQuerySet);
            //wsQuerySet.dwNameSpace = BTSafeNativeMethods.NS_BTH;
           
        }

        /// <summary>
        /// Property for bluetooth radio mode
        /// </summary>
        public RadioMode RadioMode
        {
            get
            {
                return m_radio.Mode;
            }
            set
            {
                m_radio.Mode = value;
            }
        }

        /// <summary>
        /// event fired when bluetooth radio mode is changed
        /// </summary>
        public event BluetoothRadioModeChangedHandler BluetoothRadioModeChanged;
      
        #region Protected Methods

        protected void OnBluetoothRadioModeChanged()
        {
            if (BluetoothRadioModeChanged != null)
            {
                BluetoothRadioModeChanged();
            }
        }

        
        protected void RegistryBluetoothRadioModeChanged(object sender, ChangeEventArgs args)
        {
            OnBluetoothRadioModeChanged();

        }

        private void InitWinSock()
        {
            ushort winsockVersion = ((ushort)(((byte)(2)) | ((ushort)((byte)(2))) << 8));

            byte[] wsaData = new byte[512];

            int result = 0;

            result = BTSafeNativeMethods.WSAStartup(winsockVersion, wsaData);

            if (result != 0)
            {
                throw new System.Net.Sockets.SocketException();
            }
        }

        #endregion Protected Methods

        #endregion Methods

    }

    public delegate void BluetoothRadioModeChangedHandler();
}