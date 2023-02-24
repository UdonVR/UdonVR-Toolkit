using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UdonVR_CameraImage : MonoBehaviour
{
    public int resWidth = 1200; // resolution width of saved image
    public int resHeight = 900; // resolution height of saved image

    private new Camera camera; // camera to take image from

    /// <summary>
    /// Formats the final string to contain the date and the resolution of the image in the final name.
    /// </summary>
    /// <param name="path"> Path to save image. </param>
    /// <param name="name"> Name of saved image. </param>
    /// <param name="width"> Width of saved image. </param>
    /// <param name="height"> Height of saved image. </param>
    /// <returns> Formated FileName and Path. </returns>
    private string ScreenShotName(string path, string name, int width, int height)
    {
        if (path.EndsWith("/") || path.EndsWith("\\")) path = path.Remove(path.Length - 1, 1); // remove unnessasary slashes

        path = Path.GetFullPath(path); // get full path from relative path

        return string.Format("{0}\\{1}_{2}x{3}_{4}.png",
            path, name,
            width, height,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    /// <summary>
    /// Takes an image using the camera the Script is attatched to.
    /// </summary>
    /// <param name="fileName"> Name of file when saved. </param>
    /// <param name="filePath"> Absolute path to save the file. </param>
    /// <param name="removeComponentAfter"> Remove the Script from the camera when finished. </param>
    public void TakeImage(string fileName, ref string filePath, bool removeComponentAfter = true)
    {
        if (camera == null) camera = GetComponent<Camera>(); // get camera component
        if (camera == null) Debug.LogError("Camera Component could not be found on GameObject."); // throw error if no camera was found

        #region Get screenshot from camera to save
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24); // create render texture
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false); // create image to save render texture to
        camera.targetTexture = rt; // set camera render texture
        camera.Render(); // enable camera render texture
        RenderTexture.active = rt; // set active render texture to created render texture
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0); // save pixels of render texture
        camera.targetTexture = null; // remove render texture
        RenderTexture.active = null; // disable render texture to be save

#if UNITY_EDITOR
        DestroyImmediate(rt); // remove render texture
#else
        Destroy(rt); // remove render texture
#endif
        #endregion

        #region Save image to disc
        byte[] _bytes = screenShot.EncodeToPNG(); // convert screenshot to byte array
        string _filePath = ScreenShotName(filePath, fileName, resWidth, resHeight); // get formated filepath
        System.IO.File.WriteAllBytes(_filePath, _bytes); // save screenshot to disc
        Debug.Log(string.Format("Took screenshot to: {0}", _filePath)); // send message to console that a file was saved
        #endregion

        filePath = _filePath; // set filepath to the formated filepath for loading the image

        #region Remove component after screenshot is saved
#if UNITY_EDITOR
        if (removeComponentAfter) DestroyImmediate(this); // remove script
#else
        if (removeComponentAfter) Destroy(this); // remove script
#endif
        #endregion
    }

    /// <summary>
    /// Takes an image using the camera the Script is attatched to.
    /// </summary>
    /// <param name="fileName"> Name of file when saved. </param>
    /// <param name="filePath"> Absolute path to save the file. </param>
    /// <param name="width"> Sets resolution Width for image set to -1 to use default. </param>
    /// <param name="height"> Sets resolution Height for image set to -1 to use default.</param>
    /// <param name="removeComponentAfter"> Remove the Script from the camera when finished. </param>
    public void TakeImage(string fileName, ref string filePath, int width = -1, int height = -1, bool removeComponentAfter = true)
    {
        if (width != -1) resWidth = width;
        if (height != -1) resHeight = height;

        TakeImage(fileName, ref filePath, removeComponentAfter);
    }
}

