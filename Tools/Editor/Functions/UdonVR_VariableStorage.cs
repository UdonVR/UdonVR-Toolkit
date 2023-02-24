using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public class UdonVR_VariableStorage
    {
        // created by Hamster9090901

        #region Get and Create Instance
        private static UdonVR_VariableStorage instance;

        /// <summary>
        /// Get instance / Create one if there wasn't one.
        /// </summary>
        public static UdonVR_VariableStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UdonVR_VariableStorage();
                    instance.groups = new Hamster_GenericDictionary<object, UdonVR_Variables<object, object>>();
                    instance.groups.Initalize();
                }
                return instance;
            }
        }
        #endregion

        private Hamster_GenericDictionary<object, UdonVR_Variables<object, object>> groups;

        /// <summary>
        /// Get variable group if it exists and create a new group if it dosen't.
        /// </summary>
        /// <param name="name"> Name of the group. If left empty generates a group. </param>
        /// <returns></returns>
        public UdonVR_Variables<object, object> GetVariableGroup(
            string name = null,
            [System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            #region Get guid from script caller filepath to get correct variable group
            if (string.IsNullOrEmpty(name?.Trim())) name = UdonVR_Encryption.GuidFromStringMD5(callerFilePath).ToString();
            #endregion

            groups.TryGetValue(name, out UdonVR_Variables<object, object> _value, out _); // get the group

            // create new group and get the created group
            if (_value == null)
            {
                groups.Add(name, new UdonVR_Variables<object, object>());
                groups.TryGetValue(name, out _value, out _);
            }

            return _value;
        }

        /// <summary>
        /// List all the variable groups and their contents
        /// </summary>
        public void ListContainers()
        {
            for (int i = 0; i < groups.Count; i++)
            {
                Debug.Log("Container: " + groups.Keys[i]);
                groups.TryGetValue(groups.Keys[i], out UdonVR_Variables<object, object> _value, out _);
                _value.ListVariables(4);
            }
        }

        public class UdonVR_Variables<TKey, TValue>
        {
            /// <summary>
            /// Dictionary containing variable names and values
            /// </summary>
            private Hamster_GenericDictionary<object, object> variables;

            /// <summary>
            /// Initalize variable dictinary if it hasn't been initalized.
            /// </summary>
            public void Initalize()
            {
                if (variables == null)
                {
                    variables = new Hamster_GenericDictionary<object, object>();
                    variables.Initalize();
                }
            }

            /// <summary>
            /// Get the value of the variable with the same name.
            /// </summary>
            /// <param name="name"> Name of the variable. </param>
            /// <returns></returns>
            public object GetVariable(object name)
            {
                if (variables == null) Initalize();
                variables.TryGetValue(name, out object _value, out _);
                return _value;
            }

            /// <summary>
            /// Save a value to a variable with the same name.
            /// </summary>
            /// <param name="name"> Variable name to save the value to. </param>
            /// <param name="value"> Value to set the variable to. </param>
            public void SetVariable(object name, object value)
            {
                if (variables == null) variables.Initalize();
                variables.SetValue(name, value);
            }

            /// <summary>
            /// List variables.
            /// </summary>
            public void ListVariables(int indentLevel = 0)
            {
                if (variables == null) Initalize();
                variables.ListContents(indentLevel);
            }

            /// <summary>
            /// Clear variables.
            /// </summary>
            public void ClearVariables()
            {
                if (variables == null)
                {
                    Initalize();
                }
                else
                {
                    variables.Clear();
                }
            }
        }
    }

    public class Hamster_GenericDictionary<TKey, TValue>
    {
        private List<TKey> keys = null;
        private List<TValue> values = null;

        /// <summary>
        /// Number of key value pairs in the dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return keys.Count;
            }
        }

        /// <summary>
        /// Keys in the dictionary.
        /// </summary>
        public List<TKey> Keys
        {
            get
            {
                return keys;
            }
        }

        /// <summary>
        /// Values in the dictionary.
        /// </summary>
        public List<TValue> Values
        {
            get
            {
                return values;
            }
        }

        /// <summary>
        /// Initalize the dictionary
        /// </summary>
        public void Initalize()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        /// <summary>
        /// Checks to see if the dictionary was initalized or not.
        /// </summary>
        /// <returns></returns>
        public bool IsInitalized()
        {
            return keys != null && values != null ? true : false;
        }

        /// <summary>
        /// Clear the dictionary.
        /// </summary>
        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        /// <summary>
        /// Add a key value pair to the dictionary.
        /// </summary>
        /// <param name="key"> Key to add. </param>
        /// <param name="value"> Value to add. </param>
        public void Add(TKey key, TValue value)
        {
            if (!IsInitalized()) return;
            keys.Add(key);
            values.Add(value);
        }

        /// <summary>
        /// Try to get a Value in the dictionary.
        /// </summary>
        /// <param name="key"> Key to find the value of. </param>
        /// <param name="value"> Found value. ("out _" to ignore output.)</param>
        /// <param name="index"> Index of the found value. ("out _" to ignore output.) </param>
        /// <returns> True if Value was found. </returns>
        public bool TryGetValue(TKey key, out TValue value, out int index)
        {
            value = default(TValue); // set output value to null
            index = -1; // set output index to -1 (null)

            if (!IsInitalized() || !ContainsKey(key) || Count == 0) return false;

            int _index = keys.BinarySearch(key); // binary search for key (-1 if not found)
            if (_index < 0) return false; // exit if no key was found

            value = values[_index]; // output value
            index = _index; // output index
            return true;
        }

        /// <summary>
        /// Remove a key value pair from the dictionary.
        /// </summary>
        /// <param name="key"> Key to remove. </param>
        public void Remove(TKey key)
        {
            if (!IsInitalized()) return;

            if (!TryGetValue(key, out TValue _value, out _)) return;  // exit no value was found

            keys.Remove(key); // remove key
            values.Remove(_value); // remove value
        }

        /// <summary>
        /// Update a key value pair in the dictionary.
        /// </summary>
        /// <param name="key"> Key of value to update. </param>
        /// <param name="value"> New Value. </param>
        public void Update(TKey key, TValue value)
        {
            if (!IsInitalized()) return;

            if (!TryGetValue(key, out _, out int _valueIndex)) return; // exit if no value was found

            values[_valueIndex] = value; // upade the value at the index to the new value
        }

        /// <summary>
        /// Set a value in the dictionary.
        /// </summary>
        /// <param name="key"> Key in the dictionary. </param>
        /// <param name="value"> Value to save. </param>
        public void SetValue(TKey key, TValue value)
        {
            if (!IsInitalized()) return;

            // try to get the value and if we found it upade it with the new value
            if (TryGetValue(key, out _, out _))
            {
                Update(key, value); // update the key value pair
                return; // exit after update so we dont add a new key value pair
            }

            Add(key, value); // add new key value pair
        }

        /// <summary>
        /// Does the dictionary contain this key.
        /// </summary>
        /// <param name="key"> Key to check for. </param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }

        /// <summary>
        /// Does the dictionary contain this value.
        /// </summary>
        /// <param name="value"> Value to check for. </param>
        /// <returns></returns>
        public bool ContainsValue(TValue value)
        {
            return values.Contains(value);
        }

        /// <summary>
        /// Lists the content of the dictionary in the Console.
        /// </summary>
        public void ListContents(int indentLevel = 0)
        {
            if (!IsInitalized()) return;

            string _indent = "";
            for (int i = 0; i < indentLevel; i++)
            {
                _indent += " ";
            }

            for (int i = 0; i < Count; i++)
            {
                TKey _key = keys[i];
                TValue _value = values[i];
                Debug.Log(_indent + "Key: " + _key + " | Value: " + _value);
            }
        }
    }
}