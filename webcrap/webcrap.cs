
namespace webcrap
{
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;

  using Thrift;
  using Thrift.Protocol;
  using Thrift.Collections;
  using Thrift.Transport;
  using Thrift.Net;

  using webcrap.web;
  using Bench;

  public class Program : CommandLine
  {
    [CommandLineSwitchParameter(ShortName = "dh")]
    public bool DumpHeader { get; set; }

    [CommandLineSwitchParameter(ShortName = "dc")]
    public bool DumpCookies { get; set; }

    [CommandLineSwitchParameter(ShortName = "pc")]
    public bool PrintContent { get; set; }

    [CommandLineParameter(ShortName = "sc")]
    public string SaveContent { get; set; }

    [CommandLineParameter]
    public string Charset { get; set; } = "utf-8";

    [CommandLineParameter]
    public string Match { get; set; }

    public static void Main(string[] args)
    {
      Program prog = new Program { InputArgs = args };
      prog.Run();
    }

    public void Run()
    {
      CookieContainer cookies = new CookieContainer();
      HttpWebRequest request = WebRequest.Create(this.Args[0]) as HttpWebRequest;
      request.CookieContainer = cookies;
      HttpWebResponse response = request.GetResponseAsync().Result as HttpWebResponse;

      if (this.DumpHeader)
      {
        foreach (string header in response.Headers.AllKeys)
        {
          Console.WriteLine("{0}:{1}", header, response.Headers[header]);
        }
      }

      if (this.DumpCookies)
      {
        Dump(response.ResponseUri.AbsolutePath, response.Cookies);
        Dump2(response.ResponseUri.AbsolutePath, response.Cookies);        
      }

      if (this.PrintContent)
      {
        string html = GetHtml(response);
        Console.WriteLine(html);
      }

      if (this.SaveContent != null)
      {
        byte[] content = GetContent(response);
        File.WriteAllBytes(this.SaveContent, content);
      }

      if (this.Match != null)
      {
        Regex regex = new Regex(this.Match, RegexOptions.Compiled|RegexOptions.Singleline);
        string html = GetHtml(response);
        MatchCollection matches = regex.Matches(html);
        foreach (Match match in matches)
        {
        //   Console.WriteLine(match.ToString());
        //   foreach (Group g in match.Groups)
          for(int i = 1; i < match.Groups.Count; i++)
          {
              Group g = match.Groups[i];
            // CaptureCollection
            Console.WriteLine(
                "g({0}):{1}",
            g.Name,
            string.Join("|||", g.Captures.Cast<Capture>().Select(c => c.Value)));
          }
          Console.WriteLine();
        }
      }
    }

    private string GetHtml(HttpWebResponse response)
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      Encoding enc = Encoding.GetEncoding(this.Charset);
      byte[] content = GetContent(response);
      string html = enc.GetString(content);
      return html;
    }

    private byte[] GetContent(HttpWebResponse response)
    {
      var ms = new MemoryStream();
      var rs = response.GetResponseStream();
      rs.CopyTo(ms);
      return ms.ToArray();
    }

    [Proto]
    public class PCookie
    {
      [ProtoColumn(1)] public string Name { get; set; }
      [ProtoColumn(2)] public string Value { get; set; }
      [ProtoColumn(3)] public int Version { get; set; }
      [ProtoColumn(4)] public string Comment { get; set; }
      [ProtoColumn(5)] public string CommentUri { get; set; }
      [ProtoColumn(6)] public string Domain { get; set; }
      [ProtoColumn(7)] public string Port { get; set; }
      [ProtoColumn(8)] public string Path { get; set; }
      [ProtoColumn(9)] public bool Expired { get; set; }
      [ProtoColumn(10)] public DateTime Expires { get; set; }
      [ProtoColumn(11)] public bool HttpOnly { get; set; }
      [ProtoColumn(12)] public bool Secure { get; set; }
      [ProtoColumn(13)] public DateTime Timestamp { get; set; }
    }

    [Proto]
    public class PCookies
    {
      [ProtoColumn(1)]
      public Dictionary<string, List<PCookie>> Cookies { get; set; }
    }

    static void Dump(string url, CookieCollection cookies)
    {
      List<PCookie> list = new List<PCookie>();
      foreach (System.Net.Cookie cookie in cookies)
      {
        PCookie c = new PCookie
        {
          Name = cookie.Name,
          Value = cookie.Value,
          Version = cookie.Version,
          Comment = cookie.Comment,
          CommentUri = cookie.CommentUri?.AbsolutePath,
          Domain = cookie.Domain,
          Port = cookie.Port,
          Path = cookie.Path,          
          Expired = cookie.Expired,
          Expires = cookie.Expires,
          HttpOnly = cookie.HttpOnly,
          Secure = cookie.Secure,
          Timestamp = cookie.TimeStamp
        };
        list.Add(c);
      }
      var dict = new Dictionary<string, List<PCookie>>();
      dict.Add(url, list);

      PCookies cc = new PCookies
      {
        Cookies = dict
      };

      
      string json = Proto<PCookies>.GetJson(cc);
      Console.WriteLine(json);
      Console.WriteLine();
      Console.WriteLine(Proto<PCookies>.GetJson(Proto<PCookies>.FromJson(json)));
      Console.WriteLine();
    }

    static void Dump2(string url, CookieCollection cookies)
    {
      List<webcrap.web.Cookie> list = new List<webcrap.web.Cookie>();
      foreach (System.Net.Cookie cookie in cookies)
      {
        webcrap.web.Cookie c = new webcrap.web.Cookie
        {
          Name = cookie.Name,
          Value = cookie.Value,
          Version = cookie.Version,
          Path = cookie.Path,
          Domain = cookie.Domain,
          Port = cookie.Port,
          Comment = cookie.Comment,
          CommentUri = cookie.CommentUri?.AbsolutePath,
          Expired = cookie.Expired,
          Expires = cookie.Expires.ToUniversalTime().Ticks,
          HttpOnly = cookie.HttpOnly,
          Secure = cookie.Secure,
          Timestamp = cookie.TimeStamp.ToUniversalTime().Ticks
        };
        list.Add(c);
      }
      var dict = new Dictionary<string, List<webcrap.web.Cookie>>();
      dict.Add(url, list);

      Cookies cc = new Cookies
      {
        Cookies_ = dict
      };

      TByteBuffer trans = new TByteBuffer(102400);
      TProtocol prot = new TSimpleJSONProtocol(trans);
      cc.Write(prot);
      string json = Encoding.UTF8.GetString(trans.GetBuffer(), 0, trans.Length);
      Console.WriteLine(json);
      Console.WriteLine();


      TMemoryBuffer trans1 = new TMemoryBuffer(Encoding.UTF8.GetBytes(json));
      TProtocol prot1 = new TSimpleJSONProtocol(trans1);

      Cookies cs = new Cookies();
      cs.Read(prot1);

      TByteBuffer trans2 = new TByteBuffer(102400);
      TProtocol prot2 = new TSimpleJSONProtocol(trans2);
      cs.Write(prot2);
      string json2 = Encoding.UTF8.GetString(trans2.GetBuffer(), 0, trans2.Length);
      Console.WriteLine(json2);
      Console.WriteLine();
      


    }
  }
}
