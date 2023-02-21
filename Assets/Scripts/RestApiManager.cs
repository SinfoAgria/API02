using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RestApiManager : MonoBehaviour
{

    [SerializeField] private RawImage YourRawImage;
    [SerializeField] private RawImage YourRawImage2;
    [SerializeField] private RawImage YourRawImage3;
    [SerializeField] private RawImage YourRawImage4;
    [SerializeField] private RawImage YourRawImage5;

    [SerializeField]
    private int UserId = 1;

    [SerializeField]
    private TextMeshProUGUI UserName;

    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI characterName2;
    [SerializeField] private TextMeshProUGUI characterName3;
    [SerializeField] private TextMeshProUGUI characterName4;
    [SerializeField] private TextMeshProUGUI characterName5;

    private string[] charactersID = new string[5];

    private string MyServerApiPath = "https://my-json-server.typicode.com/SinfoAgria/JsonAPI01/users/";
    private string RickandMortyApi = "https://rickandmortyapi.com/api/character/";

    void Start()
    {
        StartCoroutine(GetPlayerInfo());
    }

    public void GetCharactersClick()
    {
        StartCoroutine(GetCharacters());
    }

    IEnumerator GetPlayerInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get(MyServerApiPath + UserId);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR : " + www.error);
        }
        else
        {
            //Debug.Log(www.GetResponseHeader("content-type"));

            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {
                UserJsonData user = JsonUtility.FromJson<UserJsonData>(www.downloadHandler.text);
                Debug.Log(user.name);
                UserName.text = user.name;
                for (int i = 0; i < user.deck.Length; i++)
                {
                    charactersID[i] = user.deck[i].ToString();
                    Debug.Log(charactersID[i]);
                }
            }
            else
            {
                string mensaje = "Status: " + www.responseCode;
                mensaje += "\ncontent-type: " + www.GetResponseHeader("content-type");
                mensaje += "\nError: " + www.error;
                Debug.Log(mensaje);
            }
        }
    }

    IEnumerator GetCharacters()
    {
        for (int i = 0; i < charactersID.Length; i++)
        {
            UnityWebRequest www = UnityWebRequest.Get(RickandMortyApi + charactersID[i]);
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log("NETWORK ERROR : " + www.error);
            }
            else
            {
                //Debug.Log(www.GetResponseHeader("content-type"));

                Debug.Log(www.downloadHandler.text);

                if (www.responseCode == 200)
                {
                    Character characters = JsonUtility.FromJson<Character>(www.downloadHandler.text);
                    Debug.Log(characters.name);

                    StartCoroutine(DownloadImage(characters.image, i, characters.name));
                }
                else
                {
                    string mensaje = "Status: " + www.responseCode;
                    mensaje += "\ncontent-type: " + www.GetResponseHeader("content-type");
                    mensaje += "\nError: " + www.error;
                    Debug.Log(mensaje);
                }
            }
        }
    }

    IEnumerator DownloadImage(string MediaUrl, int imageIndex, string characterNickname)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else if (imageIndex == 0)
        {
            YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            characterName.text = characterNickname;
        }
        else if (imageIndex == 1)
        {
            YourRawImage2.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            characterName2.text = characterNickname;
        }
        else if (imageIndex == 2)
        {
            YourRawImage3.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            characterName3.text = characterNickname;
        }
        else if (imageIndex == 3)
        {
            YourRawImage4.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            characterName4.text = characterNickname;
        }
        else
        {
            YourRawImage5.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            characterName5.text = characterNickname;
        }
    }

    public class UserJsonData
    {
        public int id;
        public string name;
        public int[] deck;
    }

    [System.Serializable]
    public class CharactersList
    {
        public CharactersListInfo info;
        public List<Character> results;
    }
    [System.Serializable]
    public class CharactersListInfo
    {
        public int count;
        public int pages;
        public string next;
        public string prev;
    }
    [System.Serializable]
    public class Character
    {
        public int id;
        public string name;
        public string species;
        public string image;
    }
}
