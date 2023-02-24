using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public static class UdonVR_Extensions
    {
        // created by Hamster9090901

        #region String
        /// <summary>
        /// Takes a string and makes the first letter uppercase and returns the string.
        /// </summary>
        /// <param name="str"> String to uppercase the first letter of. </param>
        /// <returns> String </returns>
        public static string ToUpperFirst(this string str)
        {
            if (str.Length == 1) return str[0].ToString().ToUpper();
            return str[0].ToString().ToUpper() + str.Substring(1);
        }
        #endregion

        #region Vector
        #region Vector2
        /// <summary>
        /// Set xy components of existing Vector2.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <returns></returns>
        public static Vector2 Set(this Vector2 vector, Vector2 newXY)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            return vector;
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Set xy, z components of existing Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 Set(this Vector3 vector, Vector2 newXY, float newZ)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            vector.z = newZ;
            return vector;
        }

        /// <summary>
        /// Set x, yz components of existing Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="x"></param>
        /// <param name="yz"></param>
        /// <returns></returns>
        public static Vector3 Set(this Vector3 vector, float newX, Vector2 newXY)
        {
            vector.x = newX;
            vector.y = newXY.x;
            vector.z = newXY.y;
            return vector;
        }

        /// <summary>
        /// Set xyz components of existing Vector3.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXYZ"></param>
        /// <returns></returns>
        public static Vector3 Set(this Vector3 vector, Vector3 newXYZ)
        {
            vector.x = newXYZ.x;
            vector.y = newXYZ.y;
            vector.z = newXYZ.z;
            return vector;
        }
        #endregion

        #region Vector4
        /// <summary>
        /// Set xy, z, w components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <param name="newZ"></param>
        /// <param name="newW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, Vector2 newXY, float newZ, float newW)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            vector.z = newZ;
            vector.w = newW;
            return vector;
        }

        /// <summary>
        /// Set x, yz, w components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newX"></param>
        /// <param name="newYZ"></param>
        /// <param name="newW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, float newX, Vector2 newYZ, float newW)
        {
            vector.x = newX;
            vector.y = newYZ.x;
            vector.z = newYZ.y;
            vector.w = newW;
            return vector;
        }

        /// <summary>
        /// Set x, y, zw components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newX"></param>
        /// <param name="newY"></param>
        /// <param name="newZW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, float newX, float newY, Vector2 newZW)
        {
            vector.x = newX;
            vector.y = newY;
            vector.z = newZW.x;
            vector.w = newZW.y;
            return vector;
        }

        /// <summary>
        /// Set xy, zw components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <param name="newZW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, Vector2 newXY, Vector2 newZW)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            vector.z = newZW.x;
            vector.w = newZW.y;
            return vector;
        }

        /// <summary>
        /// Set xyz, w components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXYZ"></param>
        /// <param name="newW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, Vector3 newXYZ, float newW)
        {
            vector.x = newXYZ.x;
            vector.y = newXYZ.y;
            vector.z = newXYZ.z;
            vector.w = newW;
            return vector;
        }

        /// <summary>
        /// Set x, yzw components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newX"></param>
        /// <param name="newYZW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, float newX, Vector3 newYZW)
        {
            vector.x = newX;
            vector.y = newYZW.x;
            vector.z = newYZW.y;
            vector.w = newYZW.z;
            return vector;
        }

        /// <summary>
        /// Set xyzw components of existing Vector4.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXYZW"></param>
        /// <returns></returns>
        public static Vector4 Set(this Vector4 vector, Vector4 newXYZW)
        {
            vector.x = newXYZW.x;
            vector.y = newXYZW.y;
            vector.z = newXYZW.z;
            vector.w = newXYZW.w;
            return vector;
        }
        #endregion

        #region Vector2Int
        /// <summary>
        /// Set xy components of existing Vector2Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <returns></returns>
        public static Vector2Int Set(this Vector2Int vector, Vector2Int newXY)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            return vector;
        }
        #endregion

        #region Vector3Int
        /// <summary>
        /// Set xy, z components of existing Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXY"></param>
        /// <param name="newZ"></param>
        /// <returns></returns>
        public static Vector3Int Set(this Vector3Int vector, Vector2Int newXY, int newZ)
        {
            vector.x = newXY.x;
            vector.y = newXY.y;
            vector.z = newZ;
            return vector;
        }

        /// <summary>
        /// Set x, yz components of existing Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newX"></param>
        /// <param name="newYZ"></param>
        /// <returns></returns>
        public static Vector3Int Set(this Vector3Int vector, int newX, Vector2Int newYZ)
        {
            vector.x = newX;
            vector.y = newYZ.x;
            vector.z = newYZ.y;
            return vector;
        }

        /// <summary>
        /// Set xyz components of existing Vector3Int.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="newXYZ"></param>
        /// <returns></returns>
        public static Vector3Int Set(this Vector3Int vector, Vector3Int newXYZ)
        {
            vector.x = newXYZ.x;
            vector.y = newXYZ.y;
            vector.z = newXYZ.z;
            return vector;
        }
        #endregion
        #endregion

        #region Array
        /// <summary>
        /// Adds an item to the first null spot in the array if that fails it will append the item to the end of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"> Item to add. </param>
        /// <returns></returns>
        public static T[] Add<T>(this T[] array, T item)
        {
            // loop over array and find the first null spot and replace with the item
            for (int _i = 0; _i < array.Length; _i++)
            {
                if (array[_i] == null)
                {
                    array[_i] = item;
                    return array;
                }
            }

            // if there where no empty spots append item to the end of the array
            return ArrayHelpers.ArrayAppendItem(array, item);
        }

        public static T[] Add<T>(this T[] array, params T[] items)
        {
            int _itemsIndex = 0;
            // loop over array and find the null spots and replace with the items
            for (int _i = 0; _i < array.Length; _i++)
            {
                if (array[_i] == null)
                {
                    array[_i] = items[_itemsIndex];
                    _itemsIndex++;
                    //return array;
                }
            }

            // if there are left over items append them to the end of the array
            if (_itemsIndex < items.Length)
            {
                T[] _itemsLeft = ArrayHelpers.ArrayRemoveItem(items, 0, _itemsIndex - 1);
                array = ArrayHelpers.ArrayAppendItem(array, _itemsLeft);
            }

            return array;
        }

        /// <summary>
        /// Append item to the end of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"> Item to append. </param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, T item)
        {
            return ArrayHelpers.ArrayAppendItem(array, item);
        }

        /// <summary>
        /// Append items to the end of the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="items"> Items to append. </param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, params T[] items)
        {
            return ArrayHelpers.ArrayAppendItem(array, items);
        }

        /// <summary>
        /// Clear the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T[] Clear<T>(this T[] array)
        {
            return new T[0];
        }

        /// <summary>
        /// Remove item from the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"> Item to remove. </param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T item)
        {
            return ArrayHelpers.ArrayRemoveItem(array, item);
        }

        /// <summary>
        /// Remove index from the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"> Index to remove. </param>
        /// <param name="count"> Number of additional indexes / indices to remove. </param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, int index, int count = 0)
        {
            return ArrayHelpers.ArrayRemoveItem(array, index, count);
        }

        /// <summary>
        /// Find the intex of the first item if item exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"> Item to find index of. </param>
        /// <returns> -1 if no item was found. </returns>
        public static int FindIndex<T>(this T[] array, T item)
        {
            for (int _i = 0; _i < array.Length; _i++)
            {
                if (array[_i].Equals(item))
                {
                    return _i;
                }
            }
            return -1;
        }
        #endregion
    }
}
