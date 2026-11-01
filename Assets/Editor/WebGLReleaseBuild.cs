using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLReleaseBuild
{
    public static void Build()
    {
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray(),
            locationPathName = "Builds/WebGLReleaseBuild",
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });

        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new System.InvalidOperationException(
                $"WebGL release build failed: {report.summary.result}");
        }

        Debug.Log($"WebGL release build succeeded: {report.summary.totalSize} bytes");
    }
}
