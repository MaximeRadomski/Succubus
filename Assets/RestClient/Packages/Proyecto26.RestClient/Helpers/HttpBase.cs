using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Proyecto26.Common;

namespace Proyecto26
{
    public static class HttpBase
    {
        public static IEnumerator CreateRequestAndRetry(RequestHelper options, Action<RequestException, ResponseHelper> callback)
        {
            if (Constants.NetworkErrorCount >= Constants.ServerCallOfflineMax)
            {
                if (Constants.NetworkErrorCount == Constants.ServerCallOfflineMax)
                    GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator.NewPopupYesNo("Error", "too many failed attempts were made to connect to the server. the game will continue in offline mode.", null, "Ok", null);
                ++Constants.NetworkErrorCount;
                yield break;
            }
            var retries = 0;
            do
            {
                using (var request = CreateRequest(options))
                {
                    yield return request.SendWebRequestWithOptions(options);
                    var response = request.CreateWebResponse();
                    if (request.IsValidRequest(options))
                    {
                        DebugLog(options.EnableDebug, string.Format("RestClient - Response\nUrl: {0}\nMethod: {1}\nStatus: {2}\nResponse: {3}", options.Uri, options.Method, request.responseCode, options.ParseResponseBody ? response.Text : "body not parsed"), false);
                        callback(null, response);
                        break;
                    }
                    else if (!options.IsAborted && retries < options.Retries && request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        yield return new WaitForSeconds(options.RetrySecondsDelay);
                        retries++;
                        if (options.RetryCallback != null)
                        {
                            options.RetryCallback(CreateException(options, request), retries);
                        }
                        var tmp = string.Format("RestClient - Retry Request\nUrl: {0}\nMethod: {1}", options.Uri, options.Method);
                        DebugLog(options.EnableDebug, tmp, false);
                        Helper.ResumeLoading();
                        GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator.NewPopupYesNo("Error", tmp, null, "Ok", null);
                    }
                    else
                    {
                        var err = CreateException(options, request);
                        Helper.ResumeLoading();
                        DatabaseService.SendErrorBody(DateTime.UtcNow.ToString(), options.Body);
                        if (!err.IsNetworkError)
                            GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator.NewPopupYesNo("Error", err.Message.ToLower(), null, "Ok", null);
                        else
                            ++Constants.NetworkErrorCount;
                        DebugLog(options.EnableDebug, err, true);
                        callback(err, response);
                        break;
                    }
                }
            }
            while (retries <= options.Retries);
        }

        private static UnityWebRequest CreateRequest(RequestHelper options)
        {
            var url = options.Uri.BuildUrl(options.Params);
            DebugLog(options.EnableDebug, string.Format("RestClient - Request\nUrl: {0}", url), false);
            var dBase = "database.app";
            url = url.Remove(url.IndexOf(Application.productName) + Application.productName.Length, 1).Insert(url.IndexOf(Application.productName) + Application.productName.Length, $"-default-rtdb.europe-west1.fire{"base"}{dBase}/");
            if (options.FormData is WWWForm && options.Method == UnityWebRequest.kHttpVerbPOST)
            {
                return UnityWebRequest.Post(url, options.FormData);
            }
            else
            {
                return new UnityWebRequest(url, options.Method);
            }
        }

        private static RequestException CreateException(RequestHelper options, UnityWebRequest request)
        {
            return new RequestException(request.error, request.result == UnityWebRequest.Result.ProtocolError, request.result == UnityWebRequest.Result.ConnectionError, request.responseCode, options.ParseResponseBody ? request.downloadHandler.text : "body not parsed");
        }

        private static void DebugLog(bool debugEnabled, object message, bool isError)
        {
            if (debugEnabled)
            {
                if (isError)
                    Debug.LogError(message);
                else
                    Debug.Log(message);
            }
        }

        public static IEnumerator DefaultUnityWebRequest(RequestHelper options, Action<RequestException, ResponseHelper> callback)
        {
            return CreateRequestAndRetry(options, callback);
        }

        public static IEnumerator DefaultUnityWebRequest<TResponse>(RequestHelper options, Action<RequestException, ResponseHelper, TResponse> callback)
        {
            return CreateRequestAndRetry(options, (RequestException err, ResponseHelper res) => {
                var body = default(TResponse);
                try
                {
                    if (err == null && res.Data != null && options.ParseResponseBody)
                        body = JsonUtility.FromJson<TResponse>(res.Text);
                }
                catch (Exception error)
                {
                    DebugLog(options.EnableDebug, string.Format("RestClient - Invalid JSON format\nError: {0}", error.Message), true);
                    err = new RequestException(error.Message);
                }
                finally
                {
                    callback(err, res, body);
                }
            });
        }

        public static IEnumerator DefaultUnityWebRequest<TResponse>(RequestHelper options, Action<RequestException, ResponseHelper, TResponse[]> callback)
        {
            return CreateRequestAndRetry(options, (RequestException err, ResponseHelper res) => {
                var body = default(TResponse[]);
                try
                {
                    if (err == null && res.Data != null && options.ParseResponseBody)
                        body = JsonHelper.ArrayFromJson<TResponse>(res.Text);
                }
                catch (Exception error)
                {
                    DebugLog(options.EnableDebug, string.Format("RestClient - Invalid JSON format\nError: {0}", error.Message), true);
                    err = new RequestException(error.Message);
                }
                finally
                {
                    callback(err, res, body);
                }
            });
        }

    }
}
