﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Collections;
using Cosmos.Resource;

namespace Cosmos.Editor.Resource
{
    public class AssetBundleTab
    {
        public Func<EditorCoroutine> BuildDataset;
        public const string AssetBundleTabDataName = "ResourceEditor_AsseBundleTabData.json";
        AssetBundleTabData tabData;
        AssetBundleBuilder assetBundleBuilder = new AssetBundleBuilder();

        public void OnEnable()
        {
            GetTabData();
        }
        public void OnDisable()
        {
            SaveTabData();
        }
        public void OnGUI(Rect rect)
        {
            DrawBuildOptions();
            GUILayout.Space(16);
            DrawPathOptions();
            GUILayout.Space(16);
            DrawEncryption();
            GUILayout.Space(16);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Build assetBundle"))
                {
                    if (ResourceEditorDataProxy.ResourceDataset == null)
                    {
                        EditorUtil.Debug.LogError("ResourceDataset is invalid !");
                        return;
                    }
                    var buildParams = new AssetBundleBuildParams()
                    {
                        AssetBundleBuildPath = tabData.AssetBundleBuildPath,
                        AssetBundleOffsetValue = tabData.AssetBundleOffsetValue,
                        BuildAssetBundleOptions = GetBuildAssetBundleOptions(),
                        BuildedAssetNameType = tabData.BuildedAssetNameType,
                        BuildIedAssetsEncryptionKey = tabData.BuildIedAssetsEncryptionKey,
                        BuildTarget = tabData.BuildTarget,
                        BuildVersion = tabData.BuildVersion
                    };
                    EditorUtil.Coroutine.StartCoroutine(BuildAssetBundle(buildParams, ResourceEditorDataProxy.ResourceDataset));
                }
                if (GUILayout.Button("Reset options"))
                {
                    tabData = new AssetBundleTabData();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        public void OnDatasetAssign()
        {

        }
        public void OnDatasetUnassign()
        {

        }
        void DrawBuildOptions()
        {
            EditorGUILayout.LabelField("Build Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build target", tabData.BuildTarget);
                tabData.AssetBundleCompressType = (AssetBundleCompressType)EditorGUILayout.EnumPopup("Build compression type", tabData.AssetBundleCompressType);

                tabData.ForceRebuildAssetBundle = EditorGUILayout.ToggleLeft("Force rebuild assetBundle", tabData.ForceRebuildAssetBundle);
                tabData.DisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable write type tree", tabData.DisableWriteTypeTree);
                tabData.DeterministicAssetBundle = EditorGUILayout.ToggleLeft("Deterministic assetBundle", tabData.DeterministicAssetBundle);
                tabData.IgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore type tree changes", tabData.IgnoreTypeTreeChanges);
            }
            EditorGUILayout.EndVertical();
        }
        void DrawPathOptions()
        {
            EditorGUILayout.LabelField("Path Options", EditorStyles.boldLabel);
            bool versionValid = false;
            EditorGUILayout.BeginVertical();
            {
                tabData.BuildVersion = EditorGUILayout.TextField("Build version", tabData.BuildVersion);
                EditorGUILayout.BeginHorizontal();
                {
                    tabData.BuildPath = EditorGUILayout.TextField("Build path", tabData.BuildPath.Trim());
                    if (GUILayout.Button("Browse", GUILayout.MaxWidth(128)))
                    {
                        var newPath = EditorUtility.OpenFolderPanel("Bundle output path", tabData.BuildPath, string.Empty);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            tabData.BuildPath = newPath.Replace("\\", "/");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                versionValid = !string.IsNullOrEmpty(tabData.BuildVersion);
                if (versionValid)
                {
                    var version = tabData.BuildVersion.Replace(".", "_");
                    tabData.AssetBundleBuildPath = Utility.IO.WebPathCombine(tabData.BuildPath, tabData.BuildTarget.ToString(), version);
                }
                else
                    EditorGUILayout.HelpBox("BuildVersion is invalid !", MessageType.Error);
                EditorGUILayout.LabelField("AssetBundle build path", tabData.AssetBundleBuildPath);
            }
            EditorGUILayout.EndVertical();
        }

        void DrawEncryption()
        {
            EditorGUILayout.LabelField("Encryption Options", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            {
                tabData.BuildedAssetsEncryption = EditorGUILayout.ToggleLeft("Builded assets encryption", tabData.BuildedAssetsEncryption);
                if (tabData.BuildedAssetsEncryption)
                {
                    tabData.BuildIedAssetsEncryptionKey = EditorGUILayout.TextField("Builded assets encryption key", tabData.BuildIedAssetsEncryptionKey);
                    var aesKeyStr = tabData.BuildIedAssetsEncryptionKey;
                    var aesKeyLength = Encoding.UTF8.GetBytes(aesKeyStr).Length;
                    EditorGUILayout.LabelField($"Builded assets AES encryption key, key should be 16,24 or 32 bytes long, current key length is : {aesKeyLength} ");
                    if (aesKeyLength != 16 && aesKeyLength != 24 && aesKeyLength != 32 && aesKeyLength != 0)
                    {
                        EditorGUILayout.HelpBox("Key should be 16,24 or 32 bytes long", MessageType.Error);
                    }
                    //打包输出的资源加密，如buildInfo，assetbundle 文件名加密
                    tabData.BuildedAssetNameType = (BuildedAssetNameType)EditorGUILayout.EnumPopup("Builded assets name type ", tabData.BuildedAssetNameType);
                    GUILayout.Space(16);
                }
                tabData.AssetBundleEncryption = EditorGUILayout.ToggleLeft("AssetBundle encryption", tabData.AssetBundleEncryption);
                if (tabData.AssetBundleEncryption)
                {
                    EditorGUILayout.LabelField("AssetBundle encryption offset");
                    tabData.AssetBundleOffsetValue = EditorGUILayout.IntField("Encryption offset", tabData.AssetBundleOffsetValue);
                    if (tabData.AssetBundleOffsetValue < 0)
                        tabData.AssetBundleOffsetValue = 0;
                }
            }
            EditorGUILayout.EndVertical();
        }
        void GetTabData()
        {
            try
            {
                tabData = EditorUtil.GetData<AssetBundleTabData>(AssetBundleTabDataName);
            }
            catch
            {
                tabData = new AssetBundleTabData();
                EditorUtil.SaveData(AssetBundleTabDataName, tabData);
            }
        }
        void SaveTabData()
        {
            EditorUtil.SaveData(AssetBundleTabDataName, tabData);
        }
        IEnumerator BuildAssetBundle(AssetBundleBuildParams buildParams, ResourceDataset dataset)
        {
            yield return BuildDataset.Invoke();
            assetBundleBuilder.PrepareBuildAssetBundle(buildParams, dataset);
            var manifest = BuildPipeline.BuildAssetBundles(buildParams.AssetBundleBuildPath, buildParams.BuildAssetBundleOptions, buildParams.BuildTarget);
            assetBundleBuilder.ProcessAssetBundle(buildParams, dataset, manifest);
        }
        BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
            var compressType = tabData.AssetBundleCompressType;
            switch (compressType)
            {
                case AssetBundleCompressType.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case AssetBundleCompressType.StandardCompression_LZMA:
                    //None=StandardCompression_LZMA
                    break;
                case AssetBundleCompressType.ChunkBasedCompression_LZ4:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }
            if (tabData.DisableWriteTypeTree)
                options |= BuildAssetBundleOptions.DisableWriteTypeTree;
            if (tabData.DeterministicAssetBundle)
                options |= BuildAssetBundleOptions.DeterministicAssetBundle;
            if (tabData.ForceRebuildAssetBundle)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            if (tabData.IgnoreTypeTreeChanges)
                options |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
            return options;
        }
    }
}
