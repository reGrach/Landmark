
using System.IO.Ports;
using Landmark.ER301Driver.Abstract;
using Landmark.ER301Driver.Enums;
using Landmark.ER301Driver.Extensions;
using Landmark.ER301Driver.Models;

namespace Landmark.ER301Driver;

public class TagDriver : IDisposable
{
    #region constans

    public const int MifareClassicS70_BlockCount = 256;
    public const int MifareClassicS70__SectorCount = 40;
    public const int MifareClassicS70_BlockSize = 16;

    const int INIT_REQUEST_RETRY_COUNT = 5;

    const ushort MAGIC_BYTES = 0xbbaa;
    const ushort NODE_BROADCAST = 0x0000;

    #endregion

    #region fields
    readonly SerialPort port;
    readonly IDebugCallback debugCallback;
    #endregion

    #region constructor

    public TagDriver(IDebugCallback debug)
    {
        port = new SerialPort();
        debugCallback = debug;
    }

    public void Dispose()
    {
        if (port.IsOpen)
        {
            port.Close();
        }
    }

    #endregion

    #region public methods
    public void Start(string portName)
    {
        try
        {
            if (OpenPort(portName))
            {
                debugCallback.SendStatus($"Successfully openend port {portName}");

                if (InitPort(UartSpeed.BAUD_115200))
                {
                    debugCallback.SendStatus("Succesfull initialization of hardware");

                    debugCallback.SendStatus("Connected to ER301 hardware, waiting for card...");
                }
                else
                {
                    debugCallback.SendStatus("Error during initialization of hardware");
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }
            }
            else
            {
                debugCallback.SendStatus($"Error while trying to open port {portName}");
            }
        }
        catch (Exception ex)
        {
            debugCallback.SendStatus($"Connection interupted due to checksum error. Error: {ex}");
        }
    }

