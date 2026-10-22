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

using System;
using UnityEditor;
using UnityEngine;
using VRCTools.Common.Attributes;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Utils;

namespace VRCTools.World.Editor.Values.Sources
{
  public abstract class AbstractSourceEditor : AbstractCustomUdonEditor
  {
    protected SerializedProperty _output;
    protected SerializedProperty _localOutput;
    protected SerializedProperty _synchronizedOutput;

    [MustCallBase]
    protected virtual void OnEnable()
    {
      this._output = this.serializedObject.FindProperty("output");
      this._localOutput = this.serializedObject.FindProperty("localOutput");
      this._synchronizedOutput = this.serializedObject.FindProperty("synchronizedOutput");
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Output"), this._output, this._localOutput,
        this._synchronizedOutput, "Output is not set - Source will not run");
    }
  }
}
