using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
//using Parse;

namespace DarkcupGames
{
    public class AssetDownloader : MonoBehaviour
    {
        private const string DB_FILE = "4823754324275785482736";

        private const string PARSE_TABLE_ANALYTICS = "AnalyticsDownloadData";
        private const string PARSE_DATANAME_ROW = "dataName";
        private const string PARSE_COUNTERS_ROW = "counters";
        //"self-create-in-hierarchy" singleton type
        private static AssetDownloader instance;
        private static WaitForSeconds waitWhenCheckProgress = new WaitForSeconds(0.1f);

        private static DownloadedAssetDatabase db;

        private static bool isInitialized = false;

        private static void Initialize()
        {
            if (!isInitialized)
            {
                db = FileUtilities.DeserializeObjectFromFile<DownloadedAssetDatabase>(DB_FILE, "tng2903");
                if (db == null)
                {
                    db = new DownloadedAssetDatabase();
                    db.assets = new Dictionary<string, DownloadedAssetItem>();
                }

                isInitialized = true;
            }
        }

        public static AssetDownloader Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("AssetDownloader");
                    instance = go.AddComponent<AssetDownloader>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        public void DownloadAndCacheAssetBundle(string url, int version, Action<float> progress, Action<string> fail, Action<WWW> finished, float timeout = 15f)
        {
            //Debug.Log("loadAssetAndCache : " + url);
            StartCoroutine(CoroutineDownloadAndCacheAssetBundle(url, version, progress, fail, finished, timeout));
        }

