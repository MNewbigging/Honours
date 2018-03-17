using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO.Ports;
using System;

// Spawns 2 threads, one to incoming and outgoing com ports

public class ControllerListenerV2 : MonoBehaviour {

    // Singleton
    public static ControllerListenerV2 instance = null;
    // COM port properties  
    private string incoming = "COM6";
    private int baudRate = 9600;
    private int readTimeOut = 1000;
    private int bufferSize = 1;
    private bool programActive = true;
    SerialPort inPort;
    Thread inThread;
    // Send input events to Input manager
    private InputMgr inputMgr;

    // Keep this alive throughout
    private void Awake()
    {
        // Check if this instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Initialise ports, set threads running
    private void Start() {

        // Link up with InputMgr
        inputMgr = GameObject.FindGameObjectWithTag("InputMgr").GetComponent<InputMgr>();

        // Incoming com port
        try
        {
            inPort = new SerialPort(incoming, baudRate, Parity.None, 8, StopBits.One);
            inPort.ReadTimeout = readTimeOut;
            inPort.Open();
            inThread = new Thread(new ThreadStart(() => IncomingThread()));
            inThread.Start();
        } catch (Exception e) { Debug.Log("IN ERROR: " + e.Message); }       
    }

    // Stop threads on program exit
    private void OnDisable()
    {
        programActive = false;
        if (inPort.IsOpen) 
            inPort.Close();
    }


    // Incoming thread - listen for input events from controller
    private void IncomingThread()
    {
        Debug.Log("In thread started.");
        // Read in data to this buffer
        Byte[] buffer = new Byte[bufferSize];
        // Details number of bytes read on inPort
        int bytesRead = 0;

        while (programActive)
        {
            try
            { 
                // Try reading inPort
                bytesRead = inPort.Read(buffer, 0, bufferSize);
                // If something was read into buffer
                if (bytesRead > 0)
                {
                    // Parse byte to int 
                    int inputEvent = buffer[0] - 48; // avoids actually converting/parsing
                    // Send to input manager to handle
                    inputMgr.ReceiveInputEvent(inputEvent);
                }        
            } catch (Exception e) {
                Debug.Log("In thread error: "+ e.Message);
            }       
        }

        // Close the com port if still open
        if (inPort.IsOpen)
            inPort.Close();

        Debug.Log("In thread dead");
    }

    



}
