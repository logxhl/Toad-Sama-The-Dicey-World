using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSave
{
    // ============ BASIC GAME DATA ============
    [Header("------------Basic Game Data------------")]

    // Trạng thái mở khóa booster
    public bool isNew;                          // Đánh dấu save mới
    public int level;                           // Level hiện tại

    // ===== Constructor =====
    public GameSave()
    {
        level = 1;
    }
}