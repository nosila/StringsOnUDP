using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Receives strings by UDP using multithreading.
    /// </summary>
    class Program
    {
        /// <summary>
        /// IP of your machine.
        /// </summary>
        private static string _IP = "127.0.0.1";

        /// <summary>
        /// Port where you want to listen to data.
        /// </summary>
        private static int _port = 8100; 

        /// <summary>
        /// Will hold the string received from StringOnUDP Server.
        /// </summary>
        private static string receivedData = "";

        /// <summary>
        /// Used to check if user wants to terminate the program.
        /// </summary>
        private static bool finished;

        private static UdpClient _client;

        private static void Main(string[] args)
        {
            if (args.Length > 0)
                checkArgs(args);

            Thread receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Initialize();

            string input = "";
            while (!finished)
            {
                Console.Write("\nSOnU> ");
                input = Console.ReadLine();
                finished = checkInputFinished(input);
            }
        }

        /// <summary>
        /// Checks if user wants to terminate program.
        /// </summary>
        /// <param name="input">User input</param>
        /// <returns>True if user inserts "quit" or "exit" or "q" or "e".</returns>
        private static bool checkInputFinished(string input)
        {
            string cmd = input.ToLower();
            switch (cmd)
            {
                case "quit":
                case "exit":
                case "q":
                case "e":
                    return true;
                default:
                    Console.WriteLine("Command not found!");
                    return false;
            }
        }

        /// <summary>
        /// Initializes Console.
        /// </summary>
        private static void Initialize()
        {
            Console.Title = "StringsOnUDP - Client v0.1";
            Console.WriteLine("Type 'exit' or 'quit' or 'q' or 'e' to terminate.\n");
            Console.WriteLine("Waiting for StringsOnUDP Server to send us something...");
        }

        /// <summary>
        /// Receives data from StringOnUDP Server. This will work concurrently, 
        /// on another thread.
        /// </summary>
        private static void ReceiveData()
        {
            _client = new UdpClient(_port);
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(_IP), _port);

            while (!finished)
            {
                try
                {

                    byte[] data = _client.Receive(ref anyIP);
                    receivedData = Encoding.UTF8.GetString(data);
                    Console.WriteLine(">> " + receivedData);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.ToString());
                }
            }
            _client.Close();
            
            anyIP = null;
            _client = null;
        }

        /// <summary>
        /// Used to check program arguments.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        private static void checkArgs(string[] args)
        {
            string argLower = "";

            foreach (string arg in args)
            {
                argLower = arg.ToLower();

                //Do arguments check HERE
                //You can use bool fields for this
            }
            argLower = null;
        }
    }
}