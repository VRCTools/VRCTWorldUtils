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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using VRCTools.World.Attributes;
using VRCTools.World.Values;

namespace VRCTools.World.Editor.Values.Debugging
{
  public class ValueGraphView : GraphView
  {
    private const float ColumnSpacing = 80;
    private const float RowSpacing = 20;
    private const float GroupSpacing = 60;
    private const int FramePadding = 30;

    private readonly Dictionary<MonoBehaviour, ValueGraphNode> _graphLookupDictionary = new();

    private bool _graphFramed;

    public ValueGraphView()
    {
      // TODO: Use package reference from editor utilities?
      var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/io.vrctools.world/Editor/Scripts/Values/Debugging/ValueGraph.uss");
      if (styleSheet != null)
      {
        this.styleSheets.Add(styleSheet);
      }

      this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
      this.AddManipulator(new ContentDragger());
      this.AddManipulator(new SelectionDragger());
      this.Insert(0, new GridBackground());

      // prevent deletion and modification since we are only displaying values at the moment
      this.deleteSelection = (_, _) => { };
      this.graphViewChanged = change =>
      {
        change.elementsToRemove?.Clear();
        return change;
      };
    }

    // prevent new connections from being made as we are only displaying at the moment
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => new();

    /// <summary>
    /// Clears all graph elements from this view.
    /// </summary>
    private void ClearGraph()
    {
      foreach (var element in this.graphElements.ToList())
      {
        this.RemoveElement(element);
      }

      this._graphLookupDictionary.Clear();
    }

    /// <summary>
    /// Re-Build the value graph from a set of known annotated components.
    /// </summary>
    /// <param name="components">a list of components</param>
    public void Rebuild(IEnumerable<MonoBehaviour> components)
    {
      this.ClearGraph();

      foreach (var component in components)
      {
        this.AddComponent(component);
      }

      foreach (var node in this._graphLookupDictionary.Values)
      {
        // value nodes are "inert" (e.g. do not hold references themselves) and can thus be ignored
        if (node.IsValue)
        {
          continue;
        }

        this.ConnectComponentReferences(node);
      }

      this.RebuildLayout();
    }

    private void AddComponent(MonoBehaviour component)
    {
      var kind = component.GetType().GetCustomAttribute<ValueComponentKindAttribute>();
      if (kind == null)
      {
        return;
      }

      var node = new ValueGraphNode(component, kind.kind);
      this._graphLookupDictionary.Add(component, node);
      this.AddElement(node);
    }

    private void ConnectComponentReferences(ValueGraphNode node)
    {
      var fields = node.Component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

      foreach (var field in fields)
      {
        this.ConnectComponentReference(node, field);
      }
    }

    private void ConnectComponentReference(ValueGraphNode node, FieldInfo field)
    {
      var fieldBinding = field.GetCustomAttribute<ValueBindingAttribute>();

      if (fieldBinding == null)
      {
        return;
      }

      var references = this.FindReferences(node, field, fieldBinding);
      var localPort = node[field.Name];

      foreach (var reference in references)
      {
        if (reference == null || !this._graphLookupDictionary.TryGetValue(reference, out var remoteNode))
        {
          continue;
        }

        Port remotePort;
        if (fieldBinding.bindingType == ValueBindingType.Input)
        {
          remotePort = remoteNode.OutputPort;
        }
        else
        {
          // this includes bidirectional since we don't have a solid way to represent this right now
          remotePort = remoteNode.InputPort;
        }

        if (remotePort == null)
        {
          continue;
        }

        var edge = remotePort.ConnectTo(localPort);
        this.AddElement(edge);
      }
    }

    private IEnumerable<MonoBehaviour> FindReferences(ValueGraphNode node, FieldInfo field,
      ValueBindingAttribute bindingAttribute)
    {
      var backingField = bindingAttribute.GetBackingField(node.Component, field);
      if (backingField == null)
      {
        return Enumerable.Empty<MonoBehaviour>();
      }

      return backingField.GetValue(node.Component) switch
      {
        Array array => array.Cast<MonoBehaviour>(),
        MonoBehaviour reference => new[] { reference },
        _ => Enumerable.Empty<MonoBehaviour>()
      };
    }

