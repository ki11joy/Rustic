using Caliburn.Micro;

namespace Rustic.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _login;
        private string _password;
        private bool _rememberMe;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                NotifyOfPropertyChange();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyOfPropertyChange();
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set
            {
                _rememberMe = value;
                NotifyOfPropertyChange();
            }
        }

        public void Ok()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
