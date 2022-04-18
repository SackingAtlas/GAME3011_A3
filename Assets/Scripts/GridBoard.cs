using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBoard : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,
        Med,
        Hard
    }

    public Difficulty difficulty;

    public GameObject gridpoint, tile, GridHolder, game, endGameScreen, gameScreen;
    public Sprite[] imagesFull;
    [HideInInspector] public Sprite[] images;
    public Text Score, TimeTxt, goalTxt, endGameText;
    public Transform startPos;
    public List<GameObject>[] colArray;
    public int height, width;
    public GameObject[] gridPosArray;
    float pauseTime = 0.05f;
    [HideInInspector] public int points;
    private int counter = 0;
    private int type, xBlocker, yBlocker, botIndex, goal, numOfRocks;
    private bool hasNull, didBreak;
    private float timer = 100;
    AudioSource AS;

    Sprite lastType;
    List<Sprite> imgList = new List<Sprite>();
    List<Vector2> moved = new List<Vector2>();
    //List<List<int>> movedLists = new List<List<int>>();

    private void Start()
    {
        endGameScreen.SetActive(false);
        game.SetActive(false);
        AS = GetComponent<AudioSource>();

        switch (difficulty)
        {
            case Difficulty.Easy:
                goal = 200;
                timer = 100;
                numOfRocks = 3;
                break;
            case Difficulty.Med:
                goal = 400;
                timer = 90;
                numOfRocks = 5;
                break;
            case Difficulty.Hard:
                goal = 600;
                timer = 80;
                numOfRocks = 6;
                break;
        }
        goalTxt.text = "Goal: " + goal.ToString();
        images = new Sprite[numOfRocks];
        for (int i = 0; i < numOfRocks; ++i)
            images[i] = imagesFull[i];

        //setting background size
        float cellsize = GetComponent<RectTransform>().rect.height / height;
        RectTransform rectT = GetComponent<RectTransform>();
        rectT.sizeDelta = new Vector2(width * cellsize, rectT.sizeDelta.y);
        //setting grid tile sizes
        tile.GetComponent<RectTransform>().sizeDelta = new Vector2(cellsize, cellsize);

        gridPosArray = new GameObject[height * width];
        colArray = new List<GameObject>[width];

        for (int x = 0; x < width; x++)
        {
            colArray[x] = new List<GameObject>();
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellsize, y * cellsize, 0) + (startPos.position + new Vector3(cellsize / 2, cellsize/ 2, 0));
                gridPosArray[counter++] = Instantiate(gridpoint, position, Quaternion.identity, GridHolder.transform);
                Vector3 dropPos = gridPosArray[counter - 1].transform.position + new Vector3(0, height * cellsize, 0);
                colArray[x].Add(Instantiate(tile, dropPos, Quaternion.identity, this.transform));
                colArray[x][y].name = "tile" + (counter - 1);
                colArray[x][y].GetComponent<GameToken>().currPos = position;
                StopMatchOnSpawn(x,y);
            }
        }
        Destroy(GridHolder);
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            TimeTxt.text = "Time: " + Mathf.RoundToInt(timer).ToString();
        }
        else
        {
            TimeTxt.text = "Time: 0";
            LoseGame();
        }
    }
    public void DropRow(int y)
    {
        for(int x = 0; x < colArray.Length; ++x)
        colArray[x][y].GetComponent<DropToken>().canDrop = true;
    }

    void StopMatchOnSpawn(int x, int y)
    {
        imgList.AddRange(images);

        if (x > 0 && colArray[x-1][y].GetComponent<GameToken>().lastLeftMatched)
            imgList.Remove(colArray[x - 1][y].GetComponent<Image>().sprite);
        if (y > 0 && colArray[x][y-1].GetComponent<GameToken>().lastBotMatched)
            imgList.Remove(colArray[x][y-1].GetComponent<Image>().sprite);

        type = Random.Range(0, imgList.Count);

        colArray[x][y].GetComponent<Image>().sprite = imgList[type];
        colArray[x][y].GetComponent<GameToken>().GiveData(x, y, width, height);

        if (x > 0 && colArray[x - 1][y].GetComponent<Image>().sprite == imgList[type])
            colArray[x ][y].GetComponent<GameToken>().lastLeftMatched = true;
        if (y > 0 && lastType == imgList[type])
            colArray[x][y].GetComponent<GameToken>().lastBotMatched = true;

        lastType = imgList[type];
        imgList.Clear();
    }

    public void CheckMatches(int colID, int rowID)
    {
        List<GameObject> matchingTilesH = new List<GameObject>();
        List<GameObject> matchingTilesV = new List<GameObject>();
        matchingTilesV.Add(colArray[colID][rowID]);
        matchingTilesH.Add(colArray[colID][rowID]);

        int dumbNum = rowID;

        for(int i = 0; i < 1; ++i)
        {
            --dumbNum;
            if (dumbNum >= 0)
            {
                if (colArray[colID][rowID].GetComponent<Image>().sprite == colArray[colID][dumbNum].GetComponent<Image>().sprite)
                {
                    matchingTilesV.Add(colArray[colID][dumbNum]);
                    --i;
                }
            }
        }

        dumbNum = rowID;
        for (int i = 0; i < 1; ++i)
        {
            ++dumbNum;
            if (dumbNum < height)
            {
                if (colArray[colID][rowID].GetComponent<Image>().sprite == colArray[colID][dumbNum].GetComponent<Image>().sprite)
                {
                    if(!matchingTilesV.Contains(colArray[colID][dumbNum]))
                        matchingTilesV.Add(colArray[colID][dumbNum]);

                    --i;
                }
            }
        }

        dumbNum = colID;
        for (int i = 0; i < 1; ++i)
        {
            --dumbNum;
            if (dumbNum >= 0)
            {
                if (colArray[colID][rowID].GetComponent<Image>().sprite == colArray[dumbNum][rowID].GetComponent<Image>().sprite)
                {
                    matchingTilesH.Add(colArray[dumbNum][rowID]);
                    --i;
                }
            }
        }
        dumbNum = colID;
        for (int i = 0; i < 1; ++i)
        {
            ++dumbNum;
            if (dumbNum < width)
            {
                if (colArray[colID][rowID].GetComponent<Image>().sprite == colArray[dumbNum][rowID].GetComponent<Image>().sprite)
                {
                    if (!matchingTilesH.Contains(colArray[dumbNum][rowID]))
                        matchingTilesH.Add(colArray[dumbNum][rowID]);

                    --i;
                }
            }
        }


        if (matchingTilesH.Count > 2)
        {
            for (int i = 0; i < matchingTilesH.Count; i++)
            {
                matchingTilesH[i].GetComponent<Image>().sprite = null;
                AddPoints();
                didBreak = true;
            }
            matchingTilesH.Clear();
        }

        if (matchingTilesV.Count > 2)
        {
            for (int i = 0; i < matchingTilesV.Count; i++)
            {
                matchingTilesV[i].GetComponent<Image>().sprite = null;
                AddPoints();
                didBreak = true;
            }
            matchingTilesV.Clear();
        }
        if (didBreak)
        {
            AS.Play();
            didBreak = false;
        }
    }
    IEnumerator Cascade()
    {
        List<GameObject> TilesToMove = new List<GameObject>();
       // yield return new WaitForSeconds(pauseTime);

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; ++y)
            {
                if(colArray[x][y].GetComponent<Image>().sprite == null)
                {
                    hasNull = true;
                    botIndex = y;
                    break;
                }
            }

            if (hasNull)
            {
                for (int y = botIndex; y < height; ++y)
                {
                    if (colArray[x][y].GetComponent<Image>().sprite != null)
                    {
                        TilesToMove.Add(colArray[x][y]);
                    }
                }
                foreach (GameObject tile in TilesToMove)
                {
                    colArray[x][botIndex++].GetComponent<Image>().sprite = tile.GetComponent<Image>().sprite;
                    tile.GetComponent<Image>().sprite = null;
                    moved.Add(new Vector2( x, botIndex - 1));                    
                }
            }
            yield return new WaitForSeconds(pauseTime);
            TilesToMove.Clear();
            botIndex = 0;
            hasNull = false;
        }
        if(moved.Count > 0)
        {
            foreach (Vector2 tile in moved)
            {
                CheckMatches(Mathf.RoundToInt(tile.x), Mathf.RoundToInt(tile.y));
            }

            moved.Clear();
            StartCoroutine(Cascade());
        }
        else
        {
            for (int x = 0; x < width; x++)
            {
                if (colArray[x][height - 1].GetComponent<Image>().sprite == null)
                {
                    Refill(x);
                    break;
                }
            }
        }
    }
    void Refill(int startIndex)
    {
        for (int x = startIndex; x < width; x++)
        {
            if (colArray[x][height-1].GetComponent<Image>().sprite == null)
            {
                imgList.AddRange(images);
                if (x > 0)
                    imgList.Remove(colArray[x - 1][height-1].GetComponent<Image>().sprite);
                if(colArray[x][height - 2].GetComponent<Image>().sprite != null)
                    imgList.Remove(colArray[x][height - 2].GetComponent<Image>().sprite);

                type = Random.Range(0, imgList.Count);
                colArray[x][height-1].GetComponent<Image>().sprite = imgList[type];
                imgList.Clear();
            }
        }
        StartCoroutine(Cascade());
        for (int x = 0; x < width; x++)
        {
            if (x < width - 1)
            {
                if (colArray[x + 1][height - 1].GetComponent<Image>().sprite == colArray[x][height - 1].GetComponent<Image>().sprite)
                {
                    CheckMatches(x, height - 1);
                    break;
                }
            }
        }
        StartCoroutine(Cascade());
    }
    public void wait()
    {
        StartCoroutine(Cascade());
    }
    void AddPoints()
    {
        ++points;
        Score.text = "Score: " + points.ToString();
        if (points >= goal)
            WinGame();//you win
    }
    void LoseGame()
    {
        endGameText.text = "GameOver";
        endGameScreen.SetActive(true);
        Destroy(gameScreen);
    }
    void WinGame()
    {
        endGameText.text = "You Win!";
        endGameScreen.SetActive(true);
        Destroy(gameScreen);
    }
}

