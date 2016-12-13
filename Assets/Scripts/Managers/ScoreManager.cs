using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : MonoBehaviour {
    public int levelCurrent { get; set; }
    public int levelMaximum { get; set; }
    private readonly string delimeter = "|";

	public void EventWinLevel(bool editorLevel)
	{
        if (!editorLevel)
        {
            GetComponent<MenuManager>().UpdateIndicator(levelCurrent);
            levelCurrent ++;
            if (levelCurrent > levelMaximum)
            {
                levelMaximum = levelCurrent;
            }
            SetPlayerScores();
        }
        else
        {
            GetComponent<MenuManager>().UpdateIndicator(-1);
        }
	}

    public void EventResetPlayerScores()
    {
        PlayerPrefs.DeleteAll();
        levelCurrent = 0;
        levelMaximum = 0;
        SetPlayerScores();
    }

    public void GetPlayerScores()
    {
        string value = EncryptedPlayerPrefs.GetString("USER", "");
        if (value != "")
        {
            value = value.Replace("{", "");
            value = value.Replace("}", "");
            string[] values = value.Split(delimeter.ToCharArray());
            try
            {
                if (SystemInfo.deviceUniqueIdentifier != values[0])
                {
                    EventResetPlayerScores();
                }
                levelMaximum = int.Parse(values[1]);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EventResetPlayerScores();
            }
        }
        else
        {
            EventResetPlayerScores();
		}
		GetComponent<MenuManager>().UpdateIndicator(levelMaximum);
    }

    public void SetPlayerScores()
    {
		string value = "{" + SystemInfo.deviceUniqueIdentifier + delimeter + levelMaximum + "}";
        string encrypted = EncryptedPlayerPrefs.SetString("USER", value);
        PlayerPrefs.Save();
        StartCoroutine(DBSetScores(encrypted));
    }

    private IEnumerator DBSetScores(string value)
    {
        string url = "http://modelformat.com/api_ioreboot/PlayerSubmitData.php?d=" + value;
        yield return new WWW(url);
    }


    private IEnumerator DBGetRank()
    {
        string url = "http://omarchehabio.dx.am/GetPlayerRank.php/?value=" + Encryption.Encrypt(SystemInfo.deviceUniqueIdentifier);
        WWW www;
        yield return www = new WWW(url);

        string rank = www.text.Substring(6);
        if (rank != string.Empty)
        {
            int iRank = int.Parse(rank);
            int ones = iRank % 10;
            int tens = Mathf.FloorToInt(iRank / 10) % 10;
            if (tens == 1)
            {
                rank += "th";
            }
            else
            {
                switch (ones)
                {
                    case 1: rank += "st"; break;
                    case 2: rank += "nd"; break;
                    case 3: rank += "rd"; break;
                    default: rank += "th"; break;
                }
            }
        }
    }

}
