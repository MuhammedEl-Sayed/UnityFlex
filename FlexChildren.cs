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

    [Serializable]
    public class ChildrenData
    {
        public RectTransform childRect;
        public Vector2 childHeightMinMax;
        public Vector2 childWidthMinMax;
        public int childOrder;
        public int childFlexGrow;
        public int childFlexShrink;
        public float childFlexBasis;
        public int flexBasisType;
        public float definedBasis;
        public int violateType = 0;
        public bool isFrozen = false;
        public Vector2 targetMainSize;
        public float hypotheticalMainSize;
        public float hypotheticalCrossSize;
        public bool autoHeight;
        public bool autoWidth;
        public int LineNumber = 1;
        public bool nestedContainer = false;
        public Vector4 marginTypes = new Vector4();
        public Vector4 marginValues = new Vector4();


    }
    public int line;
    void Awake()
    {
        parentCanvas = transform.root.gameObject;
        parentContainer = transform.parent.gameObject;
    }

    // Update is called once per frame
    public ChildrenData ConstructData()
    {
        ChildrenData cd = new ChildrenData();
        FlexChildren flex = gameObject.GetComponent<FlexChildren>();

        float xmin;
        float xmax;
        float ymin;
        float ymax;

        cd.childRect = gameObject.GetComponent<RectTransform>();
        if (flex.constraintTypeIndex.x == 0)
        {
            ymin = 0;
        }
        else
        {
            ymin = flex.containerConstraintsHeightx;
        }
        if (flex.constraintTypeIndex.y == 0)
        {
            ymax = Mathf.Infinity;
        }
        else
        {
            ymax = flex.containerConstraintsHeighty;
        }
        if (flex.constraintTypeIndex.z == 0)
        {
            xmin = 0;
        }
        else
        {
            xmin = flex.containerConstraintsWidthx;
        }
        if (flex.constraintTypeIndex.w == 0)
        {
            xmax = Mathf.Infinity;
        }
        else
        {
            xmax = flex.containerConstraintsWidthy;
        }
        cd.childHeightMinMax = new Vector2(ymin, ymax);
        cd.childWidthMinMax = new Vector2(xmin, xmax);
        cd.childFlexGrow = flex.childFlexGrow;
        cd.childFlexShrink = flex.childFlexShrink;
        // cd.childOrder = flex.childOrder;
        cd.flexBasisType = flex.childFlexTypeIndex;
        if (cd.flexBasisType == 2)
        {
            cd.childFlexBasis = flex.flexBasisSize;

        }
        //Im automating childOrder, need to distinguish between auto and manual


        cd.autoHeight = flex.autoHeight;
        cd.autoWidth = flex.autoWidth;

        cd.marginTypes = new Vector4(flex.topMarginType, flex.bottomMarginType, flex.rightMarginType, flex.leftMarginType);
        cd.marginValues = new Vector4(flex.topMarginValue, flex.bottomMarginValue, flex.rightMarginValue, flex.leftMarginValue);
        return cd;
    }
    public void ReconstructData(FlexChildren.ChildrenData cd){
        line = cd.LineNumber;
    }
}
