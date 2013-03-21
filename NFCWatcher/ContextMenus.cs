using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using net.encausse.nfc.Properties;

namespace net.encausse.nfc {

  class ContextMenus {

    bool isAboutLoaded = false;
    ToolStripMenuItem alert;
    ToolStripMenuItem request;

    public ContextMenuStrip Create() {
      // Add the default menu options.
      ContextMenuStrip menu = new ContextMenuStrip();
      ToolStripMenuItem item;
      ToolStripSeparator sep;

      // Write URL.
      item = new ToolStripMenuItem();
      item.Text = "Write URL";
      item.Click += new EventHandler(WriteURL_Click);
      menu.Items.Add(item);

      // Send request.
      request = new ToolStripMenuItem();
      request.Text = "Send request";
      request.Click += new EventHandler(Request_Click);
      request.CheckOnClick = true;
      request.Checked = true;
      menu.Items.Add(request);

      // Show alerts.
      alert = new ToolStripMenuItem();
      alert.Text = "Show alerts";
      alert.Click += new EventHandler(Alert_Click);
      alert.CheckOnClick = true;
      menu.Items.Add(alert);

      // About.
      item = new ToolStripMenuItem();
      item.Text = "About";
      item.Click += new EventHandler(About_Click);
      item.Image = Resources.Info;
      menu.Items.Add(item);

      // Separator.
      sep = new ToolStripSeparator();
      menu.Items.Add(sep);

      // Exit.
      item = new ToolStripMenuItem();
      item.Text = "Exit";
      item.Click += new System.EventHandler(Exit_Click);
      item.Image = Resources.Exit;
      menu.Items.Add(item);

      return menu;
    }

    void WriteURL_Click(object sender, EventArgs e) {
      String prompt = Microsoft.VisualBasic.Interaction.InputBox("Enter URL to write: ","NFC Writer","http://");
      if (prompt != null && prompt != "") {
        NFCHelper.getInstance().WriteURIRecord(prompt);
      }
    }

    void Alert_Click(object sender, EventArgs e) {
      NFCManager.getInstance().ShowAlert = alert.Checked;
    }

    void Request_Click(object sender, EventArgs e) {
      NFCManager.getInstance().SendURL = request.Checked;
    }

    void About_Click(object sender, EventArgs e) {
      if (!isAboutLoaded) {
        isAboutLoaded = true;
        new AboutBox().ShowDialog();
        isAboutLoaded = false;
      }
    }

    /// <summary>
    /// Processes a menu item.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Exit_Click(object sender, EventArgs e) {
      // Quit without further ado.
      Application.Exit();
    }
  }
}