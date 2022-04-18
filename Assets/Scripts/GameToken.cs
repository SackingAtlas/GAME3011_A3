using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GridBoard GameGrid;
    public Vector3 currPos, moveToPos;
    public Vector2 StartDragPos, currDragPos, endDragPos;
    public float DeadZone;
    public int TypeID;
    public bool lastLeftMatched, lastBotMatched;
    private int rowIndex, coloumIndex, maxH, maxW;
    private bool canSwitch, TargetSetY, TargetSetX, newClick;

    private void Start()
    {
        GameGrid = GetComponentInParent<GridBoard>();
    }
    public void GiveData(int col, int row, int gridW, int gridH)
    {
        rowIndex = row;
        coloumIndex = col;
        maxH = gridH;
        maxW = gridW;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StartDragPos = eventData.position;
        newClick = true;
        if(GetComponent<Image>().sprite == GameGrid.imagesFull[5])
            canSwitch = false;
        else
            canSwitch = true;
    }

    // Drag the selected item.
    public void OnDrag(PointerEventData data)
    {
        if (canSwitch)
        {
            currDragPos = data.position;
            Vector2 direction = (currDragPos - StartDragPos);

            if (Vector2.Distance(currDragPos, StartDragPos) > DeadZone && newClick)
            {
                if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y) && !TargetSetX)
                {
                    TargetSetY = true;
                    if (direction.y > 0)
                        IndicateSwitch(coloumIndex, rowIndex + 1, new Vector3(transform.position.x, currDragPos.y, transform.position.z));//("up");
                    else if (direction.y < 0)
                        IndicateSwitch(coloumIndex, rowIndex - 1, new Vector3(transform.position.x, currDragPos.y, transform.position.z));//("down");
                }
                else if (Mathf.Abs(direction.y) < Mathf.Abs(direction.x) && !TargetSetY)
                {
                    TargetSetX = true;
                    if (direction.x > 0)
                        IndicateSwitch(coloumIndex + 1, rowIndex, new Vector3(currDragPos.x, transform.position.y, transform.position.z)); //("right");
                    else if (direction.x < 0)
                        IndicateSwitch(coloumIndex - 1, rowIndex, new Vector3(currDragPos.x, transform.position.y, transform.position.z)); //("left");
                }
            }
        }     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (newClick)
        {
            transform.position = currPos;
        }
        TargetSetY = false;
        TargetSetX = false;
    }
    void IndicateSwitch(int C, int R, Vector3 movement)
    {
        if (C >= 0 && C < maxW && R >= 0 && R < maxH) //grid bounds check 
        {
            if (GameGrid.colArray[C][R].GetComponent<Image>().sprite != GameGrid.imagesFull[5])
            {
                moveToPos = GameGrid.colArray[C][R].GetComponent<GameToken>().currPos;
                transform.position = movement;

                if (Vector2.Distance(transform.position, moveToPos) < 10)
                {
                    newClick = false;
                    TargetSetY = false;
                    TargetSetX = false;
                    TileSwitch(C, R);
                }
                if (Vector2.Distance(transform.position, currPos) < 10)
                {
                    TargetSetY = false;
                    TargetSetX = false;
                }
            }
        }
    }
    public void TileSwitch(int C, int R)
    {
        GameObject otherTile = GameGrid.colArray[C][R];
        (otherTile.GetComponent<Image>().sprite, this.gameObject.GetComponent<Image>().sprite) = (this.gameObject.GetComponent<Image>().sprite, otherTile.GetComponent<Image>().sprite);
        transform.position = currPos;

        GameGrid.CheckMatches(coloumIndex, rowIndex);
        GameGrid.CheckMatches(C, R);
        GameGrid.wait();
    }
}
//GameObject otherTile = GameGrid.colArray[C][R];
//GameToken otherData = otherTile.GetComponent<GameToken>();

//transform.position = otherData.currPos;
//otherTile.transform.position = currPos;
//currPos = transform.position;
//otherData.currPos = otherData.transform.position;


//GameGrid.colArray[coloumIndex][rowIndex] = otherTile;
//GameGrid.colArray[C][R] = this.gameObject;

//(coloumIndex, otherData.coloumIndex) = (otherData.coloumIndex, coloumIndex);
//(rowIndex, otherData.rowIndex) = (otherData.rowIndex, rowIndex);