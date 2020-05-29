using System.Configuration;
using UnityEngine.UI;
using UnityEngine;

namespace Common.Utility
{
    public static class UtilityUI
    {
        public static void SetValue(this Toggle toggle, bool value)
        {
            toggle.isOn = value;
        }
    }
}