    private void RebuildLayout()
    {
      var nodes = this._graphLookupDictionary.Values.ToList();

      // sometimes this rebuild step runs a little early so we might need to delay it when the nodes have not been
      // rebuilt quite yet
      if (nodes.Any(node => float.IsNaN(node.layout.width)))
      {
        this.schedule.Execute(this.RebuildLayout);
        return;
      }

      var successors = nodes.ToDictionary(node => node, _ => new List<ValueGraphNode>());
      var predecessors = nodes.ToDictionary(node => node, _ => new List<ValueGraphNode>());

      foreach (var edge in this.edges.ToList())
      {
        var from = (ValueGraphNode)edge.output.node;
        var to = (ValueGraphNode)edge.input.node;

        successors[from].Add(to);
        predecessors[to].Add(from);
      }

      var columns = this.ComputeColumns(nodes, predecessors);

      var columnCount = columns.Values.DefaultIfEmpty(-1).Max() + 1;
      var columnOffsets = new float[columnCount];
      for (var i = 1; i < columnCount; ++i)
      {
        var previousWidth = nodes
          .Where(node => columns[node] == i - 1)
          .Select(node => node.layout.width)
          .DefaultIfEmpty(0)
          .Max();

        columnOffsets[i] = columnOffsets[i - 1] + previousWidth + ColumnSpacing;
      }

      var top = 0f;
      foreach (var group in FindGroups(nodes, successors, predecessors))
      {
        top = LayoutGroup(group, columns, columnOffsets, predecessors, top) + GroupSpacing;
      }

      if (!this._graphFramed)
      {
        var width = nodes
          .Select(node => columnOffsets[columns[node]] + node.layout.width)
          .DefaultIfEmpty(0)
          .Max();

        var height = Math.Max(0, top - GroupSpacing);
        this._graphFramed = this.FrameRect(new Rect(0, 0, width, height));
      }
    }

    private Dictionary<ValueGraphNode, int> ComputeColumns(IEnumerable<ValueGraphNode> nodes,
      Dictionary<ValueGraphNode, List<ValueGraphNode>> predecessors)
    {
      var columns = new Dictionary<ValueGraphNode, int>();
      foreach (var node in nodes)
      {
        Visit(node);
      }
      return columns;

      int Visit(ValueGraphNode node)
      {
        if (columns.TryGetValue(node, out var column))
        {
          return column;
        }

        columns[node] = 0;
        column = predecessors[node]
          .Select(predecessor => Visit(predecessor) + 1)
          .DefaultIfEmpty(0)
          .Max();

        columns[node] = column;
        return column;
      }
    }

    private static IEnumerable<List<ValueGraphNode>> FindGroups(IEnumerable<ValueGraphNode> nodes,
      Dictionary<ValueGraphNode, List<ValueGraphNode>> successors,
      Dictionary<ValueGraphNode, List<ValueGraphNode>> predecessors)
    {
      var visited = new HashSet<ValueGraphNode>();
      foreach (var start in nodes)
      {
        if (!visited.Add(start))
        {
          continue;
        }

        var group = new List<ValueGraphNode>();
        var queue = new Queue<ValueGraphNode>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
          var node = queue.Dequeue();
          group.Add(node);

          foreach (var neighbor in successors[node].Concat(predecessors[node]))
          {
            if (!visited.Add(neighbor))
            {
              continue;
            }

            queue.Enqueue(neighbor);
          }
        }

        yield return group;
      }
    }

    private static float LayoutGroup(List<ValueGraphNode> group, Dictionary<ValueGraphNode, int> columns,
      float[] columnOffsets, Dictionary<ValueGraphNode, List<ValueGraphNode>> predecessors, float top)
    {
      var rows = new Dictionary<ValueGraphNode, int>();

      foreach (var column in group.GroupBy(node => columns[node]).OrderBy(column => column.Key))
      {
        var desiredRows = column.ToDictionary(node => node,
          node => (int)Math.Round(
            predecessors[node]
              .Where(rows.ContainsKey)
              .Select(predecessor => (double)rows[predecessor])
              .DefaultIfEmpty(0)
              .Average()));

        var nextFreeRow = 0;
        foreach (var node in column.OrderBy(node => desiredRows[node]))
        {
          rows[node] = Math.Max(desiredRows[node], nextFreeRow);
          nextFreeRow = rows[node] + 1;
        }
      }

      var rowCount = rows.Values.Max() + 1;
      var rowHeights = new float[rowCount];
      foreach (var node in group)
      {
        rowHeights[rows[node]] = Math.Max(rowHeights[rows[node]], node.layout.height);
      }

      var rowOffsets = new float[rowCount];
      for (var i = 1; i < rowCount; ++i)
      {
        rowOffsets[i] = rowOffsets[i - 1] + rowHeights[i - 1] + RowSpacing;
      }

      foreach (var node in group)
      {
        var position = new Vector2(columnOffsets[columns[node]], top + rowOffsets[rows[node]]);
        node.SetPosition(new Rect(position, node.layout.size));
      }

      return top + rowOffsets[rowCount - 1] + rowHeights[rowCount - 1];
    }

    private bool FrameRect(Rect rect)
    {
      if (rect.width <= 0 || rect.height <= 0)
      {
        return false;
      }

      CalculateFrameTransform(rect, this.layout, FramePadding, out var translation, out var scale);
      this.UpdateViewTransform(translation, scale);
      return true;
    }
  }
}
