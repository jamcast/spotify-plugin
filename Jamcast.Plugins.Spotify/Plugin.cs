/*-
 * Copyright (c) 2015 Software Development Solutions, Inc.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 */

using System;

using Jamcast.Extensibility;
using Jamcast.Plugins.Spotify.Renderers;

namespace Jamcast.Plugins.Spotify
{
    public class Plugin : SimplePlugin
    {
        internal static string LOG_MODULE = "Spotify";

        public override Type ConfigurationPanelType
        {
            get { return typeof(Jamcast.Plugins.Spotify.UI.View.SpotifyPanel); }
        }

        public string DisplayName
        {
            get { return "Spotify"; }
        }

        public override Type RootObjectRendererType
        {
            get { return typeof(RootRenderer); }
        }

        public override Type PromotedPlaylistsRendererType
        {
            get
            {
                return typeof(PlaylistMenuRenderer);
            }
        }

        public override Type SearchRendererType
        {
            get
            {
                return typeof(SearchRenderer);
            }
        }

        public override void OnPreRender()
        {
            if (!Spotify.IsLoggedIn)
                throw new PluginNotLoggedInException("Channel requires Spotify Premium subscription. Please log in using Jamcast Server Manager.");
            
            if(!Spotify.IsRunning)
                throw new ApplicationException("Plugin is not running.");
        }

        public override bool Startup()
        {
            try
            {
                PluginDataProvider.OnPluginDataChanged += PluginDataProvider_OnPluginDataChanged;
                Spotify.Initialize();
                tryLogin();
            }
            catch (Exception ex)
            {
                Log.Error(LOG_MODULE, "Could not initialize the Spotify interface.", ex);
                return false;
            }

            Log.Info(Plugin.LOG_MODULE, "Spotify plugin initialized successfully.");
            return true;
        }

        void PluginDataProvider_OnPluginDataChanged(string key)
        {
            if(key.Equals(Configuration.CONFIGURATION_KEY))
            {
                Configuration.Refresh();
                if(Configuration.Instance.HasLoginCredentials)
                {
                    tryLogin();
                }
                else
                {
                    Spotify.Logout();
                }
            }
        }

        public override void Shutdown()
        {
            Spotify.ShutDown();
            Log.Debug(Plugin.LOG_MODULE, "Spotify was shut down successfully", null);
        }

        private void tryLogin()
        {
            if (!Configuration.Instance.HasLoginCredentials)
                return;
            Spotify.Login(Configuration.Instance.ApplicationKey, Configuration.Instance.Username, Configuration.Instance.Password, "Plugin-Service");
        }
    }
}