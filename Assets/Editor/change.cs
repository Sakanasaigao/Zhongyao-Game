using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class TuanJieGuidTools
    {
        [MenuItem("Tools/转换团结guid到Unity")]
        private static void ConvertGuid()
        {
            var allAssets = AssetDatabase.GetAllAssetPaths();
            var total = allAssets.Length;
            var progress = 0f;
            var needsRefresh = false; // 标记是否需要刷新
            
            foreach (var path in allAssets)
            {
                EditorUtility.DisplayProgressBar("处理GUID", $"处理 {path}", progress / total);
                progress += 1;

                var metaFilePath = path + ".meta";
                if (!File.Exists(metaFilePath))
                    continue;

                try
                {
                    var metaContent = File.ReadAllText(metaFilePath);
                    var currentGuid = GetGuidFromMeta(metaContent);
                    //使用Unity的api获取Unity的guid
                    var realGuid = AssetDatabase.AssetPathToGUID(path);
                    if (currentGuid != realGuid)
                    {
                        var newMetaContent = ReplaceGuidInMeta(metaContent, realGuid);
                        File.WriteAllText(metaFilePath, newMetaContent);

                        needsRefresh = true;
                        Debug.Log($"文件: {path}\n原GUID: {currentGuid}\n新GUID: {realGuid}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"处理 {path} 时出错: {ex.Message}");
                }
            }

            EditorUtility.ClearProgressBar();
            // 所有文件处理完成后统一刷新
            if (needsRefresh)
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Debug.Log("所有GUID已更新，数据库已刷新。");
            }
            else
            {
                Debug.Log("所有GUID已正确，无需刷新。");
            }
        }

        /// <summary>
        /// 使用正则表达式提取当前meta文件里面的GUID
        /// </summary>
        /// <param name="metaContent">文件内容</param>
        /// <returns></returns>
        private static string GetGuidFromMeta(string metaContent)
        {
            var match = Regex.Match(metaContent, @"^guid:\s*(\S+)", RegexOptions.Multiline);
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        /// <summary>
        /// 使用正则表达式精准替换GUID
        /// </summary>
        /// <param name="metaContent">文件内容</param>
        /// <param name="newGuid">Unity的guid</param>
        /// <returns></returns>
        private static string ReplaceGuidInMeta(string metaContent, string newGuid)
        {
            return Regex.Replace(metaContent, @"^guid:\s*\S+", "guid: " + newGuid, RegexOptions.Multiline);
        }
    }
}

