using System;
using Smr.Commands;
using Smr.Common;
using UnityEngine;
using YG;

namespace Game {
    public class InitializeServerTimeCommand : AbstractCommand {

        protected override void ExecuteInternal() {
            var serverTime = GetServerTime();
            App.Logger.GetChannel(LogChannel.Initialization).Log($"Sync server time: {serverTime}");
            App.TimeService.Sync(serverTime);
            NotifyComplete();
        }

        private DateTime GetServerTime() {
            if (Application.isEditor) {
                return DateTime.Now;
            }
                
            var serverTimeMs = YG2.ServerTime();
            return DateTimeOffset.FromUnixTimeMilliseconds(serverTimeMs).DateTime;
        }
    }
}