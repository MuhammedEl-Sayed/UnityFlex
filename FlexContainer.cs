using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
[ExecuteInEditMode]
/*
Notes for Josh, so problems stems from getting defined basis. Getting min size doesn't really work because
I'm changing the size of the object, and trying to get its own min size, meaning that the min size is always its own current size
This is fucking up putting the children in a single line.

Possible solutions:
- Tried setting flex basis as 0, really fucks with hypothetical size which fucks with checking if it will fit in the line.
*/
public class FlexContainer : MonoBehaviour
{
    [HideInInspector]
    public bool RootContainer;
    [HideInInspector]
    public bool ChildContainer;
    [HideInInspector]
    public int displayTypeIndex;
    [HideInInspector]
    public int flexDirectionIndex;

    [HideInInspector]
    public int flexWrapIndex;



    [HideInInspector]
    public int justifyContentIndex;

    [HideInInspector]
    public int alignItemsIndex;

    [HideInInspector]
    public int alignContentIndex;



    [Serializable]
    [HideInInspector]
    public class LineData
    {
        public int lineNumber;
        public float crossSize;
    }
    [HideInInspector]
    public Dictionary<int, LineData> LineDataDic = new Dictionary<int, LineData>();
    [HideInInspector]
    public List<int> childFlexGrowList = new List<int>();


    [HideInInspector]
    public List<int> childFlexShrinkList = new List<int>();

    [HideInInspector]

    private List<float> flexBasisSize;

    [HideInInspector]
    public Dictionary<int, FlexChildren.ChildrenData> childrenDict = new Dictionary<int, FlexChildren.ChildrenData>();
    [HideInInspector]
    public Dictionary<int, FlexChildren.ChildrenData> containerDict = new Dictionary<int, FlexChildren.ChildrenData>();
    [HideInInspector]
    public string currentChildIndex; //need to make it an int


    private Vector2 containerCenter;

    private float height;
    private float width;
    RectTransform cont;
    [HideInInspector]
    public GameObject parentCanvas;
    public bool normalContainer;

    private int wrapping;
    private float mainSize;



    [HideInInspector]
    public int numberOfChildren = 0;
    public List<int> childKeys = new List<int>();
    [HideInInspector]
    public int numberOfChildContainers;
    private bool firstItem;
    List<float> listOfTotalWidths = new List<float>();

    void Update()
    {
        parentCanvas = transform.root.gameObject;
        cont = gameObject.GetComponent<RectTransform>();
        SetContainerSize();
        GetContainerInfo();
        GetChildren();

        LineLengthDetermination();


        MainSizeDetermination();


    }

    [HideInInspector]

    public void GetChildren()
    {
        childrenDict.Clear();
        childKeys.Clear();
        numberOfChildContainers = 0;

        for (int i = 0; i < cont.childCount; i++)
        {
            if ((RootContainer && cont.GetChild(i).gameObject.GetComponent<FlexContainer>()) || ChildContainer)
            {
                FlexChildren flex = cont.GetChild(i).gameObject.GetComponent<FlexChildren>();
                FlexChildren.ChildrenData cd = flex.ConstructData();

                if (flex == null)
                {
                    cont.GetChild(i).gameObject.AddComponent<FlexChildren>();
                }
                if (cont.GetChild(i).gameObject.GetComponent<FlexContainer>() != null)
                {
                    numberOfChildContainers++;
                    cd.nestedContainer = true;
                }

                //Im automating childOrder, need to distinguish between auto and manual
                cd.childOrder = i;
                flex.childOrder = i;

                cd.autoHeight = flex.autoHeight;
                cd.autoWidth = flex.autoWidth;

                cd.marginTypes = new Vector4(flex.topMarginType, flex.bottomMarginType, flex.rightMarginType, flex.leftMarginType);
                cd.marginValues = new Vector4(flex.topMarginValue, flex.bottomMarginValue, flex.rightMarginValue, flex.leftMarginValue);


                childrenDict.Add(cont.GetChild(i).gameObject.GetInstanceID(), cd);

                childKeys.Add(cont.GetChild(i).gameObject.GetInstanceID());

            }
        }
        childrenDict = childrenDict.OrderBy(x => x.Value.childOrder).ToDictionary(x => x.Key, x => x.Value);


    }

