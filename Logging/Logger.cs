using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Gitmanik.Logging
{
    public static class Log
    {
        public static LogLevel MinimumLevel = LogLevel.DEBUG;
        public static bool EnableFile = false;
        public static string FileName = "GitmanikLogger.log";

        public static string FilePath = Application.persistentDataPath;

        public static void Debug(string message) => Common(LogLevel.DEBUG, message);
        public static void Info(string message) => Common(LogLevel.INFO, message);
        public static void Warn(string message) => Common(LogLevel.WARN, message);
        public static void Error(string message) => Common(LogLevel.ERROR, message);
        public static void Fatal(string message) => Common(LogLevel.FATAL, message);

        [OnOpenAsset(0)]
        private static bool Redirect(int _, int __) // Inspired by Fox.Huang Huang Wenye
        {
            string stack = GetStackTrace();
            if (stack == null || !stack.StartsWith("<b>"))
                return false;

            Match matches = Regex.Match(stack, @"\(at(.+)\)");
            if (matches.Success)
            {
                matches = matches.NextMatch().NextMatch(); // Ignore Log's stack.
                if (matches.Success)
                {
                    string internalPathWithLine = matches.Groups[1].Value.Trim();
                    int separatorIndex = internalPathWithLine.LastIndexOf(":");
                    string internalPath = internalPathWithLine.Substring(0, separatorIndex);
                    int fileLine = Convert.ToInt32(internalPathWithLine.Substring(separatorIndex + 1));
                    string filePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets")) + internalPath;

                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(filePath.Replace('/', '\\'), fileLine);
                }
            }
            return true;
        }

        static string GetStackTrace() // Inspired by Fox.Huang Huang Wenye
        {
            Assembly editorWindowAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            if (editorWindowAssembly == null) return null;

            Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null) return null;

            FieldInfo consoleWindowField = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (consoleWindowField == null) return null;

            object consoleWindowInstance = consoleWindowField.GetValue(null);
            if (consoleWindowInstance == null) return null;

            if ((object)EditorWindow.focusedWindow != consoleWindowInstance)
                return null;

            var activeTextField = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (activeTextField == null) return null;

            return activeTextField.GetValue(consoleWindowInstance).ToString();
        }

        private static void Common(LogLevel level, string message)
        {
            if (level < MinimumLevel)
                return;

            StackFrame stack = new StackTrace(true).GetFrame(2);
            MethodBase method = stack.GetMethod();
            string info = $"{method.ReflectedType.Name}:{stack.GetFileLineNumber()}.{method.Name}";
            UnityEngine.Debug.Log($"<b>{info}</b>: {message}");
            if (EnableFile)
                using (StreamWriter sw = File.AppendText(Path.Combine(FilePath, FileName)))
                    sw.WriteLine($"{info}: {message}");

        }

        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR,
            FATAL
        }
    }
}