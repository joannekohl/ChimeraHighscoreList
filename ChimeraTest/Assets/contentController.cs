using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class contentController : MonoBehaviour {


    public Text rank;
    public Text score;
    public Image trophyIcon;
    public GameObject avatarTransform;
    public Text name;



    public void SetContents(int rankInput, int scoreInput, Sprite trophyIconTexture, GameObject avatarObject, string nameinput)
    {

        rank.text = rankInput.ToString();

        score.text = scoreInput.ToString();

        trophyIcon.sprite = trophyIconTexture;

        Instantiate(avatarObject, avatarTransform.transform);

        name.text = nameinput.ToString();

    }
}
