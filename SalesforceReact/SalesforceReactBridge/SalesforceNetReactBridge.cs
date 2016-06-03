using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using Salesforce.SDK.Rest;
using System;
using System.Net.Http;

namespace Salesforce.SDK.ReactNative
{
    /// <summary>
    /// 
    /// </summary>
    class SalesforceNetReactBridge : NativeModuleBase
    {
        // Log tag
        private static string LOG_TAG = "SalesforceNetReactBridge";

        private static string METHOD_KEY = "method";
        private static string END_POINT_KEY = "endPoint";
        private static string PATH_KEY = "path";
        private static string QUERY_PARAMS_KEY = "queryParams";
        private static string HEADER_PARAMS_KEY = "headerParams";
        private static string FILE_PARAMS_KEY = "fileParams";
        private static string FILE_MIME_TYPE_KEY = "fileMimeType";
        private static string FILE_URL_KEY = "fileUrl";
        private static string FILE_NAME_KEY = "fileName";

        /// <summary>
        /// Instantiates the <see cref="SalesforceNetReactBridge"/>.
        /// </summary>
        internal SalesforceNetReactBridge()
        {
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SalesforceNetReactBridge";
            }
        }

        [ReactMethod]
        public void sendRequest(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }
    }
}
