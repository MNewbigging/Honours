using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic; // Allows use of List<T>
using System.ComponentModel;
//using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;

public class ControllerListener : MonoBehaviour {

    // This listener class reports to InputMgr to handle input events
    private InputMgr inputMgr;
    // Arduino variables
    // List of all active serial ports
    List<SerialPort> ports = new List<SerialPort>();
    
    // Must match arduino sketch 
    int baudRate = 38400;
    // How long to check for input
    int readTimeOut = 100;

    // Use threads to check for incoming data, since this would block main app thread
    List<Thread> threads = new List<Thread>();
    // Flag to determine when the thread should cease
    bool programActive = true;


    // Keep this object alive throughout 
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Get port names, run threads to pair with controller on active ports
    private void Start()
    {
        // Link up with InputMgr
        inputMgr = GameObject.FindGameObjectWithTag("InputMgr").GetComponent<InputMgr>();

        try
        {

            // Find all active serial ports
            string[] portNames = { "COM6" };// SerialPort.GetPortNames() ;// GetPortNames();

            Debug.Log(portNames.Length + " active ports found:");

            // Create a serial port obj for each found
            for (int i = 0; i < portNames.Length; i++)
            {
                Debug.Log(portNames[i]);
                // Setup serial port properties for this port
                SerialPort sp = new SerialPort();
                sp.PortName = portNames[i];
                sp.BaudRate = baudRate;
                sp.ReadTimeout = readTimeOut;
                sp.Open();
                ports.Add(sp);
                // Create thread to check this port
                Debug.Log("id: " + i);
                Thread t = new Thread(new ThreadStart(() => ProcessData(i)));
                threads.Add(t);
            }
        }
        catch (Exception e) { Debug.Log(e.Message); }

        Debug.Log("Starting " + threads.Count + " threads...");
        // Execute threads 
        foreach (Thread t in threads)
        {
            t.Start();
        }

    } // end start()


    // Returns an array of all active com port names
    public static string[] GetPortNames()
    {
        // Find system OS
        int p = (int)Environment.OSVersion.Platform;
        // To be filled and returned as array
        List<string> serial_ports = new List<string>();

        // Are we on unix?
        if (p == 4 || p == 128 || p == 6)
        {
            // Then it's not my laptop!
        }
        else
        {   // Query registry for the active physical port names
            using (RegistryKey subkey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM"))
            {
                if (subkey != null)
                {
                    string[] names = subkey.GetValueNames();
                    foreach (string value in names)
                    {
                        string port = subkey.GetValue(value, "").ToString();
                        if (port != "")
                        {
                            serial_ports.Add(port);
                        }
                    }
                } // end if subkey isn't null
            } // end using block
        } // end else on windows
        return serial_ports.ToArray();
    } // end GetPortNames()
    
    // Listens for controller input, sends input events to InputMgr via its function call
    void ProcessData(int id)
    {
        // For some reason, id is 1 more than it should be at this point
        id -= 1; // dirty hack

        Debug.Log("Arduino thread " + id + " started. Attempting to pair...");

        // True when paired, allows main loop to run
        bool paired = false; // consider removing

        // Attempt to pair with arduino using given port at id
        // Send pair request message
        ports[id].Write("P");
        Debug.Log(id + " pair request sent.");

        // Read response
        string response = ports[id].ReadLine();
        Debug.Log("Reply: " + response);
        if (response == "Paired")
        {
            paired = true;
            Debug.Log(id + " paired with arduino");
        }
        else
            Debug.Log(id + " pairing failed");


        // Thread main loop, runs while game is still being played and if paired
        while (paired && programActive)
        {
            try
            {
                // The string being recieved from arduino should be converted to int
                int message = Convert.ToInt32(ports[id].ReadLine());

                // Send the input data to InputMgr to handle input logic for this event
                inputMgr.ReceiveInputEvent(message);        
            }
            catch (TimeoutException) { /* Do nothing, loop will be reset */ }
        }
        // If program disabled, unpair
        if (!programActive)
        {
            // Send unpair request
            ports[id].Write("U");
            // Mark response
            string reply = ports[id].ReadLine().ToString();
            Debug.Log("Unpair reply: " + reply);
            // No longer paired
            paired = false;

            // Close all open serial ports
            foreach (SerialPort sp in ports)
            {
                // Close the serial port if still open
                if (sp != null && sp.IsOpen)
                    sp.Close();
            }
        }
        Debug.Log("Thread out");
    }

    // Stop looking for data from arduino once game closes
    public void OnDisable()
    {
        // Flip flag for thread to stop its main loop
        programActive = false;
    }

}
