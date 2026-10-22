// Copyright 2026 .start <https://dotstart.tv>
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

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VRCTools.World.Editor
{
  /// <summary>
  /// Ensures that a script define named <code>VRCTOOLS_WORLDUTILS</code> is present within the current build target.
  ///
  /// This functionality is not actually used within this package but provided so that third party package maintainers
  /// can include integrations with this package directly inside their packages.
  ///
  /// If you wish to make use of this functionality, simply create a dedicated directory in which the functionality
  /// will live, add a dedicated assembly definition inside of it and ensure it has a define constraint for
  /// <code>VRCTOOLS_WORLDUTILS</code>. Scripts within this new directory will only compile when this package is
  /// present too.
  ///
  /// This implementation is loosely based on the equivalent logic inside the AudioLink package.
  /// </summary>
  [InitializeOnLoad]
  internal static class ScriptDefinitionLoader
  {
    private const string PackageDefinition = "VRCTOOLS_WORLDUTILS";

    static ScriptDefinitionLoader()
    {
      AddDefineIfMissing(EditorUserBuildSettings.selectedBuildTargetGroup, PackageDefinition);
    }

    private static void AddDefineIfMissing(BuildTargetGroup buildGroup, params string[] newDefines)
    {
#if UNITY_6000_0_OR_NEWER
            var profile = BuildProfile.GetActiveBuildProfile();
            var defines = profile != null
                ? profile.scriptingDefines
                : PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildGroup)).Split(';');
#else
      var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup).Split(';');
#endif

      var definesChanged = false;
      var defineSet = new HashSet<string>(defines);
      foreach (var newDefine in newDefines)
      {
        definesChanged |= defineSet.Add(newDefine);
      }

      if (definesChanged)
      {
#if UNITY_6000_0_OR_NEWER
                if (profile != null)
                {
                    profile.scriptingDefines = defineSet.ToArray();
                }
                else
                {
                    PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(buildGroup), defines);
                }
#else
        var defineString = string.Join(";", defineSet.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, defineString);
#endif
      }
    }
  }
}
