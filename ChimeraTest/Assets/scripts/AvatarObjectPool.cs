using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AvatarObjectPool : MonoBehaviour
{
    private static readonly DateTime MY_RETIRE_DATE = new DateTime(2056, 11, 1);
    public int avatarLifeSpanInSeconds = 10;

    class PoolObject
    {
        public string avatarName;
        public GameObject gameObject;
        public DateTime removeTime;
    }

    private List<PoolObject> avatarList;
    public GameObject[] avatarPrefabs;

    private void Start()
    {
        avatarList = new List<PoolObject>();
    }

    public GameObject GetAvatar(string avatarname)
    {
        // searching for the avatarobject. If it's found and inactive (free to use) return it
        foreach (PoolObject obj in avatarList.OrderByDescending(p=>p.removeTime))
        {
            if (obj.avatarName == avatarname && obj.gameObject.activeSelf == false)
            {
                // set remove time to something to far in the future to reach while using the avatar in the list
                obj.removeTime = MY_RETIRE_DATE;
                return obj.gameObject;
            }
        }

        GameObject avatarPrefab = avatarPrefabs.FirstOrDefault((GameObject obj) => obj.name == avatarname);
        GameObject go;
        
        if (avatarPrefab == null)
        {
            go = Instantiate(avatarPrefabs[0], this.transform);
            return go;
        }
        else
        {
            go = Instantiate(avatarPrefab, this.transform);
            avatarList.Add(new PoolObject()
            {
                avatarName = avatarname,
                gameObject = go,
                removeTime = MY_RETIRE_DATE
            });

            return go;
        }

    }

    public void ReturnAvatar(GameObject avatarToReturn)
    {
        avatarToReturn.SetActive(false);
        PoolObject obj = avatarList.First(p => p.gameObject == avatarToReturn);
        obj.removeTime = DateTime.Now.AddSeconds(avatarLifeSpanInSeconds);
    }

    // not good to check this every update - todo
    private void Update()
    {
        // searching for objects to destroy after removeTime
        var poolObjects = avatarList.Where(p => p.removeTime < DateTime.Now).ToList();
        foreach(PoolObject obj in poolObjects)
        {
            avatarList.Remove(obj);
            Destroy(obj.gameObject);
        }
    }
}


