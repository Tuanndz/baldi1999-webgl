using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class BuildScript
{
    public static void BuildWebGL()
    {
        List<string> scenePaths = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && File.Exists(scene.path))
                scenePaths.Add(scene.path);
        }
        
        if (scenePaths.Count == 0)
        {
            foreach (string file in Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories))
            {
                scenePaths.Add(file.Replace("\\", "/"));
            }
        }

        PlayerSettings.WebGL.template = "APPLICATION:Default";
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.FullWithStacktrace;
        PlayerSettings.WebGL.dataCaching = false;

        string outputDir = "build/WebGL";
        if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = scenePaths.ToArray(),
            locationPathName = outputDir,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        Debug.Log("===> Starting WebGL Build with " + scenePaths.Count + " scenes...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("===> BUILD SUCCEEDED! Total size: " + summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("===> BUILD FAILED: " + summary.result);
            throw new System.Exception("WebGL build failed: " + summary.result);
        }
    }
}
