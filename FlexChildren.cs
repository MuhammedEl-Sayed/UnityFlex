using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteInEditMode]
public class FlexChildren : MonoBehaviour
{
    [HideInInspector]

    public Vector2 childHeightMinMax;
    [HideInInspector]
    public bool autoHeight;
    [HideInInspector]
    public bool autoWidth;
    [HideInInspector]

    public Vector2 childWidthMinMax;
    [HideInInspector]

    public int childOrder;
    [HideInInspector]

    public int childFlexGrow = 1;


    [HideInInspector]

    public int childFlexShrink = 1;

    [HideInInspector]
    public int childFlexTypeIndex;
    public float flexBasisSize;
    [HideInInspector]

    public GameObject parentCanvas;
    [HideInInspector]

    public GameObject parentContainer;
    [HideInInspector]
    public Vector4 constraintTypeIndex;
    [HideInInspector]
    public float containerConstraintsHeightx;
    [HideInInspector]
    public float containerConstraintsHeighty;

    [HideInInspector]
    public float containerConstraintsWidthx;
    [HideInInspector]
    public float containerConstraintsWidthy;
    
    [HideInInspector]
    public int topMarginType;
        [HideInInspector]
    public float topMarginValue;
    
    [HideInInspector]
    public int bottomMarginType;
        [HideInInspector]
    public float bottomMarginValue;
    
    [HideInInspector]
    public int rightMarginType;
        [HideInInspector]
    public float rightMarginValue;
    
    [HideInInspector]
    public int leftMarginType;
        [HideInInspector]
    public float leftMarginValue;
    

    void Awake()
    {
        parentCanvas = transform.root.gameObject;
        parentContainer = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
