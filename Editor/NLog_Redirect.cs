/*
 * Original By: Fox.Huang Huang Wenye, Date:2018.12.18
 */
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Gitmanik.BaseCode.Editor
{
    public class NLog_Redirect
    {
        // Handle the callback function opened by the asset
        [OnOpenAsset(0)]
        static bool OnOpenAsset(int instance, int line)
        {
            // Custom function, used to get the stacktrace in the log, defined later.
            string stack_trace = GetStackTrace();
            // Use stacktrace to locate whether it is our custom log. My log has special text [FoxLog], which is well recognized.
            if (!string.IsNullOrEmpty(stack_trace)) // can customize the label to be added here; the original code is confusing and does not need to be modified, you need to locate it yourself;
            {
                string strLower = stack_trace.ToLower();
                if (strLower.StartsWith("<i>"))
                {
                    Match matches = Regex.Match(stack_trace, @"\(at(.+)\)", RegexOptions.IgnoreCase);
                    string pathline = "";
                    if (matches.Success)
                    {
                        matches = matches.NextMatch(); // Raise another layer up to enter;
                        if (matches.Success)
                        {
                            pathline = matches.Groups[1].Value;
                            pathline = pathline.Replace(" ", "");

                            int split_index = pathline.LastIndexOf(":");
                            string path = pathline.Substring(0, split_index);
                            line = Convert.ToInt32(pathline.Substring(split_index + 1));
                            string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                            fullpath = fullpath + path;
                            string strPath = fullpath.Replace('/', '\\');
                            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(strPath, line);
                        }
                        else
                        {
                            Debug.LogError("DebugCodeLocation OnOpenAsset, Error StackTrace");
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        static string GetStackTrace()
        {
            // Find the assembly of UnityEditor.EditorWindow
            var assembly_unity_editor = Assembly.GetAssembly(typeof(EditorWindow));
            if (assembly_unity_editor == null) return null;

            // Find the class UnityEditor.ConsoleWindow
            var type_console_window = assembly_unity_editor.GetType("UnityEditor.ConsoleWindow");
            if (type_console_window == null) return null;
            // Find the member ms_ConsoleWindow in UnityEditor.ConsoleWindow
            var field_console_window = type_console_window.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field_console_window == null) return null;

            // Get the value of ms_ConsoleWindow

            var instance_console_window = field_console_window.GetValue(null);
            if (instance_console_window == null) return null;


            // If the focus window of the console window, get the stacktrace

            if ((object)EditorWindow.focusedWindow == instance_console_window)
            {

                // Get the class ListViewState through the assembly
                var type_list_view_state = assembly_unity_editor.GetType("UnityEditor.ListViewState");
                if (type_list_view_state == null) return null;

                // Find the member m_ListView in the class UnityEditor.ConsoleWindow
                var field_list_view = type_console_window.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_list_view == null) return null;

                // Get the value of m_ListView
                var value_list_view = field_list_view.GetValue(instance_console_window);
                if (value_list_view == null) return null;

                // Find the member m_ActiveText in the class UnityEditor.ConsoleWindow
                var field_active_text = type_console_window.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field_active_text == null) return null;

                // Get the value of m_ActiveText, is the stacktrace we need

                string value_active_text = field_active_text.GetValue(instance_console_window).ToString();
                return value_active_text;
            }
            return null;
        }
    }
}