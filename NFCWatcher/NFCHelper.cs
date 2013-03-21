
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using NdefLibrary.Ndef;
using Windows.Networking.Proximity;
using System.Windows.Forms;


namespace net.encausse.nfc {
  public class NFCHelper {

    // ------------------------------------------
    //  SINGLETON
    // ------------------------------------------

    private static NFCHelper singleton = null;
    public  static NFCHelper getInstance() {
      if (singleton == null) {
        singleton = new NFCHelper();
      }
      return singleton;
    }

    // ------------------------------------------
    //  CONSTRUCTOR
    // ------------------------------------------

    private ProximityDevice _device;
    private NFCHelper() {
      // Initialize NFC
      _device = ProximityDevice.GetDefault();
      _device.SubscribeForMessage("NDEF", MessageReceivedHandler);
    }

    // ------------------------------------------
    //  PUBLIC
    // http://software.intel.com/en-us/articles/using-winrt-apis-from-desktop-applications
    // ------------------------------------------

    public void WriteURIRecord(String uri) {
      var uriRecord = new NdefUriRecord { Uri = uri };
      PublishRecord(uriRecord, true);
    }


    // ------------------------------------------
    //  PRIVATE READER
    // ------------------------------------------

    private void MessageReceivedHandler(ProximityDevice sender, ProximityMessage message) {
      // Parse raw byte array to NDEF message
      var rawMsg = message.Data.ToArray(); 
      var ndefMessage = NdefMessage.FromByteArray(rawMsg);

      // Loop over all records contained in the NDEF message
      foreach (NdefRecord record in ndefMessage) {
        Console.WriteLine("Record type: " + Encoding.UTF8.GetString(record.Type, 0, record.Type.Length));
        
        // Check the type of each record - handling a Smart Poster in this example
        var specializedType = record.CheckSpecializedType(true);

        // Convert and extract Smart Poster info
        if (specializedType == typeof(NdefSpRecord)) {
          var spRecord = new NdefSpRecord(record);
          String msg = "URI: " + spRecord.Uri + "\n"
                         + "Titles: " + spRecord.TitleCount() + "\n"
                         + "1. Title: " + spRecord.Titles[0].Text + "\n"
                         + "Action set: " + spRecord.ActionInUse();
          NFCManager.getInstance().Alert(msg);
        }

        // Convert and extract URI record info
        else if (specializedType == typeof(NdefUriRecord)) {
          var uriRecord = new NdefUriRecord(record);
          String msg = "URI: " + uriRecord.Uri + "\n";
          NFCManager.getInstance().Alert(msg);
          NFCManager.getInstance().SendRequest(uriRecord.Uri);
        }

        // Convert and extract Mailto record info
        else if (specializedType == typeof(NdefMailtoRecord)) {
          var mailtoRecord = new NdefMailtoRecord(record);
          String msg = "Address: " + mailtoRecord.Address + "\n"
                     + "Subject: " + mailtoRecord.Subject + "\n"
                     + "Body: " + mailtoRecord.Body;
          NFCManager.getInstance().Alert(msg);
        }
      }
    }

    // ------------------------------------------
    //  PRIVATE WRITER
    // ------------------------------------------

    private long _publishingMessageId;

    private void PublishRecord(NdefRecord record, bool writeToTag) {
      if (_device == null)
        return;

      // Make sure we're not already publishing another message
      StopPublishingMessage(false);
      // Wrap the NDEF record into an NDEF message
      var message = new NdefMessage { record };
      // Convert the NDEF message to a byte array
      var msgArray = message.ToByteArray();
      // Publish the NDEF message to a tag or to another device, depending on the writeToTag parameter
      // Save the publication ID so that we can cancel publication later
      _publishingMessageId = _device.PublishBinaryMessage((writeToTag ? "NDEF:WriteTag" : "NDEF"), msgArray.AsBuffer(), MessageWrittenHandler);
    }

    private void MessageWrittenHandler(ProximityDevice sender, long messageid) {
      // Stop publishing the message
      StopPublishingMessage(false);
      NFCManager.getInstance().Alert("Message Written", true);
    }

    private void StopPublishingMessage(bool writeToStatusOutput) {
      if (_publishingMessageId != 0 && _device != null) {
        // Stop publishing the message
        _device.StopPublishingMessage(_publishingMessageId);
        _publishingMessageId = 0;
      }
    }
  }
}
