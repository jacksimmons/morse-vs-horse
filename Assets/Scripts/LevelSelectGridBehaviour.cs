using UnityEngine;

public class LevelSelectGridBehaviour : LevelSelectBehaviour
{
    [SerializeField]
    private int m_map;
    public const int LEVELS_PER_MAP = ROWS_PER_GRID * LevelSelectRowBehaviour.BTNS_PER_ROW + 1;
    public const int ROWS_PER_GRID = 3;

    private void Awake()
    {
        Init(m_map);
    }

    public override bool Init(int mapIndex)
    {
        if (!base.Init(mapIndex)) return false;

        for (int i = 0; i < ROWS_PER_GRID; i++)
        {
            transform.GetChild(i).GetComponent<LevelSelectRowBehaviour>()
                .Init(mapIndex * ROWS_PER_GRID + i);
        }

        return true;
    }
}