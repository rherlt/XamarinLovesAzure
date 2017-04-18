using System;
using Windows.Security.Credentials;
using KeyChain.Net;

namespace DropIt.Mobile.Uwp.Customization
{
    public class KeyChainHelper : IKeyChainHelper
    {
        private const string Resource = "DropIt by Rico Herlt";
        private readonly PasswordVault _passValut;

        public KeyChainHelper()
        {
            _passValut = new PasswordVault();
        }

        public bool SetKey(string name, string value)
        {
            var cred = new PasswordCredential(Resource, name, value);
            _passValut.Add(cred);
            return true;
        }

        public bool SaveKey(string name, string value)
        {
            var cred = new PasswordCredential(Resource, name, value);
            _passValut.Add(cred);
            return true;
        }

        public string GetKey(string name)
        {
            try
            {
                var cred = _passValut.Retrieve(Resource, name);
                cred.RetrievePassword();
                return cred.Password;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool DeleteKey(string name)
        {
            PasswordCredential cred = null;
            try
            {
                cred = _passValut.Retrieve(Resource, name);
            }
            catch (Exception)
            {
                return false;
            }
            _passValut.Remove(cred);
            return true;
        }
    }
}