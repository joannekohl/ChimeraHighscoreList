using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public class HighscoreDataController : MonoBehaviour {

    //reading and saving highscore data
    public string dataPath;
    public DataServiceType serviceType;
    private JsonDataService service;

    //
    public GameObject highscoreContentObject;
    public GameObject contentPrefab;
    public GameObject[] avatarObjects;
    public Sprite[] trophySprite;


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
            InitializeHighscoreList();
        }


	}

    private void InitializeHighscoreList()
    {
        List<PlayerDTO> players = service.GetHighscoreList().Take(12).ToList();

        foreach (PlayerDTO player in players)
        {
            GameObject content = Instantiate(contentPrefab, highscoreContentObject.transform);
            contentController controller = content.GetComponent<contentController>();

            Sprite trophy = GetTrophyIcon(player.Score);
            GameObject playerAvatar = GetPlayerAvatar(player.Avatar);

            controller.SetContents(player.Rank, player.Score, trophy, playerAvatar, player.Playername);
        }
    }

    private Sprite GetTrophyIcon(int score)
    {
        if (score >= 900)
            return trophySprite[0];
        else if (score >= 800)
            return trophySprite[1];
        else if (score >= 700)
            return trophySprite[2];
        else if (score >= 600)
            return trophySprite[3];
        else if (score >= 500)
            return trophySprite[4];
        else if (score >= 400)
            return trophySprite[5];
        else if (score >= 300)
            return trophySprite[6];
        else if (score >= 200)
            return trophySprite[7];
        else if (score >= 100)
            return trophySprite[8];
        else
            return trophySprite[9];
    }

    private GameObject GetPlayerAvatar(string avatarName)
    {
        for(int i = 0; i < avatarObjects.Count(); i++)
        {
            if (avatarObjects[i].name == avatarName)
                return avatarObjects[i];
        }

        return avatarObjects[0];
    }
}



public enum DataServiceType
{
    Json,
    Database,
    Server
}

