using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private IEnumerable<object> dataSource;

    public GameObject contentPrefab;
    public GameObject contentHolder;
    private Transform firstVisibleElement;
    private Transform lastVisibleElement;
    private int numberOfElementsFitTheViewport;

    public int listCount = 20;
    private List<GameObject> listItems;

    private ScrollRect scrollrect;

    private int lastIndex;
    private int firstIndex;

    private int popElementAt = 3;
    private float elementHeight;
    private Vector2 previousPosition;
    public float moveThreshold = 0.01f;
    public float dragSpeed = 1f;
    private float scrollviewHeight;

    private void Awake ()
    {
        listItems = new List<GameObject>(listCount);
        scrollrect = GetComponent<ScrollRect>();
        for (int i = 0; i < listCount; i++)
        {
            listItems.Add(Instantiate(contentPrefab, contentHolder.transform));
        }
        scrollrect.StopMovement();
        elementHeight = scrollrect.content.GetChild(0).GetComponent<RectTransform>().rect.height;
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
        scrollviewHeight = scrollrect.GetComponent<RectTransform>().rect.height;
        numberOfElementsFitTheViewport = Mathf.CeilToInt(scrollviewHeight / elementHeight);

        firstVisibleElement = scrollrect.content.GetChild(0);
        lastVisibleElement = scrollrect.content.GetChild(numberOfElementsFitTheViewport);
    }


    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        //Debug.Log("start dragging");
        previousPosition = pointerEventData.position;
    }

    private float elementsOutOfView = 0f;

    public void OnDrag(PointerEventData pointerEventData)
    {
        var dragPosition = pointerEventData.position - previousPosition;
        if (dragPosition.sqrMagnitude < moveThreshold)
            return;

        var scrollDirectionDown = (dragPosition.y > 0.0f);
        var firstElement = scrollrect.content.GetChild(0);
        var lastElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);

        
        elementsOutOfView = Mathf.Abs(scrollrect.content.transform.localPosition.y / elementHeight);
        Debug.Log("out of view: " + elementsOutOfView + " local pos: " + scrollrect.content.transform.localPosition.y + " height: " + elementHeight);

            

        firstVisibleElement = scrollrect.content.GetChild((int)elementsOutOfView);
        contentController controller = firstVisibleElement.GetComponent<contentController>();

        if ((int)elementsOutOfView + numberOfElementsFitTheViewport >= scrollrect.content.childCount)
            lastVisibleElement = scrollrect.content.GetChild(scrollrect.content.childCount - 1);
        else
            lastVisibleElement = scrollrect.content.GetChild((int)elementsOutOfView + numberOfElementsFitTheViewport);

        // user scrolls down
        if (scrollDirectionDown)
        {
            Debug.Log("Scroll down");
            if (IsTheEndReached(lastVisibleElement))
            {
                Debug.Log("last element at bottom");
                if (scrollrect.content.localPosition.y == (elementHeight * (scrollrect.content.childCount - (scrollviewHeight / elementHeight))))
                    return;
            }

            if (scrollrect.content.localPosition.y + dragPosition.magnitude > (elementHeight * (scrollrect.content.childCount - (scrollviewHeight / elementHeight))))
                scrollrect.content.localPosition = new Vector3(0, (elementHeight * (scrollrect.content.childCount - (scrollviewHeight / elementHeight))), 0);
            else
                scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y + dragPosition.magnitude * dragSpeed, 0);
        }
        else  // user scrolls up
        {
            Debug.Log("Scroll up");
            if (IsTheTopReached(firstVisibleElement))
            {
                Debug.Log("first is top");
                if (scrollrect.content.localPosition.y == 0)
                    return;
            }

            if (scrollrect.content.localPosition.y - dragPosition.magnitude < 0)
                scrollrect.content.localPosition = new Vector3(0, 0, 0);
            else
                scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y - dragPosition.magnitude * dragSpeed, 0);
        }

        if (scrollDirectionDown && (scrollrect.content.localPosition.y > (popElementAt * elementHeight)))
        {
            if (lastIndex < dataSource.Count()-1)
            {
                firstIndex++;
                lastIndex++;
            }
            else
                return;

            firstElement.transform.SetSiblingIndex(scrollrect.content.childCount);

            scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y - elementHeight, 0);

            // get next element from list to display
            var data = dataSource.ElementAt(lastIndex);

            var contentController = firstElement.GetComponent<contentController>();
            contentController.SetContents(data);
        }

        if (!scrollDirectionDown && (scrollrect.content.localPosition.y < (popElementAt * elementHeight)))
        {
            if (firstIndex <= 0)
                return;
            else
            {
                firstIndex--;
                lastIndex--;
            }

            lastElement.transform.SetSiblingIndex(0);

            scrollrect.content.localPosition = new Vector3(0, scrollrect.content.localPosition.y + elementHeight, 0);

            // get next element from list to display
            var data = dataSource.ElementAt(firstIndex);

            var contentController = lastElement.GetComponent<contentController>();
            contentController.SetContents(data);
        }
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

    public void OnEndDrag(PointerEventData data)
    {
        //Debug.Log("stop dragging");
    }
}
