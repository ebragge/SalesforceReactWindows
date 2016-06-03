using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using Salesforce.SDK.Auth;
using Salesforce.SDK.SmartStore.Store;
using Salesforce.SDK.SmartSync.Manager;
using Salesforce.SDK.SmartSync.Model;
using System;

namespace Salesforce.SDK.ReactNative
{
    /// <summary>
    /// 
    /// </summary>
    class SmartSyncReactBridge : NativeModuleBase
    {
        // Log tag
        private static string LOG_TAG = "SmartSyncReactBridge";

        private static string IS_GLOBAL_STORE = "isGlobalStore";
        private static string OPTIONS = "options";
        private static string QUERY = "query";
        private static string SOUP_NAME = "soupName";
        private static string SYNC_ID = "syncId";
        private static string TARGET = "target";
        private static string TYPE = "type";
        
        private ICallback _successCallback = null;
        private ICallback _errorCallback = null;

        /// <summary>
        /// Instantiates the <see cref="SmartSyncReactBridge"/>.
        /// </summary>
        internal SmartSyncReactBridge()
        {
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SmartSyncReactBridge";
            }
        }

        [ReactMethod]
        public void syncUp(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var target = args.Value<JObject>(TARGET);
            var soupName = args.Value<string>(SOUP_NAME);

            var options = args.Value<JObject>(OPTIONS);
            var isGlobal = args.Value<bool>(IS_GLOBAL_STORE);

            var syncManager = GetSyncManager(isGlobal);

            var syncOptions = SyncOptions.FromJson(options);
            var syncTarget = new SyncUpTarget();

            try
            {
                _errorCallback = errorCallback;
                _successCallback = successCallback;

                syncManager.SyncUp(syncTarget, syncOptions, soupName, HandleSyncUpdate);
            }
            catch (SmartStoreException ex)
            {
                errorCallback.Invoke(ex.Message);
            }
        }

        [ReactMethod]
        public void syncDown(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var target = args.Value<JObject>(TARGET);
            var soupName = args.Value<string>(SOUP_NAME);

            var options = args.Value<JObject>(OPTIONS);
            var isGlobal = args.Value<bool>(IS_GLOBAL_STORE);

            var queryType = target.Value<string>(TYPE);

            var syncManager = GetSyncManager(isGlobal);
            var syncOptions = SyncOptions.FromJson(options);

            var qType = (SyncDownTarget.QueryTypes)Enum.Parse(typeof(SyncDownTarget.QueryTypes), queryType, true);

            SyncDownTarget syncTarget = null;

            switch (qType)
            {
                case SyncDownTarget.QueryTypes.Sosl:
                    {
                        var query = target.Value<string>(QUERY);
                        syncTarget = new SoslSyncDownTarget(query);
                        break;
                    }  
                                     
                case SyncDownTarget.QueryTypes.Soql:
                    {
                        var query = target.Value<string>(QUERY);
                        syncTarget = new SoqlSyncDownTarget(query);
                        break;
                    } 
                                      
                case SyncDownTarget.QueryTypes.Mru:
                    {
                        var query = target.Value<JObject>(QUERY);
                        syncTarget = new MruSyncDownTarget(query);
                        break;
                    }

                default:
                    break;
                 
            }

            try
            {
                _errorCallback = errorCallback;
                _successCallback = successCallback;

                syncManager.SyncDown(syncTarget, soupName, HandleSyncUpdate);
            }
            catch (SmartStoreException ex)
            {
                errorCallback.Invoke(ex.Message);
            }
        }

        [ReactMethod]
        public void getSyncStatus(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var syncId = args.Value<long>(SYNC_ID);
            var isGlobal = args.Value<bool>(IS_GLOBAL_STORE);

            var syncManager = GetSyncManager(isGlobal);

            try
            {
                SyncState sync = syncManager.GetSyncStatus(syncId);
                successCallback.Invoke(sync.AsJson().ToString());
            }
            catch (Exception ex)
            {
                errorCallback.Invoke(ex.Message);
            }
        }

        [ReactMethod]
        public void reSync(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var syncId = args.Value<long>(SYNC_ID);
            var isGlobal = args.Value<bool>(IS_GLOBAL_STORE);

            var syncManager = GetSyncManager(isGlobal);

            try
            {
                _errorCallback = errorCallback;
                _successCallback = successCallback;

                syncManager.ReSync(syncId, HandleSyncUpdate);
            }
            catch (SmartStoreException ex)
            {
                errorCallback.Invoke(ex.Message);
            }
        }

        private void HandleSyncUpdate(SyncState sync)
        {
            if (sync.Status == SyncState.SyncStatusTypes.Failed)
            {
                _errorCallback.Invoke("Sync failed");
            }
            else if (sync.Status == SyncState.SyncStatusTypes.Done)
            {
                _successCallback.Invoke(sync.AsJson().ToString());
            }
        }

        private SyncManager GetSyncManager(bool isGlobal)
        {
            var account = AccountManager.GetAccount();

            var syncManager = isGlobal
                    ? SyncManager.GetInstance(account)      // FIX
                    : SyncManager.GetInstance(account);

            return syncManager;
        }
    }
}
