﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using Nefarius.ViGEm.Client.Exceptions;

namespace XOutput.Input.XInput
{
    public class VigemDevice : IXOutput
    {
        protected readonly ViGEmClient client;
        protected readonly Dictionary<int, Xbox360Controller> controllers = new Dictionary<int, Xbox360Controller>();
        protected readonly Dictionary<XInputTypes, VigemXbox360ButtonMapping> buttonMappings = new Dictionary<XInputTypes, VigemXbox360ButtonMapping>();
        protected readonly Dictionary<XInputTypes, VigemXbox360AxisMapping> axisMappings = new Dictionary<XInputTypes, VigemXbox360AxisMapping>();

        public VigemDevice()
        {
            InitMapping();
            client = new ViGEmClient();
        }

        public static bool IsAvailable()
        {
            try
            {
                new ViGEmClient();
                return true;
            }
            catch(VigemBusNotFoundException)
            {
                return false;
            }
        }

        public bool Plugin(int controllerCount)
        {
            var controller = new Xbox360Controller(client);
            controller.Connect();
            controllers.Add(controllerCount, controller);
            return true;
        }

        public bool Unplug(int controllerCount)
        {
            if (controllers.ContainsKey(controllerCount))
            {
                var controller = controllers[controllerCount];
                controllers.Remove(controllerCount);
                controller.Disconnect();
                return true;
            }
            return false;
        }

        public bool Report(int controllerCount, Dictionary<XInputTypes, double> values)
        {
            if (controllers.ContainsKey(controllerCount))
            {
                var report = new Xbox360Report();
                foreach (var value in values)
                {
                    if (value.Key.IsAxis())
                    {
                        var mapping = axisMappings[value.Key];
                        report.SetAxis(mapping.Type, mapping.GetValue(value.Value));
                    }
                    else
                    {
                        var mapping = buttonMappings[value.Key];
                        report.SetButtonState(mapping.Type, mapping.GetValue(value.Value));
                    }
                }
                controllers[controllerCount].SendReport(report);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            foreach (var controller in controllers.Values)
            {
                controller?.Dispose();
            }
            client?.Dispose();
        }

        protected void InitMapping()
        {
            buttonMappings.Add(XInputTypes.A, new VigemXbox360ButtonMapping(Xbox360Buttons.A));
            buttonMappings.Add(XInputTypes.B, new VigemXbox360ButtonMapping(Xbox360Buttons.B));
            buttonMappings.Add(XInputTypes.X, new VigemXbox360ButtonMapping(Xbox360Buttons.X));
            buttonMappings.Add(XInputTypes.Y, new VigemXbox360ButtonMapping(Xbox360Buttons.Y));
            buttonMappings.Add(XInputTypes.L1, new VigemXbox360ButtonMapping(Xbox360Buttons.LeftShoulder));
            buttonMappings.Add(XInputTypes.R1, new VigemXbox360ButtonMapping(Xbox360Buttons.RightShoulder));
            buttonMappings.Add(XInputTypes.Back, new VigemXbox360ButtonMapping(Xbox360Buttons.Back));
            buttonMappings.Add(XInputTypes.Start, new VigemXbox360ButtonMapping(Xbox360Buttons.Start));
            buttonMappings.Add(XInputTypes.Home, new VigemXbox360ButtonMapping(Xbox360Buttons.Guide));
            buttonMappings.Add(XInputTypes.R3, new VigemXbox360ButtonMapping(Xbox360Buttons.RightThumb));
            buttonMappings.Add(XInputTypes.L3, new VigemXbox360ButtonMapping(Xbox360Buttons.LeftThumb));
            
            buttonMappings.Add(XInputTypes.UP, new VigemXbox360ButtonMapping(Xbox360Buttons.Up));
            buttonMappings.Add(XInputTypes.DOWN, new VigemXbox360ButtonMapping(Xbox360Buttons.Down));
            buttonMappings.Add(XInputTypes.LEFT, new VigemXbox360ButtonMapping(Xbox360Buttons.Left));
            buttonMappings.Add(XInputTypes.RIGHT, new VigemXbox360ButtonMapping(Xbox360Buttons.Right));

            axisMappings.Add(XInputTypes.LX, new VigemXbox360AxisMapping(Xbox360Axes.LeftThumbX));
            axisMappings.Add(XInputTypes.LY, new VigemXbox360AxisMapping(Xbox360Axes.LeftThumbY));
            axisMappings.Add(XInputTypes.RX, new VigemXbox360AxisMapping(Xbox360Axes.RightThumbX));
            axisMappings.Add(XInputTypes.RY, new VigemXbox360AxisMapping(Xbox360Axes.RightThumbY));
            axisMappings.Add(XInputTypes.L2, new VigemXbox360AxisMapping(Xbox360Axes.LeftTrigger));
            axisMappings.Add(XInputTypes.R2, new VigemXbox360AxisMapping(Xbox360Axes.RightTrigger));
        }
    }

    public class VigemXbox360ButtonMapping
    {
        public Xbox360Buttons Type { get; set; }

        public VigemXbox360ButtonMapping(Xbox360Buttons button)
        {
            Type = button;
        }
        
        public bool GetValue(double value)
        {
            return value > 0.5;
        }
    }

    public class VigemXbox360AxisMapping
    {
        public Xbox360Axes Type { get; set; }

        public VigemXbox360AxisMapping(Xbox360Axes button)
        {
            Type = button;
        }

        public short GetValue(double value)
        {
            if (Type == Xbox360Axes.LeftTrigger || Type == Xbox360Axes.RightTrigger)
            {
                return (byte)(value * byte.MaxValue);
            }
            else
            {
                return (short)((value - 0.5) * 2 * short.MaxValue);
            }
        }
    }
}