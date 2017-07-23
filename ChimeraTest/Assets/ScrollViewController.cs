using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollViewController : MonoBehaviour
{

    private IEnumerable<object> dataSource;

    public GameObject contentPrefab;
    public GameObject contentHolder;

    public int listCount = 20;
    private List<GameObject> listItems;


	private void Awake ()
    {
        listItems = new List<GameObject>(listCount);
        for(int i = 0; i < listCount; i++)
        {
            GameObject temp = Instantiate(contentPrefab, contentHolder.transform);
            listItems.Add(temp);
        }
	}


    public void SetDataSource(IEnumerable<object> source)
    {
        if (source == null)
            Debug.LogWarning("source is null");
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
    }
}
