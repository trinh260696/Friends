using UnityEditor;
using UnityEngine;
using VKSdk;

public class BuildAssetBundle  {

#if UNITY_EDITOR

    [MenuItem("VKSdk/BuildAsset Bundle/Win")]
    static void BuildABsWin()
    {
        // SwitchPlatfform(RuntimePlatform.WindowsEditor);
        BuildPipeline.BuildAssetBundles("AssetBundles/Win", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("VKSdk/BuildAsset Bundle/WebGL")]
    static void BuildABsWeb()
    {
        // SwitchPlatfform(RuntimePlatform.WebGLPlayer);
        BuildPipeline.BuildAssetBundles("AssetBundles/WebGL", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

    [MenuItem("VKSdk/BuildAsset Bundle/WebGLMobile")]
    static void BuildABsWebMobile()
    {
        // SwitchPlatfform(RuntimePlatform.WebGLPlayer);
        BuildPipeline.BuildAssetBundles("AssetBundles/WebGLMobile", BuildAssetBundleOptions.None, BuildTarget.WebGL);
    }

    [MenuItem("VKSdk/BuildAsset Bundle/Android")]
    static void BuildABsAndroid()
    {
        // SwitchPlatfform(RuntimePlatform.Android);
        BuildPipeline.BuildAssetBundles("AssetBundles/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
    [MenuItem("VKSdk/BuildAsset Bundle/IOS")]
    static void BuildABsIOS()
    {
        // SwitchPlatfform(RuntimePlatform.IPhonePlayer);
        BuildPipeline.BuildAssetBundles("AssetBundles/IOS", BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    // static void SwitchPlatfform(RuntimePlatform platform)
    // {
    //     Debug.Log("platform " + platform);
    //     Debug.Log("Application.platform " + platform);
    //     if(Application.platform == platform)
    //         return;

    //     switch (platform)
    //     {
    //         case RuntimePlatform.Android:
    //             EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.Android);
    //             break;
    //         case RuntimePlatform.WindowsEditor:
    //             EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
    //             break;
    //         case RuntimePlatform.WebGLPlayer:
    //             EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.WebGL);
    //             break;
    //         case RuntimePlatform.IPhonePlayer:
    //             EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.iOS);
    //             break;
    //     }
    // }

    const string kSimulationMode = "VKSdk/Simulation Mode";

    [MenuItem(kSimulationMode)]
    public static void ToggleSimulationMode()
    {
        VKAssetBundleController.SimulateAssetBundleInEditor = !VKAssetBundleController.SimulateAssetBundleInEditor;
    }

    [MenuItem(kSimulationMode, true)]
    public static bool ToggleSimulationModeValidate()
    {
        Menu.SetChecked(kSimulationMode, VKAssetBundleController.SimulateAssetBundleInEditor);
        return true;
    }
#endif
}
