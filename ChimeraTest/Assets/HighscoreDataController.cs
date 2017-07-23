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
    private JsonDataService service;

    public Sprite[] trophySprites;
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
            Initialize();
        }
	}

    private void Initialize()
    {
        players = service.GetHighscoreList().ToList();
        scrollViewController.SetDataSource(players.Cast<object>().ToList());
    }

    private Sprite GetTrophyIcon(int score)
    {
        if (score >= 900)
            return trophySprites[0];
        else if (score >= 800)
            return trophySprites[1];
        else if (score >= 700)
            return trophySprites[2];
        else if (score >= 600)
            return trophySprites[3];
        else if (score >= 500)
            return trophySprites[4];
        else if (score >= 400)
            return trophySprites[5];
        else if (score >= 300)
            return trophySprites[6];
        else if (score >= 200)
            return trophySprites[7];
        else if (score >= 100)
            return trophySprites[8];
        else
            return trophySprites[9];
    }
}

public enum DataServiceType
{
    Json,
    Database,
    Server
}