    public string? Detection()
    {
        string? result = null;
        try
        {
            var requestResponse = SendMifareRequest(TagRequestCode.ALL_TYPE_A);

            if (requestResponse.Response == ResponseReader.OK)
            {
                SendLedRequest(LEDColor.BLUE_ON_RED_OFF);
                debugCallback.SendStatus($"Card detected, type={requestResponse.NFCTagType}");

                var collisionResponse = SendMifareAnticollisionRequest();

                if (collisionResponse.Response == ResponseReader.OK)
                {
                    result = collisionResponse.SerialNoHex;
                    debugCallback.SendStatus($"Card serialNo={result}");

                    Thread.Sleep(100);

                    SendBeepRequest(BeepType.SHORT_60MS);
                }

                SendLedRequest(LEDColor.ALL_LED_OFF);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            debugCallback.SendStatus("Connection interupted!");
        }

        return result;
    }

    public void TurnOff()
    {
        SendLedRequest(LEDColor.RED_ON_BLUE_OFF);

        SendBeepRequest(BeepType.LONG_600MS);

        Thread.Sleep(300);

        SendLedRequest(LEDColor.ALL_LED_OFF);
    }


    #endregion

    #region private methods

    private bool OpenPort(string portName)
    {
        port.PortName = portName;
        port.BaudRate = 115200;
        port.DataBits = 8;
        port.StopBits = StopBits.One;
        port.Parity = Parity.None;
        port.RtsEnable = true;
        port.DtrEnable = true;
        port.WriteTimeout = 50;
        port.ReadTimeout = 50;

        try
        {
            port.Open();
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch
        {
            throw;
        }

        // Give the system a little time to open port
        Thread.Sleep(50);

        return port.IsOpen;
    }

    private bool InitPort(UartSpeed speed)
    {
        try
        {
            // Let's initialize the port with highest possible bandwidth
            var request = SendInitRequest(speed);
            if (request.Response != ResponseReader.OK)
            {
                return false;
            }

            // Let's find out what hardware we're dealing with
            var deviceModeResponse = SendReadDeviceMode();
            if (deviceModeResponse.Response != ResponseReader.OK)
            {
                return false;
            }

            Console.WriteLine($"Communicating with device name {deviceModeResponse.DeviceName}");

            // Turn LED off so we can actively use it to give visual feedback
            //Console.WriteLine (sendLedRequest (LEDColor.ALL_LED_OFF));
            //Console.WriteLine ();

            SendLedRequest(LEDColor.BLUE_ON_RED_OFF);
            Thread.Sleep(500);
            SendLedRequest(LEDColor.ALL_LED_OFF);

            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        catch
        {
            throw;
        }
    }




    private NFCResponseModel SendInitRequest(UartSpeed speed)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int retryCount = 0;
        int bytesToRead;
        do
        {
            if (retryCount++ == INIT_REQUEST_RETRY_COUNT)
            {
                throw new Exception("Failed at establishting connection!");
            }

            WriteCommand(CommandReader.INIT_PORT, NODE_BROADCAST, (byte)speed);

            Thread.Sleep(50);

            bytesToRead = port.BytesToRead;

        } while (bytesToRead == 0);

        debugCallback.SendStatus($"Connection established after {retryCount} attempts!");

        port.Read(readBuffer, offset, bytesToRead);

        var response = NFCResponseModel.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 3 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private NFCDeviceResponseModel SendReadDeviceMode()
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        WriteCommand(CommandReader.READ_DEVICE_MODE, NODE_BROADCAST);

        int bytesToRead;
        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        port.Read(readBuffer, offset, bytesToRead);

        var response = NFCDeviceResponseModel.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private NFCTypeResponseModel SendMifareRequest(TagRequestCode requestCode)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.MIFARE_REQUEST, NODE_BROADCAST, (byte)requestCode);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(10);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = NFCTypeResponseModel.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private AnticollisionResponse SendMifareAnticollisionRequest()
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.MIFARE_ANTICOLLISION, NODE_BROADCAST);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        // Evaluate data in buffer

        var response = AnticollisionResponse.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private NFCResponseModel SendLedRequest(LEDColor ledColor)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.SET_LED_COLOR, NODE_BROADCAST, (byte)ledColor);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = NFCResponseModel.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private NFCResponseModel SendBeepRequest(BeepType beepType)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.SET_BUZZER_BEEP, NODE_BROADCAST, (byte)beepType);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = NFCResponseModel.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    /// <summary>
    /// Before you can exchange data with a MiFare chip, the transponder has to be activated (or
    /// „selected“ in the ISO14443 language). Card will be in active state after this call.
    /// Note that something isn't right about the SAK byte, it appears to be hardcoded to 9 from
    /// the reader and *not* coming from the card being communicated with!
    /// </summary>
    /// <returns>
    /// The mifare select.
    /// </returns>
    /// <param name='serialNo'>
    /// Serial no.
    /// </param>
    private SelectResponse SendMifareSelect(uint serialNo)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.MIFARE_SELECT, NODE_BROADCAST, BitConverter.GetBytes(serialNo));

        //Console.WriteLine ("Card SerialNo: " + BitConverter.GetBytes(serialNo).ToHex() );

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = SelectResponse.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private NFCResponseModel SendHalt()
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.MIFARE_HLTA, NODE_BROADCAST);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(50);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = SelectResponse.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }



    private NFCReadResponse SendRead(byte blockNo)
    {
        byte[] readBuffer = GetEmptyBuffer();
        int offset = 0;
        int bytesToRead = 0;

        WriteCommand(CommandReader.MIFARE_READ, NODE_BROADCAST, blockNo);

        // Naive busy way. Rewrite to consumer stream pull!
        do
        {
            Thread.Sleep(10);
            bytesToRead = port.BytesToRead;
        } while (bytesToRead == 0);

        // TODO: Use callback, make response parser able to read lazily from input stream
        int bytesRead = port.Read(readBuffer, offset, bytesToRead);

        var response = NFCReadResponse.GetFromBuffer(readBuffer);
        byte checksum = response.GetCheckSum(readBuffer);
        byte calculatedChecksum = CalcCheckSum(readBuffer, 4, 4 + response.Length);

        ValidateChecksum(checksum, calculatedChecksum);

        return response;
    }

    private void ValidateChecksum(byte checksum, byte calculatedChecksum)
    {
        if (checksum != calculatedChecksum)
        {
            throw new InvalidChecksumException($"Invalid checksum, expected {checksum} but got {calculatedChecksum}");
        }
    }

    private void WriteCommand(CommandReader commandCode, ushort nodeId)
    {
        WriteCommand(commandCode, nodeId, new byte[0]);
    }

    private void WriteCommand(CommandReader commandCode, ushort nodeId, byte data)
    {
        WriteCommand(commandCode, nodeId, new byte[1] { data });
    }

    private void WriteCommand(CommandReader commandCode, ushort nodeId, byte[] data)
    {
        Write(MAGIC_BYTES);

        Write((ushort)(data.Length + 5));

        byte[] nodeIdCommandAndData = BitConverter.GetBytes(nodeId).Merge(
                BitConverter.GetBytes((ushort)commandCode),
                data
        );

        // The ER301 hardware relies on escaping 0xAA sequences to 0xAA00 sequences
        nodeIdCommandAndData = nodeIdCommandAndData.ReplaceBytes(
            new byte[] { 0xaa },
            new byte[] { 0xaa, 0x00 });

        //Console.WriteLine ("NODE_ID, COMMAND and <DATA>: {0}", nodeIdCommandAndData.ToHex ());

        Write(nodeIdCommandAndData);

        byte CheckSum = CalcCheckSum(nodeIdCommandAndData);

        //Console.WriteLine ("CHECKSUM:");

        Write(CheckSum);
    }

    private void Write(byte data)
    {
        Write(new byte[] { data });
    }

    private void Write(ushort data)
    {
        Write(BitConverter.GetBytes(data));
    }

    // TODO: Rewrite so that all transmissions occur out of one allocated buffer, where only the length decides
    // how much is sent and what is discarded. (less GC, more effecient and easier handling of the AA escapes) 
    private void Write(byte[] data)
    {
        //Console.WriteLine( "Writing: {0})", data.ToHex () );
        port.Write(data, 0, data.Length);
    }

    #endregion

    #region static methods

    public static int GetSectorIndexByBlockIndex(int blockIndex) =>
        blockIndex < 128 ? blockIndex / 4 : 32 + (blockIndex - 128) / 16;

    public static uint ReverseBytes(uint value) =>
        (value & 0x000000FFU) << 24
        | (value & 0x0000FF00U) << 8
        | (value & 0x00FF0000U) >> 8
        | (value & 0xFF000000U) >> 24;

    // TODO: Add other types
    private static int GetBlockCount(TagType type)
    {
        return type switch
        {
            TagType.MIFARE_CLASSIC_4K_S70 => 256,
            TagType.ULTRALIGHT => 16,
            _ => 64,
        };
    }

    private static byte[] GetEmptyBuffer() => new byte[64];

    private static byte CalcCheckSum(byte[] PacketData) =>
        CalcCheckSum(PacketData, 0, PacketData.Length);

    private static byte CalcCheckSum(byte[] data, int start, int end)
    {
        byte checksum = 0x00;

        for (int offset = start; offset < end; offset++)
        {
            checksum ^= data[offset];
        }

        return checksum;
    }

    #endregion
}