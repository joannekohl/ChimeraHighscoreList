using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

public class HighscoreDataController : MonoBehaviour
{
    public string dataPath;
    public DataServiceType serviceType;
    private IHighscoreDataService service;
    private List<PlayerDTO> players;
    public ScrollViewController scrollViewController;

    void Start ()
    {
        if (string.IsNullOrEmpty(dataPath))
            Debug.Log("no dataPath!");
        else
        {
            switch (serviceType)
            {
                case DataServiceType.Json:
                    service = new JsonDataService(dataPath);
                    break;
                case DataServiceType.Database:
                    // service = new DatabaseDataService(Path.Combine(Application.dataPath, dataPath));
                    break;
                case DataServiceType.Server:
                    // service = new ServerDataService(Path.Combine(Application.dataPath, dataPath));
                    break;
            }
            service.Initialize();
            Initialize();
        }
	}

    private void Initialize()
    {
        players = service.GetHighscoreList().ToList();
        scrollViewController.SetDataSource(players.Cast<object>().ToList());
    }
}

public enum DataServiceType
{
    Json,
    Database,
    Server
}

