
namespace Bench
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Text.RegularExpressions;

  /// <summary>
  /// 
  /// </summary>
  public class CommandModuleAttribute : Attribute
  {
    /// <summary>
    /// 
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string HelpText { get; set; }
  }

  public class CommandLineParameterAttribute : Attribute
  {
    /// <summary>
    /// 
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    virtual public bool HasValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string HelpText { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public CommandLineParameterAttribute()
    {
      this.HasValue = true;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CommandLineSwitchParameterAttribute : CommandLineParameterAttribute
  {
    /// <summary>
    /// 
    /// </summary>
    public override bool HasValue
    {
      get { return false; }
      set { }
    }

    /// <summary>
    /// 
    /// </summary>
    public CommandLineSwitchParameterAttribute()
    {
      this.HasValue = false;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class CommandLine
  {
    protected virtual string[] Prefixes
    {
      get { return new string[] { "/", "-" }; }
    }

    protected virtual string Splitter
    {
      get { return ":"; }
    }

    /// <summary>
    /// 
    /// </summary>
    private string[] inputArgs;

    /// <summary>
    /// 
    /// </summary>
    public CommandLine()
    {
      this.inputArgs = new string[0];
      this.Args = new string[0];
    }

    /// <summary>
    /// 
    /// </summary>
    public string[] InputArgs
    {
      get
      {
        return this.inputArgs;
      }

      set
      {
        string[] nonParsed = Parse(this, value);
        if (this.Debug)
        {
          System.Diagnostics.Debugger.Break();
        }
        else if (this.Attach)
        {
          System.Console.Write("Please attach to process {0} and press Enter", System.Diagnostics.Process.GetCurrentProcess().Id);
          Console.ReadLine();
        }
        else if (this.Help)
        {
          this.PrintUsage();
          Environment.Exit(0);
        }

        this.inputArgs = value;
        this.Args = nonParsed;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    [CommandLineParameter(HasValue = false, HelpText = "Break into debugger")]
    public bool Debug { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CommandLineParameter(HasValue = false, HelpText = "Attach into debugger")]
    public bool Attach { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [CommandLineParameter(ShortName = "?|h", HasValue = false, HelpText = "Break into debugger")]
    public bool Help { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CommandModule { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual string HelpText { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string[] Args { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string FirstArg
    {
      get { return this.Args.Length > 0 ? this.Args[0] : null; }
    }

    /// <summary>
    /// 
    /// </summary>
    public string SecondArg
    {
      get { return this.Args.Length > 1 ? this.Args[1] : null; }
    }

    public static void RunModule(string[] args, string method = "Run")
    {
      if (args.Length > 0)
      {
        Assembly assembly = Assembly.GetEntryAssembly();
        foreach (var type in assembly.GetTypes())
        {
          var ca = type.GetCustomAttribute<CommandModuleAttribute>(false);
          if (ca != null)
          {
            if (string.Compare(ca.ShortName, args[0], true) == 0
              || string.Compare(type.Name, args[0], true) == 0)
            {
              var mi = type.GetMethod(method);
              if (mi.IsStatic)
              {
                mi.Invoke(null, new object[] { args.Skip(1).ToArray() });
              }
              else
              {
                CommandLine obj = (CommandLine)Activator.CreateInstance(type);
                obj.InputArgs = args.Skip(1).ToArray();
                mi.Invoke(obj, new object[] { });
              }
              return;
            }
          }
        }
      }

      PrintModuleUsage();
    }

    /// <summary>
    ///
    /// </summary>
    public static void PrintModuleUsage()
    {
      Console.WriteLine("Commands:");
      Assembly assembly = Assembly.GetEntryAssembly();
      foreach (var type in assembly.GetTypes())
      {
        var ca = type.GetCustomAttribute<CommandModuleAttribute>(false);
        if (ca != null)
        {
          Console.Write("{0}", type.Name);
          if (!string.IsNullOrEmpty(ca.ShortName))
          {
            Console.Write(",{0}", ca.ShortName);
          }

          Console.WriteLine();
          if (!string.IsNullOrEmpty(ca.HelpText))
          {
            Console.WriteLine("\t{0}", ca.HelpText);
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    private string[] Parse(object cmd, string[] args)
    {
      args = args ?? Array<string>.Empty;
      var stringComparer = StringComparer.OrdinalIgnoreCase;
      var actions = new Dictionary<string, Action<object>>(stringComparer);
      var types = new Dictionary<string, Type>(stringComparer);
      var cas = new Dictionary<string, CommandLineParameterAttribute>(stringComparer);

      foreach (var pi in cmd.GetType().GetProperties())
      {
        var prop = pi;
        var ca = prop.GetCustomAttribute<CommandLineParameterAttribute>(false);
        if (ca != null)
        {
          actions[prop.Name] = delegate (object value)
          {
            prop.SetValue(cmd, value, null);
          };

          types[prop.Name] = prop.PropertyType;
          cas[prop.Name] = ca;

          if (!string.IsNullOrEmpty(ca.ShortName))
          {
            var shortNames = ca.ShortName.Split(
                new string[] { "|", "," },
                StringSplitOptions.RemoveEmptyEntries);
            foreach (var shortName in shortNames)
            {
              actions[shortName] = delegate (object value)
              {
                prop.SetValue(cmd, value, null);
              };
              types[shortName] = prop.PropertyType;
              cas[shortName] = ca;
            }
          }
        }
      }

      foreach (var fi in cmd.GetType().GetFields())
      {
        var field = fi;
        var ca = field.GetCustomAttribute<CommandLineParameterAttribute>(false);
        if (ca != null)
        {
          actions[field.Name] = delegate (object value)
          {
            field.SetValue(cmd, value);
          };

          types[field.Name] = field.FieldType;
          cas[field.Name] = ca;

          if (!string.IsNullOrEmpty(ca.ShortName))
          {
            var shortNames = ca.ShortName.Split(
              new string[] { "|", "," },
              StringSplitOptions.RemoveEmptyEntries);

            foreach (var shortName in shortNames)
            {
              actions[shortName] = delegate (object value)
              {
                field.SetValue(cmd, value);
              };
              types[shortName] = field.FieldType;
              cas[shortName] = ca;
            }
          }
        }
      }

      var nonParsed = new List<string>();

      for (int aIndex = 0; aIndex < args.Length; aIndex++)
      {
        var arg = args[aIndex];
        if (string.IsNullOrEmpty(arg))
        {
          nonParsed.Add(arg);
          continue;
        }

        bool found = false;
        foreach (string prefix in this.Prefixes)
        {
          if (!arg.StartsWith(prefix))
          {
            continue;
          }

          int index = arg.IndexOf(this.Splitter);
          if (index < 0)
          {
            index = arg.Length;
          }

          string parameter = arg.Substring(
              prefix.Length,
              index - prefix.Length);

          object parameterValue = null;
          CommandLineParameterAttribute ca = null;

          if (cas.TryGetValue(parameter, out ca))
          {
            found = true;
            var action = actions[parameter];
            var valueType = types[parameter];

            if (index < arg.Length)
            {
              var argValue = arg.Substring(index + 1);
              parameterValue = ConvertValue(valueType, argValue);
            }
            else if (ca.HasValue)
            {
              if (aIndex < args.Length - 1)
              {
                aIndex++;
                parameterValue = ConvertValue(valueType, args[aIndex]);
              }
            }
            else
            {
              parameterValue = true;
            }

            action(parameterValue);

          }

          break;
        }

        if (!found)
        {
          nonParsed.Add(arg);
        }
      }

      return nonParsed.ToArray();
    }

    /// <summary>
    /// Prints the usage.
    /// </summary>
    public void PrintUsage()
    {
      var stringComparer = StringComparer.OrdinalIgnoreCase;
      var cas = new Dictionary<string, CommandLineParameterAttribute>(stringComparer);
      var cat = new Dictionary<string, Type>(stringComparer);

      foreach (var pi in this.GetType().GetProperties())
      {
        if (pi.DeclaringType == typeof(CommandLine))
        {
          continue;
        }

        var prop = pi;
        var calist = prop.GetCustomAttributes<CommandLineParameterAttribute>(false);
        var ca = calist.FirstOrDefault();
        if (ca != null)
        {
          cas[prop.Name] = ca;
          cat[prop.Name] = prop.PropertyType;
        }
      }

      foreach (var fi in this.GetType().GetFields())
      {
        var field = fi;
        var calist = field.GetCustomAttributes<CommandLineParameterAttribute>(false);
        var ca = calist.FirstOrDefault();
        if (ca != null)
        {
          cas[field.Name] = ca;
          cat[field.Name] = field.FieldType;
        }
      }

      var assembly = System.Reflection.Assembly.GetEntryAssembly();
      if (!string.IsNullOrEmpty(this.CommandModule))
      {
        System.Console.WriteLine("Usage: {0} {1} <options>", this.CommandModule, assembly.ManifestModule.Name);
      }
      else
      {
        System.Console.WriteLine("Usage: {0} <options>", assembly.ManifestModule.Name);
      }

      System.Console.WriteLine();

      Console.WriteLine("Options:");

      foreach (var para in cas.Keys)
      {
        var ca = cas[para];
        var ct = cat[para];

        System.Console.Write("-{0}", para.ToLower());
        if (ca.HasValue)
        {
          System.Console.Write(" <{0}>", ct.FullName);
        }

        if (!string.IsNullOrEmpty(ca.ShortName))
        {
          var shortNames = ca.ShortName.Split(new string[] { "|", "," }, StringSplitOptions.RemoveEmptyEntries);
          foreach (var shortName in shortNames)
          {
            System.Console.Write(", -{0}", shortName.ToLower());
            if (ca.HasValue)
              System.Console.Write(" <value>");
          }
        }

        System.Console.WriteLine();

        if (!string.IsNullOrEmpty(ca.HelpText))
        {
          foreach (var line in ca.HelpText.Replace("\r", string.Empty).Split('\n'))
          {
            System.Console.WriteLine("    {0}", line);
          }
        }
      }

      if (!string.IsNullOrEmpty(this.HelpText))
      {
        System.Console.WriteLine();
        System.Console.WriteLine(this.HelpText);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static object ConvertValue(Type valueType, string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return null;
      }
      else if (valueType.GetTypeInfo().IsEnum)
      {
        return Enum.Parse(valueType, value);
      }
      else
      {
        return Convert.ChangeType(value, valueType, System.Globalization.CultureInfo.InvariantCulture);
      }
    }
  }
}
