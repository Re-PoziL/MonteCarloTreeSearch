using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ChessColor 
{
    White,
    Black,
}

public enum ChessBoradState
{
    Confront,//����
    WhiteWin,
    BlackWin,
    Tie,//ƽ��
}



public class ChessBoard
{
    public List<(int, int)> emptys = new List<(int, int)>();

    public Dictionary<(int, int), ChessColor> chessDic = new Dictionary<(int, int), ChessColor>();
    public ChessColor chessColor;
    public ChessBoradState chessBoardState;

    public ChessBoard()
    {
        chessColor = ChessColor.Black;
        chessBoardState = ChessBoradState.Confront;
        for (int i = -5; i <=5; i++)
        {
            for (int j = -5; j <= 5; j++)
            {
                emptys.Add((i, j));
            }
        }
    }

    private void CreateChess(ChessColor chessColor, int x, int y)
    {
        if (chessColor == ChessColor.Black)
        {
            GameObject blackChess = GameObject.Instantiate(GameManager.Instance.blackChess, new Vector2(x, y), Quaternion.identity);
            Debug.Log("Black chess instantiated.");
        }
        else
        {
            GameObject whiteChess = GameObject.Instantiate(GameManager.Instance.whiteChess, new Vector2(x, y), Quaternion.identity);
            Debug.Log("White chess instantiated.");
        }
    }

    private ChessColor ChangeChessColor(ChessColor chessColor)
    {
        if (chessColor == ChessColor.Black)
        {
            chessColor = ChessColor.White;
        }
        else
        {
            chessColor = ChessColor.Black;
        }
        return chessColor;
    }

    public void SimulateMoveTo(int x,int y)
    {
        emptys.Remove((x, y));
        chessDic.Add((x, y), chessColor);
        if (CheckWin(x, y))
        {
            switch (chessColor)
            {
                case ChessColor.White:
                    chessBoardState = ChessBoradState.WhiteWin;
                    //Debug.Log("whiteSimulationWin!");
                    break;
                case ChessColor.Black:
                    chessBoardState = ChessBoradState.BlackWin;
                    //Debug.Log("blackSimulationWin");
                    break;
                default:
                    break;
            }
            return;
        }

        if (emptys.Any())
        {
            chessBoardState = ChessBoradState.Confront;
        }
        else
        {
            chessBoardState = ChessBoradState.Tie;
        }
        chessColor = ChangeChessColor(chessColor);
    }

    public void MoveTo(int x, int y)
    {
        emptys.Remove((x, y));
        chessDic.Add((x, y), chessColor);
        CreateChess(chessColor,x,y);
        if (CheckWin(x, y))
        {
            switch (chessColor)
            {
                case ChessColor.White:
                    chessBoardState = ChessBoradState.WhiteWin;
                    Debug.Log("whiteWIN!");
                    break;
                case ChessColor.Black:
                    chessBoardState = ChessBoradState.BlackWin;
                    Debug.Log("BlackWin!!");
                    break;
                default:
                    break;
            }
            return;
        }
        if(emptys.Any())
        {
            chessBoardState = ChessBoradState.Confront;
        }
        else
        {
            chessBoardState = ChessBoradState.Tie;
        }
        chessColor = ChangeChessColor(chessColor);
        
    }

    public bool CheckWin(int x, int y)
    {

        for (int i = -4; i <= 0; i++)
        {
            bool win = true;
            for (int j = 0; j < 5; j++)
            {
                if (!chessDic.TryGetValue((x + i + j, y), out ChessColor color) || color != chessColor)
                {
                    win = false;
                    break;
                }
            }
            if (win) return true;
        }

        for (int i = -4; i <= 0; i++)
        {
            bool win = true;
            for (int j = 0; j < 5; j++)
            {
                if (!chessDic.TryGetValue((x, y + i + j), out ChessColor color) || color != chessColor)
                {
                    win = false;
                    break;
                }
            }
            if (win) return true;
        }

        for (int i = -4; i <= 0; i++)
        {
            bool win = true;
            for (int j = 0; j < 5; j++)
            {
                if (!chessDic.TryGetValue((x + i + j, y + i + j), out ChessColor color) || color != chessColor)
                {
                    win = false;
                    break;
                }
            }
            if (win) return true;
        }

        for (int i = -4; i <= 0; i++)
        {
            bool win = true;
            for (int j = 0; j < 5; j++)
            {
                if (!chessDic.TryGetValue((x - i - j, y + i + j), out ChessColor color) || color != chessColor)
                {
                    win = false;
                    break;
                }
            }
            if (win) return true;
        }

        return false;
    }
}


public class Chess
{
    public int wins;
    public int visits;
    public ChessBoard chessBoard;
    public List<Chess> child;
    public Chess parent;
    public (int, int) pos;
    public Chess(ChessBoard chessBoard, Chess parent = null)
    {
        wins = 0;
        visits = 0;
        this.chessBoard = chessBoard;
        child = new List<Chess>();
        this.parent = parent;
    }

}



