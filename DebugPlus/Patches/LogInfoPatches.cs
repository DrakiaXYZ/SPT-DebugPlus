﻿using Aki.Reflection.Patching;
using HarmonyLib;
using System.Reflection;
using Aki.Reflection.Utils;
using UnityEngine;
using DebugPlus.Utils;
using DebugPlus.Config;


namespace DebugPlus.Patches
{
    /// <summary>
    /// UnityEngine.Debug.Log(string message)
    /// </summary>
    internal class LogPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Debug))
                .SingleCustom(m => m.Name == nameof(Debug.Log) 
                && m.GetParameters()[0].Name == "message"
                && m.GetParameters().Length == 1);
        }

        [PatchPostfix]
        public static void PatchPostfix(object message)
        {
            if (!DebugPlusConfig.UnityInfoLogging.Value) return;

            if (message.GetType() == typeof(string))
            {
                Plugin.Log.LogInfo(Format.FormatString((string)message));
                return;
            }
        }
    }

    /// <summary>
    /// UnityEngine.Debug.Log(string message, Object object)
    /// </summary>
    internal class LogObjPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Debug))
                .SingleCustom(m => m.Name == nameof(Debug.Log)
                && m.GetParameters()[0].Name == "message"
                && m.GetParameters().Length == 2);
        }

        [PatchPostfix]
        public static void PatchPostfix(object message, Object context)
        {
            if (!DebugPlusConfig.UnityInfoLogging.Value) return;

            if (message.GetType() == typeof(string) && context == null)
            {
                Plugin.Log.LogInfo(Format.FormatString((string)message));
                return;
            }

            Plugin.Log.LogInfo($"OBJECT: {message} : {context}");
        }
    }

    /// <summary>
    /// UnityEngine.Debug.LogFormat(string message, param object[] args)
    /// </summary>
    internal class LogFormatPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Debug))
                .SingleCustom(m => m.Name == nameof(Debug.LogFormat)
                && m.GetParameters()[0].Name == "format"
                && m.GetParameters().Length == 2);
        }

        [PatchPostfix]
        public static void PatchPostfix(string format, object[] args)
        {
            if (!DebugPlusConfig.UnityInfoLogging.Value) return;

            Plugin.Log.LogInfo(Format.FormatString(format, args));
        }
    }

    /// <summary>
    /// UnityEngine.Debug.LogFormat(Object context, string message, param object[] args)
    /// </summary>
    internal class LogFormatObjPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.GetDeclaredMethods(typeof(Debug))
                .SingleCustom(m => m.Name == nameof(Debug.LogFormat)
                && m.GetParameters()[0].Name == "context"
                && m.GetParameters().Length == 3);
        }

        [PatchPostfix]
        public static void PatchPostfix(Object context, string format, object[] args)
        {
            if (!DebugPlusConfig.UnityInfoLogging.Value) return;

            Plugin.Log.LogInfo($"GameObject: {context} \nMessage : {Format.FormatString(format, args)}");
        }
    }
}
