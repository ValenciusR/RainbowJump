using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MessagePack;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class LeaderboardModel : JSONSerializeable<LeaderboardModel>
{
    public List<DataModel> objectListDataModel;
    public LeaderboardModel()
    {
        objectListDataModel = new List<DataModel>();
    }
}


[System.Serializable]
public class DataModel : JSONSerializeable<DataModel>, IComparable
{
    public string name;
    public string uid;
    public float score;
    public float obstclepassed;
    public int day;
    public int month;
    public int year;

    public DataModel()
    {
    }

    public DataModel(string name, string uid, float score, float obstaclepassed, int day, int month, int year)
    {
        this.name = name;
        this.uid = uid;
        this.score = score;
        this.obstclepassed = obstaclepassed;
        this.day = day;
        this.month = month;
        this.year = year;
    }

    public int CompareTo(object obj)
    {
        DataModel otherData = obj as DataModel;
        if (otherData != null)
        {
            return this.score.CompareTo(otherData.score);
        }
        else
        {
            throw new ArgumentException("Object is not a Score");
        }
    }
}
