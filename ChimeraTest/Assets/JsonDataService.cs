using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class JsonDataService {

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
        string json = File.ReadAllText(jsonPath);
        players = JsonUtility.FromJson<PlayerDTOList>(json).data.ToList();

        if (!players.Any())
            Debug.Log("oh my god!");
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