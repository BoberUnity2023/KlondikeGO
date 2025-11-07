using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using System.IO;
using BloomLines.Assets;

public class CustomBuildConfigurator : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;    

    private string _originalDefines = "DOTWEEN;TextMeshPro";

    // Когда билд начинается, выставляем все данные
    public void OnPreprocessBuild(BuildReport report)
    {
        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(report.summary.platform));

        string currentDefine = GetCurrentBuildDefine();

        if (string.IsNullOrEmpty(currentDefine))
        {
            Debug.LogError("Missing BuildData Platform Define");
            return;
        }

        // Выставляем нужный WebGLTemplate
        if (report.summary.platform == BuildTarget.WebGL)
        {
            var buildData = Resources.Load<BuildData>("BuildData");

            switch (buildData.BuildPlatform)
            {
                case BuildPlatform.Yandex:
                    PlayerSettings.WebGL.template = $"PROJECT:YandexGames";
                    break;
                case BuildPlatform.VK:
                    PlayerSettings.WebGL.template = $"PROJECT:VK";
                    break;
                case BuildPlatform.OK:
                    PlayerSettings.WebGL.template = $"PROJECT:OK";
                    break;
                case BuildPlatform.CrazyGames:
                    PlayerSettings.WebGL.template = $"PROJECT:CrazyGames";
                    break;
                case BuildPlatform.GD:
                    PlayerSettings.WebGL.template = $"PROJECT:GD";
                    break;
                case BuildPlatform.Poki:
                    PlayerSettings.WebGL.template = $"PROJECT:Poki";
                    break;
            }
        }

        PlayerSettings.SetScriptingDefineSymbols(namedTarget, currentDefine);

        // Отключаем компиляцию vkBridge.jslib если билд не под VK
        {
            bool enabled = currentDefine.Contains("VK");
            string jslibPath = "Assets/Plugins/WebGL/vkBridge.jslib";
            SetLibrary(enabled, jslibPath);
        }

        // Отключаем компиляцию crazySDK.jslib если билд не под CrazyGames
        {
            bool enabled = currentDefine.Contains("CRAZY_GAMES");
            string jslibPath = "Assets/Plugins/WebGL/crazySDK.jslib";
            SetLibrary(enabled, jslibPath);
        }

        // Отключаем компиляцию GDPlugin.jslib если билд не под GameDistribution
        {
            bool enabled = currentDefine.Contains("GD");
            string jslibPath = "Assets/Plugins/WebGL/GDPlugin.jslib";
            SetLibrary(enabled, jslibPath);
        }

        // Отключаем компиляцию PokiSDKBridge.jslib если билд не под Poki
        {
            bool enabled = currentDefine.Contains("Poki");
            string jslibPath = "Assets/Plugins/WebGL/PokiSDKBridge.jslib";
            SetLibrary(enabled, jslibPath);
        }

        Debug.Log("Build Define Set: " + currentDefine);
    }

    // Когда билд окончился возвращаем все данные
    public void OnPostprocessBuild(BuildReport report)
    {
        var defines = GetAllBuildDefine();
        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(report.summary.platform));
        PlayerSettings.SetScriptingDefineSymbols(namedTarget, defines);

        Debug.Log("Build Define Revert Changes: " + defines);
    }

    // Берем все DefineSymbols
    private string GetAllBuildDefine()
    {
        string result = string.Empty;

        for (int i = 0; i < System.Enum.GetValues(typeof(BuildPlatform)).Length; i++)
        {
            if (i == 0)
                result += GetBuildDefine((BuildPlatform)i);
            else
                result += $";{GetBuildDefine((BuildPlatform)i)}";
        }

        result += $";CONSOLE;{_originalDefines}";

        return result;
    }

    // Берем DefineSymbols от текущей выбранной платформы
    private string GetCurrentBuildDefine()
    {
        var buildData = Resources.Load<BuildData>("BuildData");
        var result = GetBuildDefine(buildData.BuildPlatform);

        result += $";{_originalDefines}";

        if (buildData.Console)
            result += $";CONSOLE";

        return result;
    }

    // Берем DefineSymbols от нужной платформы
    private string GetBuildDefine(BuildPlatform platform)
    {
        switch (platform)
        {
            case BuildPlatform.Yandex:
                return "YandexGamesPlatform_yg;Yandex;PLUGIN_YG_2;" +
                    "TMP_YG2;InterstitialAdv_yg;RewardedAdv_yg;" +
                    "StickyAdv_yg;EnvirData_yg;Authorization_yg;" +
                    "Storage_yg;Localization_yg;Leaderboards_yg;" +
                    "Payments_yg;OpenURL_yg;Review_yg;RU_YG2;NJSON_YG2";
            case BuildPlatform.VK:
                return "VK";
            case BuildPlatform.OK:
                return "OK;GAME_PUSH";
            case BuildPlatform.RuStore:
                return "RuStore";
            case BuildPlatform.CrazyGames:
                return "CRAZY_GAMES;GAME_PUSH";
            case BuildPlatform.GD:
                return "GD;GAME_PUSH";
            case BuildPlatform.Poki:
                return "Poki;GAME_PUSH";
            case BuildPlatform.GooglePlay:
                return "GooglePlay";
        }

        return string.Empty;
    }

    private void SetLibrary(bool enabled, string jslibPath)
    {
        if (!enabled && File.Exists(jslibPath))
        {
            File.Move(jslibPath, jslibPath + ".disabled");
        }
        else if (enabled && File.Exists(jslibPath + ".disabled"))
        {
            File.Move(jslibPath + ".disabled", jslibPath);
        }
    }
}
