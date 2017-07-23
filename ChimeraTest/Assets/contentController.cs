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

    private AvatarObjectPool avatarPool;

    private void Start()
    {
        avatarPool = GameObject.FindGameObjectWithTag("avatarPool").GetComponent<AvatarObjectPool>();

        if (avatarPool == null)
            Debug.Log("Casn't find avatarPool");
    }

    private void SetContents(PlayerDTO player)
    {
        rank.text = player.Rank.ToString();
        score.text = player.Score.ToString();
        //trophyIcon.sprite = player.Trophy;
        GameObject avatar = avatarPool.GetAvatar(player.Avatar);
        avatar.SetActive(true);
        avatar.transform.SetParent(avatarTransform.transform);
        avatar.transform.localRotation = Quaternion.identity;
        avatar.transform.localPosition = Vector3.zero;
        avatar.transform.localScale = Vector3.one;

        playername.text = player.Playername;
    }

    public void SetContents(object data)
    {
        SetContents((PlayerDTO)data);
    }
}


