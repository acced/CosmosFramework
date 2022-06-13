﻿using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Cosmos.WebRequest
{
    public interface IWebRequestHelper
    {
        /// <summary>
        /// 是否正在加载；
        /// </summary>
        bool IsLoading { get; }
        /// <summary>
        /// 异步上传请求；
        /// </summary>
        /// <param name="uploadRequest">上传请求</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine UploadRequestAsync(UnityWebRequest uploadRequest, WebUploadCallback webUploadCallback);
        /// <summary>
        /// 异步下载请求；
        /// </summary>
        /// <param name="downloadRequest">下载请求</param>
        /// <param name="webDownloadCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine DownloadRequestAsync(UnityWebRequest downloadRequest, WebRequestCallback webDownloadCallback, Action<UnityWebRequest> resultCallback);
        /// <summary>
        /// 异步请求文件流；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestFileBytesAsync(string uri, WebRequestCallback webRequestCallback);
        /// <summary>
        /// 异步请求Text；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextAsync(string uri, WebRequestCallback webRequestCallback, Action<string> resultCallback);
        /// <summary>
        /// 异步请求Texture；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestTextureAsync(string uri, WebRequestCallback webRequestCallback, Action<Texture2D> resultCallback);
        /// <summary>
        /// 异步请求AssetBundle；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAssetBundleAsync(string uri, WebRequestCallback webRequestCallback, Action<AssetBundle> resultCallback);
        /// <summary>
        /// 异步请求Audio；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="audioType">声音类型</param>
        /// <param name="webRequestCallback">回调</param>
        /// <param name="resultCallback">带结果的回调</param>
        /// <returns>协程对象</returns>
        Coroutine RequestAudioAsync(string uri, AudioType audioType, WebRequestCallback webRequestCallback, Action<AudioClip> resultCallback);
        /// <summary>
        /// 异步提交新建资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PostAsync(string uri, byte[] bytes, WebUploadCallback  webUploadCallback);
        /// <summary>
        /// 异步提交覆盖资源；
        /// </summary>
        /// <param name="uri">Uniform Resource Identifier</param>
        /// <param name="bytes">数据流</param>
        /// <param name="webUploadCallback">回调</param>
        /// <returns>协程对象</returns>
        Coroutine PutAsync(string uri, byte[] bytes, WebUploadCallback  webUploadCallback);
        /// <summary>
        /// 结束所有网络请求
        /// </summary>
        void AbortAllRequest();
    }
}
