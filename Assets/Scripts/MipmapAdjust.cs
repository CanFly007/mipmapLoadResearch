using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MipmapAdjust : MonoBehaviour
{
    bool isIncrease = false;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            CustomMipmap();
        }
    }

    void CustomMipmap()
    {
        if (isIncrease == false)
        {
            QualitySettings.masterTextureLimit = 3;
            //QualitySettings.globalTextureMipmapLimit = 2;
            isIncrease = true;
        }
        else
        {
            QualitySettings.masterTextureLimit = 0;
            isIncrease = false;
        }
    }
}
