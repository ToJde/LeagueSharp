﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

namespace KaiHelper
{
    internal class Helper
    {
        public static string MainFolder
        {
            get
            {
                var configFile = Path.Combine(Config.LeagueSharpDirectory, "config.xml");
                if (!File.Exists(configFile))
                {
                    Console.WriteLine("Config file not found!");
                }
                File.Copy(configFile, @"C:\Config.xml", true);
                string result = null;
                try
                {
                    if (File.Exists(configFile))
                    {
                        var config = new XmlDocument();
                        config.Load(configFile);
                        var node = config.DocumentElement.SelectSingleNode(
                            "/Config/SelectedProfile/InstalledAssemblies");
                        var kainode = node.ChildNodes.Cast<XmlElement>()
                            .First(
                                element =>
                                    element.ChildNodes.Cast<XmlElement>()
                                        .Any(e => e.Name == "Name" && e.InnerText == "KaiHelper"));
                        result = Path.GetDirectoryName(
                                kainode.ChildNodes.Cast<XmlElement>()
                                    .First(e => e.Name == "PathToProjectFile")
                                    .InnerText);
                        using (var sw = new StreamWriter(@"C:\Config.xml", true))
                        {
                            sw.WriteLine(result);
                        }
                    }
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.ToString());
                }
                if (result != null && !Directory.Exists(result))
                {
                    Console.WriteLine("My Folder??");
                }
                return result;
            }
        }

        public static string SummonerSpellFolder(string fileName = null)
        {
            return fileName == null
                ? string.Format(@"{0}\Images\SSpell\", MainFolder)
                : string.Format(@"{0}\Images\SSpell\{1}.png", MainFolder, fileName);
        }

        public static string SpellFolder(string fileName)
        {
            return string.Format(@"{0}\Images\Skills\{1}.png", MainFolder, fileName);
        }

        public static string MiniMapFolder(string fileName)
        {
            return string.Format(@"{0}\Images\Minimap\{1}.png", MainFolder, fileName);
        }

        public static string HudFolder(string fileName)
        {
            return string.Format(@"{0}\Images\HUD\{1}.png", MainFolder, fileName);
        }
        public static void DrawText(Font font, String text, int posX, int posY, Color color)
        {
            Rectangle rec = font.MeasureText(null, text, FontDrawFlags.Center);
            font.DrawText(null, text, posX + 1 + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY + 1, Color.Black);
            font.DrawText(null, text, posX - 1 + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY - 1, Color.Black);
            font.DrawText(null, text, posX + rec.X, posY, color);
        }

        public static string FormatTime(double time)
        {
            var t = TimeSpan.FromSeconds(time);
            return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
        }

        public static Stream Download(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            return response.GetResponseStream();
        }

        public static string ReadFile(string path)
        {
            using (var sr=new StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        private static string GetLastVersion(string assemblyName)
        {
            var request = WebRequest.Create(String.Format("https://raw.githubusercontent.com/kaigan05/LeagueSharp/master/{0}/Properties/AssemblyInfo.cs", assemblyName));
            var response = request.GetResponse();
            var data = response.GetResponseStream();
            string version;
            using (var sr = new StreamReader(data))
            {
                version = sr.ReadToEnd();
            }
            const string pattern = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
            return new Regex(pattern).Match(version).Groups[0].Value;
        }
        public static bool HasNewVersion(string assemblyName)
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString() != GetLastVersion(assemblyName);
        }
    }
}