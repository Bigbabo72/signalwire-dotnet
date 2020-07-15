﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalWire.Relay.Calling
{
    public sealed class PlayAction
    {
        internal Call Call { get; set; }

        public string ControlID { get; internal set; }

        public bool Completed { get; internal set; }

        public PlayResult Result { get; internal set; }

        public List<CallMedia> Payload { get; internal set; }

        public CallPlayState State { get; internal set; }

        public void Stop()
        {
            Task<LL_PlayStopResult> taskLLPlayStop = Call.API.LL_PlayStopAsync(new LL_PlayStopParams()
            {
                NodeID = Call.NodeID,
                CallID = Call.ID,
                ControlID = ControlID,
            });

            LL_PlayStopResult resultLLPlayStop = taskLLPlayStop.Result;

            // If there was an internal error of any kind then throw an exception
            Call.API.ThrowIfError(resultLLPlayStop.Code, resultLLPlayStop.Message);
        }

        public PlayVolumeResult Volume(double volume)
        {
            Task<LL_PlayVolumeResult> taskLLPlayVolume = Call.API.LL_PlayVolumeAsync(new LL_PlayVolumeParams()
            {
                NodeID = Call.NodeID,
                CallID = Call.ID,
                ControlID = ControlID,
                Volume = volume,
            });

            LL_PlayVolumeResult resultLLPlayVolume = taskLLPlayVolume.Result;

            return new PlayVolumeResult()
            {
                Successful = resultLLPlayVolume.Code == "200",
            };
        }
    }
}
