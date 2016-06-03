using Newtonsoft.Json.Linq;
using ReactNative.Bridge;
using Salesforce.SDK.Auth;
using Salesforce.SDK.SmartStore.Store;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Salesforce.SDK.ReactNative
{
    /// <summary>
    /// 
    /// </summary>
    class SmartStoreReactBridge : NativeModuleBase
    {
        // Log tag
        private static string LOG_TAG = "SmartStoreReactBridge";

        // Keys in json from/to javascript
        private static string RE_INDEX_DATA = "reIndexData";
        private static string CURSOR_ID = "cursorId";
        private static string TYPE = "type";
        private static string SOUP_NAME = "soupName";
        private static string PATH = "path";
        private static string PATHS = "paths";
        private static string QUERY_SPEC = "querySpec";
        private static string QUERY_TYPE = "queryType";
        private static string EXTERNAL_ID_PATH = "externalIdPath";
        private static string ENTRIES = "entries";
        private static string ENTRY_IDS = "entryIds";
        private static string INDEX = "index";
        private static string INDEXES = "indexes";
        private static string IS_GLOBAL_STORE = "isGlobalStore";
        private static string BEGIN_KEY = "beginKey";
        private static string END_KEY = "endKey";
        private static string INDEX_PATH = "indexPath";
        private static string LIKE_KEY = "likeKey";
        private static string MATCH_KEY = "matchKey";
        private static string SMART_SQL = "smartSql";
        private static string ORDER_PATH = "orderPath";
        private static string ORDER = "order";
        private static string PAGE_SIZE = "pageSize";

        /// <summary>
        /// Instantiates the <see cref="SmartStoreReactBridge"/>.
        /// </summary>
        internal SmartStoreReactBridge()
        {
        }

        /// <summary>
        /// The name of the native module.
        /// </summary>
        public override string Name
        {
            get
            {
                return "SmartStoreReactBridge";
            }
        }

        [ReactMethod]
        public void removeFromSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var soupName = args.Value<string>(SOUP_NAME);
            var smartStore = GetSmartStore(args);

            var ids = args.Value<JArray>(ENTRY_IDS);
            var soupEntryIds = new long[ids.Count];

            for (int i = 0; i < ids.Count; i++)
            {
                soupEntryIds[i] = ids[i].Value<long>();
            }

            smartStore.Delete(soupName, soupEntryIds, true);
            successCallback.Invoke();
        }

        [ReactMethod]
        public void retrieveSoupEntries(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void closeCursor(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void moveCursorToPageIndex(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void soupExists(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void upsertSoupEntries(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            var soupName = args.Value<string>(SOUP_NAME);
            var smartStore = GetSmartStore(args);

            var entries = args.Value<JArray>(ENTRIES);
            var externalIdPath = args.Value<string>(EXTERNAL_ID_PATH);

            var results = new JArray();

            try
            {
                foreach (var entry in entries)
                {
                    results.Add(smartStore.Upsert(soupName, entry as JObject));
                }
                successCallback.Invoke(results.ToString());
            }
            catch (Exception ex)
            {
                errorCallback.Invoke(ex.Message);
            }
        }

        [ReactMethod]
        public void registerSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            RunOnDispatcher(() =>
            {
                try
                {
                    var soupName = args.Value<string>(SOUP_NAME);
                    var indexes = args.Value<JArray>(INDEXES);

                    var indexSpecs = new IndexSpec[indexes.Count];
                    var idx = 0;
                    foreach (var item in indexes)
                    {
                        var path = ((JObject)item).Value<string>(PATH);
                        var smartStoreType = ((JObject)item).Value<string>(TYPE);

                        indexSpecs[idx++] = new IndexSpec(path, new SmartStoreType(smartStoreType));
                    }

                    var smartStore = SmartStore.Store.SmartStore.GetSmartStore();
                    smartStore.RegisterSoup(soupName, indexSpecs);
                    successCallback.Invoke();
                }
                catch (Exception ex)
                {
                    errorCallback.Invoke(ex.Message);
                }
            });
        }

        [ReactMethod]
        public void querySoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            if ( !SalesforceReactHelper.ActiveAccount(errorCallback)) return;

            var soupName = args.Value<string>(SOUP_NAME);
            var smartStore = GetSmartStore(args);
            var querySpecJson = args.Value<JObject>(QUERY_SPEC);

            QuerySpec querySpec = fromJSON(soupName, querySpecJson);
            if (querySpec.QueryType == QuerySpec.SmartQueryType.Smart)
            {
                throw new Exception("Smart queries can only be run through runSmartQuery");
            }

            RunQuery(smartStore, querySpec, successCallback);      
        }

        [ReactMethod]
        public void runSmartQuery(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void removeSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void clearSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void getDatabaseSize(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void alterSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void reIndexSoup(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        [ReactMethod]
        public void getSoupIndexSpecs(JObject args, ICallback successCallback, ICallback errorCallback)
        {
            throw new NotImplementedException();
        }

        private SmartStore.Store.SmartStore GetSmartStore(JObject args)
        {
            var account = AccountManager.GetAccount();
            var isGlobal = GetIsGlobal(args);
            return isGlobal
                    ? SmartStore.Store.SmartStore.GetGlobalSmartStore()
                    : SmartStore.Store.SmartStore.GetSmartStore(account);
        }

        private bool GetIsGlobal(JObject args)
        {
            return args != null ? args.Value<bool>(IS_GLOBAL_STORE) : false;
        }

        private static QuerySpec fromJSON(string soupName, JObject querySpecJson)
        {
            var str = querySpecJson.Value<string>(QUERY_TYPE);
            var queryType = (QuerySpec.SmartQueryType)Enum.Parse(typeof(QuerySpec.SmartQueryType), str, true);
            QuerySpec querySpec = null;

            var path = querySpecJson.Value<string>(INDEX_PATH);
            var matchKey = querySpecJson.Value<string>(MATCH_KEY);
            var beginKey = querySpecJson.Value<string>(BEGIN_KEY);
            var endKey = querySpecJson.Value<string>(END_KEY);
            var likeKey = querySpecJson.Value<string>(LIKE_KEY);
            var smartSql = querySpecJson.Value<string>(SMART_SQL);
            var orderPath = querySpecJson.Value<string>(ORDER_PATH);
            var pageSize = querySpecJson.Value<int>(PAGE_SIZE);

            var order = querySpecJson.Value<string>(ORDER);

            var sqlOrder = QuerySpec.SqlOrder.ASC;
            if (order.Equals("descending", StringComparison.OrdinalIgnoreCase))
            {
                sqlOrder = QuerySpec.SqlOrder.DESC;
            }

            switch (queryType)
            {
                case QuerySpec.SmartQueryType.Exact:   querySpec = QuerySpec.BuildExactQuerySpec(soupName, path, matchKey, pageSize); break;
                case QuerySpec.SmartQueryType.Range:   querySpec = QuerySpec.BuildRangeQuerySpec(soupName, path, beginKey, endKey, sqlOrder, pageSize); break;
                case QuerySpec.SmartQueryType.Like:    querySpec = QuerySpec.BuildLikeQuerySpec(soupName, path, likeKey, sqlOrder, pageSize); break;
                case QuerySpec.SmartQueryType.Smart:   querySpec = QuerySpec.BuildSmartQuerySpec(smartSql, pageSize); break;
                default: throw new Exception("Fell through switch: " + queryType);
            }
            return querySpec;
        }

        private void RunQuery(SmartStore.Store.SmartStore smartStore, QuerySpec querySpec, ICallback successCallback)
        {
            // FIX
            var result = smartStore.Query(querySpec, 0);
            var obj = new JObject
            {
                new JProperty("cursorId", 0),
                new JProperty("currentPageIndex", 0),
                new JProperty("PageSize", querySpec.PageSize),
                new JProperty("totalEntries", result.Count),
                new JProperty("totalPages", result.Count / querySpec.PageSize + (result.Count % querySpec.PageSize == 0 ? 0 : 1)),
                new JProperty("currentPageOrderedEntries", result),   
            };

            successCallback.Invoke(obj.ToString());      
	    }

        private static async void RunOnDispatcher(DispatchedHandler action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action);
        }
    }
}
