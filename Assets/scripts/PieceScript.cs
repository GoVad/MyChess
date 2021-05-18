using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{

    //    pawn=1,
    //    horse,
    //    bishop,
    //    rook,
    //    queen,
    //    king
    int white = 1;
    int black = -1;

    public int color;
    public int type;
    GameObject master;
    public Vector2 pos;
    public Vector2 worldPos;
    public List<GameObject> availibleMoves;
    GameManager m;

    void Start()
    {
        availibleMoves = new List<GameObject>();
    }

    public void Init(int color, int type, Vector2 pos)
    {
        master = GameObject.Find("Manager");
        m = master.GetComponent<GameManager>();
        this.color = color;
        this.type = type;
        this.pos = pos;
        worldPos.Set(m.startX + (int)pos.x * m.moveRange, m.startY + (int)pos.y * m.moveRange);
    }

    public void SetPos(Vector2 worldPos,Vector2 pos)
    {
        this.pos = pos;
        this.worldPos = worldPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        if(m.selectedPiece != gameObject)
        {
            if(m.MoveColor == color)
            {
                if(m.selectedPiece != null)
                    foreach(var a in m.selectedPiece
                        .GetComponent<PieceScript>().availibleMoves)
                        Destroy(a);

                m.selectedPiece = gameObject;
                drawMoves();
            }
            else if(m.selectedPiece!=null)
                TryEat();
        }
        else
        {
            foreach(var a in availibleMoves)
                Destroy(a);
           m.selectedPiece = null;
        }
    }

    void RookEat(PieceScript sp)
    {
        if(pos.x == sp.pos.x || pos.y == sp.pos.y)
        {
            string direction = "";
            if(pos.x > sp.pos.x)
                direction = "right";
            else if(pos.y < sp.pos.y)
                direction = "down";
            else if(pos.x < sp.pos.x)
                direction = "left";
            else if(pos.y > sp.pos.y)
                direction = "up";

            bool toTake = true;
            for(int i = 1; i <
                (pos.x == sp.pos.x ? Math.Abs(pos.y - sp.pos.y) : Math.Abs(pos.x - sp.pos.x))
                && toTake; i++)
                switch(direction)
                {
                    case "up":
                        if(m.field[(int)sp.pos.x, (int)sp.pos.y + i] != 0)
                            toTake = false;
                        break;
                    case "right":
                        if(m.field[(int)sp.pos.x + i, (int)sp.pos.y] != 0)
                            toTake = false;
                        break;
                    case "down":
                        if(m.field[(int)sp.pos.x, (int)sp.pos.y - i] != 0)
                            toTake = false;
                        break;
                    case "left":
                        if(m.field[(int)sp.pos.x - i, (int)sp.pos.y] != 0)
                            toTake = false;
                        break;
                }
            if(toTake)
                Take(sp);
        }
    }

    void BishopEat(PieceScript sp)
    {
        if(Math.Abs(pos.x - sp.pos.x) == Math.Abs(pos.y - sp.pos.y))
        {
            bool up = true;
            bool right = true;
            if(pos.x < sp.pos.x)
                right = false;
            if(pos.y < sp.pos.y)
                up = false;
            bool toTake = true;
            for(int i = 1; i < Math.Abs(pos.x - sp.pos.x) && toTake; i++)
                if(m.field[(int)sp.pos.x + (right ? 1 : -1) * i, (int)sp.pos.y + (up ? 1 : -1) * i] != 0)
                    toTake = false;
            if(toTake)
                Take(sp);
        }
    }

    private void TryEat()
    {
        var sp = m.selectedPiece.GetComponent<PieceScript>();
        switch(sp.type)
        {
            case 1:
                if(pos.y==sp.pos.y+sp.color&&(pos.x == sp.pos.x+1||pos.x == sp.pos.x-1))
                    Take(sp);
                break;
            case 2:
                if(pos.y == sp.pos.y + 1 && pos.x == sp.pos.x + 2 ||
                    pos.y == sp.pos.y - 1 && pos.x == sp.pos.x + 2 ||
                    pos.y == sp.pos.y + 1 && pos.x == sp.pos.x - 2 ||
                    pos.y == sp.pos.y - 1 && pos.x == sp.pos.x - 2 ||
                    pos.y == sp.pos.y + 2 && pos.x == sp.pos.x + 1 ||
                    pos.y == sp.pos.y - 2 && pos.x == sp.pos.x + 1 ||
                    pos.y == sp.pos.y + 2 && pos.x == sp.pos.x - 1 ||
                    pos.y == sp.pos.y - 2 && pos.x == sp.pos.x - 1)
                    Take(sp);
                break;
            case 3:
                BishopEat(sp);
                break;
            case 4:
                RookEat(sp);
                break;
            case 5:
                BishopEat(sp);
                RookEat(sp);
                break;
            case 6:
                if(Math.Abs(pos.y - sp.pos.y)<2 && Math.Abs(pos.x - sp.pos.x) <2)
                    Take(sp);
                break;
        }
    }

    void Take(PieceScript sp)
    {
        m.field[(int)sp.pos.x, (int)sp.pos.y] = 0;
        sp.transform.position = transform.position;
        sp.worldPos = worldPos;
        sp.pos = pos;
        m.field[(int)sp.pos.x, (int)sp.pos.y] = sp.type * sp.color;
        if(name == "40")
            m.Endgame("Победа черных!");
        else if(name == "47")
            m.Endgame("Белые победили!");
        Destroy(gameObject);
        foreach(var a in sp.availibleMoves)
            Destroy(a);
        m.selectedPiece = null;
        m.MoveColor *= -1;
        m.aud.Play();
    }

    private void AddAvailibleMove(float x, float y)
    {
        availibleMoves.Add(new GameObject());

        availibleMoves[availibleMoves.Count - 1].AddComponent<SpriteRenderer>().sprite =
            m.dot;
        availibleMoves[availibleMoves.Count - 1].AddComponent<BoxCollider>().size = new Vector3(1.2f,1.2f,0);
        availibleMoves[availibleMoves.Count - 1].AddComponent<MoveCheck>().pos =
            new Vector2((x-m.startX)/m.moveRange, (y - m.startY) / m.moveRange);
        availibleMoves[availibleMoves.Count - 1].GetComponent<MoveCheck>().SetPiece(gameObject);

        availibleMoves[availibleMoves.Count - 1].transform.position = new Vector3(
            x,
            y,
            -3
        );
    }

    void DrawMoveBishop()
    {
        for(int i = 1; (pos.x + i < 8 && pos.y + i < 8 &&
        (m.field[(int)pos.x + i, (int)pos.y + i] * m.field[(int)pos.x, (int)pos.y]) == 0);
        i++)
            AddAvailibleMove(worldPos.x + i * m.moveRange, worldPos.y + i * m.moveRange);

        for(int i = 1;
            (pos.x - i >= 0 && pos.y + i < 8 &&
            (m.field[(int)pos.x - i, (int)pos.y + i] * m.field[(int)pos.x, (int)pos.y]) == 0);
            i++)
            AddAvailibleMove(worldPos.x - i * m.moveRange, worldPos.y + i * m.moveRange);

        for(int i = 1; (pos.x + i < 8 && pos.y - i >= 0 &&
            (m.field[(int)pos.x + i, (int)pos.y - i] * m.field[(int)pos.x, (int)pos.y]) == 0);
            i++)
            AddAvailibleMove(worldPos.x + i * m.moveRange, worldPos.y - i * m.moveRange);

        for(int i = 1;
            (pos.x - i >= 0 && pos.y - i >= 0 &&
            (m.field[(int)pos.x - i, (int)pos.y - i] * m.field[(int)pos.x, (int)pos.y]) == 0);
            i++)
            AddAvailibleMove(worldPos.x - i * m.moveRange, worldPos.y - i * m.moveRange);
    }

    void DrawMoveRook()
    {
        for(int i = 1; pos.x + i < 8 &&
            (m.field[(int)pos.x + i, (int)pos.y] * m.field[(int)pos.x, (int)pos.y]) == 0; i++)
            AddAvailibleMove(worldPos.x + i * m.moveRange, worldPos.y);

        for(int i = 1; pos.x - i >= 0 &&
            (m.field[(int)pos.x - i, (int)pos.y] * m.field[(int)pos.x, (int)pos.y]) == 0; i++)
            AddAvailibleMove(worldPos.x - i * m.moveRange, worldPos.y);

        for(int i = 1; pos.y - i >= 0 &&
             (m.field[(int)pos.x, (int)pos.y - i] * m.field[(int)pos.x, (int)pos.y]) == 0; i++)
            AddAvailibleMove(worldPos.x, worldPos.y - i * m.moveRange);

        for(int i = 1; pos.y + i < 8 &&
            (m.field[(int)pos.x, (int)pos.y + i] * m.field[(int)pos.x, (int)pos.y]) == 0; i++)
            AddAvailibleMove(worldPos.x, worldPos.y + i * m.moveRange);
    }

    private void drawMoves()
    {
        switch(type)
        {
            case 1:
                {
                    if(pos.y == 1 && color == white || pos.y == 6 && color == black)
                        if(m.field[(int)pos.x, (int)pos.y + color * 2] == 0
                            && m.field[(int)pos.x, (int)pos.y + color] == 0)
                            AddAvailibleMove(
                                worldPos.x,
                                worldPos.y + color * 2 * m.moveRange);
                    if((int)pos.y + color >= 0 && (int)pos.y + color < 8 &&
                       m.field[(int)pos.x, (int)pos.y + color] == 0)
                        AddAvailibleMove(
                            worldPos.x,
                            worldPos.y + color*m.moveRange);
                }
                break;
            case 2:
                {
                    if(pos.x+2<8&&pos.y+1<8 && m.field[(int)pos.x+2, (int)pos.y + 1] == 0)
                        AddAvailibleMove(
                            worldPos.x + 2 * m.moveRange,
                            worldPos.y + m.moveRange
                            );
                    if(pos.x + 2 < 8 && pos.y - 1 >= 0 && m.field[(int)pos.x + 2, (int)pos.y - 1] == 0)
                        AddAvailibleMove(
                        worldPos.x + 2 * m.moveRange,
                        worldPos.y - m.moveRange
                        );
                    if(pos.x - 2 >=0 && pos.y + 1 < 8 && m.field[(int)pos.x - 2, (int)pos.y + 1] == 0)
                        AddAvailibleMove(
                        worldPos.x - 2 * m.moveRange,
                        worldPos.y + m.moveRange
                        );
                    if(pos.x - 2 >=0 && pos.y - 1 >=0 && m.field[(int)pos.x - 2, (int)pos.y - 1] == 0)
                        AddAvailibleMove(
                        worldPos.x - 2 * m.moveRange,
                        worldPos.y - m.moveRange
                        );
                    if(pos.x + 1 < 8 && pos.y + 2 < 8 && m.field[(int)pos.x + 1, (int)pos.y + 2] == 0)
                        AddAvailibleMove(
                        worldPos.x + m.moveRange,
                        worldPos.y + 2 * m.moveRange
                        );
                    if(pos.x + 1 < 8 && pos.y -2>=0 && m.field[(int)pos.x + 1, (int)pos.y - 2] == 0)
                        AddAvailibleMove(
                        worldPos.x + m.moveRange,
                        worldPos.y - 2 * m.moveRange
                        );
                    if(pos.x - 1 >=0 && pos.y + 2 < 8 && m.field[(int)pos.x - 1, (int)pos.y + 2] == 0)
                        AddAvailibleMove(
                        worldPos.x - m.moveRange,
                        worldPos.y + 2 * m.moveRange
                        );
                    if(pos.x - 1 >= 0 && pos.y - 2 >=0 && m.field[(int)pos.x - 1, (int)pos.y - 2] == 0)
                        AddAvailibleMove(
                        worldPos.x - m.moveRange,
                        worldPos.y - 2 * m.moveRange
                        );
                }
                break;
            case 3:
                DrawMoveBishop();
                break;
            case 4:
                DrawMoveRook();
                break;
            case 5:
                DrawMoveRook();
                DrawMoveBishop();
                break;
            case 6:
                {
                    if(pos.x + 1 < 8 && m.field[(int)pos.x + 1, (int)pos.y] == 0)
                        AddAvailibleMove(worldPos.x + m.moveRange, worldPos.y);
                    if(pos.x - 1 >=0 && m.field[(int)pos.x - 1, (int)pos.y] == 0)
                        AddAvailibleMove(worldPos.x - m.moveRange, worldPos.y);
                    if(pos.y + 1 < 8 && m.field[(int)pos.x, (int)pos.y+1] == 0)
                        AddAvailibleMove(worldPos.x , worldPos.y + m.moveRange);
                    if(pos.y - 1 >= 0 && m.field[(int)pos.x, (int)pos.y - 1] == 0)
                        AddAvailibleMove(worldPos.x , worldPos.y - m.moveRange);

                    if(pos.x + 1 < 8 && pos.y + 1 < 8 && m.field[(int)pos.x + 1, (int)pos.y+1] == 0)
                        AddAvailibleMove(worldPos.x + m.moveRange, worldPos.y+m.moveRange);
                    if(pos.x + 1 < 8 && pos.y - 1 >=0 && m.field[(int)pos.x + 1, (int)pos.y-1] == 0)
                        AddAvailibleMove(worldPos.x + m.moveRange, worldPos.y-m.moveRange);
                    if(pos.x - 1 >=0 && pos.y + 1 < 8 && m.field[(int)pos.x - 1, (int)pos.y + 1] == 0)
                        AddAvailibleMove(worldPos.x - m.moveRange, worldPos.y+m.moveRange);
                    if(pos.x - 1 >=0 && pos.y - 1 >= 0 && m.field[(int)pos.x - 1, (int)pos.y - 1] == 0)
                        AddAvailibleMove(worldPos.x - m.moveRange, worldPos.y-m.moveRange);


                    if((m.castle[0]&&color==1||m.castle[1]&&color==-1)&&
                        m.field[(int)pos.x + 1, (int)pos.y] == 0 &&
                        m.field[(int)pos.x + 2, (int)pos.y] == 0)
                    {
                        AddAvailibleMove(worldPos.x + 2*m.moveRange, worldPos.y);
                    }

                    if((m.castle[2] && color == 1 || m.castle[3] && color == -1) &&
                        m.field[(int)pos.x - 1, (int)pos.y] == 0 &&
                        m.field[(int)pos.x - 2, (int)pos.y] == 0 &&
                        m.field[(int)pos.x - 3, (int)pos.y] == 0
                        )
                        AddAvailibleMove(worldPos.x - 2 * m.moveRange, worldPos.y);

                }
                break;
        }
    }
}
