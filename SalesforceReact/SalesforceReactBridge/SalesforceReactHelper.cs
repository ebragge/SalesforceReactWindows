using ReactNative.Bridge;
using Salesforce.SDK.Auth;

namespace Salesforce.SDK.ReactNative
{
    class SalesforceReactHelper
    {
        public static bool ActiveAccount(ICallback errorCallback)
        {
            var account = AccountManager.GetAccount();
            if (account == null)
            {
                errorCallback.Invoke("No account");
                return false;
            }
            return true;
        }
    }
}
