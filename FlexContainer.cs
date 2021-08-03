using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
[ExecuteInEditMode]
/// <summary>
/// Contains the majority of algorithmic components. Based off of https://drafts.csswg.org/css-flexbox.
/// </summary>
public class FlexContainer : MonoBehaviour
{

    [HideInInspector]
    /// <summary>
    /// If ticked, treats the container as the Root Container. Should only be one if you want the nested Containers to interact as Children.
    /// </summary>
    public bool RootContainer;

    [HideInInspector]
    /// <summary>
    /// If ticked, treats the container as a Child Container. Will execute the algorithm on its children but also is considered a child by the Root Container.
    /// </summary>
    public bool ChildContainer;

    [HideInInspector]
    /// <summary>
    /// The index of the Dropdown for choosing the Flex Direction.
    /// </summary>
    public int flexDirectionIndex;

    [HideInInspector]
    /// <summary>
    /// The index of the Dropdown for choosing the type of Wrapping.
    /// </summary>
    public int flexWrapIndex;



    [HideInInspector]
    /// <summary>
    /// The index of the Dropdown for choosing how to Justify Content.
    /// </summary>
    public int justifyContentIndex;

    [HideInInspector]
    /// <summary>
    /// The index of the Dropdown for choosing how to Align Items.
    /// </summary>
    public int alignItemsIndex;

    [HideInInspector]
    /// <summary>
    /// The index of the Dropdown for choosing how to Align Content.
    /// </summary>
    public int alignContentIndex;

    [HideInInspector]
    /// <summary>
    /// This is the dictionary that contains all the Children that container the FlexChildren Script. Essential to every function.
    /// </summary>
    /// <typeparam name="int">The InstanceID of each child.</typeparam>
    /// <typeparam name="FlexChildren.ChildrenData">Class containing relevant properties of each Child. See FlexChildren for more information</typeparam>
    /// <returns>Dictionary containing each child, their InstanceID as a key, and their ChildrenData class</returns>
    public Dictionary<int, FlexChildren.ChildrenData> childrenDict = new Dictionary<int, FlexChildren.ChildrenData>();
    /// <summary>
    /// The RectTransform of the Container. Currently determined by whichever object has this script attached to it.
    /// </summary>
    RectTransform cont;
    [HideInInspector]
    /// <summary>
    /// The root Canvas object. Used for determining the RootContainers size.
    /// </summary>
    public GameObject parentCanvas;


    /// <summary>
    /// Unity's default Update method. Currently used for applying the Algorithm in EditMode.
    /// </summary>
    void Update()
    {
        parentCanvas = transform.root.gameObject;
        cont = gameObject.GetComponent<RectTransform>();
        SetContainerSize();
        GetChildren();

        LineLengthDetermination();


        MainSizeDetermination();


    }

    [HideInInspector]


