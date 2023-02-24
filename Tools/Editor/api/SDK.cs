using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKapi
{
    // Start is called before the first frame update
    public static void UploadImage(Texture2D _img)
    {

    }
    public static void UploadImage(Texture _img)
    {
        Texture2D _temp = Texture2D.CreateExternalTexture(_img.width,_img.height,TextureFormat.RGB24,false, false,_img.GetNativeTexturePtr());
        UploadImage(_temp);
    }
    public static void UploadTags(string _tags)
    {

    }
    public static void UploadTitle(string _title)
    {

    }
}
