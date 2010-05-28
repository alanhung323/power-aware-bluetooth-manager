﻿using System;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Microsoft.WindowsMobile.Status;
using Microsoft.Win32;
//TODO: see if this can be implemented with adam link without InTheHand.net
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
        /// creates the adapter and registers to blue-tooth events on the cellphone
        /// </summary>
        public BluetoothAdapter()
        {
            m_client = new BluetoothClient();
            m_radio = BluetoothRadio.PrimaryRadio;
            
            //register to registry bluetooth mode change event
            m_registryState = new RegistryState(RegistryPath, RegistryValue);
            m_registryState.Changed += RegistryBluetoothRadioModeChanged;
        }

        /// <summary>
        /// 1. activate bluetooth
        /// 2. search for other bluetooth devices
        /// 3. return bluetooth to previous mode
        /// </summary>
        /// <returns>true if other bluetooth devices are in range</returns>
        public bool SampleForOtherBluetooth()
        {
            //remember bluetooth last mode of operation
            RadioMode last_mode = m_radio.Mode;
            
            //1.
            m_radio.Mode = RadioMode.Discoverable;
            
            //2.
            bool res = IsOtherBluetoothExist();

            //3.
            m_radio.Mode = last_mode;
            
            return res;
        }
        
      

        /// <summary>
        /// Property for bluetooth radio mode
        /// </summary>
        public RadioMode RadioMode
        {
            get
            {
                if (m_radio != null)
                {
                    return m_radio.Mode;
                }
                else
                {
                    return RadioMode.PowerOff;
                }
            }
            set
            {
                if (m_radio != null)
                {
                    m_radio.Mode = value;
                }
                
                
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

        /// <summary>
        /// Is bluetooth device picking up another device signal and is this device a paired device
        /// </summary>
        /// <returns></returns>
        private bool IsOtherBluetoothExist()
        {
            //DiscoverDevices parameters: max 10 devices, authenticated, remembered, not unknown

            //find all paired device (last parameter = true tells function to retrieve all devices in range).
            BluetoothDeviceInfo[] btInfoInRange = m_client.DiscoverDevices(10, false, false, false, true);
            //get all paired device
            BluetoothDeviceInfo[] btInfoPaired = m_client.DiscoverDevices(10, true, false, false);
         
            //iterate on all devices in range and see if any of them is a paired device
            foreach (BluetoothDeviceInfo deviceInRange in btInfoInRange)
            {
                foreach (BluetoothDeviceInfo pairedDevice in btInfoPaired)
                {
                    if (deviceInRange.DeviceName == pairedDevice.DeviceName)
                    {
                        return true;
                    }
                }
            }


            return false;

            //initialize parameters
            //BTSafeNativeMethods.WSAQUERYSET wsQuerySet = new BTSafeNativeMethods.WSAQUERYSET();
            //wsQuerySet.dwSize = System.Runtime.InteropServices.Marshal.SizeOf(wsQuerySet);
            //wsQuerySet.dwNameSpace = BTSafeNativeMethods.NS_BTH;           
        }

        #endregion Methods

    }

    public delegate void BluetoothRadioModeChangedHandler();
}
