using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private IEnumerable<object> dataSource;

    public GameObject contentPrefab;
    public GameObject contentHolder;
    private Transform firstVisibleElement;
    private Transform lastVisibleElement;
    private float numberOfElementsFitTheViewport;

    public int listCount = 20;
    private List<GameObject> listItems;

    private ScrollRect scrollrect;

    private int lastIndex;
    private int firstIndex;

    public int popElementAt = 3;
    private float elementHeight;
    private Vector2 previousPosition;
    public float moveThreshold = 0.01f;
    public float dragSpeed = 1f;
    private float scrollviewHeight;
    private float elementsOutOfView = 0f;
    public float inertiaThreshold = 60.0f;
    private bool inertia = false;

    private PointerEventData dragEndData;

    private void Awake ()
    {
        listItems = new List<GameObject>(listCount);
        scrollrect = GetComponent<ScrollRect>();
        for (int i = 0; i < listCount; i++)
        {
            listItems.Add(Instantiate(contentPrefab, contentHolder.transform));
        }
    }


    public void SetDataSource(IEnumerable<object> source)
    {
        if (source == null)
        {
            Debug.LogWarning("source is null");
        }
        else
        {
            dataSource = source;

            GenerateInitialList();
        }
    }

    private void GenerateInitialList()
    {
        int initialListSize = Math.Min(dataSource.Count(), listCount);

        for(int i = 0; i < initialListSize; i++)
        {
            object data = dataSource.ElementAt(i);
            GameObject listItem = listItems[i];

            contentController controller = listItem.GetComponent<contentController>();
            controller.SetContents(data);
        }
        firstIndex = 0;
        lastIndex = initialListSize;

        elementHeight = scrollrect.content.GetChild(0).GetComponent<RectTransform>().rect.height;
        scrollviewHeight = scrollrect.GetComponent<RectTransform>().rect.height;
        numberOfElementsFitTheViewport = scrollviewHeight / elementHeight;

        firstVisibleElement = scrollrect.content.GetChild(0);
        lastVisibleElement = scrollrect.content.GetChild(Mathf.CeilToInt(numberOfElementsFitTheViewport));
    }


    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        previousPosition = pointerEventData.position;
    }

    private Vector2 dragPosition;

    public void OnDrag(PointerEventData pointerEventData)
    {
        dragPosition = pointerEventData.position - previousPosition;
        if (dragPosition.sqrMagnitude < moveThreshold)
            return;

        var scrollDirectionDown = (dragPosition.y > 0.0f);
        var firstElement = scrollrect.content.GetChild(0);
        var lastElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);


        elementsOutOfView = scrollrect.content.transform.localPosition.y / elementHeight;

        firstVisibleElement = scrollrect.content.GetChild((int)elementsOutOfView);


        var lastVisibleIndex = (int)(elementsOutOfView + numberOfElementsFitTheViewport);
        if (lastVisibleIndex >= scrollrect.content.childCount)
            lastVisibleElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);
        else
            lastVisibleElement = scrollrect.content.GetChild(lastVisibleIndex);


        MoveContent(scrollDirectionDown, dragPosition);

        if (scrollDirectionDown && (scrollrect.content.localPosition.y > (popElementAt * elementHeight)))
        {
            PopFirstAndMoveToEnd(firstElement);
        }

        if (!scrollDirectionDown && (scrollrect.content.localPosition.y < (popElementAt * elementHeight)))
        {
            PopLastAndMoveToFirst(lastElement);
        }
    }

    private void MoveContent(bool scrollDirectionDown, Vector2 dragPosition)
    {
        if (scrollDirectionDown)
        {
            var unvisibleElementCount = scrollrect.content.childCount - numberOfElementsFitTheViewport;
            // if the last element has the lowest rank and the scrollrects position shows the last element at the bottom, stop moving the content.
            if (IsTheEndReached(lastVisibleElement))
            {
                if (scrollrect.content.localPosition.y == (elementHeight * unvisibleElementCount))
                    return;
            }

            if (scrollrect.content.localPosition.y + dragPosition.magnitude > (elementHeight * unvisibleElementCount))
                scrollrect.content.localPosition = new Vector3(0, (elementHeight * unvisibleElementCount), 0);
            else
                scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y + dragPosition.magnitude * dragSpeed, 0);
        }
        else  // user scrolls up, move the content
        {
            if (IsTheTopReached(firstVisibleElement))
            {
                if (scrollrect.content.localPosition.y == 0)
                    return;
            }

            if (scrollrect.content.localPosition.y - dragPosition.magnitude < 0)
                scrollrect.content.localPosition = new Vector3(0, 0, 0);
            else
                scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y - dragPosition.magnitude * dragSpeed, 0);
        }
    }

    private void PopFirstAndMoveToEnd(Transform firstElement)
    {
        if (lastIndex < dataSource.Count() - 1)
        {
            firstIndex++;
            lastIndex++;
        }
        else
            return;

        firstElement.SetSiblingIndex(scrollrect.content.childCount);

        scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y - elementHeight, 0);

        // get next element from list to display
        var data = dataSource.ElementAt(lastIndex);

        var contentController = firstElement.GetComponent<contentController>();
        contentController.SetContents(data);
    }

    private void PopLastAndMoveToFirst(Transform lastElement)
    {
        if (firstIndex <= 0)
            return;
        else
        {
            firstIndex--;
            lastIndex--;
        }

        lastElement.SetSiblingIndex(0);

        scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y + elementHeight, 0);

        // get next element from list to display
        var data = dataSource.ElementAt(firstIndex);

        var contentController = lastElement.GetComponent<contentController>();
        contentController.SetContents(data);
    }

    private bool IsTheTopReached(Transform firstVisible)
    {
        int rank = Convert.ToInt32(firstVisible.GetComponent<contentController>().rank.text);

        if (rank == 1)
            return true;
        else
            return false;
    }

    private bool IsTheEndReached(Transform lastVisible)
    {
        int rank = Convert.ToInt32(lastVisible.GetComponent<contentController>().rank.text);

        if (rank == dataSource.Count())
            return true;
        else
            return false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("dragposition: " + dragPosition.magnitude);
        if (dragPosition.magnitude > inertiaThreshold)
            inertia = true;

        dragEndData = eventData;
        lerpVal = dragSpeed;
    }

    private void Update()
    {
        if(inertia)
        {
            
            AccelerateScrolling(dragEndData);
        }
    }

    private float lerpVal;

    private void AccelerateScrolling(PointerEventData data)
    {
        dragPosition = data.position - previousPosition;
        if (dragPosition.sqrMagnitude < moveThreshold)
            return;

        var scrollDirectionDown = (dragPosition.y > 0.0f);
        var firstElement = scrollrect.content.GetChild(0);
        var lastElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);

            elementsOutOfView = scrollrect.content.transform.localPosition.y / elementHeight;

            firstVisibleElement = scrollrect.content.GetChild((int)elementsOutOfView);


            var lastVisibleIndex = (int)(elementsOutOfView + numberOfElementsFitTheViewport);
            if (lastVisibleIndex >= scrollrect.content.childCount)
                lastVisibleElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);
            else
                lastVisibleElement = scrollrect.content.GetChild(lastVisibleIndex);


            if (scrollDirectionDown)
            {
                var unvisibleElementCount = scrollrect.content.childCount - numberOfElementsFitTheViewport;
                // if the last element has the lowest rank and the scrollrects position shows the last element at the bottom, stop moving the content.
                if (IsTheEndReached(lastVisibleElement))
                {
                    if (scrollrect.content.localPosition.y == (elementHeight * unvisibleElementCount))
                        return;
                }

                if (scrollrect.content.localPosition.y + dragPosition.magnitude > (elementHeight * unvisibleElementCount))
                    scrollrect.content.localPosition = new Vector3(0, (elementHeight * unvisibleElementCount), 0);
                else
                    scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y + dragPosition.magnitude * lerpVal, 0);
            }
            else  // user scrolls up, move the content
            {
                if (IsTheTopReached(firstVisibleElement))
                {
                    if (scrollrect.content.localPosition.y == 0)
                        return;
                }

                if (scrollrect.content.localPosition.y - dragPosition.magnitude < 0)
                    scrollrect.content.localPosition = new Vector3(0, 0, 0);
                else
                    scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y - dragPosition.magnitude * lerpVal, 0);
            }

            if (scrollDirectionDown && (scrollrect.content.localPosition.y > (popElementAt * elementHeight)))
            {
                PopFirstAndMoveToEnd(firstElement);
            }

            if (!scrollDirectionDown && (scrollrect.content.localPosition.y < (popElementAt * elementHeight)))
            {
                PopLastAndMoveToFirst(lastElement);
            }

        lerpVal -= 0.01f;
        Debug.Log(lerpVal);
        if(lerpVal <= 0)
           inertia = false;
    }
}
