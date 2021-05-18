using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite[] sprites;
    public Sprite dot;
    public int[,] field = new int[8,8];
    public GameObject selectedPiece;
    public int MoveColor;
    public bool[] castle = { true, true, true, true };
    public AudioSource aud;
    public Text text;
    public Button endb;

    public enum PiecesId
    { 
        pawn=1,
        horse,
        bishop,
        rook,
        queen,
        king
    }

    int white = 1;
    int black = -1;

    public float startX = -5.98f;
    public float startY = -4.35f;
    public float moveRange = 1.25f;
    float pieceSize = 0.55f;

    void Start()
    {
        endb.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        MoveColor = 1;
        for(int i = 0; i < 8; i++)
            for(int k = 0; k < 8; k++)
                field[i, k] = 0;

        for(int i = 0; i < 8; i++)
        {
            CreatePiece((int)PiecesId.pawn, i, 1, white);
            CreatePiece((int)PiecesId.pawn, i, 6, black);
        }
        for(int i = 0; i < 2; i++)
        {
            CreatePiece((int)PiecesId.horse, 1 + i * 5, 0, white);
            CreatePiece((int)PiecesId.horse, 1 + i * 5, 7, black);
        }
        for(int i = 0; i < 2; i++)
        {
            CreatePiece((int)PiecesId.bishop, 2 + i * 3, 0, white);
            CreatePiece((int)PiecesId.bishop, 2 + i * 3, 7, black);
        }
        for(int i = 0; i < 2; i++)
        {
            CreatePiece((int)PiecesId.rook, i * 7, 0, white);
            CreatePiece((int)PiecesId.rook, i * 7, 7, black);
        }
        for(int i = 0; i < 2; i++)
        {
            CreatePiece((int)PiecesId.queen, 3, 7*i, (int)Math.Pow(black,i));
            CreatePiece((int)PiecesId.king,  4, 7*i, (int)Math.Pow(black, i));
        }
    }
    
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void CreatePiece(int id,int x, int y,int color)
    {
        field[x, y] = id * color;
        var p = new GameObject();
        p.transform.position = new Vector3(startX + x * moveRange, startY + y * moveRange, -2);
        p.AddComponent<SpriteRenderer>().sprite = sprites[color!=1?(id-1):(id-1)+6];
        p.transform.localScale = new Vector3(pieceSize, pieceSize, 0);
        p.AddComponent<PieceScript>().Init(color,id,new Vector2(x,y));
        p.AddComponent<BoxCollider2D>();
        p.name = string.Format("{0}{1}", x, y);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Flip()
    {
        //Camera.main.transform.Rotate(0,0,180);
        //foreach(var p in pieces)
        //    p.transform.Rotate(0, 0, 180);
    }

    public void Endgame(string msg)
    {
        if(msg == "")
            msg = string.Format("{0} сдались!", MoveColor == 1 ? "Белые" : "Черные");
        endb.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
        text.text = msg;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
