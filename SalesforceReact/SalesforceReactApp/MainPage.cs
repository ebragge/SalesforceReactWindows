using ReactNative;
using ReactNative.Modules.Core;
using ReactNative.Shell;
using System.Collections.Generic;
using Salesforce.SDK.ReactNative;

namespace Salesforce.SalesforceReactApp
{
    public sealed class MainPage : ReactPage
    {
        public override string MainComponentName
        {
            get
            {
                return "SmartSyncExplorerReactNative";
            }
        }

        public override string JavaScriptMainModuleName
        {
            get
            {
                return "SalesforceReactApp/js/index.windows";
            }
        }

#if BUNDLE
        public override string JavaScriptBundleFile
        {
            get
            {
                return "ms-appx:///ReactAssets/index.windows.bundle";
            }
        }
#endif

        public override List<IReactPackage> Packages
        {
            get
            {
                return new List<IReactPackage>
                {
                    new MainReactPackage(),
                    new SalesforceReactPackage(),
                };
            }
        }

        public override bool UseDeveloperSupport
        {
            get
            {
                return true;
            }
        }
    }
}
