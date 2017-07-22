using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public int ID { get; set; }
    public int Rank { get; set; }
    public string Playername { get; set; }
    public int Score { get; set; }
    public string Avatar { get; set; }
}

[Serializable]
public class PlayerDTOList
{
    public PlayerDTO[] data;
}