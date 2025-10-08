// using System.IO;
// using UnityEngine;
// #if UNITY_IOS
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;
// #endif

// public class VKIOSBuildPostprocessor : MonoBehaviour
// {
// #if UNITY_IOS
//     [PostProcessBuildAttribute(1)]
//     public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
//     {
//         if (target == BuildTarget.iOS)
//         {
//             Debug.Log("Post Process Build callback, adding UnityFramework to project...");
//             var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
//             var project = new PBXProject();
//             project.ReadFromString(File.ReadAllText(projectPath));
//             project.AddFrameworkToProject(project.GetUnityMainTargetGuid(), "UnityFramework.framework", false);
//             project.WriteToFile(projectPath);
//         }
//     }
// #endif
// }
