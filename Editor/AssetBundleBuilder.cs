using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace StorytellerStudio
{
    public class AssetBundleBuilder
    {
        public class MyBuildTarget
        {
            public string name;
            public BuildTarget buildTarget;
        }
        
        public static void ExportAsset(string assetName, string assetPath)
        {
            MyBuildTarget[] buildTargets = new[]
            {
                new MyBuildTarget { name = "mac", buildTarget = BuildTarget.StandaloneOSX },
                new MyBuildTarget { name = "ios", buildTarget = BuildTarget.iOS },
                new MyBuildTarget { name = "android", buildTarget = BuildTarget.Android }
            };
            
            AssetBundleBuild build = new AssetBundleBuild
            {
                assetBundleName = assetName,
                assetNames = new[] { assetPath }
            };

            foreach (var buildTarget in buildTargets)
            {
                string assetBundleDirectory = "Temp/" + assetName + "/" + assetName + "/" + buildTarget.name;
                if (!Directory.Exists(assetBundleDirectory))
                {
                    Directory.CreateDirectory(assetBundleDirectory);
                }

                BuildPipeline.BuildAssetBundles(
                    assetBundleDirectory,
                    new[] { build },
                    BuildAssetBundleOptions.UseContentHash,
                    buildTarget.buildTarget);
                
                File.Delete(assetBundleDirectory + "/" + buildTarget.name);
                File.Delete(assetBundleDirectory + "/" + buildTarget.name + ".manifest");
            }
        }
    }
}