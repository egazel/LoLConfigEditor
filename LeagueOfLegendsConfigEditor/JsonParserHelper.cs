using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LeagueOfLegendsConfigEditor
{
    internal class JsonParserHelper
    {
        internal static string PersistedSettingsPth = MainWindow.cfgFolderPth + "PersistedSettings.json";

        internal static void CrawlJson(System.Windows.Controls.TreeView tv)
        {
            string strBuf = File.ReadAllText(PersistedSettingsPth);
            JToken node = JToken.Parse(strBuf);

            tv.Items.Clear();

            TreeViewItem rootTVItem = new TreeViewItem();
            rootTVItem.Header = "PersistedSettings.json";
            tv.Items.Add(rootTVItem);
            RecursiveGenerateTreeView(node,rootTVItem);
        }

        static void RecursiveGenerateTreeView(JToken node, TreeViewItem parentItem)
        {
            if (node.Type == JTokenType.Object)
            {
                TreeViewItem newItem = new TreeViewItem();
                if (node["name"] != null)
                {
                    newItem.Header = node["name"];
                    if (node["value"]  != null)
                    {
                        TreeViewItem valItem = new TreeViewItem();
                        valItem.Header = node["value"];
                        newItem.Items.Add(valItem);
                    }
                    parentItem.Items.Add(newItem);
                    parentItem = newItem;
                }
                foreach (JProperty child in node.Children<JProperty>())
                {
                    RecursiveGenerateTreeView(child.Value, parentItem);
                }
            }
            else if (node.Type == JTokenType.Array)
            {
                TreeViewItem newItem = new TreeViewItem();
                newItem.Header = (node.Path).Split(".")[(node.Path).Split(".").Length - 1];
                parentItem.Items.Add(newItem);
                foreach (JToken child in node.Children())
                {
                    RecursiveGenerateTreeView(child, newItem);
                }
            }
        }
    }
}
