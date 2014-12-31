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
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Jamcast.Extensibility.UI.Dialogs;

namespace Jamcast.Plugins.Spotify.UI.ViewModel
{
    internal class LoggedOutViewModel : ObservableObject
    {
        private RelayCommand _logInCommand;
        private RelayCommand _loadAppKeyCommand;
        private bool _isLoggingIn;
        private string _loginError;
        private byte[] _appKeyTemp;
        private Dispatcher _dispatcher;

        public string EmailOrUsername { get; set; }

        public event Action OnLoginSuccess;

        public LoggedOutViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _logInCommand = new RelayCommand(logIn);
            _loadAppKeyCommand = new RelayCommand(loadAppKey);
        }

        public bool IsApplicationKeyLoaded
        {
            get { return _appKeyTemp != null; }
        }

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            private set
            {
                _isLoggingIn = value;
                this.OnPropertyChanged("IsLoggingIn");
            }
        }

        public string LoginError
        {
            get { return _loginError; }
            set
            {
                _loginError = value;
                this.OnPropertyChanged("LoginError");
            }
        }

        public ICommand LoadApplicationKeyCommand
        {
            get { return _loadAppKeyCommand; }
        }

        public ICommand LogInCommand
        {
            get
            {
                return _logInCommand;
            }
        }

        private void loadAppKey(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Open Spotify API Application Key";
            dialog.Filter = "Spotify Application Key (*.key) | *.key";
            dialog.ShowDialog();

            if (!File.Exists(dialog.FileName))
                return;
            byte[] bytes = File.ReadAllBytes(dialog.FileName);
            // TODO: simple validation of key?
            _appKeyTemp = bytes;
            this.OnPropertyChanged("IsApplicationKeyLoaded");
        }

        private void logIn(object password)
        {
            this.IsLoggingIn = true;

            if (String.IsNullOrWhiteSpace(this.EmailOrUsername))
            {
                this.LoginError = "Username cannot be left blank";
                this.IsLoggingIn = false;
                return;
            }

            string username = this.EmailOrUsername.Trim();
            var pw = (password as PasswordBox).Password;

            if (String.IsNullOrWhiteSpace(pw))
            {
                this.LoginError = "Password cannot be left blank";
                this.IsLoggingIn = false;
                return;
            }

            if (_appKeyTemp == null)
            {
                this.LoginError = "Application key not loaded";
                this.IsLoggingIn = false;
                return;
            }

            var dialog = JamcastDialog.CreateWaitDialog("Verifying login, please wait...", DialogMode.Cancel);
            dialog.Cancel = new Action(() =>
            {
                this.IsLoggingIn = false;
            });
            dialog.Show(new Action(() =>
            {
                try
                {
                    try
                    {
                        Spotify.Initialize();
                        if (!Spotify.Login(_appKeyTemp, username, pw, "Plugin-Client"))
                        {
                            this.LoginError = "Login failed.";
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.LoginError = ex.Message;
                        return;
                    }
                    finally
                    {
                        Spotify.ShutDown();
                    }

                    try
                    {
                        Configuration.Instance.ApplicationKey = _appKeyTemp;
                        Configuration.Instance.Username = username;
                        Configuration.Instance.Password = pw;
                        Configuration.Instance.Save();
                        this.OnLoginSuccess();
                    }
                    catch (Exception ex)
                    {
                        Configuration.Instance.Password = null;
                        this.LoginError = ex.Message;
                        return;
                    }
                }
                finally
                {
                    this.IsLoggingIn = false;
                }
            }));
        }
    }
}