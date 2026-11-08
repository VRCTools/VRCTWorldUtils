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
using UnityEngine.UIElements;

namespace VRCTools.World.Editor.Values.Debugging
{
  /// <summary>
  /// Displays the values, converters and related components within the scene in a graph.
  /// </summary>
  public class ValueGraphWindow : EditorWindow
  {
    private ValueGraphView _graphView;

    private bool _dirty;

    /// <summary>
    /// Opens the editor window.
    /// </summary>
    [MenuItem("Tools/VRCTools/Value Graph")]
    private static void Open() => GetWindow<ValueGraphWindow>("Value Graph");

    private void OnEnable()
    {
      this._graphView = new ValueGraphView();
      this._graphView.StretchToParentSize();
      this.rootVisualElement.Add(this._graphView);

      ObjectChangeEvents.changesPublished += this.OnChangesPublished;
      this.Rebuild();
    }

    private void OnBecameVisible()
    {
      // force rebuild once we become visible
      this.QueueRebuild();
    }

    private void OnHierarchyChange()
    {
      // rebuild the queue if the hierarchy changes while we're visible - otherwise Unity will most likely not invoke
      // this function
      this.QueueRebuild();
    }

    private void OnChangesPublished(ref ObjectChangeEventStream eventStream)
    {
      this.QueueRebuild();
    }

    private void OnDisable()
    {
      ObjectChangeEvents.changesPublished -= this.OnChangesPublished;

      EditorApplication.delayCall -= this.ProcessQueuedRebuild;
      this._dirty = false;
    }

    private void QueueRebuild()
    {
      if (this._dirty || Application.isPlaying)
      {
        return;
      }

      this._dirty = true;
      EditorApplication.delayCall += this.ProcessQueuedRebuild;
    }

    private void ProcessQueuedRebuild()
    {
      this._dirty = false;
      this.Rebuild();
    }

    /// <summary>
    /// Forces a rebuild of the value graph.
    /// </summary>
    private void Rebuild()
    {
      var components = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
      this._graphView.Rebuild(components);
    }
  }
}
