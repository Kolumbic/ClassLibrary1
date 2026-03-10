using NXOpen;
using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.IO;

public class ToolExtractor
{
    public static void Main()
    {
        ToolExtractor t = new ToolExtractor();
        t.Run();
    }

    public void Run()
    {
        Session theSession = Session.GetSession();
        Part workPart = theSession.Parts.Work;

        if (workPart == null)
            return;

        CAMSetup setup = workPart.CAMSetup;

        if (setup == null)
            return;

        List<ToolInfo> tools = new List<ToolInfo>();

        // корневая группа
        NCGroup root = setup.RootGroup;

        Traverse(root, tools);

        SaveCSV(tools);
    }

    void Traverse(NCGroup group, List<ToolInfo> tools)
    {
        CAMObject[] members = group.GetMembers();

        foreach (CAMObject obj in members)
        {
            if (obj is Tool)
            {
                Tool tool = (Tool)obj;

                ToolInfo info = new ToolInfo();
                info.Name = tool.Name;

                try
                {
                    info.ToolNumber = tool.GetIntegerValue("tool_number");
                }
                catch { }

                try
                {
                    info.Diameter = tool.GetDoubleValue("tool_diameter");
                }
                catch { }

                try
                {
                    info.Length = tool.GetDoubleValue("tool_length");
                }
                catch { }

                tools.Add(info);
            }

            if (obj is NCGroup)
            {
                Traverse((NCGroup)obj, tools);
            }
        }
    }

    void SaveCSV(List<ToolInfo> tools)
    {
        string path = @"C:\Temp\ToolMap.csv";

        Directory.CreateDirectory(@"C:\Temp");

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.WriteLine("Tool;Number;Diameter;Length");

            foreach (var t in tools)
            {
                sw.WriteLine($"{t.Name};{t.ToolNumber};{t.Diameter};{t.Length}");
            }
        }
    }
}

public class ToolInfo
{
    public string Name = "";
    public int ToolNumber;
    public double Diameter;
    public double Length;
}