using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// This class will be executed before the build process
class AutoVersionIncrementer : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        // Get the current version string
        string currentVersion = PlayerSettings.bundleVersion;

        // Split the version string into major, minor, and patch components
        string[] versionParts = currentVersion.Split('.');

        // Assuming a format like "major.minor.patch"
        if (versionParts.Length == 3)
        {
            int major = int.Parse(versionParts[0]);
            int minor = int.Parse(versionParts[1]);
            int patch = int.Parse(versionParts[2]);

            // Increment the patch version
            patch++;

            // Construct the new version string
            string newVersion = $"{major}.{minor}.{patch}";
            PlayerSettings.bundleVersion = newVersion;
            Debug.Log($"Automatically updated application version to: {newVersion}");
        }
        else
        {
            Debug.LogWarning("Application version format not recognized for automatic increment. Please use 'major.minor.patch'.");
        }
    }
}
