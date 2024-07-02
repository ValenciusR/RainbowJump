using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class DataBaseManager : MonoBehaviour
{
    private string userID;
    private DatabaseReference dbreference;

    void Start()
    {
        userID = SystemInfo.deviceUniqueIdentifier;
        DatabaseReference dbreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUserFirebase(string name, string uid, float score, float obstaclescore, int day, int month, int year)
    {
        DataModel data = new DataModel(name,uid,score,obstaclescore,day,month,year);

        string jsonuser = data.CreateToJSON();
        dbreference.Child("users").Child(userID).SetRawJsonValueAsync(jsonuser);
    }
}
