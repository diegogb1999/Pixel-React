using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using Firebase.Extensions;
using Firebase;
using Firebase.Database;

public class FirebaseManager : MonoBehaviour
{
    private static string databaseURL = "https://ivandevwebsite-default-rtdb.europe-west1.firebasedatabase.app/GameData/"; // URL de tu base de datos

    private void Start()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }
    public void UploadGame()
    {
        StartCoroutine(SendDataToFirebase());
    }

    IEnumerator SendDataToFirebase()
    {
        //string jsonUrl = ;

        UnityWebRequest request = UnityWebRequest.Put(databaseURL, Encoding.UTF8.GetBytes(""));
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Data saved successfully.");
        }
        else
        {
            Debug.LogError("Error saving data: " + request.error);
        }
    }
}
