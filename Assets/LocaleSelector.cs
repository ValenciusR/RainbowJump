using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{

    private bool active = false;
    private int count;

    public void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        SetLocale(ID);
    }
    public void ChangeLocale()
    {
        if(active == true)
        {
            return;
        }
        count += 1;
        count %= 3;
        StartCoroutine(SetLocale(count));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);
        active = false;
    }
}
