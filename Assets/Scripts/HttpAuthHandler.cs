using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HttpAuthHandler : MonoBehaviour
{
    [SerializeField] private string ServerApiURL;
    [SerializeField] public TextMeshProUGUI puntaje;
    [SerializeField] public TextMeshProUGUI tablaDePuntaje;

    public string Token;
    public string Username;

    // Start is called before the first frame update
    void Start()
    {
        Token = PlayerPrefs.GetString("token");
        Username = PlayerPrefs.GetString("username");

        if (string.IsNullOrEmpty(Token))
        {
            Debug.Log("No hay token");
            //Ir a Login
        }
        else
        {
            Debug.Log(Token);
            Debug.Log(Username);
            StartCoroutine(GetPerfil());
        }
    }

    public void Registrar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Registro(postData));
    }

    public void Ingresar()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        user.password = GameObject.Find("InputPassword").GetComponent<TMP_InputField>().text;
        string postData = JsonUtility.ToJson(user);
        StartCoroutine(Login(postData));
    }

    public void Score()
    {
        User user = new User();

        user.username = GameObject.Find("InputUsername").GetComponent<TMP_InputField>().text;
        user.data = new UserData(int.Parse(GameObject.Find("InputScore").GetComponent<TMP_InputField>().text));
        string postData = JsonUtility.ToJson(user);
        Debug.Log(postData);
        StartCoroutine(Scores(postData));
    }

    public void Tabla()
    {
        StartCoroutine(GetList());
    }

    IEnumerator Registro(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "api/usuarios",postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200) //funciona
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + "Se registró con ID " + jsonData.usuario._id);
                Ingresar();

            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }

    IEnumerator Login(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "api/auth/login", postData);
        www.method = "POST";
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200) //funciona
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + "Inició sesión ");

                Token = jsonData.token;
                Debug.Log(Token);
                Username = jsonData.usuario.username;

                PlayerPrefs.SetString("token", Token);
                PlayerPrefs.SetString("username", Username);

            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }

    IEnumerator Scores(string postData)
    {

        UnityWebRequest www = UnityWebRequest.Put(ServerApiURL + "api/usuarios", postData);
        www.method = "PATCH";
        www.SetRequestHeader("x-token", Token);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200) //funciona
            {
                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                puntaje.text = "Este es su puntaje: " + jsonData.usuario.data.score;
            }
            else
            {
                string message = "status: " + www.responseCode;
                message += "\ncontent-type: " + www.GetResponseHeader("content-type");
                message += "\nError: " + www.error;
                Debug.Log(message);
            }

        }
    }

    IEnumerator GetPerfil()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiURL + "api/usuarios/" + Username);
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                AuthJsonData jsonData = JsonUtility.FromJson<AuthJsonData>(www.downloadHandler.text);

                Debug.Log(jsonData.usuario.username + " Sigue con la sesion inciada");
                //Cambiar de escena
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }

    IEnumerator GetList()
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerApiURL + "api/usuarios");
        www.SetRequestHeader("x-token", Token);

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR :" + www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {

                UserList lista = JsonUtility.FromJson<UserList>(www.downloadHandler.text);

                List<User> listaOrdenada = lista.usuarios.OrderByDescending(u => u.data.score).ToList<User>();
                string tabla ="";

                foreach (User user in listaOrdenada)
                {
                    tabla += user.username + " " + user.data.score + "\n";
                }

                tablaDePuntaje.text = "Lista: \n" + tabla;
            }
            else
            {
                string mensaje = "Status :" + www.responseCode;
                mensaje += "\ncontent-type:" + www.GetResponseHeader("content-type");
                mensaje += "\nError :" + www.error;
                Debug.Log(mensaje);
            }

        }
    }
}

[System.Serializable]
public class User
{
    public string _id;
    public string username;
    public string password;
    public UserData data;

    public User()
    {

    }
    public User(string username, string password)
    {
        this.username = username;
        this.password = password;
    }
}

[System.Serializable]
public class UserData
{
    public int score;

    public UserData(int score)
    {
        this.score = score;
    }
}

public class UserList
{
    public List<User> usuarios;
}

public class AuthJsonData
{
    public User usuario;
    public string token;
}
