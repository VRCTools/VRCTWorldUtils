// Copyright 2025 .start <https://dotstart.tv>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Linq;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Network;
using VRC.SDKBase;
using VRC.Udon;
using Assembly = System.Reflection.Assembly;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace VRCTools.World.Editor.Utils
{
  /// <summary>
  ///   Provides various simple functions for use within custom editor implementations.
  /// </summary>
  internal static class CustomEditorUtility
  {
    /// <summary>
    ///   Retrieves the package metadata for the calling assembly (if possible).
    /// </summary>
    private static PackageInfo LocalPackageInfo => PackageInfo.FindForAssembly(Assembly.GetExecutingAssembly());

    private static readonly string LogoPath = $"{LocalPackageInfo.assetPath}/Editor/Textures/logo.png";
    private static readonly string LightLogoPath = $"{LocalPackageInfo.assetPath}/Editor/Textures/logo-light.png";

    private static Texture2D _logo;

    private static Texture2D Logo
    {
      get
      {
        if (_logo != null)
        {
          return _logo;
        }

        _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorGUIUtility.isProSkin ? LogoPath : LightLogoPath);
        return _logo;
      }
    }

    internal static string PackageName
    {
      get
      {
        var package = LocalPackageInfo;
        return package == null ? "Unknown" : package.displayName;
      }
    }

    internal static string PackageVersion
    {
      get
      {
        var package = LocalPackageInfo;
        return package == null ? "dev" : package.version;
      }
    }

    internal static void ValidateIntBounds(SerializedProperty lowerBound, SerializedProperty upperBound)
    {
      if (lowerBound.intValue > upperBound.intValue) upperBound.intValue = lowerBound.intValue;
    }

    internal static void ValidateFloatBounds(SerializedProperty lowerBound, SerializedProperty upperBound)
    {
      if (lowerBound.floatValue > upperBound.floatValue) upperBound.floatValue = lowerBound.floatValue;
    }

    internal static void DrawLine(Color color)
    {
      var rect = EditorGUILayout.GetControlRect();
      EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1), color);
    }

    internal static void DrawPropertiesSideBySide(GUIContent label, params SerializedProperty[] properties)
    {
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.PrefixLabel(label);
      foreach (var property in properties) EditorGUILayout.PropertyField(property, GUIContent.none);

      EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    ///   Displays a warning when one or more Udon behaviors within a given game object makes use of a synchronization
    ///   mode other than manual.
    ///   This method is provided to make sure users do not arbitrarily combine components which were designed for manual
    ///   synchronization with others.
    /// </summary>
    /// <param name="gameObject"></param>
    internal static void DrawManualSynchronizationWarning(GameObject gameObject)
    {
      var behaviours = gameObject.GetComponents<Component>();
      if (behaviours.Length <= 1) return;

      {
        behaviours = (
          from b in behaviours
          where b is UdonBehaviour or UdonSharpBehaviour or VRCNetworkBehaviour
          select b
        ).ToArray();
      }

      var mismatch = false;
      foreach (var behaviour in behaviours)
      {
        Networking.SyncType syncMethod;
        if (behaviour is UdonBehaviour udonBehaviour)
        {
          syncMethod = udonBehaviour.SyncMethod;
        }
        else if (behaviour is UdonSharpBehaviour udonSharpBehaviour)
        {
          var backingBehaviour = UdonSharpEditorUtility.GetBackingUdonBehaviour(udonSharpBehaviour);
          syncMethod = backingBehaviour.SyncMethod;
        }
        else
        {
          // catch-all for SDK network objects
          syncMethod = Networking.SyncType.Continuous;
        }

        if (syncMethod != Networking.SyncType.Manual) mismatch = true;
      }

      if (!mismatch) return;

      EditorGUILayout.HelpBox(
        "This object contains multiple Udon behaviours with differing synchronization modes - This script " +
        "requires manual synchronization and should only be combined with scripts in manual mode",
        MessageType.Error);
    }

    public static void DrawSupporterLinks()
    {
      GUILayout.Space(20);
      DrawLine(Color.gray);

      EditorGUILayout.BeginHorizontal();
      {
        var iconRect = GUILayoutUtility.GetRect(20f, 20f, GUILayout.ExpandWidth(false));
        GUI.DrawTexture(iconRect, Logo, ScaleMode.ScaleToFit);

        EditorGUIUtility.AddCursorRect(iconRect, MouseCursor.Link);
        if (UnityEngine.Event.current.type == EventType.MouseUp &&
            iconRect.Contains(UnityEngine.Event.current.mousePosition))
        {
          Application.OpenURL("https://vrctools.io");
        }

        GUILayout.Label($"{PackageName} v{PackageVersion}");

        if (EditorGUILayout.LinkButton("Documentation")) Application.OpenURL("https://vrctools.io/docs/world-utils/");

        if (EditorGUILayout.LinkButton("Discord")) Application.OpenURL("https://discord.gg/FGrt9wv5CF");

        if (EditorGUILayout.LinkButton("Patreon")) Application.OpenURL("https://patreon.com/dotStart");
      }
      EditorGUILayout.EndHorizontal();
    }
  }
}
