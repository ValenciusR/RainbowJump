using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONSerializeable<T> where T : class
{
    public static T CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<T>(jsonString);
    }

    public string CreateToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}