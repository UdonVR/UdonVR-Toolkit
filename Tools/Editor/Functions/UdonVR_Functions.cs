using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Security.Cryptography;
using System.Text;

// created by Hamster9090901

namespace UdonVR.EditorUtility
{
    public class UdonVR_Functions
    {
        // created by Hamster9090901

        private static string[] fileSizes = { "B", "KB", "MB", "GB", "TB" };

        /// <summary>
        /// Get a formated string of the inputed fileSize.
        /// </summary>
        /// <param name="fileSize"> File size in bytes. </param>
        /// <returns></returns>
        public static string FormatFileSize(float fileSize)
        {
            int order = 0;
            while (fileSize >= 1024 && order < fileSizes.Length - 1)
            {
                order++;
                fileSize = fileSize / 1024;
            }
            return String.Format("{0:0.##} {1}", fileSize, fileSizes[order]);
        }

        /// <summary>
        /// Gets a random Vector4 color.
        /// </summary>
        /// <returns></returns>
        public static Vector4 RandomColor()
        {
            Vector4 output = Vector4.zero;
            output.x = UnityEngine.Random.Range(0f, 1f);
            output.y = UnityEngine.Random.Range(0f, 1f);
            output.z = UnityEngine.Random.Range(0f, 1f);
            output.w = 1f;
            return output;
        }
    }

    public class UdonVR_Encryption
    {
        // created by Hamster9090901

        #region Get and Create Instance
        private static System.Security.Cryptography.MD5 md5;

        /// <summary>
        /// Get instance / Create one if there wasn't one.
        /// </summary>
        public static System.Security.Cryptography.MD5 MD5
        {
            get
            {
                if (md5 == null)
                {
                    md5 = System.Security.Cryptography.MD5.Create();
                }
                return md5;
            }
        }
        #endregion

        /// <summary>
        /// Create a Guid from a string using MD5 hashing. (Probability of Collision: 2^20.96)
        /// </summary>
        /// <param name="input"> String to get a Guid from. </param>
        /// <returns></returns>
        public static Guid GuidFromStringMD5(string input)
        {
            byte[] hash = MD5.ComputeHash(Encoding.Default.GetBytes(input));
            Guid result = new Guid(hash);
            return result;
        }
    }
}
