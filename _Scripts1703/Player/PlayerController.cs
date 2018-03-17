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

public class PlayerController : MonoBehaviour {

    // Movement variables
    private float speed = 0.0f; // This changes
    private const float playerSpeed = 5.0f; // Actual player speed value
    private float turnSpeed = 100.0f; 
    // Used in ProcessData() to determine axis of rotation
    private float rotation;

    // Arduino variables
    // List of all active serial ports
    List<SerialPort> ports = new List<SerialPort>();
    // Must match arduino sketch 
    int baudRate = 9600;
    // How long to check for input
    int readTimeOut = 100;

    // Use threads to check for incoming data, since this would block main app thread
    List<Thread> threads = new List<Thread>();
    // Flag to determine when the thread should cease
    bool programActive = true;
    // Countdown timer for receiving input (speed is lowered when countdown reaaches zero)
    private float countdown = 0.0f; // This changes,
    private const float countdownLimit = 5.0f; // Set to this 
                                               // Runs once on scene load

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        try
        {            
            // Find all active serial ports
            string[] portNames = GetPortNames();
            Debug.Log(portNames.Length + " active ports found:");

            // Create a serial port obj for each found
            for(int i = 0; i < portNames.Length; i++)
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

        } catch (Exception e) { Debug.Log(e.Message); }

        Debug.Log("Starting "+ threads.Count + " threads...");
        // Execute threads 
        foreach(Thread t in threads)
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
        {   // Query registry for the active port names
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


    // Each thread's task; opens a given serial port name, tries to pair with arduino
    // Process incoming data from arduino controller
    void ProcessData(int id)
    {
        // For some reason, id is 1 more than it should be at this point
        id -= 1; // dirty hack

        Debug.Log("Arduino thread " + id + " started. Attempting to pair...");

        // True when paired, allows main loop to run
        bool paired = false;

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
        while (paired)
        {
            try
            {
                // The string being recieved from arduino should be converted to int
                int message = Convert.ToInt32(ports[id].ReadLine());
                
                // 2 is when the magnet was just detected              
                if (message > 1)
                {
                    // Magnet detected; user is pedalling - reset countdown
                    countdown = countdownLimit;
                   
                }
                // -1,0,1 is for rotation data
                else
                {
                    // Arduino sends the correct value, retrieve it
                    rotation = message;                 
                }

                // Determine player speed - 0 if countdown ended, 5 otherwise
                speed = (countdown < 0 ? 0.0f : playerSpeed);

                // If program disabled, unpair
                if (!programActive)
                {
                    // Send unpair request
                    ports[id].Write("U");
                    // Mark response
                    string reply = ports[id].ReadLine().ToString();
                    Debug.Log(reply);
                    paired = false;

                    // Close all open serial ports
                    foreach (SerialPort sp in ports)
                    {
                        // Close the serial port if still open
                        if (sp != null && sp.IsOpen)
                            sp.Close();
                    }
                }
            }
            catch (TimeoutException) { /* Do nothing, loop will be reset */ }
        }
    }

    // Stop looking for data from arduino once game closes
    public void OnDisable()
    {
        // Flip flag for thread to stop its main loop
        programActive = false;
    }


    // All player input and movement
    private void MovePlayer()
    {
        // Check for keyboard rotation keys pressed; this will be 0 if not pressed
        //float rotation = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        // If using controller, use rotation value given by ProcessData() listening to arduino
        float rot = rotation * turnSpeed * Time.deltaTime;
        // Rotate by above amount
        transform.Rotate(0, rot, 0);

        // Always move forward (speed will be 0 when we don't want to move)
        transform.position += transform.forward * speed * Time.deltaTime;

        // Check for holding down space; move forwards
        if (Input.GetKey(KeyCode.Space))
        {
            // Move forwards in the forward facing direction
           // transform.position += transform.forward * speed * Time.deltaTime;

            
        }   
    }


 
    // Update is called once per frame
    void Update () {

        // Handle player movement
        MovePlayer();

        // Reduce countdown clock
        countdown -= Time.deltaTime;
        
    }
}
