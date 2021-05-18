using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCheck : MonoBehaviour
{
    GameObject piece;
    PieceScript ps;
    GameManager gm;
    public Vector2 pos;
    void Start()
    {
        
    }

    public void SetPiece(GameObject piece)
    {
        gm = GameObject.Find("Manager").GetComponent<GameManager>();
        this.piece = piece;
        ps = piece.GetComponent<PieceScript>();
    }

    private void OnMouseDown()
    {
        if(ps.type == 6)
        {
            gm.castle[ps.color == 1 ? 0 : 1] = false;
            gm.castle[ps.color == 1 ? 2 : 3] = false;

            if(Mathf.Abs(pos.x-ps.pos.x)==2)
                if(ps.pos.x<pos.x)
                {
                    gm.field[(int)ps.pos.x + 3, (int)ps.pos.y] = 0;
                    gm.field[(int)ps.pos.x + 1, (int)ps.pos.y] = ps.color*4;
                    var rook = GameObject.Find(string.Format("{0}{1}", 7, ps.pos.y));
                    rook.transform.position =
                        new Vector3(transform.position.x-gm.moveRange,transform.position.y,-2);
                    rook.GetComponent<PieceScript>().pos.x = pos.x - 1;
                    rook.GetComponent<PieceScript>().worldPos = rook.transform.position;
                }
                else
                {
                    gm.field[(int)ps.pos.x - 4, (int)ps.pos.y] = 0;
                    gm.field[(int)ps.pos.x - 1, (int)ps.pos.y] = ps.color * 4;
                    var rook = GameObject.Find(string.Format("{0}{1}", 0, ps.pos.y));
                    rook.transform.position =
                        new Vector3(transform.position.x + gm.moveRange, transform.position.y, -2);
                    rook.GetComponent<PieceScript>().pos.x = pos.x + 1;
                    rook.GetComponent<PieceScript>().worldPos = rook.transform.position;
                }

            gm.field[(int)ps.pos.x, (int)ps.pos.y] = 0;
            ps.worldPos = transform.position;
            ps.transform.position = transform.position;
            ps.pos = pos;
            gm.field[(int)ps.pos.x, (int)ps.pos.y] = ps.type * ps.color;
        }
        else
        {
            if(ps.type==4)
                if(ps.pos.x == 0 && ps.pos.y == 0)
                    gm.castle[2] = false;
                else if(ps.pos.x == 7 && ps.pos.y == 0)
                    gm.castle[0] = false;
                else if(ps.pos.x == 0 && ps.pos.y == 7)
                    gm.castle[3] = false;
                else if(ps.pos.x == 7 && ps.pos.y == 7)
                    gm.castle[1] = false;

            gm.field[(int)ps.pos.x, (int)ps.pos.y] = 0;
            ps.worldPos = transform.position;
            ps.transform.position = transform.position;
            ps.pos = pos;
            gm.field[(int)ps.pos.x, (int)ps.pos.y] = ps.type * ps.color;
        }
        gm.selectedPiece = null;
        gm.MoveColor *= -1;
        gm.aud.Play();
        gm.Flip();
        foreach(var a in ps.availibleMoves)
            Destroy(a);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
