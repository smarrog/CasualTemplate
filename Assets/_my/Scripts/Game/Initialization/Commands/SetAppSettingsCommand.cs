using System.Globalization;
using Smr.Commands;
using UnityEngine;

namespace Game {
    public class SetAppSettingsCommand : AbstractCommand {
        protected override void ExecuteInternal() {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            QualitySettings.vSyncCount = App.Settings.System.VSyncCount;
            Application.targetFrameRate = App.Settings.System.TargetFrameRate;
            Screen.sleepTimeout = App.Settings.System.CanSleep ? SleepTimeout.SystemSetting : SleepTimeout.NeverSleep;
            NotifyComplete();
        }
    }
}