using System;
using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Utils;

namespace VRCTools.World.Editor.Utils {
  [CustomEditor(typeof(EventBroadcaster))]
  public class EventBroadcasterEditor : AbstractCustomUdonEditor {
    private SerializedProperty _behaviours;
    private SerializedProperty _eventNames;

    private SerializedProperty _delayMode;
    private SerializedProperty _delayTiming;
    private SerializedProperty _delayFrames;
    private SerializedProperty _delaySeconds;

    private MultiPropertyList _targetEvents;

    private void OnEnable() {
      this._behaviours = this.serializedObject.FindProperty(nameof(EventBroadcaster.behaviours));
      this._eventNames = this.serializedObject.FindProperty(nameof(EventBroadcaster.eventNames));

      this._delayMode = this.serializedObject.FindProperty(nameof(EventBroadcaster.delayMode));
      this._delayTiming = this.serializedObject.FindProperty(nameof(EventBroadcaster.delayTiming));
      this._delayFrames = this.serializedObject.FindProperty(nameof(EventBroadcaster.delayFrames));
      this._delaySeconds = this.serializedObject.FindProperty(nameof(EventBroadcaster.delaySeconds));

      this._targetEvents = new MultiPropertyList(
        new GUIContent("Events"),
        this.serializedObject,
        "Behaviour",
        this._behaviours,
        new[] { "Event Name" },
        new[] { this._eventNames });
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
      this._targetEvents.DoLayout();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Delay", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._delayMode);

      var delayMode = (DelayMode)this._delayMode.enumValueIndex;
      switch (delayMode) {
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
