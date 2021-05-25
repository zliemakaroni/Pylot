using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace Pylot
{
    class MyJoystick
    {
        private Joystick joystick { get; set; } //Наследование для слабых

        public bool FindJoystick()  //Поиск джойстика
        {
            this.joystick = initJoystick();
            if (joystick == null)
            {
                return false;
            }
            else
            {
                joystick.Properties.BufferSize = 128;
                // Acquire the joystick
                joystick.Acquire();
                return true;
            }
        }

        private Joystick initJoystick()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,      // Ищем среди всех девайсов джойстик
        DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, look for a Gamepad
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                return null;
            }
            joystick = new Joystick(directInput, joystickGuid);
            return joystick;
        }
        public string GetInformation()
        {
            return joystick.Information.InstanceName;
        }
        public double NewPointerSpeed(double pointerspeed)      //Расчет новой скорости точки-визира исходя из положения джойстика
        {
            joystick.Poll();
            var datas = joystick.GetBufferedData();
            foreach (var state in datas)
            {
                if (state.RawOffset.Equals(4))
                {
                     pointerspeed = (0 - (state.Value - 32767) / Results.sensivity);    //С джойстика приходят значения 0 - 65535, поэтому переводим их в -/+ и притупляем сенсу
                }
            }
            return pointerspeed;
        }
    }
}
