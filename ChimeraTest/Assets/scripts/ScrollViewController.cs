using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    private IEnumerable<object> dataSource;

    public GameObject contentPrefab;
    public GameObject contentHolder;

    public int listCount = 20;
    private List<GameObject> listItems;

    private ScrollRect scrollrect;

    private int lastIndex;
    private int firstIndex;

    private float minDragOffset;

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
            Debug.LogWarning("source is null");
        else
        {
            // minDragOffset is used for the verticalNormalizedPosition to set the position manually
            minDragOffset = 1f / source.Count();
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
    }

    // When the user scrolls up or down the list, the first or last elements get sorted at the end or front and their content gets changed
    // to the next elements in scroll direction
    public void OnScrollViewValueChanged(Vector2 value)
    {
        if (scrollrect.velocity.y > 0)
        {
            if (lastIndex == dataSource.Count() - 1)
                return;

            GameObject movedObject = MoveObjectInList(0, listItems.Count - 1);
            movedObject.transform.SetSiblingIndex(listItems.Count);

            lastIndex++;
            firstIndex++;

            contentController controller = movedObject.GetComponent<contentController>();
            object data = dataSource.ElementAt(lastIndex);
            controller.SetContents(data);
        }
        else if (scrollrect.velocity.y < 0)
        {
            if (firstIndex == 0)
                return;

            GameObject movedObject = MoveObjectInList(listItems.Count - 1, 0);
            movedObject.transform.SetSiblingIndex(0);
            lastIndex--;
            firstIndex--;

            contentController controller = movedObject.GetComponent<contentController>();
            object data = dataSource.ElementAt(firstIndex);
            controller.SetContents(data);
        }

        // To make the scrolling work although the scrollview just has 20 elements and the 
        // normalizedPosition is based on those 20 elements (0-1), the verticalNormalizedPosition
        // gets set manually by using the minDragOffset
        scrollrect.verticalNormalizedPosition = 1f - (firstIndex * minDragOffset);
    }

    private GameObject MoveObjectInList(int from, int to)
    {
        GameObject go = listItems[from];

        listItems.RemoveAt(from);
        listItems.Insert(to, go);

        return go;
    }
}
