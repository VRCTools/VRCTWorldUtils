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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Utils;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Values.Applicators
{
  using Applicator = BooleanEventApplicator;

  [CustomEditor(typeof(Applicator))]
  public class BooleanEventApplicatorEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _source;
    private SerializedProperty _localValue;
    private SerializedProperty _synchronizedValue;

    private SerializedProperty _falseBehaviours;
    private SerializedProperty _falseEventNames;

    private SerializedProperty _trueBehaviours;
    private SerializedProperty _trueEventNames;

    private SerializedProperty _delayMode;
    private SerializedProperty _delayTiming;
    private SerializedProperty _delayFrames;
    private SerializedProperty _delaySeconds;

    private MultiPropertyList _falseEventList;
    private MultiPropertyList _trueEventList;

    private void OnEnable()
    {
      this._source = this.serializedObject.FindProperty(nameof(Applicator.source));
      this._localValue
        = this.serializedObject.FindProperty(nameof(Applicator.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(Applicator.synchronizedValue));

      this._falseBehaviours = this.serializedObject.FindProperty(nameof(Applicator.falseBehaviours));
      this._falseEventNames = this.serializedObject.FindProperty(nameof(Applicator.falseEventNames));

      this._trueBehaviours = this.serializedObject.FindProperty(nameof(Applicator.trueBehaviours));
      this._trueEventNames = this.serializedObject.FindProperty(nameof(Applicator.trueEventNames));

      this._delayMode = this.serializedObject.FindProperty(nameof(Applicator.delayMode));
      this._delayTiming = this.serializedObject.FindProperty(nameof(Applicator.delayTiming));
      this._delayFrames = this.serializedObject.FindProperty(nameof(Applicator.delayFrames));
      this._delaySeconds = this.serializedObject.FindProperty(nameof(Applicator.delaySeconds));

      this._falseEventList = new MultiPropertyList(
        new GUIContent("False State Events"),
        this.serializedObject,
        "Behaviour",
        this._falseBehaviours,
        new[] { "Event Name" },
        new[] { this._falseEventNames });
      this._trueEventList = new MultiPropertyList(
        new GUIContent("True State Events"),
        this.serializedObject,
        "Behaviour",
        this._trueBehaviours,
        new[] { "Event Name" },
        new[] { this._trueEventNames });
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
      this._falseEventList.DoLayout();
      this._trueEventList.DoLayout();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Delay", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._delayMode);

      var delayMode = (DelayMode)this._delayMode.enumValueIndex;
      switch (delayMode)
      {
        case DelayMode.NONE: return;
        case DelayMode.FRAMES:
          EditorGUILayout.PropertyField(this._delayFrames);
          break;
        case DelayMode.SECONDS:
          EditorGUILayout.PropertyField(this._delaySeconds);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      EditorGUILayout.PropertyField(this._delayTiming);
    }
  }
}
