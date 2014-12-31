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

using System.Windows.Threading;

namespace Jamcast.Plugins.Spotify.UI.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        private object _currentView;
        private LoggedInViewModel _loggedInViewModel;
        private LoggedOutViewModel _loggedOutViewModel;
        private Dispatcher _dispatcher;

        public MainViewModel(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _loggedOutViewModel = new LoggedOutViewModel(dispatcher);
            _loggedOutViewModel.OnLoginSuccess += _loggedOutViewModel_LoginSuccess;

            if (Configuration.Instance.HasLoginCredentials)
            {
                _loggedInViewModel = new LoggedInViewModel();
                _loggedInViewModel.OnLoggedOut += _loggedInViewModel_LoggedOut;
            }

            this.CurrentView = Configuration.Instance.HasLoginCredentials ? (object)_loggedInViewModel : _loggedOutViewModel;
        }

        private void _loggedInViewModel_LoggedOut()
        {
            this.CurrentView = _loggedOutViewModel;
        }

        private void _loggedOutViewModel_LoginSuccess()
        {
            _loggedInViewModel = new LoggedInViewModel();
            this.CurrentView = _loggedInViewModel;
        }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }
    }
}