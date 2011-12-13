using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
	/// <summary>
	/// 
	/// </summary>
	class Program
	{
		private static bool finished = false;
		private static bool messagesLocked = false;

		private static string _IP = "127.0.0.1";
		private static int _port = 8100; 
		private static IPEndPoint _remoteEndPoint;
		private static UdpClient _client;
		private static Commands _cmds;

		private static string[] _messages = null;
		private static string[] _cmdSeparator = { " && " };

		private enum EInput
		{
			NULL,
			FINISHED,
			SEND,
			STOP,
			HELP
		}


		private static void Main(string[] args)
		{
			_cmds = new Commands();

			if (args.Length > 0)
				checkArgs(args);

			_remoteEndPoint = new IPEndPoint(IPAddress.Parse(_IP), _port);

			Thread sendThread = new Thread(SendMessage);
			sendThread.IsBackground = true;
			sendThread.Start();

			_client = new UdpClient();

			string inputStr = "";
			EInput input;
			bool foundCmd = false;
			string[] cmd;

			Initialize();
			while (!finished)
			{

				Console.Write("SOnU>");
				inputStr = Console.ReadLine();
				input = checkInput(inputStr);

				if (input == EInput.FINISHED)
					finished = true;
				else if (input == EInput.STOP)
					_messages = null; //message will not be sent if null
				else if (input == EInput.HELP)
					showHelp();
				else if (input == EInput.SEND)
				{
					cmd = getCommand(inputStr);
					foundCmd = checkIfCommandsExist(cmd);
					if (foundCmd)
					{
						messagesLocked = true;
						getMessagesFromTable(cmd); //changes _messages
						messagesLocked = false;
						showSendingMessages(cmd);
					}
					else
						showCmdNotFound(cmd);
				}
				else
				{
					Console.WriteLine("Invalid command line!\n");
				}
			}
			sendThread.Abort();



		}


		/// <summary>
		/// Initializes Console.
		/// </summary>
		private static void Initialize()
		{
			Console.Title = "StringsOnUDP - Server v0.1";
			Console.WriteLine("type help for . . . help lol.\n");
		}


		/// <summary>
		/// Show messages being sent.
		/// </summary>
		/// <param name="cmds">Command list.</param>
		private static void showSendingMessages(string[] cmds)
		{

			Console.WriteLine("\nSending Messages:");
			foreach (string message in _messages)
			{
				Console.WriteLine(message);
			}
			Console.WriteLine();
		}

		/// <summary>
		/// Show command not found message. Note: This will not verify which 
		/// command wasn't found.
		/// </summary>
		/// <param name="cmds">Command list.</param>
		private static void showCmdNotFound(string[] cmds)
		{
			Console.WriteLine("At least one of the commands was not found in our database.\nCommand List:");
			foreach (string cmd in cmds)
			{
				Console.WriteLine(cmd);
			}
		}

		/// <summary>
		/// Gets the messages from the commands.
		/// </summary>
		/// <param name="cmds">Command list.</param>
		private static void getMessagesFromTable(string[] cmds)
		{
			_messages = new string[cmds.Length];
			for (int i = 0; i < cmds.Length; i++)
			{
				_messages[i] = _cmds.Table[cmds[i]];
			}
		}

		/// <summary>
		/// Checks if the choosen commands exist.
		/// </summary>
		/// <param name="cmds">Command list.</param>
		/// <returns>True if all the commands in the Command list exist in memory.</returns>
		private static bool checkIfCommandsExist(string[] cmds)
		{
			return cmds.All(cmd => _cmds.Table.ContainsKey(cmd));
		}

		/// <summary>
		/// Sends the selected messages. This method is running in a different
		/// thread.
		/// </summary>
		private static void SendMessage()
		{
			try
			{
				while (!finished)
				{
					if (_messages != null && !messagesLocked)
					{
						foreach (string message in _messages)
						{
							byte[] data = Encoding.UTF8.GetBytes(message);
							_client.Send(data, data.Length, _remoteEndPoint);
						}
					}
				}
			}
			catch (Exception err)
			{
				Console.WriteLine(err.ToString());
			}
			finally
			{
				_client.Close();
			}
		}

		/// <summary>
		/// Show help.
		/// </summary>
		private static void showHelp()
		{
			Console.Clear();
			Console.WriteLine("---------------");
			Console.WriteLine("Reserved keywords:");
			Console.WriteLine("\tq, quit, exit\t\tTerminate program.");
			Console.WriteLine("\tstop\t\t\tStop sending messages.");
			Console.WriteLine("\thelp\t\t\tShows help.");
			Console.WriteLine("\tsend [command_name]\tStarts sending the message tagged with [command_name]");
			Console.WriteLine("\t&&\t\t\tCan be used to send more than one command at a time. ex:");
			Console.WriteLine("\t\tsend [command_name1] && [command_name2] && [command_name3] . . .");
			Console.WriteLine("---------------");
			Console.WriteLine("Command list in memory:");
			foreach (KeyValuePair<string, string> keyValuePair in _cmds.Table)
			{
				Console.WriteLine("\t " + keyValuePair.Key + "\n\t\t" + keyValuePair.Value);
			}
			Console.WriteLine("---------------");
			Console.Write("More Message commands (used with the 'send' command) can be added in the ");

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("cmds.cfg");
			Console.ForegroundColor = ConsoleColor.Gray;

			Console.Write(" file.\n");
			Console.WriteLine("---------------\n");
		}

		private static EInput checkInput(string input)
		{
			if (String.IsNullOrWhiteSpace(input))
				return EInput.NULL;
			if (input == "q" || input == "quit" || input == "exit")
				return EInput.FINISHED;
			if (input == "stop")
				return EInput.STOP;
			if (input == "help")
				return EInput.HELP;
			if (input.Length > 5 && input.Substring(0, 5) == "send ")
				return EInput.SEND;



			return EInput.NULL;
		}

		/// <summary>
		/// Filters the "send " from the command.
		/// </summary>
		/// <param name="input">Full command with "send ".</param>
		/// <returns>Command name in memory (which will be used to send the message).</returns>
		private static string[] getCommand(string input)
		{
			return input.Substring(5).Split(_cmdSeparator, StringSplitOptions.RemoveEmptyEntries);
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
				//Do arguments check HERE
				//You can use bool fields for this
			}

			argLower = null;
		}

#if DEBUG
		///<summary>
		///	This code can be used to receive the data inside the server.
		///</summary>
		private static void ReceiveData()
		{
			UdpClient client = new UdpClient(8888);
			while (true)
			{
				try
				{
					IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
					byte[] data = client.Receive(ref anyIP);
					string text = Encoding.UTF8.GetString(data);
					Console.WriteLine(">> " + text);
				}
				catch (Exception err)
				{
					Console.WriteLine(err.ToString());
				}
			}
		}
#endif
	}
}