    /// <summary>
    /// Clears Children Dictionary and re-assembles it. Primarily used for Editor execution when adding/removing objects. 
    /// Currently auto-orders children by position in hierarchy, may be re-visited for manual ordering later.
    /// Also checks to see if child has a FlexChildren Script, if not, add the component. 
    /// </summary>
    public void GetChildren()
    {
        childrenDict.Clear();


        for (int i = 0; i < cont.childCount; i++)
        {
            if ((RootContainer && cont.GetChild(i).gameObject.GetComponent<FlexContainer>()) || ChildContainer)
            {
                FlexChildren flex = cont.GetChild(i).gameObject.GetComponent<FlexChildren>();
                if (flex == null)
                {
                    cont.GetChild(i).gameObject.AddComponent<FlexChildren>();
                }
                          FlexChildren.ChildrenData cd = flex.ConstructData();
                if (cont.GetChild(i).gameObject.GetComponent<FlexContainer>() != null)
                {
                    cd.nestedContainer = true;
                }

      


                //Im automating childOrder, need to distinguish between auto and manual
                cd.childOrder = i;
                flex.childOrder = i;


                cd.marginTypes = new Vector4(flex.topMarginType, flex.bottomMarginType, flex.rightMarginType, flex.leftMarginType);
                cd.marginValues = new Vector4(flex.topMarginValue, flex.bottomMarginValue, flex.rightMarginValue, flex.leftMarginValue);


                childrenDict.Add(cont.GetChild(i).gameObject.GetInstanceID(), cd);



            }
        }
        childrenDict = childrenDict.OrderBy(x => x.Value.childOrder).ToDictionary(x => x.Key, x => x.Value);


    }
    /// <summary>
    /// Sets the RootContainer to be the size of the Screen and its localPosition to zero.
    /// </summary>
    public void SetContainerSize()
    {

        if (RootContainer)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

        }




    }
    /// <summary>
    /// Determines the Flex Basis of each Child. If not explicit, the default main/cross sizes of the Child is used based on the type of component.
    /// For an explicit Flex Basis, the percentage is taken and used to calculate the Flex Basis size relative to the container size.
    /// In both cases, the Hypothetical Main/Cross Sizes are then calculated by clamping the Flex Basis to each axis's constraints.
    /// See https://drafts.csswg.org/css-flexbox/#line-sizing for more information on how it should work.
    /// </summary>
    public void LineLengthDetermination()
    {


        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {

            if (k.Value.childFlexBasis != 0)
            {
                if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
                {

                    k.Value.definedBasis = cont.sizeDelta.x * (k.Value.childFlexBasis / 100);

                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);



                }
                if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
                {
                    k.Value.definedBasis = cont.sizeDelta.y * (k.Value.childFlexBasis / 100);
                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);

                }

            }
            else
            {
                if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
                {


                    k.Value.definedBasis = ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, true);

                    k.Value.hypotheticalCrossSize = Mathf.Clamp(ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, false), k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);

                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);

                }
                if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
                {
                    k.Value.definedBasis = ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, false);
                    k.Value.hypotheticalCrossSize = Mathf.Clamp(ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, true), k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);

                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);

                }

            }

        }
    }

    /// <summary>
    /// Intial Positioning for Children with consideration of the chosen properties. The resulting position is rarely the final positioning of the Children.
    /// Then, depending on whether wrapping is enabled, the CalculateChildrenLines function is called to determine the lineNumber for each Child. This is critical for the remaining functions.
    /// See https://drafts.csswg.org/css-flexbox/#main-sizing for more information on how this should work.
    /// </summary>
    public void PositionItems()
    {
        var edge = RectTransform.Edge.Left;
        var inset = cont.rect.width;
        bool row = true;


        bool firstItem = true;
        RectTransform rtPrev = null;
        RectTransform rtFirst = null;
        Vector3[] childCorners = new Vector3[4];
        float distancetoFirst = 0;
        if (justifyContentIndex == 1)
        {
            childrenDict = childrenDict.OrderByDescending(x => x.Value.childOrder).ToDictionary(x => x.Key, x => x.Value);
        }
        else
        {
            childrenDict = childrenDict.OrderBy(x => x.Value.childOrder).ToDictionary(x => x.Key, x => x.Value);

        }

        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {

            RectTransform rt = k.Value.childRect;

            switch (flexDirectionIndex)
            {
                case 0:
                    if (justifyContentIndex == 1)
                    {
                        edge = RectTransform.Edge.Right;

                    }
                    else
                    {
                        edge = RectTransform.Edge.Left;
                    }

                    inset = rt.rect.width;
                    row = true;
                    break;
                case 1:
                    if (justifyContentIndex == 1)
                    {
                        edge = RectTransform.Edge.Left;

                    }
                    else
                    {
                        edge = RectTransform.Edge.Right;
                    }

                    inset = rt.rect.width;
                    row = true;
                    break;
                case 2:
                    if (justifyContentIndex == 1)
                    {
                        edge = RectTransform.Edge.Bottom;

                    }
                    else
                    {
                        edge = RectTransform.Edge.Top;
                    }

                    inset = rt.rect.height;
                    row = false;
                    break;
                case 3:
                    if (justifyContentIndex == 1)
                    {
                        edge = RectTransform.Edge.Top;

                    }
                    else
                    {
                        edge = RectTransform.Edge.Bottom;
                    }
                    inset = rt.rect.height;
                    row = false;
                    break;
            }
            if (firstItem)
            {
                rt.localPosition = new Vector3(0, 0, 0);
                rt.SetInsetAndSizeFromParentEdge(edge, 0, inset);
                rtPrev = rt;
                rtFirst = rt;
                firstItem = false;
                rt.GetLocalCorners(childCorners);
                if (row) distancetoFirst = rt.sizeDelta.x;

                else distancetoFirst = rt.sizeDelta.y;

            }
            else
            {


                if (row)
                {

                    if ((flexDirectionIndex == 1 && justifyContentIndex != 1) || (flexDirectionIndex == 0 && justifyContentIndex == 1))
                    {
                        rt.localPosition = new Vector2(rtPrev.localPosition.x - (rtPrev.sizeDelta.x / 2 + rt.sizeDelta.x / 2), rtPrev.localPosition.y);

                    }
                    else if ((flexDirectionIndex == 0 && justifyContentIndex != 1) || (flexDirectionIndex == 1 && justifyContentIndex == 1))
                    {
                        rt.localPosition = new Vector2(rtPrev.localPosition.x + (rtPrev.sizeDelta.x / 2 + rt.sizeDelta.x / 2), rtPrev.localPosition.y);
                    }

                }
                else
                {

                    if ((flexDirectionIndex == 2 && justifyContentIndex != 1) || (flexDirectionIndex == 3 && justifyContentIndex == 1))
                    {
                        rt.localPosition = new Vector2(rtPrev.localPosition.x, rtPrev.localPosition.y - (rtPrev.sizeDelta.y / 2 + rt.sizeDelta.y / 2));

                    }
                    else if ((flexDirectionIndex == 3 && justifyContentIndex != 1) || (flexDirectionIndex == 2 && justifyContentIndex == 1))
                    {
                        rt.localPosition = new Vector2(rtPrev.localPosition.x, rtPrev.localPosition.y + (rtPrev.sizeDelta.y / 2 + rt.sizeDelta.y / 2));
                    }

                }

                rtPrev = rt;
                rt.GetLocalCorners(childCorners);


            }


        }
        if (flexWrapIndex == 0 || flexWrapIndex == 2)
            CalculateChildrenLines(edge, inset, rtFirst, row);
    }
    /// <summary>
    /// Checks properties that are common to different parts of the algorithm and passes them. This is the main function to be called for the algorithm and runs the different parts in order.
    /// </summary>
    public void MainSizeDetermination()
    {
        bool row = true;
        if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
        {
            row = false;
        }
        PositionItems();

        ResolveFlexibleLengths(row);
        MainAxisAlignment(row);
        CrossSizeDetermination(row);
        CrossAsixAlignment(row);




    }

    /// <summary>
    /// Debug function that prints specific values to the console for each Child. 
    /// </summary>

    public void printChildrenDict()
    {
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            Debug.Log("Name: " + k.Value.childRect.gameObject.name + " LineNumber: " + k.Value.LineNumber);
        }
    }
    /// <summary>
    /// Auto-resizes Children in the container dependent on their constraints, the remaining space in their respective lines, and their Flex Grow and Flex Shrink values.
    /// See https://drafts.csswg.org/css-flexbox/#resolve-flexible-lengths for more information on how this should work.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void ResolveFlexibleLengths(bool row)
    {
        int numberOfLines = GetNumberOfLines();

        List<float> freeSpacePerLine = CalculateWorldFreeSpaceList(row);
        for (int j = 1; j <= numberOfLines; j++)
        {


            bool FlexGrow = false;
            int frozenItems = 0;
            float freeSpace = 0;
            float contMainSize = 0;
            float targetSizex = 0;
            float targetSizey = 0;
            float maxSizeComparison = 0;
            bool mainAxisWidth = false;
            switch (flexDirectionIndex)
            {
                case 0:
                case 1:
                    mainAxisWidth = true;
                    contMainSize = cont.rect.width;
                    break;
                case 2:
                case 3:
                    mainAxisWidth = false;
                    contMainSize = cont.rect.height;
                    break;
            }

            if (freeSpacePerLine[j - 1] > 0)
            {
                FlexGrow = true;
            }


            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == j)
                {

                    if (mainAxisWidth)
                    {
                        targetSizex = k.Value.definedBasis;
                        targetSizey = k.Value.childRect.sizeDelta.y;
                    }
                    else
                    {
                        targetSizex = k.Value.childRect.sizeDelta.x;
                        targetSizey = k.Value.definedBasis;
                    }
                    k.Value.targetMainSize = new Vector2(targetSizex, targetSizey);
                }

            }
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == j)
                {
                    if (mainAxisWidth)
                    {
                        maxSizeComparison = k.Value.hypotheticalMainSize;
                        targetSizex = maxSizeComparison;
                        targetSizey = k.Value.childRect.sizeDelta.y;
                    }
                    else
                    {
                        maxSizeComparison = k.Value.hypotheticalMainSize;
                        targetSizex = k.Value.childRect.sizeDelta.x;
                        targetSizey = maxSizeComparison;

                    }
                    if ((FlexGrow && k.Value.definedBasis > maxSizeComparison) || (!FlexGrow && k.Value.definedBasis < maxSizeComparison) || (k.Value.childFlexGrow == 0 && k.Value.childFlexShrink == 0))
                    {

                        k.Value.targetMainSize = new Vector2(targetSizex, targetSizey);
                        k.Value.isFrozen = true;
                        frozenItems++;


                    }

                }


            }
            while (frozenItems < childrenDict.Count)
            {
                //     Debug.Log("hjere");
                bool allFrozen = true;
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.isFrozen == false)
                    {
                        allFrozen = false;
                    }
                }
                if (allFrozen)
                {
                    break;
                }

                freeSpacePerLine = CalculateWorldFreeSpaceList(row);
                int violations = 0;
                float remainingFreeSpace = 0;
                float sumOfFlexValues = 0;
                float adjustmentSum = 0;
                Vector2 clampedtargetMainSize = new Vector2();
                float violationCheck = 0;
                float clampCheck = 0;
                float scaledShrink = 0;
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.isFrozen == true)
                    {
                        continue;
                    }
                    if (k.Value.LineNumber == j)
                    {
                        if (FlexGrow)
                        {
                            sumOfFlexValues += k.Value.childFlexGrow;
                        }
                        else
                        {
                            sumOfFlexValues += (k.Value.childFlexShrink * k.Value.definedBasis);
                        }
                    }


                }

                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {

                    if (k.Value.isFrozen)
                    {
                        continue;
                    }
                    if (k.Value.LineNumber == j)
                    {
                        if (sumOfFlexValues < 1)
                        {
                            remainingFreeSpace = freeSpacePerLine[j - 1] * sumOfFlexValues;
                            if (remainingFreeSpace > freeSpacePerLine[j - 1])
                            {
                                remainingFreeSpace = freeSpacePerLine[j - 1];
                            }

                        }

                        else
                        {
                            remainingFreeSpace = freeSpacePerLine[j - 1];
                        }


                        switch (flexDirectionIndex)
                        {
                            case 0:
                            case 1:
                                if (FlexGrow)
                                {
                                    targetSizex = k.Value.definedBasis + ((k.Value.childFlexGrow / sumOfFlexValues) * remainingFreeSpace);
                                    targetSizey = k.Value.childRect.sizeDelta.y;
                                    clampedtargetMainSize = new Vector2(Mathf.Clamp(targetSizex, k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y), targetSizey);
                                    violationCheck = clampedtargetMainSize.x;
                                }
                                else
                                {

                                    scaledShrink = (k.Value.childFlexShrink * k.Value.definedBasis) / sumOfFlexValues;
                                    clampedtargetMainSize = new Vector2(Mathf.Clamp(k.Value.definedBasis - Mathf.Abs(scaledShrink * remainingFreeSpace), k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y), k.Value.childRect.sizeDelta.y);
                                    targetSizex = k.Value.definedBasis - Mathf.Abs(scaledShrink * remainingFreeSpace);
                                    targetSizey = k.Value.childRect.sizeDelta.y;
                                    violationCheck = clampedtargetMainSize.x;
                                }

                                break;
                            case 2:
                            case 3:
                                if (FlexGrow)
                                {
                                    targetSizex = k.Value.childRect.sizeDelta.x;
                                    targetSizey = k.Value.definedBasis + ((k.Value.childFlexGrow / sumOfFlexValues) * remainingFreeSpace);
                                    clampedtargetMainSize = new Vector2(targetSizex, Mathf.Clamp(targetSizey, k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y));
                                    violationCheck = clampedtargetMainSize.y;
                                }
                                else
                                {
                                    scaledShrink = (k.Value.childFlexShrink * k.Value.definedBasis) / sumOfFlexValues;

                                    clampedtargetMainSize = new Vector2(k.Value.childRect.sizeDelta.x, Mathf.Clamp(k.Value.definedBasis - Mathf.Abs(scaledShrink * remainingFreeSpace), k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y));
                                    targetSizey = k.Value.definedBasis - Mathf.Abs(scaledShrink * remainingFreeSpace);
                                    targetSizex = k.Value.childRect.sizeDelta.x;
                                    violationCheck = clampedtargetMainSize.y;
                                }

                                break;
                        }
                        if (remainingFreeSpace != 0)
                        {

                            k.Value.targetMainSize = new Vector2(targetSizex, targetSizey);
                            Vector2 beforeClamp = k.Value.targetMainSize;
                            if (mainAxisWidth)
                            {
                                clampCheck = beforeClamp.x;
                            }
                            else clampCheck = beforeClamp.y;


                            if (beforeClamp != clampedtargetMainSize)
                            {
                                violations++;
                                if (clampCheck < violationCheck)
                                {

                                    k.Value.violateType = 1; //1 == min violation

                                }
                                else
                                {
                                    k.Value.violateType = 2; //1 == min violation
                                }
                                adjustmentSum += violationCheck - clampCheck;

                            }
                            k.Value.targetMainSize = clampedtargetMainSize;
                        }
                    }

                }

                if (adjustmentSum == 0)
                {
                    break;
                }
                else
                {
                    if (adjustmentSum > 0)
                    {
                        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                        {
                            if (k.Value.LineNumber == j)
                            {
                                if (k.Value.violateType == 1)
                                {
                                    k.Value.isFrozen = true;
                                    frozenItems++;
                                }
                            }

                        }
                    }
                    else
                    {
                        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                        {
                            if (k.Value.LineNumber == j)
                            {
                                if (k.Value.violateType == 2)
                                {
                                    k.Value.isFrozen = true;
                                    frozenItems++;
                                }
                            }

                        }
                    }
                }

            }
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == j)
                {
                    k.Value.childRect.sizeDelta = k.Value.targetMainSize;

                }

            }
        }
    }
    /// <summary>
    /// Determines the cross size of each child. This is either set to the hypothetical cross size or by dividing the container's cross size by the number of lines if the align content property is set to stretch.
    /// See https://drafts.csswg.org/css-flexbox/#cross-sizing for more information on how this should work.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void CrossSizeDetermination(bool row)
    {

        List<float> crossSizePerLine = new List<float>();
        int lines = GetNumberOfLines();
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (row && alignContentIndex != 3)
            {
                k.Value.childRect.sizeDelta = new Vector2(k.Value.childRect.sizeDelta.x, k.Value.hypotheticalCrossSize);
            }
            else if (row && alignContentIndex == 3) k.Value.childRect.sizeDelta = new Vector2(k.Value.childRect.sizeDelta.x, cont.rect.height / lines);
            else if (!row && alignContentIndex != 3) k.Value.childRect.sizeDelta = new Vector2(k.Value.hypotheticalCrossSize, k.Value.childRect.sizeDelta.y);
            else if (!row && alignContentIndex == 3) k.Value.childRect.sizeDelta = new Vector2(cont.rect.width / lines, k.Value.childRect.sizeDelta.y);
        }
        for (int i = 0; i < lines; i++) crossSizePerLine.Add(0);
        if (lines == 1)
        {
            if (row) crossSizePerLine[0] = cont.rect.height;
            else crossSizePerLine[0] = cont.rect.width;
        }



        else
        {
            for (int i = 0; i < lines; i++)
            {
                float crosssize = 0;
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.LineNumber == (i + 1))
                    {
                        if (crosssize < k.Value.hypotheticalCrossSize)
                        {
                            crosssize = k.Value.hypotheticalCrossSize;
                        }
                    }
                }
                crossSizePerLine[i] = crosssize;
            }
        }

        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (k.Value.nestedContainer && RootContainer)
            {
                if (row) k.Value.childRect.sizeDelta = new Vector2(k.Value.childRect.sizeDelta.x, crossSizePerLine[0]);
                else k.Value.childRect.sizeDelta = new Vector2(crossSizePerLine[0], k.Value.childRect.sizeDelta.y);
            }

        }


    }
    /// <summary>
    /// Determines the final main axis positioning of the Children. Dependent on the Jusitfy Content Property.
    /// Also takes into consideration the margins and applies them to an objects position.
    /// See https://drafts.csswg.org/css-flexbox/#main-alignment for more information on how this should work.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void MainAxisAlignment(bool row)

    {
        int lines = GetNumberOfLines();

        List<float> freeSpace = CalculateWorldFreeSpaceListv2(row);
        List<int> numChildrenInLine = new List<int>();

        for (int i = 0; i < lines; i++)
        {
            int num = 0;
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if ((i + 1) == k.Value.LineNumber)
                {
                    num++;

                }
            }
            numChildrenInLine.Add(num);
            //Removing margins from the freespace yalllll
        }
        int counter = 0;
        for (int i = 0; i < lines; i++)
        {
            float marginForEach = freeSpace[i] / numChildrenInLine[i];
            float j = 0;

            if (justifyContentIndex == 2)
            {
                j = 1f;
            }
            else marginForEach = 0;


            bool firstItem = true;
            RectTransform rtPrev = null;
            RectTransform rt = null;

            float firstmarginX = 0;
            float firstmarginY = 0;
            float secondmarginX = 0;
            float secondmarginY = 0;

            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {

                //Adding margins. So what I'm thinking is we just add margins. problem is what about the objects that come after?
                //if an object has left and right margin, we can't just add. Maybe we can add it by default to the size delta?
                //I think for now I'll approach it by dealing with in a flex direction basis. aka if row, add right margin to original object and left margin to the next object.
                //in this case i'll also have to check the left of og and right of next.
                float topMargin = k.Value.marginValues[0];
                float bottomMargin = k.Value.marginValues[1];
                float rightMargin = k.Value.marginValues[2];
                float leftMargin = k.Value.marginValues[3];
                if (flexDirectionIndex == 0)
                {
                    firstmarginX = leftMargin;
                    firstmarginY = bottomMargin - topMargin;
                }
                else if (flexDirectionIndex == 1)
                {
                    firstmarginX = rightMargin;
                    firstmarginY = bottomMargin - topMargin;
                }
                else if (flexDirectionIndex == 2)
                {
                    firstmarginX = leftMargin - rightMargin;
                    firstmarginY = topMargin;
                }

                else if (flexDirectionIndex == 3)
                {
                    firstmarginX = leftMargin - rightMargin;
                    firstmarginY = bottomMargin;
                }


                RectTransform.Edge edge = RectTransform.Edge.Left;
                float inset = 0;

                if (k.Value.LineNumber == (i + 1))
                {
                    float fixFirst = 1;
                    rt = k.Value.childRect;

                    switch (flexDirectionIndex)
                    {
                        case 0:
                            if (justifyContentIndex == 1)
                            {
                                edge = RectTransform.Edge.Right;

                            }
                            else
                            {
                                edge = RectTransform.Edge.Left;
                            }

                            inset = rt.rect.width;
                            row = true;
                            break;
                        case 1:
                            if (justifyContentIndex == 1)
                            {
                                edge = RectTransform.Edge.Left;

                            }
                            else
                            {
                                edge = RectTransform.Edge.Right;
                            }

                            inset = rt.rect.width;
                            row = true;
                            break;
                        case 2:
                            if (justifyContentIndex == 1)
                            {
                                edge = RectTransform.Edge.Bottom;

                            }
                            else
                            {
                                edge = RectTransform.Edge.Top;
                            }

                            inset = rt.rect.height;
                            row = false;
                            break;
                        case 3:
                            if (justifyContentIndex == 1)
                            {
                                edge = RectTransform.Edge.Top;

                            }
                            else
                            {
                                edge = RectTransform.Edge.Bottom;
                            }
                            inset = rt.rect.height;
                            row = false;
                            break;
                    }

                    k.Value.childRect.SetInsetAndSizeFromParentEdge(edge, 0, inset);
                    if (firstItem)
                    {
                        rtPrev = rt;
                        firstItem = false;

                        fixFirst = 0;
                        j = 0.5f;

                    }
                    else j = 1;



                    if ((flexDirectionIndex == 0 && justifyContentIndex != 1) || (flexDirectionIndex == 1 && justifyContentIndex == 1))
                    {

                        k.Value.childRect.localPosition = new Vector2(rtPrev.localPosition.x + ((rtPrev.sizeDelta.x / 2 * fixFirst) + (rt.sizeDelta.x / 2 * fixFirst)) + firstmarginX + secondmarginX + marginForEach * j, k.Value.childRect.localPosition.y + firstmarginY + secondmarginY);



                    }
                    else if ((flexDirectionIndex == 1 && justifyContentIndex != 1) || (flexDirectionIndex == 0 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(rtPrev.localPosition.x - ((rtPrev.sizeDelta.x / 2 * fixFirst) + (rt.sizeDelta.x / 2 * fixFirst)) + firstmarginX + secondmarginX - marginForEach * j, k.Value.childRect.localPosition.y + firstmarginY + secondmarginY);

                    }
                    else if ((flexDirectionIndex == 2 && justifyContentIndex != 1) || (flexDirectionIndex == 3 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(k.Value.childRect.localPosition.x + firstmarginX + secondmarginX, rtPrev.localPosition.y - ((rtPrev.sizeDelta.y / 2 * fixFirst) + (rt.sizeDelta.y / 2 * fixFirst)) + firstmarginY + secondmarginY - marginForEach * j);

                    }
                    else if ((flexDirectionIndex == 3 && justifyContentIndex != 1) || (flexDirectionIndex == 2 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(k.Value.childRect.localPosition.x + firstmarginX + secondmarginX, rtPrev.localPosition.y + ((rtPrev.sizeDelta.y / 2 * fixFirst) + (rt.sizeDelta.y / 2 * fixFirst)) + firstmarginY + secondmarginY + marginForEach * j);

                    }
                    if (flexDirectionIndex == 0)
                    {
                        secondmarginX = rightMargin;
                        secondmarginY = 0;
                    }
                    else if (flexDirectionIndex == 1)
                    {
                        secondmarginX = leftMargin;
                        secondmarginY = 0;
                    }
                    else if (flexDirectionIndex == 2)
                    {
                        secondmarginX = 0;
                        secondmarginY = bottomMargin;
                    }

                    else if (flexDirectionIndex == 3)
                    {
                        secondmarginX = 0;
                        secondmarginY = topMargin;
                    }

                    //j++;
                    rtPrev = rt;
                }
                else
                {
                    continue;
                }

            }
            counter++;
        }


    }
    /// <summary>
    /// Determines the final cross axis position of the Children. Dependent on the Align Content and Align Item Properties.
    /// Uses the container's loca corners to determine how to position each Child in the line, while positioning each line by the size of the largest cross size in the previous line.
    /// See https://drafts.csswg.org/css-flexbox/#cross-alignment for more information on how this should work.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void CrossAsixAlignment(bool row)
    {
        int lines = GetNumberOfLines();
        Vector3[] childCorners = new Vector3[4];
        Vector3[] parentCorners = new Vector3[4];
        RectTransform rt = null;
        if (alignContentIndex == 0 || alignContentIndex == 3)
        {
            float maxCrossSizePerLine = 0;
            for (int i = 0; i < lines; i++)
            {
                maxCrossSizePerLine += GetMaxCrossSizePerLine(i + 1, row);
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {

                    if (k.Value.LineNumber == (i + 1))
                    {

                        rt = k.Value.childRect;
                        rt.GetLocalCorners(childCorners);
                        cont.GetLocalCorners(childCorners);
                        //So what I'm thinking of doing is getting the cross size distance via local corners. Then change it based on setting.
                        if (i == 0)
                        {
                            float distanceToEdge = parentCorners[1].y - childCorners[0].y;
                            if (row)
                            {


                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y - rt.sizeDelta.y / 2) + distanceToEdge);
                            }
                            else
                            {
                                distanceToEdge = parentCorners[1].x - childCorners[0].x;
                                rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.x / 2) - distanceToEdge, rt.localPosition.y);

                            }

                        }
                        else if (i > 0)
                        {
                            float distanceToEdge = parentCorners[1].y - childCorners[0].y;
                            if (row)
                            {

                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) + (distanceToEdge) - maxCrossSizePerLine);
                            }
                            else
                            {
                                distanceToEdge = parentCorners[1].x - childCorners[0].x;

                                // Debug.Log(distanceToEdge + " " + maxCrossSizePerLine);
                                rt.localPosition = new Vector2((rt.localPosition.x - rt.sizeDelta.x / 2) - distanceToEdge + maxCrossSizePerLine, rt.localPosition.y);
                            }


                        }

                    }
                }

            }
        }
        if (alignContentIndex == 1)
        {
            float maxCrossSizePerLine = 0;
            RectTransform rtPrev = null;
            for (int i = lines; i > 0; i--)
            {
                maxCrossSizePerLine += GetMaxCrossSizePerLine(i + 1, row);
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.LineNumber == (i))
                    {

                        rt = k.Value.childRect;

                        rt.GetLocalCorners(childCorners);
                        cont.GetLocalCorners(childCorners);
                        float distanceToEdge = 0;
                        if (row) distanceToEdge = parentCorners[1].y - childCorners[1].y;
                        else distanceToEdge = parentCorners[1].x - childCorners[1].x;
                        if (i == lines)
                        {
                            if (row)
                            {

                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) + distanceToEdge);
                            }
                            else
                            {
                                rt.localPosition = new Vector2((rt.localPosition.x - rt.sizeDelta.x / 2) + distanceToEdge, rt.localPosition.y);
                            }
                            rtPrev = rt;

                        }
                        else
                        {

                            if (row)
                            {
                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) + distanceToEdge + maxCrossSizePerLine);
                            }
                            else
                            {

                                rt.localPosition = new Vector2((rt.localPosition.x - rt.sizeDelta.x / 2) - maxCrossSizePerLine + distanceToEdge, rt.localPosition.y);

                            }
                        }


                    }

                }


            }
        }
        if (alignContentIndex == 2)
        {
            float total = 0;
            for (int i = 0; i < lines; i++)
            {
                total += GetMaxCrossSizePerLine(i + 1, row);
            }
            if (row) total = (cont.rect.height - total) / 2;
            else total = (cont.rect.width - total) / 2;
            float maxCrossSizePerLine = 0;
            for (int i = 0; i < lines; i++)
            {
                maxCrossSizePerLine += GetMaxCrossSizePerLine(i + 1, row);
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    //ezaf   to do this. Get remaining size in height. Divide it by two and give it to the first line LOL ez LMAO
                    if (k.Value.LineNumber == (i + 1))
                    {
                        rt = k.Value.childRect;
                        rt.GetLocalCorners(childCorners);
                        cont.GetLocalCorners(childCorners);
                        //So now we have remaining crosssize space/2. Now we get the distance to that point and add it.
                        if (i == 0)
                        {
                            if (row)
                            {
                                float distanceToEdge = (parentCorners[1].y + total) - childCorners[1].y;
                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y - rt.sizeDelta.y / 2) - distanceToEdge);

                            }
                            else
                            {
                                float distanceToEdge = (parentCorners[1].x - total) - childCorners[0].x;
                                rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.x / 2) - distanceToEdge, rt.localPosition.y);
                            }


                        }
                        else if (i > 0)
                        {
                            if (row)
                            {
                                float distanceToEdge = (parentCorners[1].y + total) - childCorners[1].y;
                                float incrementDistace = GetMaxCrossSizePerLine(i + 1, row);
                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) - distanceToEdge - maxCrossSizePerLine);
                            }
                            else
                            {
                                float distanceToEdge = (parentCorners[1].x - total) - childCorners[0].x;
                                float incrementDistace = GetMaxCrossSizePerLine(i + 1, row);
                                rt.localPosition = new Vector2((rt.localPosition.x - rt.sizeDelta.x / 2) - distanceToEdge + maxCrossSizePerLine, rt.localPosition.y);
                            }

                        }

                    }


                }
            }
        }
        AlignItems(row);
    }
    /// <summary>
    /// Determines the Alignment of each Child in their line. Called in the CrossAxisAlignment() function and is similar in how it calculates the position by using the local corners and adds the offset.
    /// Though, instead of using the container's corners, it uses the largest cross size Child for the calculation.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void AlignItems(bool row)
    {
        int lines = GetNumberOfLines();
        Vector3[] largestChildCorners = new Vector3[4];
        Vector3[] currentChildCorners = new Vector3[4];
        for (int i = 1; i <= lines; i++)
        {
            RectTransform largestChild = GetMaxCrossSizeObjPerLine(i, row);
            largestChild.GetLocalCorners(largestChildCorners);


            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                RectTransform rt = k.Value.childRect;
                if (k.Value.LineNumber == i && ((row && rt.sizeDelta.y < largestChild.sizeDelta.y) || (!row && rt.sizeDelta.x < largestChild.sizeDelta.x)))
                {
                    rt.GetLocalCorners(currentChildCorners);
                    if (alignItemsIndex == 0)
                    {
                        if (row)
                        {
                            float distanceToTop = ((largestChild.localPosition.y + largestChildCorners[1].y) - (rt.localPosition.y + currentChildCorners[1].y));
                            rt.localPosition = new Vector2(rt.localPosition.x, rt.localPosition.y + distanceToTop);
                        }
                        else
                        {
                            float distanceToTop = ((largestChild.localPosition.x + largestChildCorners[2].x) - (rt.localPosition.x + currentChildCorners[2].x));
                            rt.localPosition = new Vector2(rt.localPosition.x + distanceToTop, rt.localPosition.y);
                        }
                    }
                    else if (alignItemsIndex == 1)
                    {
                        if (row)
                        {
                            float distanceToTop = ((largestChild.localPosition.y + largestChildCorners[0].y) - (rt.localPosition.y + currentChildCorners[0].y));

                            rt.localPosition = new Vector2(rt.localPosition.x, rt.localPosition.y + distanceToTop);
                        }
                        else
                        {
                            float distanceToTop = ((largestChild.localPosition.x + largestChildCorners[1].x) - (rt.localPosition.x + currentChildCorners[1].x));
                            rt.localPosition = new Vector2(rt.localPosition.x + distanceToTop, rt.localPosition.y);
                        }
                    }
                    else if (alignItemsIndex == 2)
                    {
                        if (row)
                        {
                            float distanceToTop = (largestChild.sizeDelta.y - rt.sizeDelta.y);
                            Debug.Log(distanceToTop);
                            rt.localPosition = new Vector2(rt.localPosition.x, largestChild.localPosition.y);
                        }
                        else
                        {
                            float distanceToTop = ((largestChild.localPosition.x + largestChildCorners[1].x) - (rt.localPosition.x + currentChildCorners[1].x));
                            rt.localPosition = new Vector2(largestChild.localPosition.x, rt.localPosition.y);
                        }
                    }
                }

            }
        }
    }
    /// <summary>
    /// Helper function that returns the size of the largest cross size of the Children in a given line.
    /// </summary>
    /// <param name="line">The line to check.</param>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>Largest cross size in given line.</returns>
    public float GetMaxCrossSizePerLine(int line, bool row)
    {
        float maxCrossSize = 0;
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (k.Value.LineNumber == line)
            {
                if (row && k.Value.childRect.sizeDelta.y > maxCrossSize) maxCrossSize = k.Value.childRect.sizeDelta.y;
                else if (!row && k.Value.childRect.sizeDelta.x > maxCrossSize) maxCrossSize = k.Value.childRect.sizeDelta.x;
            }
        }

        return maxCrossSize;
    }
    /// <summary>
    /// Helper function that returns the RectTransform of the Child with the largest cross size in a given line.
    /// </summary>
    /// <param name="line">The line to check</param>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>RectTransform of the Child with the largest cross size</returns>
    public RectTransform GetMaxCrossSizeObjPerLine(int line, bool row)
    {
        float maxCrossSize = 0;
        RectTransform child = null;
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (k.Value.LineNumber == line)
            {
                if (row && k.Value.childRect.sizeDelta.y > maxCrossSize)
                {
                    maxCrossSize = k.Value.childRect.sizeDelta.y;
                    child = k.Value.childRect;
                }
                else if (!row && k.Value.childRect.sizeDelta.x > maxCrossSize)
                {
                    maxCrossSize = k.Value.childRect.sizeDelta.x;
                    child = k.Value.childRect;

                }
            }
        }

        return child;
    }
    /// <summary>
    /// Helper function for returning the max order value of a Child in a given line. Currently only used in the CalculateChildrenLines() function and likely will be replaced.
    /// </summary>
    /// <param name="line">The line to check</param>
    /// <returns></returns>
    public int GetMaxOrderPerLine(int line)
    {
        int maxOrder = 0;
        int lines = GetNumberOfLines();
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (k.Value.LineNumber == line)
            {
                if (k.Value.childOrder > maxOrder)
                {
                    maxOrder = k.Value.childOrder;
                }
            }
        }
        return maxOrder;
    }
    /// <summary>
    /// Super useful helper function. Returns the largest line value in the Container.
    /// </summary>
    /// <returns>Largest line value in Container</returns>
    public int GetNumberOfLines()
    {
        int lines = 0;
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (k.Value.LineNumber > lines)
            {
                lines = k.Value.LineNumber;
            }
        }
        return lines;
    }
    /// <summary>
    /// Helper Function. Calculates the remaining free space in each line and takes into consideration the margins. 
    /// This variation is specifically used in the ResolvingFlexibleLengths() function, and will likely be merged into its similar functions for simplicities sake in the future.
    /// The difference lies in the use of the Flex Basis in the case of a Child being considered "Frozen".
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>List of free spaces per line</returns>
    public List<float> CalculateWorldFreeSpaceList(bool row)
    {

        Vector3[] childCorners = new Vector3[4];


        float freeSpace = 0;
        if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
        {

            freeSpace = cont.rect.width;
        }
        else if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
        {

            freeSpace = cont.rect.height;
        }



        int NumberOfLines = GetNumberOfLines();

        List<float> freeSpacePerLine = new List<float>();
        for (int i = 0; i < NumberOfLines; i++)
        {
            freeSpacePerLine.Add(freeSpace);
        }
        for (int i = 0; i < NumberOfLines; i++)
        {
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == (i + 1))
                {
                    if (k.Value.isFrozen)
                    {
                        if (row) freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.targetMainSize.x;
                        else freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.targetMainSize.y;
                    }
                    else
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.definedBasis;

                    }
                    if (flexDirectionIndex == 1 || flexDirectionIndex == 0)
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[2] - k.Value.marginValues[3];
                    }
                    else
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[0] - k.Value.marginValues[1];
                    }
                }


            }

        }




        return freeSpacePerLine;
    }
    /// <summary>
    /// Helper Function. Calculates the remaining free space in each line and takes into consideration the margins. 
    /// This variation uses the current real size of each child for the determination. 
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>List of free spaces per line</returns>
    public List<float> CalculateWorldFreeSpaceListv2(bool row)
    {

        Vector3[] childCorners = new Vector3[4];


        float freeSpace = 0;
        if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
        {

            freeSpace = cont.rect.width;
        }
        else if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
        {

            freeSpace = cont.rect.height;
        }



        int NumberOfLines = GetNumberOfLines();

        List<float> freeSpacePerLine = new List<float>();
        for (int i = 0; i < NumberOfLines; i++)
        {
            freeSpacePerLine.Add(freeSpace);
        }
        for (int i = 0; i < NumberOfLines; i++)
        {
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == (i + 1))
                {
                    if (row) freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.childRect.sizeDelta.x;
                    else freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.childRect.sizeDelta.y;
                    if (flexDirectionIndex == 1 || flexDirectionIndex == 0)
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[2] - k.Value.marginValues[3];
                    }
                    else
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[0] - k.Value.marginValues[1];
                    }
                }


            }

        }




        return freeSpacePerLine;
    }
    /// <summary>
    /// Helper Function. Calculates the remaining free space in each line and takes into consideration the margins. 
    /// This variation uses the Hypothetical Main Size of each Child for its determination.
    /// </summary>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    /// <returns>List of free spaces per line</returns>
    public List<float> CalculateWorldFreeSpaceListHypo(bool row)
    {

        Vector3[] childCorners = new Vector3[4];


        float freeSpace = 0;
        if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
        {

            freeSpace = cont.rect.width;
        }
        else if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
        {

            freeSpace = cont.rect.height;
        }



        int NumberOfLines = GetNumberOfLines();

        List<float> freeSpacePerLine = new List<float>();
        for (int i = 0; i < NumberOfLines; i++)
        {
            freeSpacePerLine.Add(freeSpace);
        }
        for (int i = 0; i < NumberOfLines; i++)
        {
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == (i + 1))
                {
                    freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.hypotheticalMainSize;

                    if (flexDirectionIndex == 1 || flexDirectionIndex == 0)
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[2] - k.Value.marginValues[3];
                    }
                    else
                    {
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.marginValues[0] - k.Value.marginValues[1];
                    }
                }


            }

        }




        return freeSpacePerLine;
    }



    /// <summary>
    /// Determines the line of each Child. Works by:
    /// 1. Determine number of lines required based on each Child's Hypothetical Main Size and the container's main size
    /// 2. Iterates through the number of lines and keeps each Child in the line unless that Child will exceed the size of the container.
    /// 3. If a Child exceeds, the next line iteration begins and the cycle is repeated until all Children are fit.
    /// </summary>
    /// <param name="edge">The edge to align the Children to when moved to a new line.</param>
    /// <param name="inset">The inset of each Child, equal to their current real main axis size.</param>
    /// <param name="rtFirst">The first Child.</param>
    /// <param name="row">If Row/Row-reverse is selected, this is true. If Column/Column-reverse is selected, this is false.</param>
    public void CalculateChildrenLines(RectTransform.Edge edge, float inset, RectTransform rtFirst, bool row)
    {
        float total = 0;
        bool first = true;
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            total += k.Value.hypotheticalMainSize;

        }
        float numberofLines = 0;
        if (row) numberofLines = total / cont.rect.width;
        else numberofLines = total / cont.rect.height;

        int roundedLines = Mathf.CeilToInt(numberofLines);

        /*
        So I think the way to do this is in clumps.
        problem was it wasn't doing the first line right, because it was trying its best to fill the last line first.
        So maybe, we just iterate through, and add hypothetical main sizes until it is larger than container.
        Once we are done, we can assign them to i.
        restart the loop, and do the same until we run out of items

        DONE!!!

        Next step is to go ahead and fix this shit. So the problem happens when something is too large to fit in ANY line.
        I see two ways to approach this:
        1. Clamp the max comparison size to the small hypothetical size. I like this approach.
        2. Something bullshit with checking yeah whatever ill just go with the previous
        */


        if (numberofLines > 1)
        {
            float contMainSize = 0;
            if (row) contMainSize = cont.rect.width;
            else contMainSize = cont.rect.height;
            float minSizeComparison = 0;
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.hypotheticalMainSize > minSizeComparison)
                {
                    minSizeComparison = k.Value.hypotheticalMainSize;
                }
            }
            if (contMainSize > minSizeComparison)
            {
                if (row) minSizeComparison = cont.rect.width;
                else minSizeComparison = cont.rect.height;
            }
            for (int i = 1; i <= roundedLines; i++)
            {
                float TotalPerLine = 0;
                if (i == 1)
                {
                    if (row) TotalPerLine = rtFirst.sizeDelta.x;
                    else TotalPerLine = rtFirst.sizeDelta.y;
                }
                else TotalPerLine = 0;
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    float objectSize = 0;
                    if (row) objectSize = k.Value.hypotheticalMainSize + k.Value.marginValues[2] + k.Value.marginValues[3];
                    else objectSize = k.Value.hypotheticalMainSize + k.Value.marginValues[0] + k.Value.marginValues[1];
                    if ((k.Value.childOrder == 0 && justifyContentIndex == 0) || (k.Value.childOrder == GetMaxOrderPerLine(1) && justifyContentIndex == 1))
                    {
                        continue;
                    }

                    if (!k.Value.doesFit && (objectSize + TotalPerLine <= minSizeComparison))
                    {

                        TotalPerLine += objectSize;
                        k.Value.LineNumber = i;
                        k.Value.doesFit = true;

                    }

                    else if (!k.Value.doesFit)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }

                }

                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.childOrder == 0)
                    {
                        continue;
                    }
                    if (!k.Value.doesFit)
                    {

                        k.Value.LineNumber = i;


                    }

                }
            }

        }
        printChildrenDict();
    }




}
