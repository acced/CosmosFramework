﻿using Cosmos.Quark;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace Cosmos.CosmosEditor
{
    public class AssetDatabaseTab
    {
        IncludeDirectoriesOperation includeDirectoriesOperation = new IncludeDirectoriesOperation();
        WindowTabData WindowTabData { get { return QuarkAssetWindow.WindowTabData; } }
        QuarkAssetDataset quarkAssetDataset { get { return QuarkEditorDataProxy.QuarkAssetDataset; } }
        public void Reload()
        {
            includeDirectoriesOperation.Reload();
        }
        public void OnDisable()
        {
        }
        public void OnEnable()
        {
            //includeDirectoriesOperation.FolderPath = QuarkAssetDataset.IncludeDirectories;
            includeDirectoriesOperation.OnEnable();
        }
        public void OnGUI()
        {
            DrawFastDevelopTab();
        }
        public EditorCoroutine EnumUpdateADBMode()
        {
            return EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
        }
        void DrawFastDevelopTab()
        {
            EditorUtil.DrawVerticalContext(() =>
            {
                WindowTabData.UnderAssetsDirectory = EditorGUILayout.ToggleLeft("UnderAssetsDirectory", WindowTabData.UnderAssetsDirectory);
                WindowTabData.GenerateAssetPathCode = EditorGUILayout.ToggleLeft("GenerateAssetPath", WindowTabData.GenerateAssetPathCode);
            });
            GUILayout.Space(16);
            EditorUtil.DrawHorizontalContext(() =>
            {
                if (GUILayout.Button("Build"))
                {
                    EditorUtil.Coroutine.StartCoroutine(EnumBuildADBMode());
                }
                if (GUILayout.Button("Clear"))
                {
                    ADBModeClear();
                }
            });
            if (!WindowTabData.UnderAssetsDirectory)
            {
                GUILayout.BeginVertical("box");
                GUILayout.Space(16);
                EditorUtil.DrawHorizontalContext(() =>
                {
                    if (GUILayout.Button("ClearAssets"))
                    {
                        includeDirectoriesOperation.Clear();
                    }
                });
                includeDirectoriesOperation.OnGUI();
                GUILayout.EndVertical();
            }
        }

        #region AssetDataBaseMode
        IEnumerator EnumBuildADBMode()
        {
            if (quarkAssetDataset == null)
            {
                EditorUtil.Debug.LogError("QuarkAssetDataset is invalid !");
                yield break;
            }
            if (WindowTabData.UnderAssetsDirectory)
            {
                ADBModeUnderAssetsDirectoryBuild();
            }
            else
            {
                ADBModeNotUnderAssetsDirectoryBuild();
            }
            EditorUtility.SetDirty(quarkAssetDataset);
            EditorUtil.SaveData(QuarkAssetWindow.QuarkAssetWindowTabDataFileName, WindowTabData);
            if (WindowTabData.GenerateAssetPathCode)
                AssetDataBaseModeCreatePathScript();
            yield return null;
            EditorUtil.Debug.LogInfo("Quark asset  build done ");
        }
        void ADBModeClear()
        {
            includeDirectoriesOperation.Clear();
            quarkAssetDataset.Dispose();
            EditorUtility.SetDirty(quarkAssetDataset);
            EditorUtil.ClearData(QuarkAssetWindow.QuarkAssetWindowTabDataFileName);
            EditorUtil.Debug.LogInfo("Quark asset clear done ");
        }
        void ADBModeNotUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetDatabaseObject> quarkAssetList = new List<QuarkAssetDatabaseObject>();
            quarkAssetList?.Clear();
            int currentDirIndex = 0;
            var dirs = quarkAssetDataset.DirHashPairs;
            var dirHashPair = quarkAssetDataset.DirHashPairs.ToArray();
            Dictionary<string, FileSystemInfo> fileSysInfoDict = new Dictionary<string, FileSystemInfo>();
            if (dirs != null)
            {
                foreach (var dir in dirs)
                {
                    if (Directory.Exists(dir.Dir))
                    {
                        Utility.IO.TraverseFolderFile(dir.Dir, (file) =>
                        {
                            currentDirIndex++;
                            if (currentDirIndex < dirs.Count)
                            {
                                EditorUtility.DisplayCancelableProgressBar("TraverseFolderFile", "Building", (float)currentDirIndex / (float)dirs.Count);
                            }
                            else
                            {
                                EditorUtility.ClearProgressBar();
                            }
                            if (!fileSysInfoDict.ContainsKey(file.FullName))
                            {
                                fileSysInfoDict.Add(file.FullName, file);
                            }
                        });
                    }
                    else if (File.Exists(dir.Dir))
                    {
                        var fullPath = Utility.IO.PathCombine(EditorUtil.ApplicationPath(), dir.Dir);
                        if (!fileSysInfoDict.ContainsKey(fullPath))
                        {
                            var fileInfo = new FileInfo(fullPath);
                            fileSysInfoDict.Add(fileInfo.FullName, fileInfo);
                        }
                    }
                }
                var fileCount = fileSysInfoDict.Count;
                foreach (var file in fileSysInfoDict.Values)
                {

                    currentBuildIndex++;
                    if (currentBuildIndex < fileCount)
                    {
                        EditorUtility.DisplayCancelableProgressBar("QuarkAssetDataset", "Building", (float)currentBuildIndex / (float)fileCount);
                    }
                    else
                    {
                        EditorUtility.ClearProgressBar();
                    }
                    var projLength = QuarkConsts.Extensions.Length;
                    for (int i = 0; i < projLength; i++)
                    {
                        if (QuarkConsts.Extensions[i].Equals(file.Extension))
                        {
                            var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
                            var assetName = file.Name.Replace(file.Extension, string.Empty);
                            var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                            var assetObj = new QuarkAssetDatabaseObject()
                            {
                                AssetExtension = file.Extension,
                                AssetName = assetName,
                                AssetPath = assetPath,
                                AssetType = type.ToString(),
                                AssetGuid = AssetDatabase.AssetPathToGUID(assetPath)
                            };
                            quarkAssetList.Add(assetObj);
                        }
                    }
                }
            }
            quarkAssetDataset.QuarkAssetObjectList = quarkAssetList;
            quarkAssetDataset.QuarkAssetCount = quarkAssetList.Count;
            quarkAssetDataset.DirHashPairs.Clear();
            quarkAssetDataset.DirHashPairs .AddRange( dirHashPair);
        }
        void ADBModeUnderAssetsDirectoryBuild()
        {
            EditorUtility.ClearProgressBar();
            var count = Utility.IO.FolderFileCount(Application.dataPath);
            int currentBuildIndex = 0;
            List<QuarkAssetDatabaseObject> quarkAssetList = new List<QuarkAssetDatabaseObject>();
            var dirHashPair = quarkAssetDataset.DirHashPairs.ToArray();
            quarkAssetList?.Clear();
            string path = Application.dataPath;
            Utility.IO.TraverseFolderFile(path, (file) =>
            {
                currentBuildIndex++;
                if (currentBuildIndex < count)
                {
                    EditorUtility.DisplayCancelableProgressBar("QuarkAssetDataset", "Building", (float)currentBuildIndex / (float)count);
                }
                else
                {
                    EditorUtility.ClearProgressBar();
                }
                var length = QuarkConsts.Extensions.Length;
                for (int i = 0; i < length; i++)
                {
                    if (QuarkConsts.Extensions[i].Equals(file.Extension))
                    {
                        var assetPath = file.FullName.Remove(0, QuarkAssetWindow.FilterLength);
                        assetPath = assetPath.Replace("\\", "/");
                        var assetName = file.Name.Replace(file.Extension, string.Empty);
                        var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                        var assetObj = new QuarkAssetDatabaseObject()
                        {
                            AssetExtension = file.Extension,
                            AssetName = assetName,
                            AssetPath = assetPath,
                            AssetType = type.ToString(),
                            AssetGuid = AssetDatabase.AssetPathToGUID(assetPath)
                        };
                        quarkAssetList.Add(assetObj);
                    }
                }
            });
            quarkAssetDataset.QuarkAssetObjectList = quarkAssetList;
            quarkAssetDataset.QuarkAssetCount = quarkAssetList.Count;
            quarkAssetDataset.DirHashPairs.Clear();
            quarkAssetDataset.DirHashPairs.AddRange(dirHashPair);
        }
        void AssetDataBaseModeCreatePathScript()
        {
            var str = "public static class QuarkAssetDefine\n{\n";
            var con = "    public static string ";
            for (int i = 0; i < quarkAssetDataset.QuarkAssetCount; i++)
            {
                var srcName = quarkAssetDataset.QuarkAssetObjectList[i].AssetName;
                srcName = srcName.Trim();
                var fnlName = srcName.Contains(".") == true ? srcName.Replace(".", "_") : srcName;
                fnlName = srcName.Contains(" ") == true ? srcName.Replace(" ", "_") : srcName;
                str = Utility.Text.Append(str, con, fnlName, "= \"", srcName, "\"", " ;\n");
            }
            str += "\n}";
            Utility.IO.OverwriteTextFile(Application.dataPath, "QuarkAssetDefine.cs", str);
            AssetDatabase.Refresh();
        }
        #endregion
    }
}
