﻿using Nefarius.ViGEm.Client.Targets.Xbox360;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.XInput.Vigem
{
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