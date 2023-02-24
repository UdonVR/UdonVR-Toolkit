using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public static class UdonVR_ColorHelpers
    {
        // created by Hamster9090901

        /// <summary>
        /// Get Color from RGB input.
        /// </summary>
        /// <param name="red"> Red value. </param>
        /// <param name="green"> Green value. </param>
        /// <param name="blue"> Blue value. </param>
        /// <param name="alpha"> Alpha value. </param>
        /// <returns></returns>
        public static Color FromRGB(float red, float green, float blue, float alpha = -1f)
        {
            bool _isRGB = red > 1.0f || green > 1.0f || blue > 1.0f || alpha > 1.0f ? true : false;

            return new Color
            {
                r = _isRGB ? red / 255.0f : red,
                g = _isRGB ? green / 255.0f : green,
                b = _isRGB ? blue / 255.0f : blue,
                a = alpha == -1 ? 1.0f : _isRGB ? alpha / 255.0f : 1.0f,
            };
        }

        /// <summary>
        /// Get Color from RGB input. Vector3 for RGB input.
        /// </summary>
        /// <param name="rgb"> Red Green Blue values.</param>
        /// <returns></returns>
        public static Color FromRGB(Vector3 rgb)
        {
            return FromRGB(rgb.x, rgb.y, rgb.z);
        }

        /// <summary>
        /// Get Color from RGB input. Vector4 for RGBA input.
        /// </summary>
        /// <param name="rgba"> Red Green Blue Alpha values. </param>
        /// <returns></returns>
        public static Color FromRGB(Vector4 rgba)
        {
            return FromRGB(rgba.x, rgba.y, rgba.z, rgba.w);
        }

        /// <summary>
        /// Returns the color in a RGB format with a range of 0 to 255
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToRGB(this Color color)
        {
            return new Color
            {
                r = color.r * 255.0f,
                g = color.g * 255.0f,
                b = color.b * 255.0f,
                a = color.a * 255.0f
            };
        }

        /// <summary>
        /// Takes an input color and returns it in a Vector4 format.
        /// </summary>
        /// <typeparam name="GenericColor"></typeparam>
        /// <param name="color"> Generic Color, Vector4 | Color | UdonVR_Predefined.Color </param>
        /// <returns></returns>
        public static Vector4 GetFromGenericColor<GenericColor>(GenericColor color)
        {
            Vector4 _color = Vector4.one;
            if (typeof(GenericColor) == typeof(Vector4))
            {
                _color = (Vector4)Convert.ChangeType(color, typeof(Vector4));
            }
            else if (typeof(GenericColor) == typeof(Color))
            {
                _color = (Vector4)(Color)Convert.ChangeType(color, typeof(Color));
            }
            else if (typeof(GenericColor) == typeof(UdonVR_Predefined.Color))
            {
                _color = UdonVR_Predefined.GetColor((UdonVR_Predefined.Color)Convert.ChangeType(color, typeof(UdonVR_Predefined.Color)));
            }
            return _color;
        }
    }

    public static class UdonVR_EnumHelpers
    {
        /// <summary>
        /// Gets an Integer from Enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"> Enum to get integer from. </param>
        /// <returns></returns>
        public static int IntFromEnum<T>(T value)
        {
            Enum _enum = (Enum)Convert.ChangeType(value, typeof(T));
            return (int)(object)_enum;
        }

        /// <summary>
        /// Gets an Enum from an Integer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="integer"> Integer to get enum for. </param>
        /// <param name="type"> Typeof enum to return. </param>
        /// <returns></returns>
        public static Enum EnumFromInt(int integer, Type type)
        {
            return (Enum)Enum.ToObject(type, integer);
        }
    }

    public static class ArrayHelpers
    {
        /// <summary>
        /// Appends an item to the passed array lengthing it in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to append item to. </param>
        /// <param name="item"> Item to append. </param>
        /// <returns></returns>
        public static T[] ArrayAppendItem<T>(T[] array, T item)
        {
            array = ArrayIncreaseLength(array);
            array[array.Length - 1] = item;
            return array;
        }

        /// <summary>
        /// Appends items to the passed array lengthing it in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to append items to. </param>
        /// <param name="items"> Optional items to append to the end of the array. </param>
        /// <returns></returns>
        public static T[] ArrayAppendItem<T>(T[] array, params T[] items)
        {
            int _itemCount = items.Length;
            int _previousLength = array.Length;
            array = ArrayIncreaseLength(array, _itemCount);

            for (int _i = 0; _i < _itemCount; _i++)
            {
                array[_previousLength + _i] = items[_i];
            }

            return array;
        }

        /// <summary>
        /// Removes an item from the passed array shortening it in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to remove item from. </param>
        /// <param name="item"> Item to remove. (Will remove first found) </param>
        /// <returns></returns>
        public static T[] ArrayRemoveItem<T>(T[] array, T item)
        {
            array = ArrayDecreaseLength(array, array.FindIndex(item));
            return array;
        }

        /// <summary>
        /// Removes an index from the passed array shortening it in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to remove item from. </param>
        /// <param name="index"> Index to remove. </param>
        /// <returns></returns>
        public static T[] ArrayRemoveItem<T>(T[] array, int index)
        {
            array = ArrayDecreaseLength(array, index);
            return array;
        }

        /// <summary>
        /// Removes an index from the passed array shortening it in the process.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to remove item from. </param>
        /// <param name="index"> Index to remove. </param>
        /// <param name="count"> Number of additional indexes / indices to remove. </param>
        /// <returns></returns>
        public static T[] ArrayRemoveItem<T>(T[] array, int index, int count)
        {
            array = ArrayDecreaseLength(array, index, count);
            return array;
        }

        /// <summary>
        /// Increases the length of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to increase the length of. </param>
        /// <param name="amount"> Amount of slots to add to the end of the array. </param>
        /// <returns></returns>
        public static T[] ArrayIncreaseLength<T>(T[] array, int amount = 1)
        {
            T[] _extendedArray = new T[array.Length + amount];
            for (int _i = 0; _i < array.Length; _i++)
            {
                _extendedArray[_i] = array[_i];
            }
            for (int _i = array.Length + 1; _i < _extendedArray.Length; _i++)
            {
                _extendedArray[_i] = default(T);
            }
            return _extendedArray;
        }

        /// <summary>
        /// Decreases the length of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to decrease the length of. </param>
        /// <param name="index"> Index to remove. (-1 is last index in the array) </param>
        /// <returns></returns>
        public static T[] ArrayDecreaseLength<T>(T[] array, int index = -1)
        {
            T[] _shortenedArray = new T[array.Length - 1];
            int _index = 0;
            for (int _i = 0; _i < array.Length; _i++)
            {
                if (_i != index)
                {
                    _shortenedArray[_index] = array[_i];
                    _index++;
                }
            }
            return _shortenedArray;
        }

        /// <summary>
        /// Decreases the length of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"> Array to decrease the length of. </param>
        /// <param name="index"> Index to remove. </param>
        /// <param name="count"> Number of additional indexes / indices to remove. </param>
        /// <returns></returns>
        public static T[] ArrayDecreaseLength<T>(T[] array, int index, int count = 0)
        {
            T[] _shortenedArray = new T[(array.Length - 1) - count];
            int _index = 0;
            for (int _i = 0; _i < array.Length; _i++)
            {
                if (!(_i >= index && _i <= index + count))
                {
                    _shortenedArray[_index] = array[_i];
                    _index++;
                }
            }
            return _shortenedArray;
        }
    }

    public static class UdonVR_MathHelpers
    {
        // created by Hamster9090901

        #region Remap
        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static float Remap(float value, float low1, float high1, float low2, float high2)
        {
            return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector2 Remap(Vector2 value, float low1, float high1, float low2, float high2)
        {
            return new Vector2
            {
                x = Remap(value.x, low1, high1, low2, high2),
                y = Remap(value.y, low1, high1, low2, high2)
            };
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector2 Remap(Vector2 value, Vector2 low1, Vector2 high1, Vector2 low2, Vector2 high2)
        {
            return new Vector2
            {
                x = Remap(value.x, low1.x, high1.x, low2.x, high2.x),
                y = Remap(value.y, low1.y, high1.y, low2.y, high2.y)
            };
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector3 Remap(Vector3 value, float low1, float high1, float low2, float high2)
        {
            return new Vector3
            {
                x = Remap(value.x, low1, high1, low2, high2),
                y = Remap(value.y, low1, high1, low2, high2),
                z = Remap(value.z, low1, high1, low2, high2)
            };
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector3 Remap(Vector3 value, Vector3 low1, Vector3 high1, Vector3 low2, Vector3 high2)
        {
            return new Vector3
            {
                x = Remap(value.x, low1.x, high1.x, low2.x, high2.x),
                y = Remap(value.y, low1.y, high1.y, low2.y, high2.y),
                z = Remap(value.z, low1.z, high1.z, low2.z, high2.z)
            };
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector4 Remap(Vector4 value, float low1, float high1, float low2, float high2)
        {
            return new Vector4
            {
                x = Remap(value.x, low1, high1, low2, high2),
                y = Remap(value.y, low1, high1, low2, high2),
                z = Remap(value.z, low1, high1, low2, high2),
                w = Remap(value.w, low1, high1, low2, high2)
            };
        }

        /// <summary>
        /// Changes value range from low1 -> high1 (Current range) to low2 -> high2 (New range)
        /// </summary>
        /// <param name="value"> Value to change range of. </param>
        /// <param name="low1"> Current minimum. </param>
        /// <param name="high1"> Current maximum. </param>
        /// <param name="low2"> New minimum. </param>
        /// <param name="high2"> New maximum. </param>
        /// <returns></returns>
        public static Vector4 Remap(Vector4 value, Vector4 low1, Vector4 high1, Vector4 low2, Vector4 high2)
        {
            return new Vector4
            {
                x = Remap(value.x, low1.x, high1.x, low2.x, high2.x),
                y = Remap(value.y, low1.y, high1.y, low2.y, high2.y),
                z = Remap(value.z, low1.z, high1.z, low2.z, high2.z),
                w = Remap(value.w, low1.w, high1.w, low2.w, high2.w)
            };
        }
        #endregion

        #region Lerp
        /// <summary>
        /// Lerp between values. (Linear Interpolation)
        /// </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="t"> Time. </param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        /// Lerp between values. (Linear Interpolation)
        /// </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="t"> Time. </param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return new Vector2
            {
                x = Lerp(a.x, b.x, t),
                y = Lerp(a.y, b.y, t)
            };
        }

        /// <summary>
        /// Lerp between values. (Linear Interpolation)
        /// </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="t"> Time. </param>
        /// <returns></returns>
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return new Vector3
            {
                x = Lerp(a.x, b.x, t),
                y = Lerp(a.y, b.y, t),
                z = Lerp(a.z, b.z, t)
            };
        }

        /// <summary>
        /// Lerp between values. (Linear Interpolation)
        /// </summary>
        /// <param name="a"> Start value. </param>
        /// <param name="b"> End value. </param>
        /// <param name="t"> Time. </param>
        /// <returns></returns>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return new Vector4
            {
                x = Lerp(a.x, b.x, t),
                y = Lerp(a.y, b.y, t),
                z = Lerp(a.z, b.z, t),
                w = Lerp(a.w, b.w, t)
            };
        }
        #endregion

        #region ClampMin
        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static float ClampMin(float value, float minimum)
        {
            return value < minimum ? minimum : value;
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector2 ClampMin(Vector2 value, float minimum)
        {
            return new Vector2
            {
                x = value.x < minimum ? minimum : value.x,
                y = value.y < minimum ? minimum : value.y
            };
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector2 ClampMin(Vector2 value, Vector2 minimum)
        {
            return new Vector2
            {
                x = value.x < minimum.x ? minimum.x : value.x,
                y = value.y < minimum.y ? minimum.y : value.y
            };
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector3 ClampMin(Vector3 value, float minimum)
        {
            return new Vector3
            {
                x = value.x < minimum ? minimum : value.x,
                y = value.y < minimum ? minimum : value.y,
                z = value.z < minimum ? minimum : value.z
            };
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector3 ClampMin(Vector3 value, Vector3 minimum)
        {
            return new Vector3
            {
                x = value.x < minimum.x ? minimum.x : value.x,
                y = value.y < minimum.y ? minimum.y : value.y,
                z = value.z < minimum.z ? minimum.z : value.z
            };
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector4 ClampMin(Vector4 value, float minimum)
        {
            return new Vector4
            {
                x = value.x < minimum ? minimum : value.x,
                y = value.y < minimum ? minimum : value.y,
                z = value.z < minimum ? minimum : value.z,
                w = value.w < minimum ? minimum : value.w
            };
        }

        /// <summary>
        /// Clamps the value to be above or equal to the minimum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <returns></returns>
        public static Vector4 ClampMin(Vector4 value, Vector4 minimum)
        {
            return new Vector4
            {
                x = value.x < minimum.x ? minimum.x : value.x,
                y = value.y < minimum.y ? minimum.y : value.y,
                z = value.z < minimum.z ? minimum.z : value.z,
                w = value.w < minimum.w ? minimum.w : value.w
            };
        }
        #endregion

        #region ClampMax
        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static float ClampMax(float value, float maximum)
        {
            return value > maximum ? maximum : value;
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector2 ClampMax(Vector2 value, float maximum)
        {
            return new Vector2
            {
                x = value.x > maximum ? maximum : value.x,
                y = value.y > maximum ? maximum : value.y
            };
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector2 ClampMax(Vector2 value, Vector2 maximum)
        {
            return new Vector2
            {
                x = value.x > maximum.x ? maximum.x : value.x,
                y = value.y > maximum.y ? maximum.y : value.y
            };
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector3 ClampMax(Vector3 value, float maximum)
        {
            return new Vector3
            {
                x = value.x > maximum ? maximum : value.x,
                y = value.y > maximum ? maximum : value.y,
                z = value.z > maximum ? maximum : value.z
            };
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector3 ClampMax(Vector3 value, Vector3 maximum)
        {
            return new Vector3
            {
                x = value.x > maximum.x ? maximum.x : value.x,
                y = value.y > maximum.y ? maximum.y : value.y,
                z = value.z > maximum.z ? maximum.z : value.z
            };
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector4 ClampMax(Vector4 value, float maximum)
        {
            return new Vector4
            {
                x = value.x > maximum ? maximum : value.x,
                y = value.y > maximum ? maximum : value.y,
                z = value.z > maximum ? maximum : value.z,
                w = value.w > maximum ? maximum : value.w
            };
        }

        /// <summary>
        /// Clamps the value to be below or equal to the maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector4 ClampMax(Vector4 value, Vector4 maximum)
        {
            return new Vector4
            {
                x = value.x > maximum.x ? maximum.x : value.x,
                y = value.y > maximum.y ? maximum.y : value.y,
                z = value.z > maximum.z ? maximum.z : value.z,
                w = value.w > maximum.w ? maximum.w : value.w
            };
        }
        #endregion

        #region Clamp
        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static float Clamp(float value, float minimum, float maximum)
        {
            value = ClampMin(value, minimum);
            value = ClampMax(value, maximum);
            return value;
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector2 Clamp(Vector2 value, float minimum, float maximum)
        {
            return new Vector2
            {
                x = Clamp(value.x, minimum, maximum),
                y = Clamp(value.y, minimum, maximum)
            };
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector2 Clamp(Vector2 value, Vector2 minimum, Vector2 maximum)
        {
            return new Vector2
            {
                x = Clamp(value.x, minimum.x, maximum.x),
                y = Clamp(value.y, minimum.y, maximum.y)
            };
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector3 Clamp(Vector3 value, float minimum, float maximum)
        {
            return new Vector3
            {
                x = Clamp(value.x, minimum, maximum),
                y = Clamp(value.y, minimum, maximum),
                z = Clamp(value.z, minimum, maximum)
            };
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector3 Clamp(Vector3 value, Vector3 minimum, Vector3 maximum)
        {
            return new Vector3
            {
                x = Clamp(value.x, minimum.x, maximum.x),
                y = Clamp(value.y, minimum.y, maximum.y),
                z = Clamp(value.z, minimum.z, maximum.z)
            };
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector4 Clamp(Vector4 value, float minimum, float maximum)
        {
            return new Vector4
            {
                x = Clamp(value.x, minimum, maximum),
                y = Clamp(value.y, minimum, maximum),
                z = Clamp(value.z, minimum, maximum),
                w = Clamp(value.w, minimum, maximum)
            };
        }

        /// <summary>
        /// Clamps the value to be within the minimum and maximum.
        /// </summary>
        /// <param name="value"> Value to clamp. </param>
        /// <param name="minimum"> Minimum the value can be. </param>
        /// <param name="maximum"> Maximum the value can be. </param>
        /// <returns></returns>
        public static Vector4 Clamp(Vector4 value, Vector4 minimum, Vector4 maximum)
        {
            return new Vector4
            {
                x = Clamp(value.x, minimum.x, maximum.x),
                y = Clamp(value.y, minimum.y, maximum.y),
                z = Clamp(value.z, minimum.z, maximum.z),
                w = Clamp(value.w, minimum.w, maximum.w)
            };
        }
        #endregion
    }
}
