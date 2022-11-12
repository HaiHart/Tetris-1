using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board {get; private set;}
    public TetrominosData data {get; private set;}
    public Vector3Int position {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public int rotIndex {get; private set;}
    public float stepDelay = 1f;
    public float lockDelay = 0.5f;
    private float stepTime;
    private float lockTime;
    public void Initialize(Board board, Vector3Int position, TetrominosData data){
        this.board = board;
        this.data = data;
        this.position = position;
        this.rotIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if (this.cells == null){
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public void Update(){

        this.board.Clear(this);

        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            Move(Vector2Int.left);
        }else if (Input.GetKeyDown(KeyCode.RightArrow)){
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)){
            Move(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            HardDrop();
        }

        if (Input.GetKeyDown(KeyCode.Q)){
            Rotate(-1);
        }else if (Input.GetKeyDown(KeyCode.E)){
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.W)){
            this.board.Swap();
        }

        if (Input.GetKeyDown(KeyCode.Z)){
            this.board.AddPenalty();
        }

        if (Time.time>= this.stepTime){
            Step();
        }

        this.board.Set(this);
    }

    private void Step(){
        this.stepTime = Time.time+stepDelay;
        Move(Vector2Int.down);
        if (this.lockTime >= this.lockDelay){
            Lock();
        }
    }

    private void Lock(){
        this.board.Set(this);
        this.board.ClearLine();
        this.board.SpawnPiece();
    }

    private void HardDrop(){
        while(Move(Vector2Int.down)){
            continue;
        }
        Lock();
    }

    private void Spin(int index){
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;
            switch (this.data.tetrominos)
            {
                case Tetrominos.I:
                case Tetrominos.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * index) + (cell.y * Data.RotationMatrix[1] * index));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * index) + (cell.y * Data.RotationMatrix[3] * index));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * index) + (cell.y * Data.RotationMatrix[1] * index));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * index) + (cell.y * Data.RotationMatrix[3] * index));
                    break;
            }

            this.cells[i] = new Vector3Int(x,y);
        }
    }

    private void Rotate(int index){

        int originalIndex = this.rotIndex;

        this.rotIndex = ((this.rotIndex + index) + 4)%4;

        Spin(index);

        if (!TestWalls(this.rotIndex, index)){
            this.rotIndex = originalIndex;
            Spin(-index);
        }
    }

    private bool TestWalls(int index, int rotInd){
        int WKindex = GetWKIndex(index, rotInd);
        for (int i = 0; i < this.data.walkKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.walkKicks[WKindex, i];
            if (Move(translation)){
                return true;
            }
        }
        return false;
    }

    private int GetWKIndex(int index, int rotInd){
        int WKindex = index * 2;
        if (rotInd < 0){
            WKindex--;
        }

        return (WKindex + this.data.walkKicks.GetLength(0))%this.data.walkKicks.GetLength(0);
    }

    public bool Move(Vector2Int translation){
        Vector3Int NewPos = this.position;
        NewPos.x += translation.x;
        NewPos.y += translation.y;
        bool valid = this.board.isValidPosition(this,NewPos);
        if (valid){
            this.position = NewPos;
            this.lockTime = 0f;
        }

        return valid;
    }

    
}
