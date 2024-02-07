using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public float currentScore = 0;
    public float failed = 0;

    public bool isFirstLaunch = true;

    public List<float> performances = new List<float>();
    public List<float> StickSpeeds = new List<float>();
    public List<float> StickWidth = new List<float>();
    public List<float> StickLength = new List<float>();
    public bool IsStickSpeedsReady = false;

    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI failedText;

    public TMPro.TextMeshProUGUI stickLeftText;

    private TcpListener server;
    private Thread listenerThread;
    private bool isRunning = true;

    public List<float> perfList = new List<float>();

    public bool algoHasStarted = false;
    public bool algoIsRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        server.Start();
        Debug.Log("Server started on localhost:5000");
        listenerThread = new Thread(new ThreadStart(ListenForClients));
        listenerThread.Start();
    }

    private void ListenForClients()
    {
        while (isRunning)
        {
            TcpClient client = server.AcceptTcpClient();
            Debug.Log("Client connected");

            Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
            clientThread.Start(client);
        }
    }

    private void HandleClient(object clientObject)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
        TcpClient client = (TcpClient)clientObject;
        NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;

        Debug.Log("Waiting for data...");
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log("Received: " + data);

            lock ("performances")
            {
                algoIsRunning = false;
            }

            // Parse the data and return the speeds
            lock ("lockStats")
            {
                //StickSpeeds = ParseAndReturnSpeeds(data);
                var numbers = data.Trim(new char[] { ' ', '[', ']' })
                           .Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(str => double.Parse(str, CultureInfo.InvariantCulture))
                           .ToArray();

                // Grouper les nombres par trois et ajouter les vitesses et les longueurs à la liste
                for (int inc = 0; inc < numbers.Length; inc += 2)
                {
                    StickSpeeds.Add((float)numbers[inc]);
                    StickLength.Add((float)numbers[inc + 1]);
                }

                IsStickSpeedsReady = true;
            }
            

            // When currentScore + failed = 10, we send the performances to the python script.

            while (currentScore + failed < 10)
            {
                // Wait for the score to be 10
            }

            bool listEmpty = true;

            while (listEmpty)
            {
                lock ("performances")
                {
                    listEmpty = perfList.Count == 0;
                }

                Thread.Sleep(100);
            }

            lock ("performances") {
                Debug.Log("PerfList: " + perfList.Count);

                foreach (float dist in perfList){
                    Debug.Log("Perf" + dist);
                }
            }

            string performances_string = "[ ";
            int i = 0;
            // Lock the performances list to avoid concurrency issues
            lock (perfList)
            {
                foreach (float performance in perfList)
                {
                    performances_string += performance.ToString() + " ";
                    i++;
                    if (i == 10)
                    {
                        break;
                    }
                }
                performances_string += "]";

                // Remove the first 10 elements of the list
                performances.RemoveRange(0, 9);
            }

            Debug.Log("Sending: " + performances_string);

            byte[] performances_bytes = Encoding.ASCII.GetBytes(performances_string);
            stream.Write(performances_bytes, 0, performances_bytes.Length);
            Debug.Log("Sent: " + performances_string);
            lock ("performances")
            {
                algoIsRunning = true;
                algoHasStarted = true;
            }
            
            currentScore = 0;
            failed = 0;
            isFirstLaunch = true;
        }

        client.Close();
        Debug.Log("Client disconnected");
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        listenerThread.Abort();
        server.Stop();
    }

    public List<float> ParseAndReturnSpeeds(string input)
    {
        List<float> stick_speeds = new List<float>();
        
        // Supprimer les crochets [ et ] de la chaîne
        input = input.Trim('[', ']');

        // Diviser la chaîne en valeurs individuelles en utilisant l'espace comme séparateur
        string[] speedValues = input.Split(' ');

        // Convertir chaque valeur en float et l'ajouter à la liste
        foreach (string speedValue in speedValues)
        {
            if (speedValue == "")
            {
                continue;
            }
            stick_speeds.Add(float.Parse(speedValue, CultureInfo.InvariantCulture.NumberFormat));
        }

        return stick_speeds;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddScore(float n)
    {
        lock (performances)
        {
            currentScore += n;
            scoreText.text = currentScore.ToString();
            stickLeftText.text = "Bâtons restants : " + (currentScore + failed).ToString() + " / 10. Bon courage !"; 
        
            performances.Add(85.0f);
        }
    }

    public void AddFailed(float n)
    {
        lock (performances)
        {
            failed += n;
            failedText.text = failed.ToString();
            stickLeftText.text = "Bâtons restants : " + (currentScore + failed).ToString() + " / 10. Bon courage !";
        
            performances.Add(0.0f);
        }
    }
}