public class MCTS
{
    
    private Chess root;
    public MCTS()
    {
        root = null;
    }

    public (int,int) FindNextMove(ChessBoard chessBoard,int simulations)
    {
        root = new Chess(chessBoard);
        
        for (int i = 0; i < simulations; i++)
        {
            Chess chess = SelectChess(root);
            if(chess.chessBoard.chessBoardState == ChessBoradState.Confront)
            {
                Expansion(chess);
                
            }
            if(chess.child.Any())
            {
                Chess newChess = chess.child[Random.Range(0,chess.child.Count)];
                int result = Simulate(newChess);
                BackPropagation(newChess, result);
            }
            
        }
        Chess best = root.child.OrderByDescending(c => c.visits).FirstOrDefault();
        return (best.pos.Item1, best.pos.Item2);
    }


    private void BackPropagation(Chess chess, int result)
    {
        while(chess!=null)
        {
            chess.visits++;
            chess.wins += result;
            chess = chess.parent;
        }
    }

    private int Simulate(Chess chess)
    {
        if (true)
        {

            Chess newChess = CloneChess(chess);
            
            while (newChess.chessBoard.chessBoardState == ChessBoradState.Confront)
            {
                var pos = newChess.chessBoard.emptys[Random.Range(0, newChess.chessBoard.emptys.Count())];
                newChess.chessBoard.SimulateMoveTo(pos.Item1, pos.Item2);
            }
            if (newChess.chessBoard.chessBoardState == ChessBoradState.WhiteWin)
            {
                if (GameManager.Instance.aiFisrt)
                    return 0;
                else
                    return 1;
            }
            else if (newChess.chessBoard.chessBoardState == ChessBoradState.BlackWin)
            {
                if (GameManager.Instance.aiFisrt)
                    return 1;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
    }

    private Chess SelectChess(Chess chess)
    {
        Chess newChess = chess;
        if (chess.child.Any())
        {
            newChess = UCTSelectChild(chess);
        }
        return newChess;
    }

    private Chess UCTSelectChild(Chess chess)
    {
        double result = 0;
        Chess bestChess = chess;
        double uct = 0;
        foreach (var item in chess.child)
        {
            if (item.wins != 0 && item.visits != 0)
            {
                uct = item.wins / item.visits + Mathf.Sqrt((float)(Math.Log((double)chess.visits) / item.wins));
            }
            if (uct >= result)
            {
                result = uct;
                bestChess = item;
            }
        }
        return bestChess;
    }

    private void Expansion(Chess chess)
    {
        var emptys = chess.chessBoard.emptys;
        foreach ((int,int) item in emptys)
        {
            Debug.Log("Expansion");
            ChessBoard chessBoard = CloneChessBoard(chess.chessBoard);
            chessBoard.SimulateMoveTo(item.Item1,item.Item2);
            
            Chess newChess = new Chess(chessBoard, chess);
            newChess.pos = (item.Item1, item.Item2);
            chess.child.Add(newChess);
        }
    }

    private Chess CloneChess(Chess chess)
    {
        ChessBoard newChessBoard = CloneChessBoard(chess.chessBoard);
        Chess newChess = new Chess(newChessBoard, chess.parent)
        {
            child = chess.child,
            wins = chess.wins,
            visits = chess.visits,
            pos = chess.pos,
        };
        
        return newChess;
    }

    private ChessBoard CloneChessBoard(ChessBoard chessBoard)
    {
        ChessBoard newChessBoard = new ChessBoard
        {
            chessBoardState = chessBoard.chessBoardState
        };
        newChessBoard.chessDic = new Dictionary<(int, int), ChessColor>();
        
        foreach (var item in chessBoard.chessDic)
        {
            newChessBoard.chessDic.Add(item.Key, item.Value);
            newChessBoard.emptys.Remove((item.Key));
        }
        return newChessBoard;
    }
}





public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    ChessBoard curChessBoard;
    MCTS mCTS;
    private void Start()
    {
        Instance = FindObjectOfType<GameManager>();
        if(Instance == null)
        {
            GameObject _instance = new GameObject(typeof(GameManager).Name);
            Instance = _instance.AddComponent<GameManager>();
        }
        DontDestroyOnLoad(Instance);

        curChessBoard = new ChessBoard();
        mCTS = new MCTS();
        
    }

    public GameObject whiteChess;
    public GameObject blackChess;
    public int simulations;
    public bool aiFisrt;
    public ChessColor currentState = ChessColor.Black;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.y);
            if (Mathf.Abs(x) > 5 || Mathf.Abs(y) > 5 || curChessBoard.chessDic.ContainsKey((x, y)))
            {
                Debug.Log("当前位置不可下");
                return;
            }
            curChessBoard.MoveTo(x, y);
            StartCoroutine(NewMethod());
        }
    }

    private IEnumerator NewMethod()
    {
        yield return null;
        var pos = mCTS.FindNextMove(curChessBoard, simulations);
        curChessBoard.MoveTo(pos.Item1,pos.Item2);
    }
}
