using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class contentController : MonoBehaviour
{
    public Text rank;
    public Text score;
    public Image trophyIcon;
    public GameObject avatarTransform;
    public Text playername;
    public Sprite[] trophySprites;
    private GameObject avatarObject;
    private AvatarObjectPool avatarPool;

    // This value normally comes from a user object
    private int IamPlayerWithID = 53;

    private void Start()
    {
        avatarPool = GameObject.FindGameObjectWithTag("avatarPool").GetComponent<AvatarObjectPool>();

        if (avatarPool == null)
            Debug.Log("Can't find avatarPool");
    }

    private void SetContents(PlayerDTO player)
    {
        if(player.ID == IamPlayerWithID)
            GetComponent<Image>().color = Color.yellow;
        else
            GetComponent<Image>().color = Color.black;

        rank.text = player.Rank.ToString();
        score.text = player.Score.ToString();
        trophyIcon.sprite = GetTrophyIcon(player.Score);
        if (avatarObject != null)
        {
            avatarObject.transform.SetParent(null);
            avatarPool.ReturnAvatar(avatarObject);
        }
        avatarObject = avatarPool.GetAvatar(player.Avatar);
        avatarObject.SetActive(true);
        avatarObject.transform.SetParent(avatarTransform.transform);
        avatarObject.transform.localRotation = Quaternion.identity;
        avatarObject.transform.localPosition = Vector3.zero;
        avatarObject.transform.localScale = Vector3.one;
        playername.text = player.Playername;
    }

    public void SetContents(object data)
    {
        SetContents((PlayerDTO)data);
    }

    private Sprite GetTrophyIcon(int score)
    {
        if (score >= 9900)
            return trophySprites[0];
        else if (score >= 9800)
            return trophySprites[1];
        else if (score >= 9700)
            return trophySprites[2];
        else if (score >= 9400)
            return trophySprites[3];
        else if (score >= 9000)
            return trophySprites[4];
        else if (score >= 8000)
            return trophySprites[5];
        else if (score >= 7000)
            return trophySprites[6];
        else if (score >= 5000)
            return trophySprites[7];
        else if (score >= 2000)
            return trophySprites[8];
        else if (score >= 1000)
            return trophySprites[9];
        else
            return trophySprites[10];
    }
}


