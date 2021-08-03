using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Determines the default size of a known Unity UI Element. Essential for the Flex Basis of a child.
/// </summary>
public class ContentSizeCalc
{
    /// <summary>
    /// Determines the default size of a known Unity UI Element passed to it.
    /// </summary>
    /// <param name="gme">The GameObject of the UI Element</param>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>Returns the minimum size of an element. Default is 200.</returns>
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
        else{
            contentSize = 200;
        }

        return contentSize;
    }
}
