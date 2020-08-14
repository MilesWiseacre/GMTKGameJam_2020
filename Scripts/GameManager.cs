using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float deadZone = .09f;

    private Player player;

    private bool _isGameOver;

    public bool IsGameOver
    {
        get
        {
            return _isGameOver;
        }
        set
        {
            if (value == true)
            {
                // Execute code when game over happens
            }
            _isGameOver = value;
        }
    }

    private bool _hard;

    public bool Hard
    {
        get
        {
            return _hard;
        }
        set
        {
            if (value == true)
            {

            }
            _hard = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        IsGameOver = true;
        Hard = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGameOver)
        {
            if (Input.GetAxis("Face_V") < 0)
            {
                StartGame();
            } else if (Input.GetAxis("Face_H") > 0)
            {
                StartHard();
            }
        }
    }

    public void StartGame()
    {
        IsGameOver = false;
        player.StartGame();
    }

    public void StartHard()
    {
        IsGameOver = false;
        Hard = true;
        player.StartGame();
    }

    public void Refresh()
    {
        _isGameOver = false;
        Hard = false;
        player.ResetGame();
    }

    public void GameOver()
    {
        IsGameOver = true;
        Hard = false;
    }
}
