using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public Tilemap tilemap {get; private set;}
    public TetrominosData[] tetrominos;
    public TetrominosData[] blocks;
    public Queue<TetrominosData> reserve;
    public Piece activePiece {get; private set;}
    public Vector3Int spawnPosition;
    public Vector2Int size = new Vector2Int(10, 20);
    private int Score = 0;
    public RectInt Bounds{
        get{
            Vector2Int postion = new Vector2Int(-this.size.x/2, -this.size.y/2);
            return new RectInt(postion, this.size);
        }
    }
    public void Awake(){
        this.activePiece = GetComponentInChildren<Piece>();
        this.tilemap = GetComponentInChildren<Tilemap>();
        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
        for (int i = 0; i < this.blocks.Length; i++)
        {
            this.blocks[i].Initialize();
        }

        this.reserve = new Queue<TetrominosData>();

        for (int i = 0; i < 2; i++)
        {
            int rand = Random.Range(0, this.tetrominos.Length);
            this.reserve.Enqueue(this.tetrominos[rand]);
        }
   }

    private void Start(){
        SpawnPiece();
    }

    public void SpawnPiece(){

        int rand = Random.Range(0, this.tetrominos.Length);
        TetrominosData data = this.tetrominos[rand];
        this.activePiece.Initialize(this,this.spawnPosition, this.reserve.Dequeue());
        this.reserve.Enqueue(data);
        if (!isValidPosition(this.activePiece, this.spawnPosition)){
            GameOver();
        }
        Set(this.activePiece);
    }

    public void Swap(){
        this.reserve.Enqueue(this.reserve.Dequeue());
    }

    public TetrominosData GetNext(){
        TetrominosData rv = this.reserve.Dequeue();
        this.reserve.Enqueue(rv);
        this.Swap();
        return rv;
    }

    public void GameOver(){
        this.tilemap.ClearAllTiles();
    }

    public void Set(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, piece.data.brick); 
        }
    }

    public void Clear(Piece piece){
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            this.tilemap.SetTile(tilePosition, null); 
        } 
    }

    public bool isValidPosition(Piece piece, Vector3Int pos){

        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + pos;
            if (this.tilemap.HasTile(tilePosition)){
                return false;
            }
            if (!bounds.Contains((Vector2Int)tilePosition)){
                return false;
            }
        }

        return true;
    }

    public void ClearLine(){
        RectInt Bounds = this.Bounds;
        int row = Bounds.yMin;
        int score = 0;
        while (row<Bounds.yMax){
            if (isFull(row)){
                LineClear(row);
                score += 1;
            }else{
                row++;
            }
        }
        SetScore(score);
    }

    private void SetScore(int score){
        switch (score)
        {
            case 1:
                this.Score += 1;
                break;
            case 2:
                this.Score += 3;
                break;
            case 3:
                this.Score += 6;
                break;
            case 4:
                this.Score += 10;
                break;
            default:
                this.Score += 0;
                break;
        }
    }

    private void LineClear(int row){

        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col <bounds.xMax; col++ ){
            Vector3Int pos = new Vector3Int( col, row);
            this.tilemap.SetTile(pos, null);
        }
        while (row < bounds.yMax){
                for (int col = bounds.xMin; col <bounds.xMax; col++ ){
                Vector3Int pos = new Vector3Int( col, row+1);
                TileBase above = this.tilemap.GetTile(pos);
                pos = new Vector3Int(col,row);
                this.tilemap.SetTile(pos, above);
            }
            row++;
        }
    }

    private bool isFull(int row){
        RectInt bounds = this.Bounds;
        for (int col = bounds.xMin; col <bounds.xMax; col++ ){
            Vector3Int pos = new Vector3Int( col, row);
            if (!this.tilemap.HasTile(pos)){
                return false;
            }
        }
        return true;
    }

    public void AddPenalty(){
        this.Penalty(6);
    }
    private void Penalty(int num){
        int time = num/4;

        for (int i = 0; i <= time; i++)
        {
            int rand = Random.Range(Bounds.xMin, Bounds.xMax);
            if ( num>4){
                for (int j = 0; j < 4; j++)
                {
                    PushUp(rand);
                }
                num -= 4;
            }else {
                for (int j = 0; j < num; j++)
                {
                    PushUp(rand);
                }
            }
        }

        
    }

    private void PushUp(int rand){
        RectInt bounds = this.Bounds;
        int row = Bounds.yMax;
        while (row>Bounds.yMin){
                    for (int col = bounds.xMin; col <bounds.xMax; col++ ){
                    Vector3Int pos = new Vector3Int( col, row-1);
                    TileBase above = this.tilemap.GetTile(pos);
                    pos = new Vector3Int(col,row);
                    this.tilemap.SetTile(pos, above);
                }
                row--;

        }
            for (int col = bounds.xMin; col <bounds.xMax; col++ ){
                Vector3Int pos = new Vector3Int( col, row);
                if (col == rand){
                    this.tilemap.SetTile(pos, null);
                }else{
                    this.tilemap.SetTile(pos, blocks[0].brick);
                }
            }
    }
}
