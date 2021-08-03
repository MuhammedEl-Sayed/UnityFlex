using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[ExecuteInEditMode]
public class FlexChildren : MonoBehaviour
{
    [HideInInspector]
    /// <summary>
    /// The constraints for the Child's height. if not explicity, minimum is set to 0 and max to infinity.
    /// </summary>
    public Vector2 childHeightMinMax;

    [HideInInspector]
    /// <summary>
    /// The constraints for the Child's width. if not explicity, minimum is set to 0 and max to infinity.
    /// </summary>
    public Vector2 childWidthMinMax;
    [HideInInspector]
    /// <summary>
    /// The order of the child compared to its siblings. Starts from 0. Is set to auto calculate for now based on position in hierarchy.
    /// </summary>
    public int childOrder;
    [HideInInspector]
    /// <summary>
    /// The ratio of growth of the child. Used in FlexContainer.ResolveFlexibleLengths. 
    /// </summary>
    public int childFlexGrow = 1;


    [HideInInspector]
    /// <summary>
    /// The ratio of shrinking of the child. Used in FlexContainer.ResolveFlexibleLengths. 
    /// </summary>
    public int childFlexShrink = 1;

    [HideInInspector]
    /// <summary>
    /// The index for the Flex Basis type Dropdown
    /// </summary>
    public int childFlexTypeIndex;
    /// <summary>
    /// If Percentage is chosen in the Flex Basis Dropdown, this is what stores the value.
    /// </summary>
    public float flexBasisSize;


    [HideInInspector]
    /// <summary>
    /// The index for the type of constraint. A vector4 because it stores the index for the min and max for both height and width for ease of use reasons.
    /// </summary>
    public Vector4 constraintTypeIndex;
    [HideInInspector]
    /// <summary>
    /// Min Height Constraint.
    /// </summary>
    public float containerConstraintsHeightx;
    [HideInInspector]
    /// <summary>
    /// Max Height Constraint.
    /// </summary>
    public float containerConstraintsHeighty;

    [HideInInspector]
    /// <summary>
    /// Min Width Constraint.
    /// </summary>
    public float containerConstraintsWidthx;
    [HideInInspector]
    /// <summary>
    /// Max Width Constraint.
    /// </summary>
    public float containerConstraintsWidthy;

    [HideInInspector]
    public int topMarginType;
    /// <summary>
    /// Index for Top Margin Type Dropdown
    /// </summary>
    [HideInInspector]
    /// <summary>
    /// Value for the Top Margin
    /// </summary>
    public float topMarginValue;

    [HideInInspector]
    /// <summary>
    /// Index for Bottom Margin Type Dropdown
    /// </summary>
    public int bottomMarginType;
    [HideInInspector]
        /// <summary>
    /// Value for the Bottom Margin
    /// </summary>
    public float bottomMarginValue;

    [HideInInspector]
    /// <summary>
    /// Index for Right Margin Type Dropdown
    /// </summary>
    public int rightMarginType;
    [HideInInspector]
        /// <summary>
    /// Value for the Right Margin
    /// </summary>
    public float rightMarginValue;

    [HideInInspector]
    /// <summary>
    /// Index for Left Margin Type Dropdown
    /// </summary>
    public int leftMarginType;
    [HideInInspector]
        /// <summary>
    /// Value for the Left Margin
    /// </summary>
    public float leftMarginValue;

    [Serializable]
    /// <summary>
    /// This class stores all the relevant data that FlexContainer needs for the algorithm and is manipulated across several functions.
    /// </summary>
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

        public int LineNumber = 1;
        public bool nestedContainer = false;
        public bool doesFit = false;
        public Vector4 marginTypes = new Vector4();
        public Vector4 marginValues = new Vector4();


    }
 

   /// <summary>
   /// Constructs the Class and populates the necessary vectors from the information/properties inputed in the Inspector.
   /// </summary>
   /// <returns>ChildrenData class for the child.</returns>
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


        cd.marginTypes = new Vector4(flex.topMarginType, flex.bottomMarginType, flex.rightMarginType, flex.leftMarginType);
        cd.marginValues = new Vector4(flex.topMarginValue, flex.bottomMarginValue, flex.rightMarginValue, flex.leftMarginValue);
        return cd;
    }

}
