using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class HighscoreDataController : MonoBehaviour {

    public string dataPath;

    public DataServiceType serviceType;
    private JsonDataService service;

    // Use this for initialization
    void Start () {
        if (string.IsNullOrEmpty(dataPath))
            Debug.Log("no dataPath!");
        else
        {
            switch (serviceType)
            {
                case DataServiceType.Json:
                    service = new JsonDataService(Path.Combine(Application.dataPath, dataPath));
                    break;
                case DataServiceType.Database:
                    // service = new DatabaseDataService(Path.Combine(Application.dataPath, dataPath));
                    break;
                case DataServiceType.Server:
                    // service = new ServerDataService(Path.Combine(Application.dataPath, dataPath));
                    break;
            }

            service.Initialize();
        }

	}
}

public enum DataServiceType
{
    Json,
    Database,
    Server
}