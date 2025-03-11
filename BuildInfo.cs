using System;
using System.IO;
using UnityEngine;

namespace Gitmanik.BaseCode
{
    public class BuildInfo
    {
        public static readonly string FilePath = "Assets/Resources/BuildInfo.txt";
        private static BuildInfo _instance;
        public static BuildInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BuildInfo();
                }
                return _instance;
            }
        }

        public string BuildDate { get; private set; }

        protected BuildInfo()
        {
#if UNITY_EDITOR
            if (!File.Exists(FilePath))
            {
                File.Create(FilePath);
            }
            BuildDate = "EditorBuild";
            return;
#endif
            var txt = (Resources.Load("BuildInfo") as TextAsset);
            BuildDate = txt.text.Trim();

        }

    }

#if UNITY_EDITOR
    public class BuildInfoSetter : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            using (BinaryWriter Writer = new BinaryWriter(File.Open(BuildInfo.FilePath, FileMode.OpenOrCreate | FileMode.Truncate)))
            {
                Writer.Write(DateTime.Now.ToString("dd/MM/yyyy"));
            }
        }
    }
#endif
}