    public void SetContainerSize()
    {

        if (RootContainer)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
            gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            //   SetChildContainerSize();
        }




    }

    public void GetContainerInfo()
    {

        containerCenter = cont.rect.center;
        height = cont.rect.height;
        width = cont.rect.width;
    }

    public void LineLengthDetermination()
    {
        float remainingPrecent = 100;

        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {

            if (k.Value.childFlexBasis != 0)
            {
                if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
                {

                    k.Value.definedBasis = cont.sizeDelta.x * (k.Value.childFlexBasis / 100);
                    remainingPrecent -= k.Value.childFlexBasis;

                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);



                }
                if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
                {
                    k.Value.definedBasis = cont.sizeDelta.y * (k.Value.childFlexBasis / 100);
                    remainingPrecent -= k.Value.childFlexBasis;
                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);

                }

            }
            else
            {
                if (flexDirectionIndex == 0 || flexDirectionIndex == 1)
                {


                    // k.Value.definedBasis = (cont.sizeDelta.x * ((remainingPrecent / 100) / cont.childCount));
                    k.Value.definedBasis = ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, true);

                    k.Value.hypotheticalCrossSize = Mathf.Clamp(ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, false), k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);


                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);

                }
                if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
                {
                    //  k.Value.definedBasis = (cont.sizeDelta.y * ((remainingPrecent / 100) / cont.childCount));
                    k.Value.definedBasis = ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, false);
                    k.Value.hypotheticalCrossSize = Mathf.Clamp(ContentSizeCalc.DetermineContentSize(k.Value.childRect.gameObject, true), k.Value.childWidthMinMax.x, k.Value.childWidthMinMax.y);

                    k.Value.hypotheticalMainSize = Mathf.Clamp(k.Value.definedBasis, k.Value.childHeightMinMax.x, k.Value.childHeightMinMax.y);

                }

            }

        }
    }


    public void PositionItems(bool CheckForNewLines)
    {
        var edge = RectTransform.Edge.Left;
        var inset = cont.rect.width;
        bool row = true;
        previousLineNumber = 1;

        firstItem = true;
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
            /*             if(nextChildIncrement){
                            k.Value.LineNumber++;
                        } */
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


            if (row)
            {

                rt.sizeDelta = new Vector2(k.Value.hypotheticalMainSize, rt.sizeDelta.y);
                listOfTotalWidths[k.Value.LineNumber - 1] += k.Value.hypotheticalMainSize;
            }
            else
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, k.Value.hypotheticalMainSize);
                listOfTotalWidths[k.Value.LineNumber - 1] += rt.sizeDelta.y;
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

        CalculateChildrenLines(edge, inset, rtFirst, row);
    }

    public void MainSizeDetermination()
    {
        bool row = true;
        if (flexDirectionIndex == 2 || flexDirectionIndex == 3)
        {
            row = false;
        }
        PositionItems(true);

        ResolveFlexibleLengths();
        MainAxisAlignment(true);
        CrossSizeDetermination(row);
        CrossAsixAlignment(row);




    }



    public void printChildrenDict()
    {
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            Debug.Log("Name: " + k.Value.childRect.gameObject.name + " LineNumber: " + k.Value.LineNumber);
        }
    }
    public void ResolveFlexibleLengths()
    {
        int numberOfLines = GetNumberOfLines();

        List<float> freeSpacePerLine = CalculateWorldFreeSpaceList();
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

                freeSpacePerLine = CalculateWorldFreeSpaceList();
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

    public void CrossSizeDetermination(bool row)
    {
        /*
        Determine the hypothetical cross size of each item by performing layout as if it were an in-flow block-level box with the used main size and the given available space, treating auto as fit-content.
        Calculate the cross size of each flex line.
        */

        List<float> crossSizePerLine = new List<float>();
        int lines = GetNumberOfLines();
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            if (row && alignContentIndex != 3)
            {
                k.Value.childRect.sizeDelta = new Vector2(k.Value.childRect.sizeDelta.x, k.Value.hypotheticalCrossSize);
            }
            else if (row && alignContentIndex == 3) k.Value.childRect.sizeDelta = new Vector2(k.Value.childRect.sizeDelta.x, cont.rect.height / lines);
            else k.Value.childRect.sizeDelta = new Vector2(k.Value.hypotheticalCrossSize, k.Value.childRect.sizeDelta.y);
        }
        for (int i = 0; i < lines; i++) crossSizePerLine.Add(0);
        if (lines == 1)
        {
            if (row) crossSizePerLine[0] = cont.rect.height;
            else crossSizePerLine[0] = cont.rect.width;
        }



        /*

        If the flex container is single-line and has a definite cross size, the cross size of the flex line is the flex container’s inner cross size.

        Otherwise, for each flex line:

            Collect all the flex items whose inline-axis is parallel to the main-axis, whose align-self is baseline, and whose cross-axis margins are both non-auto. Find the largest of the distances between each item’s baseline and its hypothetical outer cross-start edge, and the largest of the distances between each item’s baseline and its hypothetical outer cross-end edge, and sum these two values.
            Among all the items not collected by the previous step, find the largest outer hypothetical cross size.
            The used cross-size of the flex line is the largest of the numbers found in the previous two steps and zero.

            If the flex container is single-line, then clamp the line’s cross-size to be within the container’s computed min and max cross sizes. Note that if CSS 2.1’s definition of min/max-width/height applied more generally, this behavior would fall out automatically.
        */
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
        List<float> crossSizeTotals = new List<float>();

        for (int i = 0; i < lines; i++)
        {
            crossSizeTotals.Add(0);
        }
        for (int i = 0; i < lines; i++)
        {
            if (i > 1)
            {
                crossSizeTotals[i] = crossSizeTotals[i - 1] + crossSizePerLine[i];
            }
            else crossSizeTotals[i] = crossSizeTotals[i] + crossSizePerLine[i];
        }
        for (int i = 0; i < lines; i++)
        {
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.LineNumber == (i + 1) && k.Value.LineNumber > 1)
                {
                    if (row) k.Value.childRect.localPosition = new Vector2(k.Value.childRect.localPosition.x, k.Value.childRect.localPosition.y - crossSizeTotals[i]);
                    //     if (i > 2) Debug.Log(crossSizeTotals[i]);
                }
            }
        }
        /*
        Handle 'align-content: stretch'. If the flex container has a definite cross size, align-content is stretch, and the sum of the flex lines' cross sizes is less than the flex container’s inner cross size, increase the cross size of each flex line by equal amounts such that the sum of their cross sizes exactly equals the flex container’s inner cross size.
        Collapse visibility:collapse items. If any flex items have visibility: collapse, note the cross size of the line they’re in as the item’s strut size, and restart layout from the beginning.
        */

        /*
        In this second layout round, when collecting items into lines, treat the collapsed items as having zero main size. For the rest of the algorithm following that step, ignore the collapsed items entirely (as if they were display:none) except that after calculating the cross size of the lines, if any line’s cross size is less than the largest strut size among all the collapsed items in the line, set its cross size to that strut size.

        Skip this step in the second layout round.
        Determine the used cross size of each flex item. If a flex item has align-self: stretch, its computed cross size property is auto, and neither of its cross-axis margins are auto, the used outer cross size is the used cross size of its flex line, clamped according to the item’s used min and max cross sizes. Otherwise, the used cross size is the item’s hypothetical cross size.

        If the flex item has align-self: stretch, redo layout for its contents, treating this used size as its definite cross size so that percentage-sized children can be resolved
        */
    }

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

            int maxOrder = GetMaxOrderPerLine(i + 1);
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




                if (k.Value.LineNumber == (i + 1))
                {
                    float fixFirst = 1;
                    rt = k.Value.childRect;

                    k.Value.childRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rt.rect.width);


                    if (firstItem)
                    {
                        rtPrev = rt;
                        firstItem = false;

                        fixFirst = 0;
                        j = 0.5f;
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

                        //continue;

                    }
                    else j = 1;



                    if ((flexDirectionIndex == 0 && justifyContentIndex != 1) || (flexDirectionIndex == 1 && justifyContentIndex == 1))
                    {

                        k.Value.childRect.localPosition = new Vector2(rtPrev.localPosition.x + ((rtPrev.sizeDelta.x / 2 * fixFirst) + (rt.sizeDelta.x / 2 * fixFirst)) + firstmarginX + secondmarginX + marginForEach * j, k.Value.childRect.localPosition.y + firstmarginY + secondmarginY);



                    }
                    else if ((flexDirectionIndex == 1 && justifyContentIndex != 1) || (flexDirectionIndex == 0 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(rtPrev.localPosition.x - ((rtPrev.sizeDelta.x / 2 * fixFirst) + (rt.sizeDelta.x / 2 * fixFirst)) + firstmarginX + secondmarginX, k.Value.childRect.localPosition.y + firstmarginY + secondmarginY);

                    }
                    else if ((flexDirectionIndex == 2 && justifyContentIndex != 1) || (flexDirectionIndex == 3 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(k.Value.childRect.localPosition.x + firstmarginX + secondmarginX, rtPrev.localPosition.y - ((rtPrev.sizeDelta.y / 2 * fixFirst) + (rt.sizeDelta.y / 2 * fixFirst)) + firstmarginY + secondmarginY);

                    }
                    else if ((flexDirectionIndex == 3 && justifyContentIndex != 1) || (flexDirectionIndex == 2 && justifyContentIndex == 1))
                    {
                        k.Value.childRect.localPosition = new Vector2(k.Value.childRect.localPosition.x + firstmarginX + secondmarginX, rtPrev.localPosition.y + ((rtPrev.sizeDelta.y / 2 * fixFirst) + (rt.sizeDelta.y / 2 * fixFirst)) + firstmarginY + secondmarginY);

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
    public void CrossAsixAlignment(bool row)
    {
        int lines = GetNumberOfLines();
        Vector3[] childCorners = new Vector3[4];
        Vector3[] parentCorners = new Vector3[4];
        RectTransform rt = null;
        if (alignContentIndex == 0 || alignContentIndex == 3)
        {
            for (int i = 0; i < lines; i++)
            {
                float maxCrossSize = 0;
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
                            if (row)
                            {
                                float distanceToEdge = parentCorners[1].y - childCorners[1].y;

                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y - rt.sizeDelta.y / 2) - distanceToEdge);
                            }
                            else
                            {
                                float distanceToEdge = parentCorners[1].x - childCorners[1].x;
                                rt.localPosition = new Vector2((rt.localPosition.x - rt.sizeDelta.x / 2) - distanceToEdge, rt.localPosition.y);

                            }

                        }
                        else if (i > 0)
                        {
                            if (row)
                            {
                                float distanceToEdge = parentCorners[1].y - childCorners[1].y;
                                float incrementDistace = GetMaxCrossSizePerLine(i, row);
                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) - (distanceToEdge) - incrementDistace);
                            }
                            else
                            {
                                float distanceToEdge = parentCorners[1].x - childCorners[1].x;
                                float incrementDistace = GetMaxCrossSizePerLine(i, row);
                                rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.x / 2) - (distanceToEdge) - incrementDistace, rt.localPosition.y);
                            }


                        }

                    }
                }
            }
        }
        if (alignContentIndex == 1)
        {
            for (int i = lines; i > 0; i--)
            {
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
                            float increment = GetMaxCrossSizePerLine(i - 1, row);
                            if (row)
                            {

                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) + distanceToEdge + increment);
                            }
                            else
                            {
                                rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.y / 2) + distanceToEdge + increment, rt.localPosition.y);
                            }

                        }
                        else if (lines > 1)
                        {
                            float incrementDistace = GetMaxCrossSizePerLine(i + 1, row);
                            if (row)
                            {
                                rt.localPosition = new Vector2(rt.localPosition.x, (rt.localPosition.y + rt.sizeDelta.y / 2) + distanceToEdge + incrementDistace);
                            }
                            else
                            {
                                rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.x/ 2) + distanceToEdge + incrementDistace, rt.localPosition.y);
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
            total = (cont.rect.height - total) / 2;
            for (int i = 0; i < lines; i++)
            {
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
                                float distanceToEdge = (parentCorners[1].x + total) - childCorners[1].x;
                                rt.localPosition = new Vector2((rt.localPosition.x- rt.sizeDelta.x / 2) - distanceToEdge, rt.localPosition.y );
                            }


                        }
                        else if (i > 0)
                        {
                            float distanceToEdge = (parentCorners[1].y + total) - childCorners[1].y;
                            float incrementDistace = GetMaxCrossSizePerLine(i + 1, row);
                            rt.localPosition = new Vector2((rt.localPosition.x + rt.sizeDelta.x / 2) - distanceToEdge - incrementDistace, rt.localPosition.y);
                        }

                    }


                }
            }
        }

    }
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

    public List<float> CalculateWorldFreeSpaceList()
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
                        freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.targetMainSize.x;
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
    public List<float> CalculateWorldFreeSpaceListv3(bool row)
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
                    if (row) freeSpacePerLine[k.Value.LineNumber - 1] = freeSpacePerLine[k.Value.LineNumber - 1] - k.Value.hypotheticalMainSize;
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

    int previousLineNumber = 1;


    public void CalculateChildrenLines(RectTransform.Edge edge, float inset, RectTransform rtFirst, bool row)
    {
        float total = 0;
        bool first = true;
        foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
        {
            total += k.Value.hypotheticalMainSize;

        }
        float numberofLines = total / cont.rect.width;

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
            else contMainSize = cont.rect.width;
            float minSizeComparison = 0;
            foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
            {
                if (k.Value.hypotheticalMainSize > minSizeComparison)
                {
                    minSizeComparison = k.Value.hypotheticalMainSize;
                }
            }
            if (contMainSize > minSizeComparison) minSizeComparison = cont.rect.width;

            //    Debug.Log(minSizeComparison);
            for (int i = 1; i <= roundedLines; i++)
            {
                float TotalPerLine = 0;
                if (i == 1) TotalPerLine = rtFirst.sizeDelta.x;
                else TotalPerLine = 0;
                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.childOrder == 0)
                    {
                        continue;
                    }

                    if (k.Value.childRect.gameObject.name == "Image (2)") Debug.Log(minSizeComparison + " " + TotalPerLine + " " + i);
                    if (!k.Value.doesFit && (k.Value.hypotheticalMainSize + TotalPerLine <= minSizeComparison))
                    {

                        TotalPerLine += k.Value.hypotheticalMainSize;
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
            for (int i = 2; i <= roundedLines; i++)
            {

                foreach (KeyValuePair<int, FlexChildren.ChildrenData> k in childrenDict)
                {
                    if (k.Value.LineNumber == i)
                    {
                        k.Value.childRect.SetInsetAndSizeFromParentEdge(edge, 0, inset);
                        break;

                    }



                }

            }
        }
        //printChildrenDict();
    }




}
