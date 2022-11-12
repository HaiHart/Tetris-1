using UnityEngine;
using UnityEngine.Tilemaps;

public class NextPiece : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Tilemap tilemap {get; private set;}
    public Vector3Int[] cells;
    public Vector3Int position;
    private void Awake(){
        this.tilemap = GetComponentInChildren<Tilemap>();
        BoundsInt size = this.tilemap.cellBounds;
        this.position.x = -1 ;
        this.position.y = -1 ;
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
        TetrominosData data = this.board.GetNext();
        this.tile = data.brick;
        this.cells = new Vector3Int[data.cells.Length];
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    

    private void Set(){
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            Debug.Log(tilePosition);
            this.tilemap.SetTile(tilePosition, this.tile); 
        }
    }

    private void LateUpdate(){
        Clear();
        Copy();
        Set();
    }
}
