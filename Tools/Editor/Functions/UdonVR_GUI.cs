using System.Diagnostics.Tracing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public class UdonVR_GUI_DragAndDrop
    {
        public static class Controller
        {
            public static Node SelectedNode = null;
            public static Node.InOutData? ConnectionStart = null;

            private static List<Node> _nodes = new List<Node>();
            public static List<Node> Nodes { get { return _nodes; } }

            private static Vector2 _mousePosition = Vector2.zero;

            private static List<Node> _removeNodes = new List<Node>();

            public static void Update()
            {
                if (ConnectionStart == null && Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    _mousePosition = Event.current.mousePosition;
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Node"), false, AddNode, typeof(Node));
                    menu.AddItem(new GUIContent("TestNode"), false, AddNode, typeof(TestNode));
                    menu.AddItem(new GUIContent("Constant"), false, AddNode, typeof(Constant_Node));
                    menu.AddItem(new GUIContent("Debug"), false, AddNode, typeof(Debug_Node));
                    menu.AddItem(new GUIContent("Remap"), false, AddNode, typeof(Remap_Node));
                    menu.AddItem(new GUIContent("Gate"), false, AddNode, typeof(Gate_Node));
                    menu.AddItem(new GUIContent("Clamp"), false, AddNode, typeof(Clamp_Node));
                    menu.AddItem(new GUIContent("Math"), false, AddNode, typeof(Math_Node));
                    menu.AddItem(new GUIContent("Random"), false, AddNode, typeof(Random_Node));
                    menu.AddItem(new GUIContent("Dampening"), false, AddNode, typeof(Dampening_Node));
                    menu.AddItem(new GUIContent("Object"), false, AddNode, typeof(Object_Node));
                    menu.ShowAsContext();
                    Event.current.Use();
                }

                for (int i = 0; i < _nodes.Count; i++) { _nodes[i].OnGUI(); }
                for (int i = 0; i < _removeNodes.Count; i++) { _nodes.Remove(_removeNodes[i]); }
                if (_removeNodes.Count > 0)
                {
                    for (int i = 0; i < _nodes.Count; i++) { _nodes[i].ValidateInputs = true; }
                    _removeNodes.Clear();
                }
            }

            private static void AddNode(object type)
            {
                if (_nodes == null) return;
                Type _nodeType = (Type)type;
                if (_nodeType == typeof(Node))
                {
                    _nodes.Add(new Node(new GUIContent("Node"), _mousePosition));
                }
                else if (_nodeType == typeof(TestNode))
                {
                    _nodes.Add(new TestNode(new GUIContent("Test Node"), _mousePosition));
                }
                else if (_nodeType == typeof(Constant_Node))
                {
                    _nodes.Add(new Constant_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Debug_Node))
                {
                    _nodes.Add(new Debug_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Remap_Node))
                {
                    _nodes.Add(new Remap_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Gate_Node))
                {
                    _nodes.Add(new Gate_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Clamp_Node))
                {
                    _nodes.Add(new Clamp_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Math_Node))
                {
                    _nodes.Add(new Math_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Random_Node))
                {
                    _nodes.Add(new Random_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Dampening_Node))
                {
                    _nodes.Add(new Dampening_Node(_mousePosition));
                }
                else if (_nodeType == typeof(Object_Node))
                {
                    _nodes.Add(new Object_Node(_mousePosition));
                }
            }

            public static void RemoveNode(Node node) { _removeNodes.Add(node); }
        }

        #region Node
        public class Node
        {
            public GUIContent content;
            public Vector2 position;
            private Vector2 _lastPosition;

            /// <summary>
            /// The Size of the node (Automatically sets height if set to 0)
            /// </summary>
            public Vector2 NodeSize = new Vector2(100, 0);

            private Rect _dragArea = Rect.zero;
            private Rect _removeNodeRect = Rect.zero;
            private Rect _nodeBody = Rect.zero;

            private bool _isBeingDraged = false;
            private Vector2 _mouseOffset = Vector2.zero;

            public struct InOutData
            {
                public Node node;
                public string varName;
                public Type type;
                public string displayName;
                public bool isEnabled;
            }

            private InOutData[] _inputs = new InOutData[0];
            public InOutData[] Inputs
            {
                get { return _inputs; }
                private set { _inputs = value; }
            }

            private InOutData[] _outputs = new InOutData[0];

            private Rect[] _inputPositions = new Rect[0];
            private Dictionary<string, Rect> _outputPositions = new Dictionary<string, Rect>();

            private Texture _inputOutputTexture;

            private Rect? _inputRect = null;
            private Rect? _bodyRect = null;
            private Rect? _outputRect = null;

            private bool _validateInputs = false;
            public bool ValidateInputs
            {
                get { return _validateInputs; }
                set { _validateInputs = value; }
            }

            public Node(GUIContent content, Vector2 position)
            {
                this.content = content;
                this.position = position;
                this._lastPosition = this.position;

                this._inputOutputTexture = Resources.Load<Texture>("_UdonVR/Icons/NodeInputOutput");
            }

            private void Update()
            {
                _lastPosition = position;
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && _dragArea.Contains(Event.current.mousePosition))
                {
                    if (Controller.SelectedNode == null) Controller.SelectedNode = this;
                    _isBeingDraged = true;
                    _mouseOffset = position - Event.current.mousePosition;
                }
                if (Event.current.isMouse && _isBeingDraged && Controller.SelectedNode == this) position = Event.current.mousePosition + _mouseOffset;
                if (Event.current.type == EventType.MouseUp && _isBeingDraged)
                {
                    if (Controller.SelectedNode == this) Controller.SelectedNode = null;
                    _isBeingDraged = false;
                    _mouseOffset = Vector2.zero;
                }

                if (position != _lastPosition) EditorWindow.focusedWindow.Repaint(); // repaint if node was moved (makes movement smooth) also updates node link lines

                _dragArea = new Rect(position.x, position.y, NodeSize.x, _dragArea.height = GUI.skin.box.CalcSize(content).y);
                _removeNodeRect = new Rect(_dragArea.x + _dragArea.width - _dragArea.height, _dragArea.y, _dragArea.height, _dragArea.height);
                _dragArea.width -= _dragArea.height;
                _nodeBody = new Rect(position.x, position.y + _dragArea.height, NodeSize.x, NodeSize.y);

                #region Get Height of different regions and save largest
                if (NodeSize.y == 0) // only run if a custom height was not set
                {
                    Rect _overrideRect = Rect.zero;
                    if (_inputRect.HasValue && _bodyRect.HasValue && _outputRect.HasValue)
                    {
                        if (_bodyRect?.height + 2 > _overrideRect.height)
                        {
                            _overrideRect.height = _bodyRect.Value.height + 2;
                        }
                        if (_inputRect?.height + 3 > _overrideRect.height)
                        {
                            _overrideRect.height = _inputRect.Value.height + 3;
                        }
                        if (_outputRect?.height + 3 > _overrideRect.height)
                        {
                            _overrideRect.height = _outputRect.Value.height + 3;
                        }
                    }
                    if (_overrideRect != Rect.zero) _nodeBody.height = _overrideRect.height; // apply generated override height
                }
                #endregion
            }

            private void Validate_Inputs()
            {
                if (!_validateInputs) return;
                for (int i = 0; i < _inputs.Length; i++)
                {
                    if (_inputs[i].node != null)
                    {
                        if (Controller.Nodes.Contains(_inputs[i].node)) { continue; }
                        else
                        {
                            _inputs[i].node = null;
                            _inputs[i].varName = string.Empty;
                        }
                    }
                }
            }

            public void OnGUI()
            {
                Update();
                Validate_Inputs();

                if (Event.current.type == EventType.Repaint)
                {
                    _inputPositions = _inputPositions.Clear();
                    _outputPositions.Clear();
                }

                GUILayout.BeginArea(_dragArea, EditorStyles.helpBox);
                UdonVR_GUI.Header(content);
                GUILayout.EndArea();
                if (GUI.Button(_removeNodeRect, new GUIContent("X", "Remove Node"))) { Controller.RemoveNode(this); }

                GUILayout.BeginArea(_nodeBody, EditorStyles.helpBox);
                GUILayout.BeginHorizontal();
                #region Inputs
                if (_inputs != null)
                {
                    GUILayout.BeginVertical();
                    if (_inputs.Length > 0)
                    {
                        for (int i = 0; i < _inputs.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUIContent _labelContent = new GUIContent(_inputs[i].displayName.ToString(), _inputs[i].type.ToString());
                            Vector2 _labelSize = GUI.skin.label.CalcSize(_labelContent);
                            _labelSize.x += 16;
                            if (_labelSize.y < 8) _labelSize.y = 8;
                            Rect _rect = GUILayoutUtility.GetRect(_labelSize.x, _labelSize.y, GUI.skin.label);
                            Rect _labelRect = _rect;
                            _rect.width = 8;
                            _rect.height = 8;
                            //_rect.x += _labelRect.width + (_rect.width / 2);
                            _rect.y += (_labelRect.height / 2) - (_rect.height / 2);
                            GUI.enabled = _inputs[i].isEnabled;
                            _rect = UdonVR_GUI.DrawImage(_rect, _inputOutputTexture, _rect.width, _rect.height);
                            _labelRect.x += _rect.width + (_rect.width / 2);
                            _labelRect.width = GUI.skin.label.CalcSize(_labelContent).x;
                            GUI.Label(_labelRect, _labelContent);
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && _rect.Contains(Event.current.mousePosition) && _inputs[i].isEnabled)
                            {
                                if (Controller.ConnectionStart.HasValue)
                                {
                                    if (_inputs[i].node == Controller.ConnectionStart.Value.node && _inputs[i].varName == Controller.ConnectionStart.Value.varName)
                                    {
                                        if (_inputs[i].type == Controller.ConnectionStart.Value.type || _inputs[i].type == typeof(object))
                                        {
                                            _inputs[i].node = null;
                                            _inputs[i].varName = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        if (_inputs[i].type == Controller.ConnectionStart.Value.type || _inputs[i].type == typeof(object))
                                        {
                                            _inputs[i].node = Controller.ConnectionStart.Value.node;
                                            _inputs[i].varName = Controller.ConnectionStart.Value.varName;
                                        }
                                    }
                                }
                            }
                            _rect.x += _nodeBody.x + (_rect.width / 2);
                            _rect.y += _nodeBody.y + (_rect.height / 2) + 1;
                            if (_inputPositions.Length < _inputs.Length) _inputPositions = _inputPositions.Append(_rect);
                        }
                    }
                    GUILayout.EndVertical();
                    if (Event.current.type == EventType.Repaint) _inputRect = GUILayoutUtility.GetLastRect();
                }
                #endregion

                #region Body
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                NodeBody();
                GUILayout.EndVertical();
                if (Event.current.type == EventType.Repaint) _bodyRect = GUILayoutUtility.GetLastRect();
                #endregion

                #region Outputs
                if (_outputs != null)
                {
                    GUILayout.BeginVertical();
                    if (_outputs.Length > 0)
                    {
                        for (int i = 0; i < _outputs.Length; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUIContent _labelContent = new GUIContent(_outputs[i].displayName.ToString(), _outputs[i].type.ToString());
                            Vector2 _labelSize = GUI.skin.label.CalcSize(_labelContent);
                            _labelSize.x += 16;
                            if (_labelSize.y < 8) _labelSize.y = 8;
                            Rect _rect = GUILayoutUtility.GetRect(_labelSize.x, _labelSize.y, GUI.skin.label);
                            Rect _labelRect = _rect;
                            _labelRect.width = GUI.skin.label.CalcSize(_labelContent).x;
                            GUI.enabled = _outputs[i].isEnabled;
                            GUI.Label(_labelRect, _labelContent);
                            _rect.width = 8;
                            _rect.height = 8;
                            _rect.x += _labelRect.width + (_rect.width / 2);
                            _rect.y += (_labelRect.height / 2) - (_rect.height / 2);
                            _rect = UdonVR_GUI.DrawImage(_rect, _inputOutputTexture, _rect.width, _rect.height);
                            GUI.enabled = true;
                            GUILayout.EndHorizontal();
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && _rect.Contains(Event.current.mousePosition) && _outputs[i].isEnabled)
                            {
                                InOutData _inOutData = new InOutData();
                                _inOutData.node = this;
                                _inOutData.varName = _outputs[i].varName;
                                _inOutData.type = _outputs[i].type;
                                _inOutData.displayName = string.Empty;
                                _inOutData.isEnabled = _outputs[i].isEnabled;

                                if (Controller.ConnectionStart.HasValue)
                                {
                                    if (Controller.ConnectionStart.Value.Equals(_inOutData)) { Controller.ConnectionStart = null; }
                                }
                                else { Controller.ConnectionStart = _inOutData; }
                            }
                            _rect.x += _nodeBody.x + (_rect.width / 2);
                            _rect.y += _nodeBody.y + (_rect.height / 2) + 1;
                            if (_outputPositions.Count < _outputs.Length && !_outputPositions.ContainsKey(_outputs[i].varName)) _outputPositions.Add(_outputs[i].varName, _rect);
                        }
                    }
                    GUILayout.EndVertical();
                    if (Event.current.type == EventType.Repaint) _outputRect = GUILayoutUtility.GetLastRect();
                }
                #endregion
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                if (_inputs != null)
                {
                    if (_inputs.Length > 0)
                    {
                        for (int i = 0; i < _inputs.Length; i++)
                        {
                            if (_inputs[i].node != null)
                            {
                                if (_inputs[i].node._outputPositions.TryGetValue(_inputs[i].varName, out Rect pos))
                                {
                                    Handles.BeginGUI();
                                    Handles.DrawLine(
                                    new Vector3(_inputPositions[i].x, _inputPositions[i].y),
                                    new Vector3(pos.x, pos.y));
                                    Handles.EndGUI();
                                }
                            }
                        }
                    }
                }

                if (_outputs != null)
                {
                    if (_outputs.Length > 0)
                    {
                        if (Controller.ConnectionStart.HasValue)
                        {
                            InOutData connection = Controller.ConnectionStart.Value;
                            if (connection.node == this)
                            {
                                if (_outputPositions.TryGetValue(connection.varName, out Rect pos))
                                {
                                    Handles.BeginGUI();
                                    Handles.DrawLine(
                                    new Vector3(pos.x, pos.y),
                                    new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y));
                                    Handles.EndGUI();

                                    EditorWindow.focusedWindow.Repaint();
                                }
                            }
                            if (Event.current.type == EventType.MouseDown && Event.current.button == 1) Controller.ConnectionStart = null;
                        }
                    }
                }
            }

            public virtual void NodeBody() { } // used to set whats inside the nodes body

            /// <summary>
            /// Converts an object to the passed type.
            /// </summary>
            /// <typeparam name="T"> Type to convert to. </typeparam>
            /// <param name="input"> Input. </param>
            /// <returns></returns>
            public T ConvertObject<T>(object input) { return (T)Convert.ChangeType(input, typeof(T)); }

            #region GetPropertyValue
            /// <summary>
            /// Gets a property value from a node.
            /// </summary>
            /// <typeparam name="T"> Typeof property. </typeparam>
            /// <param name="node"> Node to get the object from. </param>
            /// <param name="name"> Name of the property to get. </param>
            /// <returns></returns>
            public T GetPropertyValue<T>(object node, string name)
            {
                if (node == null) return default(T);

                var _field = node.GetType().GetField(name);
                if (_field != null)
                {
                    return ConvertObject<T>(_field.GetValue(node));
                }
                var _prop = node.GetType().GetProperty(name);
                if (_prop != null)
                {
                    return ConvertObject<T>(_prop.GetValue(node));
                }

                Debug.Log("No property / field: (" + name + ") found in: (" + node + ")");
                return default(T);
            }

            /// <summary>
            /// Gets a property value from a node.
            /// </summary>
            /// <typeparam name="T"> Typeof property. </typeparam>
            /// <param name="inputIndex"> Index of Inputs to get value from. </param>
            /// <returns></returns>
            public T GetPropertyValue<T>(int inputIndex)
            {
                if (inputIndex > Inputs.Length - 1)
                {
                    Debug.Log("No input at index: (" + inputIndex.ToString() + ")");
                    return default(T);
                }
                return GetPropertyValue<T>(Inputs[inputIndex].node, Inputs[inputIndex].varName);
            }

            /// <summary>
            /// Gets a property value from a node. (less efficent)
            /// </summary>
            /// <typeparam name="T"> Typeof property. </typeparam>
            /// <param name="displayName"> Display name of Inputs to get value from. </param>
            /// <returns></returns>
            public T GetPropertyValue<T>(string displayName)
            {
                for (int i = 0; i < Inputs.Length; i++)
                {
                    if (Inputs[i].displayName == displayName)
                    {
                        return GetPropertyValue<T>(i);
                    }
                }
                Debug.Log("No input with displayName: (" + displayName + ") found.");
                return default(T);
            }
            #endregion

            /// <summary>
            /// Adds an Input to the Node.
            /// </summary>
            /// <param name="type"> Typeof input. </param>
            /// <param name="displayName"> Display name of the input. </param>
            public void AddInput(Type type, string displayName)
            {
                InOutData _data = new InOutData();
                _data.node = null;
                _data.varName = string.Empty;
                _data.type = type;
                _data.displayName = displayName;
                _data.isEnabled = true;
                _inputs = _inputs.Append(_data);
            }

            /// <summary>
            /// Adds an Output to the Node.
            /// </summary>
            /// <param name="node"> Node output variable is on. (this) </param>
            /// <param name="varName"> Name of the variable to output. </param>
            /// <param name="type"> Typeof output. </param>
            /// <param name="displayName"> Display name of the output. </param>
            public void AddOutput(Node node, string varName, Type type, string displayName)
            {
                InOutData _data = new InOutData();
                _data.node = node;
                _data.varName = varName;
                _data.type = type;
                _data.displayName = displayName;
                _data.isEnabled = true;
                _outputs = _outputs.Append(_data);
            }

            /// <summary>
            /// Enables / Disables a Node Input.
            /// </summary>
            /// <param name="nodeIndex"> Index of the node to enable / disable. </param>
            /// <param name="state"> Enabled / Disabled state. </param>
            public void EnableInput(int nodeIndex, bool state)
            {
                if (nodeIndex > _inputs.Length - 1) return;
                InOutData _data = _inputs[nodeIndex];
                _data.isEnabled = state;
                _inputs[nodeIndex] = _data;
            }

            /// <summary>
            /// Enables / Disables a Node Output.
            /// </summary>
            /// <param name="nodeIndex"> Index of the node to enable / disable. </param>
            /// <param name="state"> Enabled / Disabled state. </param>
            public void EnableOutput(int nodeIndex, bool state)
            {
                if (nodeIndex > _outputs.Length - 1) return;
                InOutData _data = _outputs[nodeIndex];
                _data.isEnabled = state;
                _outputs[nodeIndex] = _data;
            }
        }
        #endregion

        public class TestNode : Node
        {
            public bool A = true;
            public int B = 25;
            public float C = 0.5f;

            public TestNode(GUIContent content, Vector2 position) : base(content, position)
            {
                NodeSize = new Vector2(250, 0);

                AddInput(typeof(bool), "A Input");
                AddInput(typeof(int), "B Input");
                AddInput(typeof(float), "C Input");

                AddOutput(this, "A", typeof(bool), "A Output");
                AddOutput(this, "B", typeof(int), "B Output");
                AddOutput(this, "C", typeof(float), "C Output");
            }

            public override void NodeBody()
            {
                GUILayout.Button("body1");
                GUILayout.Button("body2");
                GUILayout.Button("body3");
                GUILayout.Button("body4");
            }
        }

        public class Constant_Node : Node
        {
            public float floatOutput = 0;

            public Constant_Node(Vector2 position) : base(new GUIContent("Constant"), position)
            {
                NodeSize = new Vector2(150, 0);

                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();
                floatOutput = EditorGUILayout.FloatField(floatOutput);
            }
        }

        public class Debug_Node : Node
        {
            private bool _printInput = false;

            public Debug_Node(Vector2 position) : base(new GUIContent("Debug"), position)
            {
                NodeSize = new Vector2(150, 0);

                AddInput(typeof(object), "Input");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                object _val = null;
                _val = GetPropertyValue<object>(0);
                if (_val != null) { _val = _val.ToString(); }
                else { _val = string.Empty; }

                GUI.enabled = false;
                GUILayout.TextField(_val.ToString());
                GUI.enabled = true;

                _printInput = UdonVR_GUI.ToggleButton(new GUIContent("Print", "Prints output to console."), _printInput);
                if (_printInput) Debug.Log(_val.ToString());
            }
        }

        public class Remap_Node : Node
        {
            public float floatOutput = 0;

            private float _low1 = 0, _high1 = 1;
            private float _low2 = 0, _high2 = 100;

            public Remap_Node(Vector2 position) : base(new GUIContent("Remap"), position)
            {
                NodeSize = new Vector2(225, 0);

                AddInput(typeof(float), "Input");
                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            // ( function:<name>, input:<1|4|6|2|0> ,output:<1> ( function:<name>, input:<1|4|6|2|0> ,output:<1>

            /*

            int index = 0;

            string[index] functions;
            vector4[index] mode;
            int[][] inputs; // array of array
            int[][] outputs; // array of array

            vector4[] variables;

            update{

                variables[0] = lowMid;
                variables[1] = Mid;
                variables[2] = highMid;
                variables[3] = treble;
                Remap(variables, index)
            }

            */


            public override void NodeBody()
            {
                base.NodeBody();

                float _val = GetPropertyValue<float>(0);
                _val = UdonVR_MathHelpers.Remap(_val, _low1, _high1, _low2, _high2);
                floatOutput = _val;

                UdonVR_GUI.FieldArea(
                    new GUIContent[]
                    {
                        new GUIContent(),
                        new GUIContent()
                    },
                    new UdonVR_GUI.FieldAreaValues[]
                    {
                        UdonVR_GUI.FieldAreaValues.SetValue(_low1),
                        UdonVR_GUI.FieldAreaValues.SetValue(_high1)
                    },
                    new Action<UdonVR_GUI.FieldAreaValues>[]
                    {
                        (areaValues) =>
                        {
                            _low1 = areaValues.floatValue.Value;
                        },
                        (areaValues) =>
                        {
                            _high1 = areaValues.floatValue.Value;
                        }
                    }
                );
                UdonVR_GUI.FieldArea(
                    new GUIContent[]
                    {
                        new GUIContent(),
                        new GUIContent()
                    },
                    new UdonVR_GUI.FieldAreaValues[]
                    {
                        UdonVR_GUI.FieldAreaValues.SetValue(_low2),
                        UdonVR_GUI.FieldAreaValues.SetValue(_high2)
                    },
                    new Action<UdonVR_GUI.FieldAreaValues>[]
                    {
                        (areaValues) =>
                        {
                            _low2 = areaValues.floatValue.Value;
                        },
                        (areaValues) =>
                        {
                            _high2 = areaValues.floatValue.Value;
                        }
                    }
                );
            }
        }

        public class Gate_Node : Node
        {
            public float floatOutput = 0;

            private float _threshold = 0.5f;
            private bool _overThreshold = false;

            public enum Mode
            {
                GreaterThen, LessThen
            }
            private Mode _mode = Mode.GreaterThen;

            private bool _triggerOnce = true;

            public Gate_Node(Vector2 position) : base(new GUIContent("Gate"), position)
            {
                NodeSize = new Vector2(275, 0);

                AddInput(typeof(float), "Input");
                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                float _labelWidth = EditorGUIUtility.labelWidth; // store current label width

                GUIContent _label = new GUIContent("Mode");
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(_label).x + 5;
                var _enumPopup = UdonVR_GUI.EnumPopup(_label, _mode);
                _mode = ConvertObject<Mode>(_enumPopup.Item1);

                _label = new GUIContent("Threshold");
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(_label).x + 5;
                _threshold = EditorGUILayout.FloatField(_label, _threshold);

                EditorGUIUtility.labelWidth = _labelWidth; // reset label width

                _triggerOnce = UdonVR_GUI.ToggleButton(new GUIContent("Trigger Once"), _triggerOnce);

                float _val = GetPropertyValue<float>(0);

                if (_overThreshold) { floatOutput = 0; }
                switch (_mode)
                {
                    case Mode.GreaterThen:
                        if (_val >= _threshold && !_overThreshold)
                        {
                            floatOutput = _val;
                            _overThreshold = true;
                        }
                        if (!_triggerOnce) { _overThreshold = false; }
                        else
                        {
                            if (_val < _threshold)
                            {
                                floatOutput = 0;
                                _overThreshold = false;
                            }
                        }
                        break;
                    case Mode.LessThen:
                        if (_val <= _threshold && !_overThreshold)
                        {
                            floatOutput = _val;
                            _overThreshold = true;
                        }
                        if (!_triggerOnce) { _overThreshold = false; }
                        else
                        {
                            if (_val > _threshold)
                            {
                                floatOutput = 0;
                                _overThreshold = false;
                            }
                        }
                        break;
                }
            }
        }

        public class Clamp_Node : Node
        {
            public float floatOutput = 0;

            private float _min = 0, _max = 1;

            public Clamp_Node(Vector2 position) : base(new GUIContent("Clamp"), position)
            {
                NodeSize = new Vector2(225, 0);

                AddInput(typeof(float), "Input");
                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                UdonVR_GUI.FieldArea(
                    new GUIContent[]
                    {
                        new GUIContent(),
                        new GUIContent()
                    },
                    new UdonVR_GUI.FieldAreaValues[]
                    {
                        UdonVR_GUI.FieldAreaValues.SetValue(_min),
                        UdonVR_GUI.FieldAreaValues.SetValue(_max)
                    },
                    new Action<UdonVR_GUI.FieldAreaValues>[]
                    {
                        (areaValues) =>
                        {
                            _min = areaValues.floatValue.Value;
                        },
                        (areaValues) =>
                        {
                            _max = areaValues.floatValue.Value;
                        }
                    }
                );

                floatOutput = Mathf.Clamp(GetPropertyValue<float>(0), _min, _max);
            }
        }

        public class Math_Node : Node
        {
            public float floatOutput = 0;

            public enum MathOperation
            {
                Add, Subtract, Multiply, Divide, Absolute
            }

            private MathOperation _operation = MathOperation.Add;

            public Math_Node(Vector2 position) : base(new GUIContent("Math"), position)
            {
                NodeSize = new Vector2(275, 0);

                AddInput(typeof(float), "A Input");
                AddInput(typeof(float), "B Input");
                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                float _labelWidth = EditorGUIUtility.labelWidth;
                GUIContent _label = new GUIContent("Operation");
                EditorGUIUtility.labelWidth = GUI.skin.label.CalcSize(_label).x + 5;
                var _enumPopup = UdonVR_GUI.EnumPopup(_label, _operation);
                EditorGUIUtility.labelWidth = _labelWidth;
                _operation = ConvertObject<MathOperation>(_enumPopup.Item1);

                float _a = GetPropertyValue<float>(0);
                float _b = GetPropertyValue<float>(1);
                float _val = 0;

                switch (_operation)
                {
                    case MathOperation.Add:
                        EnableInput(1, true);
                        _val = _a + _b;
                        break;
                    case MathOperation.Subtract:
                        EnableInput(1, true);
                        _val = _a - _b;
                        break;
                    case MathOperation.Multiply:
                        EnableInput(1, true);
                        _val = _a * _b;
                        break;
                    case MathOperation.Divide:
                        EnableInput(1, true);
                        if (_a == 0 || _b == 0) { _val = 0; }
                        else { _val = _a / _b; }
                        break;
                    case MathOperation.Absolute:
                        EnableInput(1, false);
                        _val = Math.Abs(_a);
                        break;
                }
                floatOutput = _val;

                GUI.enabled = false;
                GUILayout.TextField(floatOutput.ToString());
                GUI.enabled = true;
            }
        }

        public class Random_Node : Node
        {
            public float floatOutput = 0;

            public float _min = 0, _max = 1;

            public Random_Node(Vector2 position) : base(new GUIContent("Random"), position)
            {
                NodeSize = new Vector2(225, 0);

                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                UdonVR_GUI.FieldArea(
                    new GUIContent[]
                    {
                        new GUIContent(),
                        new GUIContent()
                    },
                    new UdonVR_GUI.FieldAreaValues[]
                    {
                        UdonVR_GUI.FieldAreaValues.SetValue(_min),
                        UdonVR_GUI.FieldAreaValues.SetValue(_max)
                    },
                    new Action<UdonVR_GUI.FieldAreaValues>[]
                    {
                        (areaValues) =>
                        {
                            _min = areaValues.floatValue.Value;
                        },
                        (areaValues) =>
                        {
                            _max = areaValues.floatValue.Value;
                        }
                    }
                );

                floatOutput = UnityEngine.Random.Range(_min, _max);

                GUI.enabled = false;
                GUILayout.TextField(floatOutput.ToString());
                GUI.enabled = true;
            }
        }

        public class Dampening_Node : Node
        {
            public float floatOutput = 0;

            public Dampening_Node(Vector2 position) : base(new GUIContent("Dampening"), position)
            {
                NodeSize = new Vector2(130, 0);

                AddInput(typeof(float), "Target");
                AddOutput(this, "floatOutput", typeof(float), "Output");
            }

            public override void NodeBody()
            {
                base.NodeBody();

                floatOutput = UdonVR_MathHelpers.Lerp(floatOutput, GetPropertyValue<float>(0), Time.fixedDeltaTime);
            }
        }

        // future me make happen k thx
        public class Object_Node : Node
        {
            public Object_Node(Vector2 position) : base(new GUIContent("Object"), position)
            {
                NodeSize = new Vector2(100, 0);
            }

            public override void NodeBody()
            {
                base.NodeBody();
            }
        }
    }


    /// <summary>
    /// Used for adding additional options to some elements in UdonVR_GUI
    /// </summary>
    public class UdonVR_GUIOption
    {
        // created by Hamster9090901

        #region Color
        public Color? tintColorDefault;
        public Color? tintColorActive;
        public Color? textColor;

        /// <summary>
        /// Sets Default Tint Color.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="color"> Generic Color, Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <returns></returns>
        public static UdonVR_GUIOption TintColorDefault<GenericColor>(GenericColor color)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.tintColorDefault = UdonVR_ColorHelpers.GetFromGenericColor(color);
            return _option;
        }

        /// <summary>
        /// Sets Active Tint Color.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="color"> Generic Color, Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <returns></returns>
        public static UdonVR_GUIOption TintColorActive<GenericColor>(GenericColor color)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.tintColorActive = UdonVR_ColorHelpers.GetFromGenericColor(color);
            return _option;
        }

        /// <summary>
        /// Sets text Color.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="color"> Generic Color, Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <returns></returns>
        public static UdonVR_GUIOption TextColor<GenericColor>(GenericColor color)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.textColor = UdonVR_ColorHelpers.GetFromGenericColor(color);
            return _option;
        }
        #endregion

        #region Text
        public int? fontSize;
        public TextAnchor? textAnchor;

        /// <summary>
        /// Sets Font Size.
        /// </summary>
        /// <param name="fontSize"> Size of the font. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption FontSize(int fontSize)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.fontSize = fontSize;
            return _option;
        }

        /// <summary>
        /// Sets Text Anchor
        /// </summary>
        /// <param name="textAnchor"> Text anchor location. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption TextAnchor(TextAnchor textAnchor)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.textAnchor = textAnchor;
            return _option;
        }
        #endregion

        #region Positioning
        public RectOffset padding = null;
        public float? width;
        public float? height;

        /// <summary>
        /// Sets Padding.
        /// </summary>
        /// <param name="padding"> Padding. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption Padding(RectOffset padding)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.padding = padding;
            return _option;
        }

        /// <summary>
        /// Sets Width.
        /// </summary>
        /// <param name="width"> Width. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption Width(float width)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.width = width;
            return _option;
        }
        /// <summary>
        /// Sets Height.
        /// </summary>
        /// <param name="height"> Height. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption Height(float height)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.height = height;
            return _option;
        }
        /// <summary>
        /// Sets Width and Height.
        /// </summary>
        /// <param name="width"> Width. </param>
        /// <param name="height"> Height. </param>
        /// <returns></returns>
        public static UdonVR_GUIOption WidthHeight(float width, float height)
        {
            UdonVR_GUIOption _option = new UdonVR_GUIOption();
            _option.width = width;
            _option.height = height;
            return _option;
        }
        #endregion

    }

    public class UdonVR_GUI
    {
        // created by Hamster9090901

        #region DrawImage
        /// <summary>
        /// Draws an image that follows the layout spacing and padding.
        /// </summary>
        /// <param name=""> Position of the texture. </param>
        /// <param name="texture"> Texture to draw. </param>
        /// <param name="width"> Width of drawn image. </param>
        /// <param name="height"> Height of drawn image. </param>
        /// <param name="tooltip"> Tooltip to show when hovered over. </param>
        /// <returns></returns>
        public static Rect DrawImage(Rect? position, Texture texture, float width = 0, float height = 0, string tooltip = "")
        {
            if (texture == null) return Rect.zero; // return an empty rect if there was no image

            GUIStyle _style = new GUIStyle(GUI.skin.box);
            if (width != 0) _style.fixedWidth = width;
            if (height != 0) _style.fixedHeight = height;

            GUIContent _content = new GUIContent();
            _content.image = texture;
            _content.tooltip = tooltip;

            if (position == null) position = GUILayoutUtility.GetRect(_content, _style);

            #region Tooltip
            if (tooltip != "")
            {
                GUIStyle _tooltipStyle = new GUIStyle();
                _tooltipStyle.fixedWidth = _style.fixedWidth;
                _tooltipStyle.fixedHeight = _style.fixedHeight;
                GUI.Box(position.Value, new GUIContent("", tooltip), _tooltipStyle);
            }
            #endregion

            GUI.DrawTexture(position.Value, texture, ScaleMode.ScaleToFit, true, (width / height)); // draw texture
            return position.Value;
        }

        /// <summary>
        /// Draws an image that follows the layout spacing and padding.
        /// </summary>
        /// <param name="texture"> Texture to draw. </param>
        /// <param name="width"> Width of drawn image. </param>
        /// <param name="height"> Height of drawn image. </param>
        /// <param name="tooltip"> Tooltip to show when hovered over. </param>
        public static Rect DrawImage(Texture texture, float width = 0, float height = 0, string tooltip = "")
        {
            return DrawImage(null, texture, width, height, tooltip);
        }
        #endregion



        #region Href
        /// <summary>
        /// Displays an image that links to a website.
        /// </summary>
        /// <param name="texture"> Texture to draw. </param>
        /// <param name="href"> Link to website. </param>
        /// <param name="width"> Width of drawn image. </param>
        /// <param name="height"> Height of drawn image. </param>
        /// <param name="tooltip"> Tooltip to show when hovered over. </param>
        public static void Href(Texture texture, string href, float width = 0, float height = 0, string tooltip = "")
        {
            Rect _rect = DrawImage(texture, width, height, tooltip);
            if (Event.current.type == EventType.MouseUp && _rect.Contains(Event.current.mousePosition)) Application.OpenURL(href);
        }

        /// <summary>
        /// Displays a link.
        /// </summary>
        /// <param name="content"> Content </param>
        /// <param name="href"> Link to website. </param>
        public static void Href(GUIContent content, string href)
        {
            GUIStyle _style = new GUIStyle(GUI.skin.box);
            Rect _rect = GUILayoutUtility.GetRect(content, _style);
            GUI.Label(_rect, content);
            if (Event.current.type == EventType.MouseUp && _rect.Contains(Event.current.mousePosition)) Application.OpenURL(href);
        }

        /// <summary>
        /// Displays a link.
        /// </summary>
        /// <param name="href"> Link to website. </param>
        public static void Href(string href)
        {
            Href(new GUIContent(href), href);
        }
        #endregion



        #region UdonVR Links
        /// <summary>
        /// Draws the UdonVR links. (Centered)
        /// </summary>
        /// <param name="logoPath"> Path to the logos to display. </param>
        /// <param name="width"> Width of drawn images. </param>
        /// <param name="height"> Height of drawn images. </param>
        public static void ShowUdonVRLinks(int width = 32, int height = 32, bool forceBottom = false, GUIStyle style = null, bool showDebugContainers = false)
        {
            Texture _discord = (Texture)Resources.Load("_UdonVR/Logos/discord");
            Texture _github = (Texture)Resources.Load("_UdonVR/Logos/github");
            Texture _udonvr = (Texture)Resources.Load("_UdonVR/Logos/udonVR");
            Texture _kofi = (Texture)Resources.Load("_UdonVR/Logos/ko-fi");
            Texture _patreon = (Texture)Resources.Load("_UdonVR/Logos/patreon");

            GUIStyle _style = new GUIStyle(EditorStyles.helpBox);
            _style.padding = new RectOffset(10, 10, 10, 10);
            _style = style != null ? style : _style;

            if (forceBottom) GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal(showDebugContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.25f, 1)) : new GUIStyle());
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(_style);
            Href(_discord, "http://discord.Udonvr.com", width, height, "Discord");
            Href(_github, "http://github.UdonVr.com", width, height, "Github");
            Href(_udonvr, "http://discord.Udonvr.com", width, height, "Website");
            Href(_kofi, "http://kofi.UdonVR.com", width, height, "Ko-Fi");
            Href(_patreon, "http://patreon.Udonvr.com", width, height, "Patreon");
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (forceBottom) GUILayout.Space(16);
        }
        #endregion



        #region Header
        /// <summary>
        /// Creates a label with centered text to be used as a header.
        /// </summary>
        /// <param name="content"> GUIContent of the header. </param>
        /// <param name="options"> Additional options for the header. </param>
        public static void Header(GUIContent content, params UdonVR_GUIOption[] options)
        {
            GUIStyle _style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter, // set text alignment
                fontSize = 13 // set font size
            };
            Color _tintColorDefault = Color.white;

            foreach (UdonVR_GUIOption option in options)
            {
                if (option.tintColorDefault.HasValue) _tintColorDefault = option.tintColorDefault.Value;
                //if (option.tintColorActive.HasValue) _tintColorActive = option.tintColorActive.Value;
                if (option.textColor.HasValue) _style.normal.textColor = option.textColor.Value;

                if (option.fontSize.HasValue) _style.fontSize = option.fontSize.Value;
                if (option.textAnchor.HasValue) _style.alignment = option.textAnchor.Value;

                if (option.padding != null) _style.padding = option.padding;
                if (option.width.HasValue) _style.fixedWidth = option.width.Value;
                if (option.height.HasValue) _style.fixedHeight = option.height.Value;
            }

            Color _backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = _backgroundColor * Color.Lerp(Color.white, _tintColorDefault, 0.15f);
            EditorGUILayout.LabelField(content, _style, GUILayout.ExpandWidth(true)); // create label field
            GUI.backgroundColor = _backgroundColor;
        }
        #endregion



        #region Tinted Button
        /// <summary>
        /// Creates a button that can be tinted with a color.
        /// </summary>
        /// <param name="position"> Button position. </param>
        /// <param name="content"> Content of the button. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static bool TintedButton(Rect position, GUIContent content, params UdonVR_GUIOption[] options)
        {
            GUIStyle _style = new GUIStyle(GUI.skin.button);
            Color _tintColorDefault = Color.white;

            foreach (UdonVR_GUIOption option in options)
            {
                if (option.tintColorDefault.HasValue) _tintColorDefault = option.tintColorDefault.Value;
                if (option.tintColorActive.HasValue) Debug.LogWarning("UdonVR_GUI.TintedButton does not support UdonVR_GUIOption.TintColorActive()");

                if (option.textColor.HasValue) _style.normal.textColor = option.textColor.Value;
                if (option.fontSize.HasValue) _style.fontSize = option.fontSize.Value;
                if (option.textAnchor.HasValue) _style.alignment = option.textAnchor.Value;

                if (option.padding != null) _style.padding = option.padding;
                if (option.width.HasValue) _style.fixedWidth = option.width.Value;
                if (option.height.HasValue) _style.fixedHeight = option.height.Value;
            }

            if (position == Rect.zero) position = GUILayoutUtility.GetRect(content, _style);

            //_tintColor = UdonVR_ColorHelpers.GetFromGenericColor(_tintColor); // get color from generic color
            Color _backgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = _backgroundColor * Color.Lerp(Color.white, _tintColorDefault, 0.15f);
            bool state = GUI.Button(position, content, _style);
            GUI.backgroundColor = _backgroundColor;

            return state;
        }

        /// <summary>
        /// Creates a button that can be tinted with a color.
        /// </summary>
        /// <param name="content"> Content of the button. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static bool TintedButton(GUIContent content, params UdonVR_GUIOption[] options)
        {
            return TintedButton(Rect.zero, content, options);
        }
        #endregion



        #region Dynamic Scroll View
        /// <summary>
        /// Begin Dynamic Scroll View
        /// </summary>
        /// <param name="widthHeight"> Width and Height of the Scroll View. </param>
        /// <param name="scrollPosition"> Position of the Scroll Content. </param>
        /// <param name="viewArea"> Size of the View Area. </param>
        /// <param name="showDebugContainers"></param>
        /// <returns> Vector2 scrollPosition. </returns>
        public static Vector2 BeginDynamicScrollViewHeight(
            Vector2 widthHeight,
            Vector2 scrollPosition,
            Rect viewArea,
            bool showDebugContainers = false)
        {
            GUIStyle _debug = showDebugContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.5f, 1)) : new GUIStyle();
            float _height = viewArea.height >= widthHeight.y ? widthHeight.y : viewArea.height;
            EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(widthHeight.x, _height, _debug));
            //scrollPosition = GUI.BeginScrollView(new Rect(position.x, position.y, position.width, _height), scrollPosition, new Rect(0, 0, viewArea.width, viewArea.height));
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(_height));
            return scrollPosition;
        }

        /// <summary>
        /// Begin Dynamic Scroll View
        /// </summary>
        /// <param name="position"> Position of the Scroll View. </param>
        /// <param name="scrollPosition"> Position of the Scroll Content. </param>
        /// <param name="viewArea"> Size of the View Area. </param>
        /// <param name="showDebugContainers"></param>
        /// <returns> Vector2 scrollPosition. </returns>
        public static Vector2 BeginDynamicScrollViewHeight(
            Vector2 widthHeight,
            ref Vector2 scrollPosition,
            Rect viewArea,
            bool showDebugContainers = false)
        {
            scrollPosition = BeginDynamicScrollViewHeight(widthHeight, scrollPosition, viewArea, showDebugContainers);
            return scrollPosition;
        }

        /// <summary>
        /// Ends the Dynamic Scroll View.
        /// </summary>
        /// <param name="contentRect"> Content Rect of the Scroll View. </param>
        /// <returns></returns>
        public static Rect EndDynamicScrollViewHeight(Rect contentRect)
        {
            if (Event.current.type == EventType.Repaint) contentRect = GUILayoutUtility.GetLastRect();
            //GUI.EndScrollView();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            return contentRect;
        }

        /// <summary>
        /// Ends the Dynamic Scroll View.
        /// </summary>
        /// <param name="contentRect"> Content Rect of the Scroll View. </param>
        /// <returns></returns>
        public static Rect EndDynamicScrollViewHeight(ref Rect contentRect)
        {
            contentRect = EndDynamicScrollViewHeight(contentRect);
            return contentRect;
        }
        #endregion



        #region Label
        /// <summary>
        /// Displays a label that only takes up enough space for its content.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="content"> Content of the Label. </param>
        /// <param name="color"> Color, Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <param name="padding"> RectOffset padding. </param>
        /// <param name="textAnchor"> TextAnchor where does the text anchor. </param>
        public static void Label<GenericColor>(GUIContent content, GenericColor color, RectOffset padding = null, TextAnchor textAnchor = TextAnchor.MiddleLeft, int fontSize = 13)
        {
            GUIStyle _style = new GUIStyle(GUI.skin.label)
            {
                alignment = textAnchor, // set text alignment
                fontSize = fontSize // set font size
            };
            Vector2 _contentSize = _style.CalcSize(content);
            _style = UdonVR_Style.SetWidthHeight(_contentSize.x, _contentSize.y, _style);
            if (padding != null) _style.padding = padding;
            _style.normal.textColor = UdonVR_ColorHelpers.GetFromGenericColor(color); // get color from generic color
            Rect _rect = GUILayoutUtility.GetRect(content, _style);
            GUI.Label(_rect, content, _style);
        }
        #endregion



        #region Toggle Button
        /// <summary>
        /// Creates a toggleable button.
        /// </summary>
        /// <param name="position"> Button position. </param>
        /// <param name="content"> Content of the button. </param>
        /// <param name="state"> State of the button. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static bool ToggleButton(Rect position, GUIContent content, bool state, params UdonVR_GUIOption[] options)
        {
            GUIStyle _style = new GUIStyle(GUI.skin.button);
            Color _tintColorDefault = Color.white;
            Color _tintColorActive = Color.green;

            foreach (UdonVR_GUIOption option in options)
            {
                if (option.tintColorDefault.HasValue) _tintColorDefault = option.tintColorDefault.Value;
                if (option.tintColorActive.HasValue) _tintColorActive = option.tintColorActive.Value;

                if (option.textColor.HasValue) _style.normal.textColor = option.textColor.Value;
                if (option.fontSize.HasValue) _style.fontSize = option.fontSize.Value;
                if (option.textAnchor.HasValue) _style.alignment = option.textAnchor.Value;

                if (option.padding != null) _style.padding = option.padding;
                if (option.width.HasValue) _style.fixedWidth = option.width.Value;
                if (option.height.HasValue) _style.fixedHeight = option.height.Value;
            }

            if (position == Rect.zero) position = GUILayoutUtility.GetRect(content, _style);

            if (content == null) content = new GUIContent("I WAS LEFT NULL", "DO NOT LEAVE CONTENT FIELD NULL");

            Color _backgroundColor = GUI.backgroundColor; // get background color
            GUI.backgroundColor = state ? _backgroundColor * Color.Lerp(Color.white, _tintColorActive, 0.15f) : _backgroundColor * Color.Lerp(Color.white, _tintColorDefault, 0.15f); // pick color based on state
            if (GUI.Button(position, content))
            {
                state = !state; // invert state
            }
            GUI.backgroundColor = _backgroundColor; // reset background color

            return state;
        }

        /// <summary>
        /// Creates a toggleable button.
        /// </summary>
        /// <param name="content"> Content of the button. </param>
        /// <param name="state"> State of the button. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static bool ToggleButton(GUIContent content, bool state, params UdonVR_GUIOption[] options)
        {
            return ToggleButton(Rect.zero, content, state, options);
        }

        /// <summary>
        /// Creates a toggleable button.
        /// </summary>
        /// <param name="property"> Reference to the property to represent. </param>
        /// <param name="content"> Content of the button. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static void ToggleButton(SerializedProperty property, GUIContent content, params UdonVR_GUIOption[] options)
        {
            if (content == null) content = new GUIContent(property.displayName, property.tooltip);
            property.boolValue = ToggleButton(Rect.zero, content, property.boolValue, options);
        }

        /// <summary>
        /// Creates a toggleable button.
        /// </summary>
        /// <param name="property"> Reference to the property to represent. </param>
        /// <param name="options"> Additional options for the button. </param>
        /// <returns></returns>
        public static void ToggleButton(SerializedProperty property, params UdonVR_GUIOption[] options)
        {
            GUIContent _content = new GUIContent(property.displayName, property.tooltip);
            property.boolValue = ToggleButton(Rect.zero, _content, property.boolValue, options);
        }
        #endregion



        #region Enum Popup
        /// <summary>
        /// Displays an EnumPopup.
        /// </summary>
        /// <typeparam name="T"> Enum type. </typeparam>
        /// <param name="content"> Content to display. </param>
        /// <param name="value"> Enum value. </param>
        /// <returns> (Enum, int) Selected enum, Selected index. </returns>
        public static (Enum, int) EnumPopup(GUIContent content, Enum value)
        {
            value = EditorGUILayout.EnumPopup(content, value);
            return (value, (int)(object)value);
        }

        /// <summary>
        /// Displays an EnumPopup.
        /// </summary>
        /// <param name="property"> Reference to the property to represent. </param>
        /// <param name="content"> Content to display. </param>
        /// <param name="enumType"> Typeof enum to display. </param>
        /// <returns></returns>
        public static void EnumPopup(SerializedProperty property, GUIContent content, Type enumType)
        {
            if (content == null) content = new GUIContent(property.displayName, property.tooltip);
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                property.enumValueIndex = EnumPopup(content, UdonVR_EnumHelpers.EnumFromInt(property.enumValueIndex, enumType)).Item2;
            }
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EnumPopup(content, UdonVR_EnumHelpers.EnumFromInt(property.intValue, enumType)).Item2;
            }
        }

        /// <summary>
        /// Displays an EnumPopup.
        /// </summary>
        /// <param name="property"> Reference to the property to represent. </param>
        /// <param name="enumType"> Typeof enum to display. </param>
        public static void EnumPopup(SerializedProperty property, Type enumType) { EnumPopup(property, null, enumType); }
        #endregion



        #region Button Foldout
        /// <summary>
        /// Creates a ButtonFoldout area where content can be placed inbetween 'BeginButtonFoldout' and 'EndButtonFoldout' to create a Foldout area.
        /// <br> --- Requires EndButtonFoldout after content --- </br>
        /// </summary>
        /// <param name="content"> Content of the Foldout. </param>
        /// <param name="state"> State of the Foldout. </param>
        /// <param name="guid"> Name of the variable being used to store some data for the button foldout. (MUST BE ENTERED AGAIN IN EndButtonFoldout)</param>
        /// <param name="style"> Style of the master BeginVertical. (Everything is inside of this.) </param>
        /// <param name="parentStyle"> Style of the BeginHorizontal or BeginVertical this is inside of. (Only important if "parentStyle" parameter was not set.) </param>
        /// <param name="indentLevel"> Indent Level of the Foldout (Matches indent of EditorGUI.indentLevel++) </param>
        /// <param name="foldoutOffsetX"> Additional offset for the foldout arrow if it doesnt lineup. (Material Editors for example)</param>
        /// <param name="invertFoldoutArrow"> Inverts the foldout arrow icon. </param>
        /// <returns></returns>
        public static bool BeginButtonFoldout(
            GUIContent content,
            bool state,
            string guid,
            GUIStyle style = null,
            GUIStyle parentStyle = null,
            int indentLevel = 0,
            int foldoutOffsetX = 0,
            bool invertFoldoutArrow = false,
            bool showDebugContainers = false,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            #region Get guid from script caller filepath to get correct variable group
            string group_guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath).ToString();
            #endregion

            #region Get guid from script caller filepath and caller functio to create variable name guid
            guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath + "_" + callerMemberName + "_" + guid).ToString();
            #endregion

            object _output = null;
            try
            {
                _output = UdonVR_VariableStorage.Instance.GetVariableGroup(group_guid).GetVariable(guid); // load the variable
            }
            catch (System.Exception) { }
            Rect _rect = _output != null ? (Rect)Convert.ChangeType(_output, typeof(Rect)) : Rect.zero; // get the rectangle from the variable object

            GUIStyle _style = style == null ? GUIStyle.none : style;
            _style.padding = new RectOffset();
            GUILayout.BeginVertical(_style);

            GUILayout.BeginHorizontal(showDebugContainers == false ? GUIStyle.none : UdonVR_Style.Get(new Vector4(0.5f, 0.5f, 0.25f, 1f)));
            GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button);
            Rect _buttonRect = GUILayoutUtility.GetRect(content, _buttonStyle);
            state = ToggleButton(_buttonRect, content, state);
            _buttonRect.x += 3 + foldoutOffsetX;
            state = invertFoldoutArrow ? !state : state;
            state = EditorGUI.Foldout(_buttonRect, state, (string)null, true);
            state = invertFoldoutArrow ? !state : state;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(showDebugContainers == false ? GUIStyle.none : UdonVR_Style.Get(new Vector4(0.25f, 0.5f, 0.5f, 1f)));
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical(showDebugContainers == false ? GUIStyle.none : UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.5f, 1f)), GUILayout.Width(_rect.width - ((indentLevel + 1) * 20) + (parentStyle != null ? parentStyle.padding.left : 0)));
            return state;
        }

        /// <summary>
        /// Creates a ButtonFoldout area where content can be placed inbetween 'BeginButtonFoldout' and 'EndButtonFoldout' to create a Foldout area.
        /// <br> --- Requires EndButtonFoldout after content --- </br>
        /// </summary>
        /// <param name="content"> Content of the Foldout. </param>
        /// <param name="state"> ref state | State of the Foldout. Updates variable passed without using return. </param>
        /// <param name="guid"> Name of the variable being used to store some data for the button foldout. (MUST BE ENTERED AGAIN IN EndButtonFoldout)</param>
        /// <param name="style"> Style of the master BeginVertical. (Everything is inside of this.) </param>
        /// <param name="parentStyle"> Style of the BeginHorizontal or BeginVertical this is inside of. (Only important if "parentStyle" parameter was not set.) </param>
        /// <param name="indentLevel"> Indent Level of the Foldout (Matches indent of EditorGUI.indentLevel++) </param>
        /// <param name="foldoutOffsetX"> Additional offset for the foldout arrow if it doesnt lineup. (Material Editors for example)</param>
        /// <param name="invertFoldoutArrow"> Inverts the foldout arrow icon. </param>
        /// <returns></returns>
        public static bool BeginButtonFoldout(
            GUIContent content,
            ref bool state,
            string guid,
            GUIStyle style = null,
            GUIStyle parentStyle = null,
            int indentLevel = 0,
            int foldoutOffsetX = 0,
            bool invertFoldoutArrow = false,
            bool showDebugContainers = false,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            state = BeginButtonFoldout(
                content,
                state,
                guid,
                style,
                parentStyle,
                indentLevel,
                foldoutOffsetX,
                invertFoldoutArrow,
                showDebugContainers,
                callerFilePath,
                callerMemberName);
            return state;
        }

        /// <summary>
        /// Creates a ButtonFoldout area where content can be placed inbetween 'BeginButtonFoldout' and 'EndButtonFoldout' to create a Foldout area.
        /// <br> --- Requires EndButtonFoldout after content --- </br>
        /// </summary>
        /// <param name="property"> Reference to the property to represent. </param>
        /// <param name="content"> Content of the Foldout. </param>
        /// <param name="guid"> Name of the variable being used to store some data for the button foldout. (MUST BE ENTERED AGAIN IN EndButtonFoldout)</param>
        /// <param name="style"> Style of the master BeginVertical. (Everything is inside of this.) </param>
        /// <param name="parentStyle"> Style of the BeginHorizontal or BeginVertical this is inside of. (Only important if "parentStyle" parameter was not set.) </param>
        /// <param name="indentLevel"> Indent Level of the Foldout (Matches indent of EditorGUI.indentLevel++) </param>
        /// <param name="foldoutOffsetX"> Additional offset for the foldout arrow if it doesnt lineup. (Material Editors for example)</param>
        /// <param name="invertFoldoutArrow"> Inverts the foldout arrow icon. </param>
        /// <returns></returns>
        public static void BeginButtonFoldout(
            SerializedProperty property,
            GUIContent content,
            string guid,
            GUIStyle style = null,
            GUIStyle parentStyle = null,
            int indentLevel = 0,
            int foldoutOffsetX = 0,
            bool invertFoldoutArrow = false,
            bool showDebugContainers = false,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "")
        {
            if (content == null) content = new GUIContent(property.displayName, property.tooltip);
            property.boolValue = BeginButtonFoldout(
                content,
                property.boolValue,
                guid,
                style,
                parentStyle,
                indentLevel,
                foldoutOffsetX,
                invertFoldoutArrow,
                showDebugContainers,
                callerFilePath,
                callerMemberName);
        }

        /// <summary>
        /// Ends the ButtonFoldout.
        /// <br> --- Requires BeginButtonFoldout before content --- </br>
        /// </summary>
        /// <param name="guid"> MUST BE THE SAME GUID AS BeginButtonFoldout </param>
        public static void EndButtonFoldout(
            string guid,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = ""
        )
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            #region Get guid from script caller filepath to get correct variable group
            string group_guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath).ToString();
            #endregion

            #region Get guid from script caller filepath and caller functio to create variable name guid
            guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath + "_" + callerMemberName + "_" + guid).ToString();
            #endregion

            // if were repainting then save the size of the rect
            if (Event.current.type == EventType.Repaint)
            {
                Rect _rect = GUILayoutUtility.GetLastRect();
                try
                {
                    UdonVR_VariableStorage.Instance.GetVariableGroup(group_guid).SetVariable(guid, _rect); // save the rect
                }
                catch (System.Exception) { }
            }
        }
        #endregion



        #region List
        /// <summary>
        /// Contains options for UdonVR_GUI.List
        /// </summary>
        public class ListOption
        {
            public bool? showSize;
            public bool? allowSizeChange;

            /// <summary>
            /// Does the list display the size.
            /// </summary>
            /// <param name="showSize"></param>
            /// <returns></returns>
            public static ListOption ShowSize(bool showSize)
            {
                ListOption _option = new ListOption();
                _option.showSize = showSize;
                return _option;
            }

            /// <summary>
            /// Does the list allow the size to be modified. (Size of list is still displayed)
            /// </summary>
            /// <param name="allowSizeChange"></param>
            /// <returns></returns>
            public static ListOption AllowSizeChange(bool allowSizeChange)
            {
                ListOption _option = new ListOption();
                _option.allowSizeChange = allowSizeChange;
                return _option;
            }
        }

        /// <summary>
        /// Displays an array or a list.
        /// </summary>
        /// <param name="property"> Property to display. </param>
        /// <param name="content"> Content of the property. </param>
        public static void List(SerializedProperty property, GUIContent content, params ListOption[] options)
        {
            bool _showSize = true;
            bool _allowSizeChange = true;

            if (content == null) content = new GUIContent(property.displayName, property.tooltip);
            Rect _position = GUILayoutUtility.GetRect(content, GUI.skin.label);

            foreach (ListOption option in options)
            {
                if (option.showSize.HasValue) _showSize = option.showSize.Value;
                if (option.allowSizeChange.HasValue) _allowSizeChange = option.allowSizeChange.Value;
            }

            _position.x += 12; // account for foldout arrow
            property.isExpanded = EditorGUI.Foldout(_position, property.isExpanded, content);
            EditorGUI.indentLevel += 1;
            if (property.isExpanded)
            {
                GUI.enabled = _allowSizeChange;
                if (_showSize) EditorGUILayout.PropertyField(property.FindPropertyRelative("Array.size"));
                GUI.enabled = true;
                for (int i = 0; i < property.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), true);
                }
            }
            EditorGUI.indentLevel -= 1;
        }
        /// <summary>
        /// Displays an array or list.
        /// </summary>
        /// <param name="property"> Property to display. </param>
        public static void List(SerializedProperty property, params ListOption[] options) { List(property, null, options); }
        #endregion



        #region Field Area
        /// <summary>
        /// Contains values to be fed into and recieved from UdonVR_GUI.FieldArea
        /// </summary>
        public class FieldAreaValues
        {
            public bool? boolValue;
            public string stringValue = null;
            public int? intValue;
            public float? floatValue;
            public double? doubleValue;
            public Vector4? vectorValue;
            public Color? colorValue;
            public object objectValue = null;

            public Type type; // type of field to make

            public bool isEnabled = true; // is field enabled

            #region Slider
            public bool isSlider = false; // is field a slider
            public float sliderMin = 0.0f; // minimum slider value
            public float sliderMax = 1.0f; // maximum slider value
            #endregion

            #region Color
            public bool colorShowEyeDropper = true;
            public bool colorShowAlpha = true;
            public bool colorIsHdr = false;
            #endregion

            #region Options
            #region Tint Color
            public Color tintDefault = Color.white; // tint of field
            public Color tintActive = Color.white; // tint of field (button active state)
            #endregion

            #region Font
            public Color? textColor;
            //public int? fontSize;
            //public TextAnchor? textAnchor;
            #endregion
            #endregion

            /// <summary>
            /// Sets the field value and type. (Used as a base to make overloading easier.)
            /// <br> Supported Types: bool, string, int, float, double, Vector2, Vector3, Vector4, Color, object </br>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value"> Current value of the field. (will make field the same type) </param>
            /// <param name="isEnabled"> Is the field enabled. </param>
            /// <param name="isSlider"> Is the field a slider. (Numbers Only) </param>
            /// <param name="sliderMin"> Minimum slider value (Numbers Only) </param>
            /// <param name="sliderMax"> Maximum slider value (Numbers Only) </param>
            /// <param name="colorShowEyeDropper"> Does the color field show the eye dropper. (Color Only) </param>
            /// <param name="colorShowAlpha"> Does the color field allow alpha. (Color Only)</param>
            /// <param name="colorIsHdr"> Does the color field support HDR (Color Only) </param>
            /// <param name="options"> Additional options for the field. </param>
            /// <returns> All the data nessasary to create a input field inside the field area. </returns>
            private static FieldAreaValues InternalSetValue<T>(
                T value,
                bool isEnabled = true,
                bool isSlider = false,
                float sliderMin = 0.0f,
                float sliderMax = 1.0f,
                bool colorShowEyeDropper = true,
                bool colorShowAlpha = true,
                bool colorIsHdr = false,
                params UdonVR_GUIOption[] options)
            {
                FieldAreaValues values = new FieldAreaValues();
                values.type = typeof(T);
                values.isEnabled = isEnabled;

                values.isSlider = isSlider;
                values.sliderMin = sliderMin;
                values.sliderMax = sliderMax;

                values.colorShowEyeDropper = colorShowEyeDropper;
                values.colorShowAlpha = colorShowAlpha;
                values.colorIsHdr = colorIsHdr;

                foreach (UdonVR_GUIOption option in options)
                {
                    if (option.tintColorDefault.HasValue) values.tintDefault = option.tintColorDefault.Value;
                    if (option.tintColorActive.HasValue) values.tintActive = option.tintColorActive.Value;

                    if (option.textColor.HasValue) values.textColor = option.textColor.Value;
                    if (option.fontSize.HasValue) Debug.LogWarning("UdonVR_GUI.FieldArea does not support UdonVR_GUIOption.FontSize()");
                    if (option.textAnchor.HasValue) Debug.LogWarning("UdonVR_GUI.FieldArea does not support UdonVR_GUIOption.TextAnchor()");
                    //if (option.fontSize.HasValue) values.fontSize = option.fontSize.Value;
                    //if (option.textAnchor.HasValue) values.textAnchor = option.textAnchor.Value;

                    if (option.padding != null) Debug.LogWarning("UdonVR_GUI.FieldArea does not support UdonVR_GUIOption.Padding()");
                    if (option.width.HasValue) Debug.LogWarning("UdonVR_GUI.FieldArea does not support UdonVR_GUIOption.Width()");
                    if (option.height.HasValue) Debug.LogWarning("UdonVR_GUI.FieldArea does not support UdonVR_GUIOption.Height()");
                }

                if (typeof(T).Equals(typeof(bool)))
                {
                    values.boolValue = (bool)Convert.ChangeType(value, typeof(T));
                    return values;
                }
                if (typeof(T).Equals(typeof(string)))
                {
                    values.stringValue = (string)Convert.ChangeType(value, typeof(T));
                    return values;
                }
                if (typeof(T).Equals(typeof(int)))
                {
                    values.intValue = (int)Convert.ChangeType(value, typeof(T));
                    return values;
                }
                if (typeof(T).Equals(typeof(float)))
                {
                    values.floatValue = (float)Convert.ChangeType(value, typeof(T));
                    return values;
                }
                if (typeof(T).Equals(typeof(double)))
                {
                    values.doubleValue = (double)Convert.ChangeType(value, typeof(T));
                    return values;
                }
                if (typeof(T).Equals(typeof(Vector2)) || typeof(T).Equals(typeof(Vector3)) || typeof(T).Equals(typeof(Vector4)))
                {
                    if (typeof(T).Equals(typeof(Vector2)))
                    {
                        values.vectorValue = Convert.ChangeType(value, typeof(T)) as Vector2?;
                    }
                    if (typeof(T).Equals(typeof(Vector3)))
                    {
                        values.vectorValue = Convert.ChangeType(value, typeof(T)) as Vector3?;
                    }
                    if (typeof(T).Equals(typeof(Vector4)))
                    {
                        values.vectorValue = Convert.ChangeType(value, typeof(T)) as Vector4?;
                    }
                    return values;
                }
                if (typeof(T).Equals(typeof(Color)))
                {
                    values.colorValue = (Color)Convert.ChangeType(value, typeof(Color));
                    return values;
                }

                values.objectValue = value;
                return values;
            }

            /// <summary>
            /// Creates a button field that is always returning false unless pressed.
            /// </summary>
            /// <param name="isEnabled"> Is button enabled. </param>
            /// <param name="options"> Additional options for the button. </param>
            /// <returns> All the data nessasary to create a momentary button field inside the field area. </returns>
            public static FieldAreaValues SetValueMomentary(bool isEnabled = true, params UdonVR_GUIOption[] options)
            {
                return InternalSetValue(false, isEnabled, false, 0.0f, 1.0f, true, true, false, options);
            }

            /// <summary>
            /// Creates a toggle button field that has its active tint color already set to Color.green
            /// </summary>
            /// <param name="value"> Current value of the button. </param>
            /// <param name="isEnabled"> Is button enabled. </param>
            /// <param name="options"> Additional options for the button. </param>
            /// <returns> All the data nessasary to create a toggle button field inside the field area. </returns>
            public static FieldAreaValues SetValueToggle(bool value, bool isEnabled = true, params UdonVR_GUIOption[] options)
            {
                options = options.Add(UdonVR_GUIOption.TintColorActive(Color.green));
                return InternalSetValue(value, isEnabled, false, 0.0f, 1.0f, true, true, false, options);
            }

            /// <summary>
            /// Creates a field that represents the input value.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value"> Current value of the field. (Will make field of the same type.) </param>
            /// <param name="isEnabled"> Is the field enabled. </param>
            /// <param name="options"> Additional options for the field. </param>
            /// <returns> All the data nessasary to create a input field inside the field area. </returns>
            public static FieldAreaValues SetValue<T>(T value, bool isEnabled = true, params UdonVR_GUIOption[] options)
            {
                return InternalSetValue(value, isEnabled, false, 0.0f, 1.0f, true, true, false, options);
            }

            /// <summary>
            /// Creates a slider matching the input type.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="value"> Current value of the field. (Will make field of the same type.) </param>
            /// <param name="isEnabled"> Is the slider field enabled. </param>
            /// <param name="sliderMin"> Minimum slider value. </param>
            /// <param name="sliderMax"> Maximum slider value. </param>
            /// <param name="options"> Additional options for the slider. </param>
            /// <returns> All the data nessasary to create a slider field inside the field area. </returns>
            public static FieldAreaValues SetValueSlider<T>(
                T value,
                bool isEnabled = true,
                float sliderMin = 0.0f,
                float sliderMax = 1.0f,
                params UdonVR_GUIOption[] options)
            {
                return InternalSetValue(value, isEnabled, true, sliderMin, sliderMax, true, true, false, options);
            }

            /// <summary>
            /// Creates a color field.
            /// </summary>
            /// <param name="value"> Current Color value. </param>
            /// <param name="isEnabled"> Is the color field enabled. </param>
            /// <param name="showEyeDropper"> Does the color field show the eye dropper. </param>
            /// <param name="showAlpha"> Does the color field allow alpha. </param>
            /// <param name="isHdr"> Does the color field support HDR. </param>
            /// <param name="options"> Additional options for the slider. </param>
            /// <returns> All the data nessasary to create a color field inside the field area. </returns>
            public static FieldAreaValues SetValueColor(
                Color value,
                bool isEnabled = true,
                bool showEyeDropper = true,
                bool showAlpha = true,
                bool isHdr = false,
                params UdonVR_GUIOption[] options)
            {
                return InternalSetValue(value, isEnabled, false, 0.0f, 1.0f, showEyeDropper, showAlpha, isHdr, options);
            }
        }

        /// <summary>
        /// Create a horizontal area that supports different field types all evenly spaced to fill the area.
        /// </summary>
        /// <param name="fieldContents"> Contents of the fields. </param>
        /// <param name="currentValues"> Current values of the fields. </param>
        /// <param name="actions"> Actions the fields call when modified. </param>
        /// <param name="fieldsEnabled"> Are the fields enabled / disabled. </param>
        /// <param name="areaStyle"> Style of the area. </param>
        /// <param name="guid"> Custom GUID (Generated if left empty) </param>
        /// <param name="showDebugContainers"> Show areas inside for debug purposes. </param>
        public static void FieldArea(
            GUIContent[] fieldContents,
            FieldAreaValues[] currentValues,
            Action<FieldAreaValues>[] actions,
            GUIStyle areaStyle = null,
            string guid = null,
            bool showDebugContainers = false,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "",
            [System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int callerLineNumber = 0)
        {
            #region Get guid from script caller filepath to get correct variable group
            string group_guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath).ToString();
            #endregion

            #region Get guid from script caller filepath, caller function and caller line number to create variable name guid
            if (string.IsNullOrEmpty(guid?.Trim()))
            {
                guid = UdonVR_Encryption.GuidFromStringMD5(callerFilePath + "_" + callerMemberName + "_" + callerLineNumber.ToString() + "nullable_fieldArea").ToString();
            }
            #endregion

            object _output = null;
            try
            {
                _output = UdonVR_VariableStorage.Instance.GetVariableGroup(group_guid).GetVariable(guid); // load the variable
            }
            catch (System.Exception) { }
            Rect _rect = _output != null ? (Rect)Convert.ChangeType(_output, typeof(Rect)) : Rect.zero; // get the rectangle from the variable object
            if (_rect.width > EditorGUIUtility.currentViewWidth) _rect.width = EditorGUIUtility.currentViewWidth; // restrict width to the maximum view width
            float _width = _rect != Rect.zero ? _rect.width / fieldContents.Length : -1; // get the width / number of fields
            _width -= (GUI.skin.button.padding.left + GUI.skin.button.padding.right) / 2;

            bool isEven = fieldContents.Length % 2 == 0 ? true : false; // check if we have an even number of fields
            int middle = fieldContents.Length / 2; // get the middle slider index
            bool evenSplit = _rect.width % fieldContents.Length == 0 ? true : false; // can the current rect width be split evenly between the fields

            // begin area and set the style to use for the area
            GUILayout.BeginHorizontal(showDebugContainers == false ? areaStyle != null ? areaStyle : GUIStyle.none : UdonVR_Style.Get(new Vector4(0.25f, 0.25f, 0.5f, 1f)));
            for (int i = 0; i < fieldContents.Length; i++)
            {
                if (showDebugContainers) GUILayout.BeginHorizontal(i % 2 == 0 ? UdonVR_Style.Get(Color.red * Color.grey) : UdonVR_Style.Get(Color.blue * Color.grey)); // used to dislay different field areas in alternating colors during debug
                Type type = currentValues[i].type; // type of field to make

                Color _backgroundColor = GUI.backgroundColor; // save background color before modifing
                Color _textColor = GUI.contentColor; // save text color before modifing

                GUI.enabled = currentValues[i].isEnabled; // set if the button is enabled or not
                EditorGUIUtility.labelWidth = GUIStyle.none.CalcSize(fieldContents[i]).x + 5;
                GUI.backgroundColor = _backgroundColor * Color.Lerp(Color.white, currentValues[i].tintDefault, 0.15f);
                GUI.contentColor = currentValues[i].textColor.HasValue ? currentValues[i].textColor.Value : _textColor;
                if (type.Equals(typeof(bool)))
                {
                    bool boolVal = currentValues[i].boolValue.Value;
                    GUI.backgroundColor = boolVal ? _backgroundColor * Color.Lerp(Color.white, currentValues[i].tintActive, 0.15f) : _backgroundColor * Color.Lerp(Color.white, currentValues[i].tintDefault, 0.15f);
                    if (GUILayout.Button(
                        fieldContents[i],
                        GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width)))
                    {
                        boolVal = !boolVal;
                    }
                    GUI.backgroundColor = _backgroundColor;
                    currentValues[i].boolValue = boolVal;
                }
                else if (type.Equals(typeof(string)))
                {
                    string strVal = currentValues[i].stringValue;
                    strVal = EditorGUILayout.TextField(
                        fieldContents[i],
                        strVal,
                        GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    currentValues[i].stringValue = strVal;
                }
                else if (type.Equals(typeof(int)))
                {
                    int intVal = currentValues[i].intValue.Value;
                    if (!currentValues[i].isSlider)
                    {
                        intVal = EditorGUILayout.IntField(
                            fieldContents[i],
                            intVal,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    else
                    {
                        intVal = EditorGUILayout.IntSlider(
                            fieldContents[i],
                            intVal,
                            (int)currentValues[i].sliderMin,
                            (int)currentValues[i].sliderMax,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    currentValues[i].intValue = intVal;
                }
                else if (type.Equals(typeof(float)))
                {
                    float floatVal = currentValues[i].floatValue.Value;
                    if (!currentValues[i].isSlider)
                    {
                        floatVal = EditorGUILayout.FloatField(
                            fieldContents[i],
                            floatVal,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    else
                    {
                        floatVal = EditorGUILayout.Slider(
                            fieldContents[i],
                            floatVal,
                            currentValues[i].sliderMin,
                            currentValues[i].sliderMax,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    currentValues[i].floatValue = floatVal;
                }
                else if (type.Equals(typeof(double)))
                {
                    double doubleVal = currentValues[i].doubleValue.Value;
                    if (!currentValues[i].isSlider)
                    {
                        doubleVal = EditorGUILayout.DoubleField(
                            fieldContents[i],
                            doubleVal,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    else
                    {
                        doubleVal = (double)EditorGUILayout.Slider(
                            fieldContents[i],
                            (float)doubleVal,
                            (float)currentValues[i].sliderMin,
                            (float)currentValues[i].sliderMax,
                            GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    }
                    currentValues[i].doubleValue = doubleVal;
                }
                else if (type.Equals(typeof(Vector2)) || type.Equals(typeof(Vector3)) || type.Equals(typeof(Vector4)))
                {
                    if (currentValues[i].vectorValue.HasValue)
                    {
                        Vector4 vectorVal = Vector4.zero;
                        if (type.Equals(typeof(Vector2)))
                        {
                            vectorVal = EditorGUILayout.Vector2Field(
                                fieldContents[i],
                                currentValues[i].vectorValue.Value,
                                GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                        }
                        if (type.Equals(typeof(Vector3)))
                        {
                            vectorVal = EditorGUILayout.Vector3Field(
                                fieldContents[i],
                                currentValues[i].vectorValue.Value,
                                GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                        }
                        if (type.Equals(typeof(Vector4)))
                        {
                            vectorVal = EditorGUILayout.Vector4Field(
                                fieldContents[i],
                                currentValues[i].vectorValue.Value,
                                GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                        }
                        currentValues[i].vectorValue = vectorVal;
                    }
                }
                else if (type.Equals(typeof(Color)))
                {
                    Color colorVal = currentValues[i].colorValue.Value;
                    colorVal = EditorGUILayout.ColorField(
                        fieldContents[i],
                        colorVal,
                        currentValues[i].colorShowEyeDropper,
                        currentValues[i].colorShowAlpha,
                        currentValues[i].colorIsHdr,
                        GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    currentValues[i].colorValue = colorVal;
                }
                else
                {
                    UnityEngine.Object objVal = (UnityEngine.Object)Convert.ChangeType(currentValues[i].objectValue, typeof(UnityEngine.Object));
                    objVal = EditorGUILayout.ObjectField(
                        fieldContents[i],
                        objVal,
                        type,
                        true,
                        GUILayout.Width(!isEven && i == middle && !evenSplit ? _width + 1 : _width));
                    currentValues[i].objectValue = objVal;
                }
                actions[i](currentValues[i]); // output the current values to the actions
                GUI.contentColor = _textColor; // reset text color
                GUI.backgroundColor = _backgroundColor; // reset background color
                EditorGUIUtility.labelWidth = 0; // reset width
                GUI.enabled = true; // re enable GUI elements

                if (showDebugContainers) GUILayout.EndHorizontal(); // used to dislay different field areas in alternating colors during debug
                if (i < fieldContents.Length - 1) GUILayout.FlexibleSpace(); // add flexable space between the buttons
            }
            GUILayout.EndHorizontal();


            // if were repainting then save the size of the rect
            if (Event.current.type == EventType.Repaint)
            {
                _rect = GUILayoutUtility.GetLastRect();
                try
                {
                    UdonVR_VariableStorage.Instance.GetVariableGroup(group_guid).SetVariable(guid, _rect); // save the rect
                }
                catch (System.Exception) { }
            }
        }
        #endregion
    }

    public static class UdonVR_Predefined
    {
        // created by Hamster9090901

        public enum Color
        {
            // general
            General_Clear,

            // background
            Background_UnityDefault,
            Background_Default,
            Background_Light,

            // style colors
            Style_DefaultTextColor,
        }

        private static Vector4[] PrefefinedColors = {
            // general
            new Vector4(0f, 0f, 0f, 0f), // clear

            // background
            UdonVR_ColorHelpers.FromRGB(56f, 56f, 56f, 255f), // unity default
            new Vector4(0.25f, 0.25f, 0.25f, 1f), // default
            new Vector4(0.3f, 0.3f, 0.3f, 1f), // light

            // style
            GUI.skin.button.normal.textColor, // default text color
        };

        /// <summary>
        /// Returns a predefined color.
        /// </summary>
        /// <param name="color"> Predefined Color enum. </param>
        /// <returns></returns>
        public static Vector4 GetColor(Color color)
        {
            return PrefefinedColors[(int)color];
        }
    }

    public static class UdonVR_Style
    {
        // created by Hamster9090901

        /// <summary>
        /// Gets a blank GUI style with a background color.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="color"> Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <returns></returns>
        public static GUIStyle Get<GenericColor>(GenericColor color)
        {
            Vector4 _color = UdonVR_ColorHelpers.GetFromGenericColor(color); // get color from generic color
            GUIStyle _style = new GUIStyle();
            Texture2D _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, _color);
            _texture.Apply();
            _style.normal.background = _texture;
            return _style;
        }

        /// <summary>
        /// Gets a GUI style with a fixed Width and Height
        /// </summary>
        /// <param name="width"> Width of GUI Style. </param>
        /// <param name="height"> Height of GUI Style. </param>
        /// <param name="style"> Style to copy. </param>
        /// <returns></returns>
        public static GUIStyle SetWidthHeight(float width, float height, GUIStyle style = null)
        {
            // get new style or new style based off input style
            GUIStyle _style = style != null ? new GUIStyle(style) : new GUIStyle();
            if (width != -1) _style.fixedWidth = width; // set width
            if (height != -1) _style.fixedHeight = height; // set height

            return _style; // return new style
        }

        /// <summary>
        /// Gets a GUI style with a fixed Width and Height, And padding offset
        /// </summary>
        /// <param name="width"> Width of GUI Style</param>
        /// <param name="height"> Height of GUI Style. </param>
        /// <param name="style"> Style to copy. </param>
        /// <param name="padding"> Padding to apply to style. </param>
        /// <returns></returns>
        public static GUIStyle SetWidthHeight(float width, float height, GUIStyle style, RectOffset padding = null)
        {
            GUIStyle _style = SetWidthHeight(width, height, style);
            if (padding != null) _style.padding = padding;

            return _style;
        }

        public static GUIStyle SetPadding(GUIStyle style, RectOffset padding)
        {
            GUIStyle _style = style != null ? new GUIStyle(style) : new GUIStyle();
            _style.padding = padding;

            return _style;
        }

        public static GUIStyle SetTextSettings<GenericColor>(GUIStyle style, GenericColor genericColor, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            GUIStyle _style = style != null ? new GUIStyle(style) : new GUIStyle();
            _style.normal.textColor = UdonVR_ColorHelpers.GetFromGenericColor(genericColor);
            _style.alignment = alignment;

            return _style;
        }

        public static GUIStyle SetFontSettings(GUIStyle style, Font font = null, int fontSize = -1)
        {
            GUIStyle _style = style != null ? new GUIStyle(style) : new GUIStyle();
            if (font != null) _style.font = font;
            if (fontSize != -1) _style.fontSize = fontSize;

            return _style;
        }

        public static GUIStyle SetFontSettings(GUIStyle style, Font font = null, int fontSize = -1, FontStyle fontStyle = FontStyle.Normal)
        {
            GUIStyle _style = SetFontSettings(style, font, fontSize);
            _style.fontStyle = fontStyle;

            return _style;
        }
    }
}
