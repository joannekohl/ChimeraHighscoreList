using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class JsonDataService : IHighscoreDataService
{
    private string jsonPath;
    private List<PlayerDTO> players;

    public JsonDataService(string jsonPath)
    {
        this.jsonPath = jsonPath;
    }


    public IEnumerable<PlayerDTO> GetHighscoreList()
    {
        return players;
    }


    public void Initialize()
    {
        // Read JSON Data and buffer it
        TextAsset fileContent = Resources.Load(jsonPath) as TextAsset; //File.ReadAllText(jsonPath);
        string json = fileContent.ToString();
        players = JsonUtility.FromJson<PlayerDTOList>(json).data.ToList();

        if (!players.Any())
            Debug.Log("Couldn't read the JsonFile");
        else
            Debug.Log("player: " + JsonUtility.ToJson(players[0]));
    }

    public void Deinitialize()
    {      
    }

}

[Serializable]
public class PlayerDTO
{
    public int ID;
    public int Rank;
    public string Playername;
    public int Score;
    public string Avatar;
}

[Serializable]
public class PlayerDTOList
{
    public PlayerDTO[] data;
}