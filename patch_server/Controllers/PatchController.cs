using System.Text;
using Microsoft.AspNetCore.Mvc;
using System;
using MonoTorrent;
using Patch_Server.Constants;

namespace Patch_Server.Controllers;

[ApiController]
[Route("/patch/vercheck/ffxiv/win32/release")]
public class PatchController : Controller
{
    private const string LASTBOOTVERSION = "2010.09.18.0000";
    private const string LASTGAMEVERSION = "2012.09.19.0001";
    private const string BOOTHASH = "2d2a390f";
    private const string GAMEHASH = "48eca647";
    private readonly ILogger<PatchController> _logger;
    private readonly string _patchData;

    public PatchController(ILogger<PatchController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _patchData = configuration.GetValue("PatchData", "./");
    }
    
    [HttpGet("{type}/{version}")]
    public async Task GetTest(string type, string version)
    {
        if (string.Equals(type, "boot",StringComparison.OrdinalIgnoreCase))
        {
            await BootVersionCheck(version);
        }
        else
        {
            await GameVersionCheck(version);
        }
    }

    private async Task BootVersionCheck(string version)
    {
        bool update = !string.Equals(version.TrimEnd(), LASTBOOTVERSION);

        
        // Update Boot.exe 
        SetHeaders(update,"boot",LASTBOOTVERSION);
        if (!update)
        {
            return;
        }

        await SetBody("boot", version, UpdateMatrix.Bootversion); 
    }

    private  async Task  GameVersionCheck(string version)
    {
        bool update = !string.Equals(version.TrimEnd(), LASTGAMEVERSION);

        SetHeaders(update,"game",LASTGAMEVERSION);

        if (!update)
        {
            return;
        }
        
        await SetBody("game", version, UpdateMatrix.Gameversion);
    }

    private void SetHeaders(bool isUpdate, string type, string latestVersion)
    {
        var typeHash = GetTypeHash(type);
        var headerDictionary = new HeaderDictionary();
        
        headerDictionary.Add("Content-Location",$"ffxiv/{typeHash}/vercheck.dat");
        if (isUpdate)
        {
            headerDictionary.Add("Content-Type","multipart/mixed; boundary=477D80B1_38BC_41d4_8B48_5273ADB89CAC");
        }

        headerDictionary.Add("X-Repository","ffxiv/win32/release/boot");
        headerDictionary.Add("X-Patch-Module","ZiPatch");
        headerDictionary.Add("X-Protocol","torrent");
        headerDictionary.Add("X-Info-Url","http://example.com");
        headerDictionary.Add("X-Latest-Version",$"{latestVersion}");
        if (isUpdate)
        {
            headerDictionary.Add("Connection","keep-alive");
        }

        foreach (var VARIABLE in headerDictionary)
        {
            Response.Headers.Add(VARIABLE);
        }

        if (isUpdate)
        {
            Response.StatusCode = 200;
        }
        else
        {
            Response.StatusCode = 204;
        }

    }

    private async Task<bool> SetBody(string type, string version, Dictionary<string, string> versions)
    {
        var typeHash =  GetTypeHash(type);
        var versionVals = versions.Values.ToArray();
        var idx = Array.IndexOf(versionVals, version);
        if (idx <= -1) return false;
        
        var vals = versionVals.Skip(idx + 1);

        using var writer = new StreamWriter(Response.Body, leaveOpen: true);
        var header = "477D80B1_38BC_41d4_8B48_5273ADB89CAC";
        foreach (var update in vals)
        {
            var path = Path.Join(_patchData, typeHash, "metainfo", $"D{update}.torrent");
            using var reader = System.IO.File.OpenRead(path);
            var f = await Torrent.LoadAsync(reader);
            reader.Seek(0, SeekOrigin.Begin);
            
            await writer.WriteAsync($"--{header}\r\n");
            await writer.WriteAsync("Content-Type: application/octet-stream\r\n");
            await writer.WriteAsync($"Content-Location: ffxiv/{type}/metainfo/D{update}.torrent\r\n");
            await writer.WriteAsync($"X-Patch-Length: {f.Files.First().Length}\r\n");
            await writer.WriteAsync(
                "X-Signature: jqxmt9WQH1aXptNju6CmCdztFdaKbyOAVjdGw_DJvRiBJhnQL6UlDUcqxg2DeiIKhVzkjUm3hFXOVUFjygxCoPUmCwnbCaryNqVk_oTk_aZE4HGWNOEcAdBwf0Gb2SzwAtk69zs_5dLAtZ0mPpMuxWJiaNSvWjEmQ925BFwd7Vk=\r\n");
            await writer.WriteAsync("\r\n");
            await writer.FlushAsync();
            await reader.CopyToAsync(writer.BaseStream);
        }
        await writer.WriteAsync($"\r\n--{header}--\r\n\r\n");

        return true;
    }

    private string GetTypeHash(string type)
    {
        if (String.Equals(type, "boot",StringComparison.OrdinalIgnoreCase))
        {
            return BOOTHASH;
        }
        else
        {
            return GAMEHASH;
        }
    }
}