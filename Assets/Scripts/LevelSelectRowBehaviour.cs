using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectRowBehaviour : LevelSelectBehaviour
{
    public const int BTNS_PER_ROW = 3;

    public override bool Init(int rowIndex)
    {
        if (!base.Init(rowIndex)) return false;

        for (int i = 0; i < BTNS_PER_ROW; i++)
        {
            transform.GetChild(i).GetComponent<LevelSelectBtnBehaviour>()
                .Init(rowIndex * BTNS_PER_ROW + i);
        }

        return true;
    }
}
