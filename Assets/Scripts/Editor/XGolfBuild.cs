using UnityEditor;
using System.IO;
using UnityEngine;
using XGolf.Service;
using System.Collections.Generic;
using System.Linq;

namespace XGolf.Build
{
    public class XGolfBuild
    {
        private static readonly string[] SCENES = new string[] {
            "Assets/Scenes/XGolfStart.unity",
            "Assets/Scenes/LightReviser.unity",
        };
        private static readonly string UTIL_PATH = "Utilities/";
        private static readonly string BUILT_PLUGIN_PATH = "XGolf_Data/Plugins/x86_64/";
        private static readonly string BUILT_EXEC = "XGolf.exe";
        private static readonly string INSTALLER_TEMPLATE = "installer.iss.template";
        private static readonly string OUTPUT_INSTALLER_SCRIPT_NAME = "installer.iss";
        private static readonly string BUILD_PATH_VARIABLE_LINE_START_WITH = "#define MyBuildFilePath";
        private static readonly string INSTALLER_OUTPUT_PATH_VARIABLE_LINE_START_WITH = "#define MyBuildOutputPath";
        private static readonly string APP_VERSION_VARIABLE_LINE_START_WITH = "#define MyAppVersion";

        [MenuItem("Build/Build XGolf")]
        public static void BuildGameWithPath()
        {
            string buildPath = EditorUtility.SaveFolderPanel("Choose Location to build", "", "");

            if (buildPath == "")
            {
                throw new System.Exception("Path is not set");
            }
            // Players.
            Build(buildPath);
        }

        public static void BuildGame()
        {
            WriteEnvFile();
            SetbundleVersion();
            string buildPath = Path.GetDirectoryName(Application.dataPath) + "/build";

            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }
            else
            {
                Directory.Delete(buildPath, true);
            }

            Build(buildPath);
        }
        private static void SetbundleVersion()
        {
            string version = System.Environment.GetEnvironmentVariable("VERSION");
            if (!string.IsNullOrEmpty(version))
            {
                PlayerSettings.bundleVersion = version;
            }
        }
        
        private static void WriteEnvFile()
        {
            // TODO: move this process out of this. Luancher should be responsible for it.(store json data in unity dataPath)
            EnvConfig config = ScriptableObject.CreateInstance<EnvConfig>();
            config.ENV = System.Environment.GetEnvironmentVariable("ENV");
            config.API_URL = System.Environment.GetEnvironmentVariable("API_URL");
            config.SIMULATOR_EMAIL = System.Environment.GetEnvironmentVariable("SIMULATOR_EMAIL");
            config.SIMULATOR_PASSWORD = System.Environment.GetEnvironmentVariable("SIMULATOR_PASSWORD");
            Debug.Log($"{config.API_URL} {config.SIMULATOR_EMAIL} {config.SIMULATOR_PASSWORD}");
            AssetDatabase.CreateAsset(config, "Assets/Resources/ENV.asset");
            AssetDatabase.SaveAssets();
        }

        private static void Build(string buildPath)
        {
            BuildPipeline.BuildPlayer(SCENES, buildPath + "/" + BUILT_EXEC, BuildTarget.StandaloneWindows64, BuildOptions.None);
            // CloneFiles(buildPath);
            WriteInstallerPath(buildPath);
            CloneInstallerScriptToBuilt(buildPath);
        }

        public static void WriteInstallerPath(string buildPath)
        {
            string[] allLines = File.ReadAllLines(INSTALLER_TEMPLATE);
            var output = new StreamWriter(OUTPUT_INSTALLER_SCRIPT_NAME);

            foreach (var line in allLines)
            {
                string newLine = line;
                if (line.StartsWith(BUILD_PATH_VARIABLE_LINE_START_WITH))
                {
                    newLine = BUILD_PATH_VARIABLE_LINE_START_WITH + " \"" + buildPath + "\"";
                }
                else if (line.StartsWith(INSTALLER_OUTPUT_PATH_VARIABLE_LINE_START_WITH))
                {
                    newLine = INSTALLER_OUTPUT_PATH_VARIABLE_LINE_START_WITH + " \"" + buildPath + "\"";
                }
                else if (line.StartsWith(APP_VERSION_VARIABLE_LINE_START_WITH))
                {
                    newLine = APP_VERSION_VARIABLE_LINE_START_WITH + " \"" + Application.version + "\"";
                }
                output.WriteLine(newLine);
            }
            output.Close();
        }


        private static void CloneInstallerScriptToBuilt(string buildPath)
        {
            string projectPath = Path.GetDirectoryName(Application.dataPath);
            File.Copy($"{projectPath}/{OUTPUT_INSTALLER_SCRIPT_NAME}", $"{buildPath}/{OUTPUT_INSTALLER_SCRIPT_NAME}", overwrite: true);
        }

    }
}