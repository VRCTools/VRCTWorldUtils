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

using UnityEditor;
using UnityEngine;

namespace VRCTools.World.Editor.Abstractions {
  public abstract class AbstractUdonFlagsPropertyDrawer : PropertyDrawer {
    protected abstract string Name { get; }

    protected abstract string[] EnumNames { get; }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      if (property.propertyType != SerializedPropertyType.Integer) {
        EditorGUI.LabelField(position, label.text, $"Use {this.Name} with int");
        return;
      }

      var currentValue = property.intValue;

      var options = new GUIContent[this.EnumNames.Length + 2];
      var optionValues = new int[this.EnumNames.Length + 2];

      var accumulator = 0;
      options[0] = new GUIContent("None");
      optionValues[0] = 0;
      for (var i = 0; i < this.EnumNames.Length; i++) {
        options[i + 1] = new GUIContent(this.EnumNames[i]);
        optionValues[i + 1] = 1 << i;
        accumulator |= 1 << i;
      }

      options[options.Length - 1] = new GUIContent("All");
      optionValues[options.Length - 1] = accumulator;

      // TODO: This doesn't fully cover the functionality of EnumPopup at the moment - it does not seem like this
      //       functionality is properly exposed to us right now - investigate when we actually need this functionality
      EditorGUI.IntPopup(position, property, options, optionValues);
    }
  }
}
