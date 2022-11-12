using UnityEngine.Tilemaps;
using UnityEngine;

public enum Tetrominos
{
    I,
    O,
    J,
    T,
    L,
    S,
    Z,
    Line_1,
    Line_2,
    Line_3,
}

[System.Serializable]
public struct TetrominosData
{
    public Tetrominos tetrominos;
    public Tile brick;
    public Vector2Int[,] walkKicks {get; private set;}
    public Vector2Int[] cells {get; private set;}
    public void Initialize(){
        this.cells = Data.Cells[this.tetrominos];
        if (!(this.tetrominos == Tetrominos.Line_1 || this.tetrominos == Tetrominos.Line_2 || this.tetrominos == Tetrominos.Line_3)){
            this.walkKicks = Data.WallKicks[this.tetrominos];
        }
    }

}