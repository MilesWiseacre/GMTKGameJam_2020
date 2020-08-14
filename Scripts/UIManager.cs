using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Animator anim;

    private GameManager gm;

    [SerializeField]
    private GameObject gamePanel = null;

    [SerializeField]
    private Text scoreText = null;

    void Start()
    {
        anim = GetComponent<Animator>();
        gm = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }

    public void SetUI(int id)
    {
        anim.SetInteger("BrokeID", id);
    }

    public void SetScore(float score)
    {
        scoreText.text = score.ToString();
    }

    void Update()
    {
        if (gm.IsGameOver && gamePanel.activeSelf == false)
        {
            gamePanel.SetActive(true);
        } else if (!gm.IsGameOver && gamePanel.activeSelf == true)
        {
            scoreText.text = "0";
            gamePanel.SetActive(false);
        }
    }
}
