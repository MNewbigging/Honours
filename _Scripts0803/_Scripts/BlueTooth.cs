using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class BlueTooth : MonoBehaviour {

    // Declare serial port obj to work with
    private SerialPort sp = null;
    string portName = "COM8";
    int baudRate = 9600;
    int readTimeOut = 1;
    int bufferSize = 6;

    bool programActive = true;
    Thread thread;


	// Use this for initialization
	void Start () {

        try
        {
            sp = new SerialPort();
            sp.PortName = portName;
            sp.BaudRate = baudRate;
            sp.ReadTimeout = readTimeOut;
            sp.Open();
        }
        catch (Exception e) { Debug.Log(e.Message); }

        // Execute thread to listen for input
        thread = new Thread(new ThreadStart(ProcessData));
        thread.Start();   
    }
	

    void ProcessData()
    {

        Debug.Log("Thread started");

        // Thread main loop, runs while game is still being played
        while(programActive)
        {
            try
            {
                Byte[] buffer = new Byte[bufferSize];
                int bytesRead = 0;
                // Attempt to read data from bt device
                // Will throw exception if no data is received
                bytesRead = sp.Read(buffer, 0, bufferSize);

                if (bytesRead > 0)
                {
                    Debug.Log(bytesRead + " bytes read");
                    for (int i = 0; i < bufferSize; i++)
                    {
                        Debug.Log(buffer[i]);
                    }
                }

                bytesRead = 0;
            } catch (TimeoutException) { /* Do nothing, loop will be reset */ }
        }
    }


    public void OnDisable()
    {
        programActive = false;

        if (sp != null && sp.IsOpen)
            sp.Close();
    }





    // Update is called once per frame
    void Update () {
	
	}
}
