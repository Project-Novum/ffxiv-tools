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
    private const string Lastbootversion = "2010.09.18.0000";
    private const string Lastgameversion = "2012.09.19.0001";
    private const string Boothash = "2d2a390f";
    private const string Gamehash = "48eca647";
    private const string Header = "477D80B1_38BC_41d4_8B48_5273ADB89CAC";
    
    private readonly ILogger<PatchController> _logger;
    private readonly string _patchData;
    

    public PatchController(ILogger<PatchController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _patchData = configuration.GetValue("PatchData", "./");
    }
    
    [HttpGet("{type}/{currentVersion}")]
    public async Task GetVersionCheck(string type, string currentVersion)
    {
        if (string.Equals(type, "boot",StringComparison.OrdinalIgnoreCase))
        {
            await BootVersionCheck(currentVersion.TrimEnd());
        }
        else
        {
            await GameVersionCheck(currentVersion.TrimEnd());
        }
    }

    private async Task BootVersionCheck(string currentVersion)
    {
        bool update = !string.Equals(currentVersion, Lastbootversion);

        
        // Update Boot.exe 
        SetHeaders(update,"boot",Lastbootversion);
        if (!update)
        {
            return;
        }

        await SetBody("boot", currentVersion, UpdateMatrix.Bootversion,Lastbootversion); 
    }

    private  async Task  GameVersionCheck(string currentVersion)
    {
        bool update = !string.Equals(currentVersion, Lastgameversion);

        SetHeaders(update,"game",Lastgameversion);

        if (!update)
        {
            return;
        }
        
        await SetBody("game", currentVersion, UpdateMatrix.Gameversion,Lastgameversion);
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

    private async Task<bool> SetBody(string type, string currentVersion, Dictionary<string, string> versions,string latestVersion)
    {
        string typeHash =  GetTypeHash(type);
        using StreamWriter writer = new (Response.Body, leaveOpen: true);
        bool isUpToDate = false;

        do
        {
            string versionToUpdateTo = versions[currentVersion];
            
            string path = Path.Join(_patchData, typeHash, "metainfo", $"D{versionToUpdateTo}.torrent");
            using FileStream reader = System.IO.File.OpenRead(path);
            
            Torrent? f = await Torrent.LoadAsync(reader);
            
            reader.Seek(0, SeekOrigin.Begin);
            
            await writer.WriteAsync($"--{Header}\r\n");
            await writer.WriteAsync("Content-Type: application/octet-stream\r\n");
            await writer.WriteAsync($"Content-Location: ffxiv/{type}/metainfo/D{versionToUpdateTo}.torrent\r\n");
            await writer.WriteAsync($"X-Patch-Length: {f.Files.First().Length}\r\n");
            await writer.WriteAsync(
                "X-Signature: jqxmt9WQH1aXptNju6CmCdztFdaKbyOAVjdGw_DJvRiBJhnQL6UlDUcqxg2DeiIKhVzkjUm3hFXOVUFjygxCoPUmCwnbCaryNqVk_oTk_aZE4HGWNOEcAdBwf0Gb2SzwAtk69zs_5dLAtZ0mPpMuxWJiaNSvWjEmQ925BFwd7Vk=\r\n");
            await writer.WriteAsync("\r\n");
            await writer.FlushAsync();
            await reader.CopyToAsync(writer.BaseStream);

            if (String.Equals(versionToUpdateTo, latestVersion))
            {
                isUpToDate = true;
            }
            else
            {
                currentVersion = versionToUpdateTo;
            }

        } while (!isUpToDate);
        
        await writer.WriteAsync($"\r\n--{Header}--\r\n\r\n");

        return true;
    }

    private string GetTypeHash(string type)
    {
        if (String.Equals(type, "boot",StringComparison.OrdinalIgnoreCase))
        {
            return Boothash;
        }
        else
        {
            return Gamehash;
        }
    }
}