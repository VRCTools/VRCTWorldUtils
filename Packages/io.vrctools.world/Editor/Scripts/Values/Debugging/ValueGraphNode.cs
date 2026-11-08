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
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Attributes;
using VRCTools.World.Values;

namespace VRCTools.World.Editor.Values.Debugging
{
  public class ValueGraphNode : Node
  {
    public const string InputPortName = "Source";
    public const string OutputPortName = "State";

    private static readonly Dictionary<Type, Color> TypeColors = new()
    {
      { typeof(bool), new Color(0.85f, 0.35f, 0.35f) },
      { typeof(int), new Color(0.35f, 0.75f, 0.85f) },
      { typeof(float), new Color(0.55f, 0.85f, 0.35f) },
      { typeof(string), new Color(0.90f, 0.55f, 0.85f) },
      { typeof(Color), new Color(0.95f, 0.80f, 0.30f) },
      { typeof(Vector3), new Color(0.95f, 0.60f, 0.25f) },
      { typeof(VRCUrl), new Color(0.60f, 0.55f, 0.95f) },
      { typeof(Texture2D), new Color(0.75f, 0.75f, 0.75f) },
    };

    public MonoBehaviour Component { get; }
    public ValueComponentKind Kind { get; }

    private readonly Dictionary<string, Port> _ports = new();

    public bool IsValue => this.Kind is ValueComponentKind.LocalValue or ValueComponentKind.PersistedValue
      or ValueComponentKind.SynchronizedValue;

    public Port this[string portName] => this._ports.GetValueOrDefault(ObjectNames.NicifyVariableName(portName));

    public Port InputPort { get; private set; }
    public Port OutputPort { get; private set; }

    public ValueGraphNode(MonoBehaviour component, ValueComponentKind kind)
    {
      this.Component = component;
      this.Kind = kind;

      this.title = $"{component.gameObject.name} ({ObjectNames.NicifyVariableName(component.GetType().Name)})";
      this.AddToClassList(
        $"kind-{ObjectNames.NicifyVariableName(kind.ToString()).Replace(' ', '-').ToLowerInvariant()}");

      if (this.IsValue)
      {
        this.InputPort = this.AddPort(InputPortName, Direction.Input, Port.Capacity.Multi, component.GetType());
        this.OutputPort = this.AddPort(OutputPortName, Direction.Output, Port.Capacity.Multi, component.GetType());
        return;
      }

      foreach (var field in component.GetType().GetFields())
      {
        var bindingAttribute = field.GetCustomAttribute<ValueBindingAttribute>();
        if (bindingAttribute == null)
        {
          continue;
        }

        var portName = ObjectNames.NicifyVariableName(field.Name);
        var backingField = bindingAttribute.GetBackingField(component, field);
        if (backingField == null)
        {
          continue;
        }

        var backingType = backingField.FieldType;

        // the graph view is a bit limited as ports by default are sorted into input and output containers - to keep
        // things simple we'll just consider bidirectional bindings as outputs since this makes the most sense in our
        // own components at the moment
        // TODO: Can we make this better?
        var direction = bindingAttribute.bindingType == ValueBindingType.Input ? Direction.Input : Direction.Output;

        // TODO: Port map
        this.AddPort(portName, direction, backingType);
      }
    }

    private Port AddPort(string name, Direction direction, Type type)
    {
      var actualType = type.IsArray ? type.GetElementType() : type;
      var capacity = type.IsArray ? Port.Capacity.Multi : Port.Capacity.Single;

      return this.AddPort(name, direction, capacity, actualType);
    }

    private Port AddPort(string name, Direction direction, Port.Capacity capacity, Type type)
    {
      var port = this.InstantiatePort(Orientation.Horizontal, direction, capacity, type);
      port.portName = name;

      var stateType = type.GetProperty("State")?.PropertyType;
      if (stateType != null && TypeColors.TryGetValue(stateType, out var color))
      {
        port.portColor = color;
      }

      var container = direction == Direction.Input ? this.inputContainer : this.outputContainer;
      container.Add(port);

      this._ports.Add(name, port);
      return port;
    }

    public override void OnSelected()
    {
      base.OnSelected();

      // select the corresponding object in the hierarchy when a node is selected
      Selection.activeObject = this.Component;
    }
  }
}
