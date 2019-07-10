﻿using SignalWire.Relay.Calling;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalWire.Relay
{
    public abstract class Consumer
    {
        private Client mClient = null;
        private ManualResetEventSlim mShutdown = new ManualResetEventSlim();

        protected Client Client { get { return mClient; } }

        public string Host { get; set; }

        public string Project { get; set; }

        public string Token { get; set; }

        public List<string> Contexts { get; set; }

        protected virtual void Setup() { }

        protected virtual void Ready() { }

        protected virtual void Teardown() { }

        protected virtual void OnIncomingCall(Call call) { }

        public void Stop() { mShutdown.Set(); }

        public void Run()
        {
            Setup();

            if (string.IsNullOrWhiteSpace(Project)) throw new ArgumentNullException("Project");
            if (string.IsNullOrWhiteSpace(Token)) throw new ArgumentNullException("Token");

            using (mClient = new Client(Project, Token, host: Host))
            {
                mClient.OnReady += c =>
                {
                    Task.Run(() =>
                    {
                        mClient.Signalwire.SetupAsync().Wait();

                        if (Contexts != null)
                        {
                            mClient.Signalwire.Receive(Contexts.ToArray());
                        }
                        Ready();
                    });
                };
                mClient.Calling.OnCallReceived += (a, c, p) => Task.Run(() => OnIncomingCall(c));

                mClient.Connect();

                mShutdown.Wait();

                Teardown();
            }
        }
    }
}
