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
    public async void  GetTest(string type, string version)
    {
        byte[]? response;
        if (String.Equals(type, "boot",StringComparison.OrdinalIgnoreCase))
        {
            response = BootVersionCheck(version);
        }
        else
        {
            response = GameVersionCheck(version);
        }
        

        if (response!=null)
            await Response.Body.WriteAsync(response);
        else
            NotFound();
    }

    private byte[]? BootVersionCheck(string version)
    {
        bool update = !string.Equals(version.TrimEnd(), LASTBOOTVERSION);

        
        // Update Boot.exe 
        SetHeaders(update,"boot",LASTBOOTVERSION);
        if (!update)
        {
            return null;
        }

        if (!UpdateMatrix.Bootversion.ContainsKey(version.TrimEnd()))
        {
            return null;
        }
        
        return SetBody("boot", UpdateMatrix.Bootversion[version.TrimEnd()]); 
    }

    private byte[]? GameVersionCheck(string version)
    {
        bool update = !string.Equals(version.TrimEnd(), LASTGAMEVERSION);

        SetHeaders(update,"game",LASTGAMEVERSION);

        if (!update)
        {
            return null;
        }

        if (!UpdateMatrix.Gameversion.ContainsKey(version.TrimEnd()))
        {
            return null;
        }

        
        return SetBody("game", UpdateMatrix.Gameversion[version.TrimEnd()]); 
            
    }

    private void SetHeaders(bool isUpdate,string type,string latestVersion)
    {
        string typeHash = GetTypeHash(type);
        
        
        
        IHeaderDictionary headerDictionary = new HeaderDictionary();
        
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

    private byte[] SetBody(string type, string versionToUpdate)
    {        
        string typeHash =  GetTypeHash(type) ;

        var path = Path.Join(_patchData, typeHash, "metainfo", $"D{versionToUpdate}.torrent");
        
        byte[] torrentFile = System.IO.File.ReadAllBytes(path);

        var f = Torrent.Load(torrentFile);

        StringBuilder sb = new();
        sb.Append("--477D80B1_38BC_41d4_8B48_5273ADB89CAC\r\n");
        sb.Append("Content-Type: application/octet-stream\r\n");
        sb.Append($"Content-Location: ffxiv/{typeHash}/metainfo/D{versionToUpdate}.torrent\r\n");
        sb.Append($"X-Patch-Length: {f.Files.First().Length}\r\n");
        sb.Append(
            "X-Signature: jqxmt9WQH1aXptNju6CmCdztFdaKbyOAVjdGw_DJvRiBJhnQL6UlDUcqxg2DeiIKhVzkjUm3hFXOVUFjygxCoPUmCwnbCaryNqVk_oTk_aZE4HGWNOEcAdBwf0Gb2SzwAtk69zs_5dLAtZ0mPpMuxWJiaNSvWjEmQ925BFwd7Vk=\r\n");

        sb.Append("\r\n");
        
        byte[] bodyHead = Encoding.Default.GetBytes(sb.ToString());
        byte[] eof = Encoding.Default.GetBytes("\r\n--477D80B1_38BC_41d4_8B48_5273ADB89CAC--\r\n\r\n");
        
        
        
        byte[] final = new byte[bodyHead.Length + torrentFile.Length + eof.Length];
        Buffer.BlockCopy(bodyHead,0,final,0,bodyHead.Length);
        Buffer.BlockCopy(torrentFile,0,final,bodyHead.Length,torrentFile.Length);
        Buffer.BlockCopy(eof,0,final,bodyHead.Length + torrentFile.Length,eof.Length);


        return final;
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