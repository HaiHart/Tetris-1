using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Tilemap tilemap {get; private set;}
    public Piece trackingPiece;
    public Vector3Int[] cells;
    public Vector3Int position;

    private void Awake(){
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void Clear(){
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null); 
        }
    }

    private void Copy(){
        this.cells = new Vector3Int[this.trackingPiece.cells.Length];
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    private void Drop(){
        Vector3Int position = this.trackingPiece.position;
        int row = position.y;
        int bottom = -this.board.size.y/2 -1;

        this.board.Clear(this.trackingPiece);

        for (; row >= bottom; row--)
        {
            position.y = row;
            if(this.board.isValidPosition(this.trackingPiece, position)){
                this.position = position;
            }else{
                break;
            }
        }

        this.board.Set(this.trackingPiece);

    }

    private void Set(){
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile); 
        }
    }

    private void LateUpdate(){
        Clear();
        Copy();
        Drop();
        Set();
    }
}
