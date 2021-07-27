using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ContentSizeCalc
{
    public static float DetermineContentSize(GameObject gme, bool row)
    {
        float contentSize = 0;
        if (gme.GetComponent<Text>() != null)
        {
            if (row) contentSize = 160;
            else contentSize = 30;
        }
        else if (gme.GetComponent<TextMeshProUGUI>() != null)
        {
            if (row) contentSize = 200;
            else contentSize = 50;
        }
        else if (gme.GetComponent<Button>() != null)
        {
            if (row) contentSize = 160;
            else contentSize = 30;
        }
        else if (gme.GetComponent<Toggle>() != null)
        {
            if (row) contentSize = 160;
            else contentSize = 20;
        }
        else if (gme.GetComponent<InputField>() != null)
        {
            if (row) contentSize = 160;
            else contentSize = 30;
        }
        else if (gme.GetComponent<Image>() != null || gme.GetComponent<RawImage>() != null)
        {
            if (gme.GetComponent<Image>() != null)
            {
                Image img = gme.GetComponent<Image>();
                if (row)
                {
                    if (img.sprite != null) contentSize = img.sprite.rect.width;
                    else contentSize = 100;
                }
                else {
                    if (img.sprite != null) contentSize = img.sprite.rect.height;
                    else contentSize = 100;
                }
            }
            if (gme.GetComponent<RawImage>() != null)
            {
                RawImage img = gme.GetComponent<RawImage>();
                if (row) contentSize = img.mainTexture.width;
                else contentSize = img.mainTexture.height;
            }
        }

        return contentSize;
    }
}
