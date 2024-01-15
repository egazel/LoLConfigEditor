using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueOfLegendsConfigEditor
{
    internal class CFGHelper
    {
        // TODO custom popup w/ field for user to select the game folder path if different
        internal static OrderedDictionary gameCfgBreakout = new OrderedDictionary();
        internal static string gameCfgPth = MainWindow.cfgFolderPth + "game.cfg";

        protected internal static void ParseGameCfgToDict()
        {
            gameCfgBreakout.Clear();
            StreamReader sr = new StreamReader(gameCfgPth);
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                int idx = 0;
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] splitLine = line.Split("=");
                    if (splitLine.Length > 1)
                    {
                        gameCfgBreakout.Add(splitLine[0], splitLine[1]);
                    }
                    else
                    {
                        gameCfgBreakout.Add(splitLine[0], "");
                    }
                    ++idx;
                }
            }
            sr.Close();
        }

        protected internal static void SaveGameCfg()
        {
            FileInfo fileInfo = new FileInfo(gameCfgPth);
            fileInfo.IsReadOnly = false;
            File.WriteAllText(gameCfgPth, String.Empty);

            using (StreamWriter outputFile = new StreamWriter(gameCfgPth))
            {
                foreach (DictionaryEntry de in gameCfgBreakout)
                {
                    // Clean "_" used in place of spaces in Names to fit the keys in the cfg
                    string cleanKeyStr = (de.Key.ToString()).Replace("_", " ");
                    string valStr = de.Value.ToString();
                    if (cleanKeyStr.Contains("["))
                    {
                        outputFile.WriteLine($"\n{cleanKeyStr}");
                    }
                    else
                    {
                        outputFile.WriteLine($"{cleanKeyStr}={valStr}");
                    }
                }
            }
            fileInfo.IsReadOnly = true;
        }
    }
}
