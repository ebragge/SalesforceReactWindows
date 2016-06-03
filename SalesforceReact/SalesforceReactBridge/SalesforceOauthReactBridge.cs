using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using Salesforce.SDK.Auth;
using Salesforce.SDK.Core;
using Salesforce.SDK.Settings;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Salesforce.SDK.ReactNative
{
    /// <summary>
    /// 
    /// </summary>
    class SalesforceOauthReactBridge : NativeModuleBase
    {
        // Log tag
        private static string LOG_TAG = "SalesforceOauthReactBridge";

        private static string USER_AGENT = "userAgent";
        private static string LOGIN_URL = "loginUrl";
        private static string IDENTITY_URL = "identityUrl";
        private static string CLIENT_ID = "clientId";
        private static string ORG_ID = "orgId";
        private static string USER_ID = "userId";
        private static string REFRESH_TOKEN = "refreshToken";
        private static string ACCESS_TOKEN = "accessToken";
        private static string COMMUNITY_ID = "communityId";
        private static string COMMUNITY_URL = "communityUrl";

        private static IApplicationInformationService ApplicationInformationService
           => SDKServiceLocator.Get<IApplicationInformationService>();

        /// <summary>
        /// Instantiates the <see cref="SalesforceOauthReactBridge"/>.
        /// </summary>
        internal SalesforceOauthReactBridge()
        {
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SalesforceOauthReactBridge";
            }
        }

        [ReactMethod]
        public void authenticate(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var account = AccountManager.GetAccount();
            if (account == null)
            {
                RunOnDispatcher(() =>
                {
                    try
                    {
                        var frame = Window.Current.Content as Frame;
                        frame?.Navigate(SDKManager.RootApplicationPage);
                        successCallback.Invoke();
                    }
                    catch (Exception ex)
                    {
                        errorCallback.Invoke(ex.Message);
                    }
                });
            }
            else
            {
                successCallback.Invoke();
            }
        }

        [ReactMethod]
        public void getAuthCredentials(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            RunOnDispatcher(async () =>
            {
                try
                {
                    var account = AccountManager.GetAccount();
                    if (account != null)
                    {
                        var agent = await ApplicationInformationService.GenerateUserAgentHeaderAsync(true, String.Empty);
                        var data = new JObject()
                        {
                            new JProperty(ACCESS_TOKEN, account.AccessToken),
                            new JProperty(REFRESH_TOKEN, account.RefreshToken),
                            new JProperty(ACCESS_TOKEN, account.AccessToken),
                            new JProperty(REFRESH_TOKEN, account.RefreshToken),
                            new JProperty(USER_ID, account.UserId),
                            new JProperty(ORG_ID, account.OrganizationId),
                            new JProperty(CLIENT_ID, account.ClientId),
                            new JProperty(LOGIN_URL, account.LoginUrl),
                            new JProperty(IDENTITY_URL, account.IdentityUrl),
                            new JProperty(USER_AGENT, agent),
                            new JProperty(COMMUNITY_ID, account.CommunityId),
                            new JProperty(COMMUNITY_URL, account.CommunityUrl)
                        };
                        successCallback.Invoke(data.ToString());
                    }
                    else
                    {
                        successCallback.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    errorCallback.Invoke(ex.Message);
                }
            });
        }

        [ReactMethod]
        public void logoutCurrentUser(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            RunOnDispatcher(async () =>
            {
                try
                {
                    var frame = Window.Current.Content as Frame;
                    await SDKManager.GlobalClientManager.Logout();
                    successCallback.Invoke();
                    frame?.Navigate(SDKManager.RootApplicationPage);      
                }
                catch (Exception ex)
                {
                    errorCallback.Invoke(ex.Message);
                }
            });
        }

        private static async void RunOnDispatcher(DispatchedHandler action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }
    }
}
