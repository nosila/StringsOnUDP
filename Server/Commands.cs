using System;
using System.Collections.Generic;
using System.IO;

namespace Server
{
    /// <summary>
    /// Class used to parse and store commands from cmds.cfg file.
    /// </summary>
    class Commands
    {
        /// <summary>
        /// Used to store the commands after being parsed.
        /// </summary>
        public Dictionary<String, String> Table { get { return _table; } }
        private Dictionary<String, String> _table;

        public Commands()
        {
            _table = new Dictionary<string, string>();

            string[] txtLines = File.ReadAllLines(@"cmds.cfg");
            char[] separator = { ';' };
            string[] cmd;

            foreach (string txtLine in txtLines)
            {
                if (txtLine.Length > 2 && txtLine[0] != '/' && txtLine[1] != '/') //takes out comments
                {
                    cmd = txtLine.Split(separator, 2);
                    if (cmd.Length == 2)
                        _table.Add(cmd[0], cmd[1]);
                }
            }
#if DEBUG
            foreach (KeyValuePair<string, string> keyValuePair in _table)
            {
                Console.WriteLine(keyValuePair.ToString());
            }
#endif
        }
    }
}
