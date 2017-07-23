using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AvatarObjectPool : MonoBehaviour
{
    struct PoolObject
    {
        public string avatarName;
        public GameObject gameObject;
    }

    private List<PoolObject> avatarList;
    public GameObject[] avatarPrefabs;

    private void Start()
    {
        avatarList = new List<PoolObject>();
    }

    public GameObject GetAvatar(string avatarname)
    {
        // searching for the avatarobject. If it's found and inactive - so free to use, return it
        foreach (PoolObject obj in avatarList)
        {
            if (obj.avatarName == avatarname && obj.gameObject.activeSelf == false)
            {
                return obj.gameObject;
            }
        }

        GameObject avatarPrefab = avatarPrefabs.FirstOrDefault((GameObject obj) => obj.name == avatarname);
        GameObject go;
        
        if (avatarPrefab == null)
        {
            Debug.Log("avatarprefab name: " + avatarname + " is unknown");
            go = Instantiate(avatarPrefabs[0], this.transform);

            return go;
        }
        else
        {
            go = Instantiate(avatarPrefab, this.transform);
            avatarList.Add(new PoolObject()
            {
                avatarName = avatarname,
                gameObject = go
            });

            return go;
        }

    }

    public void ReturnObject(GameObject toReturn)
    {
        toReturn.SetActive(false);
    }
}


