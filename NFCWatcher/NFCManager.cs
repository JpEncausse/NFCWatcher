using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Diagnostics;
using NDesk.Options;
using System.Collections.Generic;
using System.Text;

namespace net.encausse.nfc {

  class NFCManager {

    // ------------------------------------------
    //  MAIN
    // ------------------------------------------

    [STAThread]
    static void Main(string[] args) {

      // ------------------------------------------
      //  OPTIONS
      // ------------------------------------------

      bool help = false;
      String language = "fr-FR";

      var p = new OptionSet() {
        { "l|lang=", "the {LANGUAGE} Culture. Default is fr-FR", v => language = v },
        { "h|help",  "show this message and exit", v => help = v != null },
      };

      List<string> extra;
      try { extra = p.Parse(args); }
      catch (OptionException e) {
        Console.Write("NFCHelper: ");
        Console.WriteLine(e.Message);
        Console.WriteLine("Try `NFCHelper --help' for more information.");
        Application.Exit();
      }

      if (help) {
        ShowHelp(p);
        Application.Exit();
      }


      // ------------------------------------------
      //  STARTING
      // ------------------------------------------

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // Show the system tray icon.					
      using (ProcessIcon pi = new ProcessIcon()) {
        pi.Display();

        // Start NFCHelper
        NFCHelper.getInstance();

        // Make sure the application runs!
        Application.Run();
      }
    }

    static void ShowHelp(OptionSet p) {
      Console.WriteLine("Usage: NFCHelper [OPTIONS]+ message");
      Console.WriteLine();
      Console.WriteLine("Options:");
      p.WriteOptionDescriptions(Console.Out);
    }

    // ------------------------------------------
    //  SINGLETON
    // ------------------------------------------

    private static NFCManager singleton = null;
    public static NFCManager getInstance() {
      if (singleton == null) {
        singleton = new NFCManager();
      }
      return singleton;
    }

    // ------------------------------------------
    //  CONSTRICTOR
    // ------------------------------------------

    private NFCManager() { }

    public bool ShowAlert = false;
    public void Alert(String message) { Alert(message, false); }
    public void Alert(String message, bool force) {
      Console.WriteLine("Alert: " + message);
      if (!force && !ShowAlert) { return; }
      MessageBox.Show(message, "NFCHelper: Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    
    public bool SendURL = true;
    public void SendRequest(String url) {
      if (!SendURL) { return; }
      if (url == null) { return; }
      // url = CleanURL(url);
      Console.WriteLine("Build HttpRequest: " + url);

      HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
      req.Method = "GET";

      Console.WriteLine("Send HttpRequest: " + req.Address);

      try {
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        //using (StreamReader sr = new StreamReader(res.GetResponseStream(), Encoding.UTF8)) {
        //  NFCManager.getInstance().Alert(sr.ReadToEnd());
        //}
      }
      catch (WebException ex) {
        Console.WriteLine(ex);
      }
    }
  }
}