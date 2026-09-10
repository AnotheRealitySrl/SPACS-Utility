using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Virtuademy.SDK.Core.Editor
{
    public static class ScriptDefineSymbolsUtilities
    {
        public static bool HasScriptingDefineSymbol(string defineSymbol)
        {
            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(
                BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));

            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget)
                .Split(';').Select(d => d.Trim());

            return defineSymbols.Contains(defineSymbol);
        }

        public static void AddScriptingDefineSymbolToAllBuildTargetGroups(string defineSymbol)
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }
                NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';').Select(d => d.Trim()).ToList();

                if (!defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Add(defineSymbol);

                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Could not " + defineSymbol + " defines for build target: " + target + " group: " + group + " named build target: " + namedBuildTarget + " error: " + e);
                    }
                }
            }
        }

        public static void RemoveScriptingDefineSymbolFromAllBuildTargetGroups(string defineSymbol)
        {
            foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }
                NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(group);

                var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(';').Select(d => d.Trim()).ToList();

                if (defineSymbols.Contains(defineSymbol))
                {
                    defineSymbols.Remove(defineSymbol);

                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(";", defineSymbols.ToArray()));
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Could not remove " + defineSymbol + " defines for build target: " + target + " group: " + group + " named build target: " + namedBuildTarget + " error: " + e);
                    }
                }
            }
        }
    }
}
