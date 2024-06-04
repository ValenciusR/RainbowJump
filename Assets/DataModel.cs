using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MessagePack;

[System.Serializable]
public class DataModel : JSONSerializeable<DataModel>
{
    public float score;
    public float obstclepassed;
    public int day;
    public int month;
    public int year;
    public DataModel()
    {
        obstclepassed = 0;
        day = 0;
        month = 0;
        year = 0;
        score = 0;
    }
}
