using System;
using System.IO;
using UnityEngine;

namespace Gitmanik.BaseCode
{
    public class BuildInfo
    {
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

        public DateTime BuildTime { get; private set; }
        public string BuildDate { get; private set; }

        protected BuildInfo()
        {
            var txt = (Resources.Load("BuildInfo") as TextAsset);
            BuildDate = txt.text.Trim();

#if UNITY_EDITOR
            BuildDate = "EditorBuild";
#endif
        }

    }

#if UNITY_EDITOR
    public class BuildInfoSetter : UnityEditor.Build.IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            using (BinaryWriter Writer = new BinaryWriter(File.Open("Assets/Resources/BuildInfo.txt", FileMode.OpenOrCreate | FileMode.Truncate)))
            {
                Writer.Write(DateTime.Now.ToString("dd/MM/yyyy"));
            }
        }
    }
#endif
}