        IEnumerator CoroutineDownloadAndCacheAssetBundle(string url, int version, Action<float> progress, Action<string> fail, Action<WWW> finished, float timeout)
        {
            // Wait for the Caching system to be ready
            while (!Caching.ready)
                yield return null;

            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            using (WWW www = WWW.LoadFromCacheOrDownload(System.Uri.EscapeUriString(url), version))
            {
                float startTime = Time.realtimeSinceStartup;
                float lastProgress = 0;
                //try to finish downloading file 
                while (!www.isDone)
                {
                    //notify callback if needed          
                    if (progress != null)
                        progress(www.progress);

                    //if there is progress being made
                    if (lastProgress < www.progress)
                    {
                        //take note
                        lastProgress = www.progress;
                        //reset timeout
                        startTime = Time.realtimeSinceStartup;
                    }

                    //if the download is taking too much time
                    if (Time.realtimeSinceStartup - startTime >= timeout)
                    {
                        //do not download anymore
                        if (fail != null)
                        {
                            fail("Request timed out");
                        }
                        yield break;
                    }

                    yield return waitWhenCheckProgress;
                }
                yield return www;

                if (www.error != null)
                {
                    //				throw new Exception("WWW download had an error:" + www.error);
                    if (fail != null)
                        fail(www.error);
                }
                else
                {
                    if (finished != null)
                        finished(www);
                }
            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
        }

        public void DownloadAndCacheAsset(string url, int version, Action<float> progress, Action<string> fail, Action<WWW> finished, bool hideRealFileName = true, float timeout = 15f)
        {
            StartCoroutine(CoroutineDownloadAndCacheAsset(url, version, progress, fail, finished, hideRealFileName, timeout));
        }

        IEnumerator CoroutineDownloadAndCacheAsset(string url, int version, Action<float> progress, Action<string> fail, Action<WWW> finished, bool hideRealFilename, float timeout)
        {
            if (!isInitialized) { Initialize(); }

            //this.Print("Receiving request for url " + url + " version: " + version);
            string assetPath;
            bool loadFromLocal = false;
            //check if the asset has been downloaded before
            if (db.assets.ContainsKey(url))
            {
                //if yes, check its version
                int downloadedVersion = db.assets[url].version;
                //this.Print("Asset already existed, with version: " + downloadedVersion);
                //if the downloaded version is not the same as expected
                if (downloadedVersion != version)
                {
                    //we need to re-download it
                    assetPath = url;
                    //and delete cached version
                    FileUtilities.DeleteFile(db.assets[url].localPath, true);
                    //this.Print("Removing old version");
                    //remove item from cache
                    db.assets.Remove(url);
                }
                else
                {
                    //if the url is requested again
                    //check if the asset is inaccessible or not
                    if (FileUtilities.IsFileExist(db.assets[url].localPath, true))
                    {
                        //if yes, load it from cache
                        assetPath = "file://" + db.assets[url].localPath;
                        loadFromLocal = true;
                    }
                    else
                    {
                        //or else, try to reload it
                        assetPath = url;
                        db.assets.Remove(url);
                        // Debug.Log("Asset file is inaccessible, will try to download it again");
                    }
                }
            }
            else
            {
                //if the asset has never been downloaded before, download it
                assetPath = url;
                //this.Print("Asset not existed, trying to download from url");
            }
            //assetPath = "file://E:/gits/Amanotes/piano-challenge-2/DownloadedData\\-1225165794/CanonRock.txt";
            //this.Print("Downloading asset from url: " + assetPath);
            //download asset from suitable path 
            using (WWW www = new WWW(assetPath))
            {
                float startTime = Time.realtimeSinceStartup;
                float lastProgress = 0;
                //try to finish downloading file 
                while (!www.isDone)
                {
                    //notify callback if needed          
                    if (progress != null)
                        progress(www.progress);

                    //print("Progress: " + www.progress);
                    //if there is progress being made
                    if (lastProgress < www.progress)
                    {
                        //take note
                        lastProgress = www.progress;
                        //reset timeout
                        startTime = Time.realtimeSinceStartup;
                    }

                    //if the download is taking too much time
                    if (Time.realtimeSinceStartup - startTime >= timeout)
                    {
                        //do not download anymore
                        if (fail != null)
                        {
                            fail("Request timed out");
                        }
                        yield break;
                    }

                    yield return waitWhenCheckProgress;
                }
                yield return www;

                //print("ProgressDone: " + www.progress);
                if (www.error != null)
                {
                    Debug.LogError("Download error: " + www.error);
                    //this.Print("Download error: " + www.error);
                    if (fail != null)
                        fail(www.error);
                }
                else
                {
                    if (finished != null)
                    {
                        //cache the file in local storage if is newly download from Internet
                        if (!loadFromLocal)
                        {
                            string fileName = ExtractFileNameFromURL(assetPath);
                            string filePath;
                            if (hideRealFilename)
                            {
                                filePath = assetPath.GetHashCode().ToString() + fileName.GetHashCode().ToString();
                                //Debug.Log(filepath);
                            }
                            else
                            {
                                filePath = fileName;
                            }
                            // TODO: Should turn this line on for security
                            //filePath = filePath.Sanitize();
                            filePath = filePath.Replace("%", "");
                            string localPath = FileUtilities.SaveFile(www.bytes, filePath);

                            //analytic data in Parse
                            AnalyticsDownloadData(fileName);

                            DownloadedAssetItem item = new DownloadedAssetItem();
                            item.url = url;
                            item.version = version;
                            item.localPath = localPath;
                            item.dateCreated = DateTime.UtcNow;

                            if (db.assets.ContainsKey(url))
                            {
                                db.assets[url] = item;
                            }
                            else
                            {
                                db.assets.Add(url, item);
                            }

                            //this.Print("Asset cached, file name: " + fileName);

                            //update db
                            FileUtilities.SerializeObjectToFile(db, DB_FILE, "tng2903");
                        }
                        //print("Download finished");
                        finished(www);
                    }
                }
            }
        }

        public void AnalyticsDownloadData(string fileName)
        {
            //var query = new ParseQuery<ParseObject>(PARSE_TABLE_ANALYTICS)
            //    .WhereEqualTo(PARSE_DATANAME_ROW, fileName);

            //query.FindAsync().ContinueWith(
            //    t =>
            //    {
            //        IEnumerator<ParseObject> obj = t.Result.GetEnumerator();
            //        obj.MoveNext();
            //        if (obj.Current == null)
            //        {
            //            //Debug.Log("null");
            //            ParseObject analyticsData = new ParseObject(PARSE_TABLE_ANALYTICS);
            //            analyticsData[PARSE_DATANAME_ROW] = fileName;
            //            analyticsData[PARSE_COUNTERS_ROW] = 1;
            //            analyticsData.SaveAsync();
            //        }
            //        else
            //        {
            //            //Debug.Log("not null");
            //            obj.Current.Increment(PARSE_COUNTERS_ROW, 1);
            //            obj.Current.SaveAsync();
            //        }
            //    });
        }

        public void RemoveAsset(string url)
        {
            if (db.assets.ContainsKey(url))
            {
                FileUtilities.DeleteFile(db.assets[url].localPath);
                db.assets.Remove(url);
            }
        }

        public void DownloadAsset(string url, Action<float> progress, Action<string> fail, Action<WWW> finished, float timeout = 15f)
        {
            StartCoroutine(CoroutineDownloadAsset(url, progress, fail, finished, timeout));
        }

        IEnumerator CoroutineDownloadAsset(string url, Action<float> progress, Action<string> fail, Action<WWW> finished, float timeout)
        {
            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            using (WWW www = new WWW(url))
            {
                float startTime = Time.realtimeSinceStartup;
                float lastProgress = 0;
                //try to finish downloading file 
                while (!www.isDone)
                {
                    //notify callback if needed          
                    if (progress != null)
                        progress(www.progress);

                    //if there is progress being made
                    if (lastProgress < www.progress)
                    {
                        //take note
                        lastProgress = www.progress;
                        //reset timeout
                        startTime = Time.realtimeSinceStartup;
                    }

                    //if the download is taking too much time
                    if (Time.realtimeSinceStartup - startTime >= timeout)
                    {
                        //do not download anymore
                        if (fail != null)
                        {
                            fail("Request timed out");
                        }
                        yield break;
                    }

                    yield return waitWhenCheckProgress;
                }
                yield return www;

                if (www.error != null)
                {
                    if (fail != null)
                        fail(www.error);
                }
                else
                {
                    if (finished != null)
                        finished(www);
                }
            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
        }

        public void DeleteCachedAsset(string url, int version)
        {
            StartCoroutine(CoroutineDeleteCachedAsset(url, version));
        }

        IEnumerator CoroutineDeleteCachedAsset(string url, int version)
        {
            while (!Caching.ready)
                yield return null;
            uint a = 0;
            using (WWW www = WWW.LoadFromCacheOrDownload(System.Uri.EscapeUriString(url), version, a))
            {
                yield return www;

                if (www.error != null)
                {
                    Debug.LogWarning("Error in delete file : " + url + "\t Error: " + www.error);
                }
                else
                {
                    Debug.Log("Deleted : " + url);
                }
            }
        }

        public string ExtractFileNameFromURL(string url)
        {
            if (string.IsNullOrEmpty(url)) { return string.Empty; }

            return url.Substring(url.LastIndexOf('/'));
        }
    }

    public class DownloadedAssetDatabase
    {
        public Dictionary<string, DownloadedAssetItem> assets;
    }
    public class DownloadedAssetItem
    {
        public string url;
        public string localPath;
        public int version;
        public DateTime dateCreated;
    }
}