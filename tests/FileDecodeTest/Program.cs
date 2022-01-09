// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text;
using FFXIVDataHandler;
using FFXIVDataHandler.IO;

var script = "7vxx9w6658p335s65o.le.lpb";

var path = $"D:\\games\\ffxiv\\SquareEnix\\FINAL FANTASY XIV\\client\\script";

/*
foreach (var file in Directory.EnumerateFiles(path, "*.lpb", SearchOption.AllDirectories))
{
    using var lpd = new Lpb(file);

    // This strips the RLE (lpd) header on save
    await lpd.SaveAsync(file + ".luac");

    Console.WriteLine($"Exported: {file + ".luac"}");
}*/

var items = new List<string>
{
    "729s9", "uy9l5s89r57y9rr", "rlrq5x"
};

var buildTable = new List<Sub>();

var knownNames = new Dictionary<string, string>
{
    { "Chara", "729s9" },
    { "PlayerBaseClass", "uy9l5s89r57y9rr"},
    { "System", "rlrq5x"},
    { "Debug", "658p3"},
    { "Debug_utility", "658p3_pq1y1ql" },
    { "Judge", "0p635" },
    { "JudgeBaseClass", "0p63589r57y9rr" },
    { "ChocoboJudge", "72v7v8v0p635"},
    { "CommonJudge", "7vxxvw0p635"},
    { "HarvestDirector", "29so5rq61s57qvs"},
    { "HalloweenDirector", "29yyvn55w61s57qvs"},
    { "AfterQuestWarpDirector", "94q5stp5rqn9su61s57qvs"},
    { "desktopwidget", "65rzqvun1635q"},
    { "ChigoeEventSummer2011", "7213v55o5wqrpxx5shjii"},
    { "zonemasterbattlewilw0", "kvw5x9rq5s89qqy5n1ynj"}
};

foreach (var known in knownNames)
{
    for (var i = 0; i < known.Key.Length; i++)
    {
        var normal = known.Key.ToLower()[i];

        if(normal == '_') continue;

        if (buildTable.All(x => x.Letter != normal))
        {
            buildTable.Add(new Sub(normal, known.Value[i]));
        }
    }
}

buildTable = buildTable.OrderBy(x => x.Letter).ToList();

var ss = new StringBuilder();
foreach (var item in buildTable)
{
    ss = ss.AppendLine($"{item.Letter}:{item.Replace}");
}

File.WriteAllText("mapped.txt", ss.ToString());
Process.Start("notepad.exe", "mapped.txt");

var outputFolder = "d:\\dev\\ffxiv-tools\\scripts";

var sb = new StringBuilder();
foreach (var item in Directory.EnumerateFiles(path, "*.luac", SearchOption.AllDirectories))
{
    var p = item.Replace(path, "").ToLower().Split(".").First();

    var str = "";
    foreach (var s in p)
    {
        var s1 = s;
        var replace = buildTable.FirstOrDefault(x => x.Replace == s1);
        str += replace?.Letter ?? s;
    }

    str += ".le";

    var fi = new FileInfo(Path.Join(outputFolder, str + ".luac"));
    fi.Directory.Create();
    File.WriteAllBytes(fi.FullName, File.ReadAllBytes(item));

    ProcessStartInfo start = new ProcessStartInfo();
    start.FileName = "D:\\dev\\ffxiv-tools\\luadec\\vcproj-5.1\\Debug\\bin\\luadec.exe";
    start.Arguments = $"-s {fi.FullName}";
    start.RedirectStandardOutput = true;
    using (Process process = Process.Start(start))
    {
        //
        // Read in all the text from the process with the StreamReader.
        //
        using (StreamReader reader = process.StandardOutput)
        {
            string result = reader.ReadToEnd();
            File.WriteAllText(fi.FullName.Replace(".luac", ".lua"), result);
        }
    }

    start = new ProcessStartInfo();
    start.FileName = "D:\\dev\\ffxiv-tools\\luadec\\vcproj-5.1\\Debug\\bin\\luadec.exe";
    start.Arguments = $"-dis {fi.FullName}";
    start.RedirectStandardOutput = true;
    using (Process process = Process.Start(start))
    {
        //
        // Read in all the text from the process with the StreamReader.
        //
        using (StreamReader reader = process.StandardOutput)
        {
            string result = reader.ReadToEnd();
            File.WriteAllText(fi.FullName.Replace(".luac", ".dis"), result);
        }
    }

    fi.Delete();

    sb = sb.AppendLine($"{p} => {str}");
}
File.WriteAllText("out.txt", sb.ToString());
Process.Start("notepad.exe", "out.txt");
class Sub
{
    public char Letter { get; set; }
    public char Replace { get; set; }

    public Sub(char letter, char replace)
    {
        Letter = letter;
        Replace = replace;
    }
}