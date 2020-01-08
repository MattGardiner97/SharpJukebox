using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpJukebox
{
    public class AudioDeviceLocater
    {
        private readonly ObservableCollection<MMDevice> _devices = new ObservableCollection<MMDevice>();

        public IEnumerable<MMDevice> Devices { get; private set; }

        public AudioDeviceLocater()
        {
            Devices = new ReadOnlyCollection<MMDevice>(_devices);
        }

        public void LocateDevices()
        {
            using (var mmDeviceEnumerator = new MMDeviceEnumerator())
            {
                using (var mmDeviceCollection = mmDeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
                {
                    foreach (var device in mmDeviceCollection)
                        _devices.Add(device);
                }
            }
        }
    }
}
