using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace cafiine_server
{
    class Program
    {
        // Command bytes
        public const byte BYTE_NORMAL = 0xff;
        public const byte BYTE_SPECIAL = 0xfe;
        public const byte BYTE_OK = 0xfd;
        public const byte BYTE_PING = 0xfc;
        public const byte BYTE_LOG_STR = 0xfb;

        public const byte BYTE_OPEN_FILE = 0x00;
        public const byte BYTE_OPEN_FILE_ASYNC = 0x01;
        public const byte BYTE_OPEN_DIR = 0x02;
        public const byte BYTE_OPEN_DIR_ASYNC = 0x03;
        public const byte BYTE_CHANGE_DIR = 0x04;
        public const byte BYTE_CHANGE_DIR_ASYNC = 0x05;
        public const byte BYTE_STAT = 0x06;
        public const byte BYTE_STAT_ASYNC = 0x07;

        public const byte BYTE_CLOSE_FILE = 0x08;
        public const byte BYTE_CLOSE_FILE_ASYNC = 0x09;
        public const byte BYTE_SETPOS = 0x0A;
        public const byte BYTE_GETPOS = 0x0B;
        public const byte BYTE_STATFILE = 0x0C;
        public const byte BYTE_EOF = 0x0D;
        public const byte BYTE_READ_FILE = 0x0E;
        public const byte BYTE_READ_FILE_ASYNC = 0x0F;
        public const byte BYTE_CLOSE_DIR = 0x10;
        public const byte BYTE_CLOSE_DIR_ASYNC = 0x11;
        public const byte BYTE_GET_CWD = 0x12;
        public const byte BYTE_READ_DIR = 0x13;
        public const byte BYTE_READ_DIR_ASYNC = 0x14;
        public const byte BYTE_MAKE_DIR = 0x15;
        public const byte BYTE_MAKE_DIR_ASYNC = 0x16;
        public const byte BYTE_RENAME = 0x17;
        public const byte BYTE_RENAME_ASYNC = 0x18;
        public const byte BYTE_REMOVE = 0x19;
        public const byte BYTE_REMOVE_ASYNC = 0x1A;

        public const byte BYTE_MOUNT_SD = 0x30;
        public const byte BYTE_MOUNT_SD_OK = 0x31;
        public const byte BYTE_MOUNT_SD_BAD = 0x32;

        public const byte BYTE_ERROR_CODE = 0x50;

        // Other defines
        public const int FS_MAX_ENTNAME_SIZE = 256;
        public const int FS_MAX_ENTNAME_SIZE_PAD = 0;

        public const int FS_MAX_LOCALPATH_SIZE = 511;
        public const int FS_MAX_MOUNTPATH_SIZE = 128;

        public const int ERROR_OFFSET = 128;

        // Logs folder
        public static string logs_root = "logs";

        public const int FS_ERROR_CANCELED = -1;
        public const int FS_ERROR_END = -2;
        public const int FS_ERROR_MAX = -3;
        public const int FS_ERROR_ALREADY_OPEN = -4;
        public const int FS_ERROR_EXISTS = -5;
        public const int FS_ERROR_NOT_FOUND = -6;
        public const int FS_ERROR_NOT_FILE = -7;
        public const int FS_ERROR_NOT_DIR = -8;
        public const int FS_ERROR_ACCESS_ERROR = -9;
        public const int FS_ERROR_PERMISSION_ERROR = -10;
        public const int FS_ERROR_FILE_TOO_BIG = -11;
        public const int FS_ERROR_STORAGE_FULL = -12;
        public const int FS_ERROR_JOURNAL_FULL = -13;
        public const int FS_ERROR_UNSUPPORTED_CMD = -14;
        public const int FS_ERROR_MEDIA_NOT_READY = -15;
        public const int FS_ERROR_INVALID_MEDIA = -16;
        public const int FS_ERROR_MEDIA_ERROR = -17;
        public const int FS_ERROR_DATA_CORRUPTED = -18;
        public const int FS_ERROR_WRITE_PROTECTED = -19;


        public static string getError(int code)
        {
            switch (code)
            {
                case FS_ERROR_CANCELED:
                    {
                        return FS_ERROR_CANCELED + ": FS_ERROR_CANCELED";                        
                    }
                case FS_ERROR_END:
                    {
                        return FS_ERROR_END + ": FS_ERROR_END";                       
                    }
                case FS_ERROR_MAX:
                    {
                        return FS_ERROR_MAX + ": FS_ERROR_MAX";                        
                    }
                case FS_ERROR_ALREADY_OPEN:
                    {
                        return FS_ERROR_ALREADY_OPEN + ": FS_ERROR_ALREADY_OPEN";                      
                    }
                case FS_ERROR_EXISTS:
                    {
                        return FS_ERROR_EXISTS + ": FS_ERROR_EXISTS";                       
                    }
                case FS_ERROR_NOT_FOUND:
                    {
                        return FS_ERROR_NOT_FOUND + ": FS_ERROR_NOT_FOUND";                     
                    }
                case FS_ERROR_NOT_FILE:
                    {
                        return FS_ERROR_NOT_FILE + ": FS_ERROR_NOT_FILE";                      
                    }
                case FS_ERROR_NOT_DIR:
                    {
                        return FS_ERROR_NOT_DIR + ": FS_ERROR_NOT_DIR";                       
                    }
                case FS_ERROR_ACCESS_ERROR:
                    {
                        return FS_ERROR_ACCESS_ERROR + ": FS_ERROR_ACCESS_ERROR";                       
                    }
                case FS_ERROR_PERMISSION_ERROR:
                    {
                        return FS_ERROR_PERMISSION_ERROR + ": FS_ERROR_PERMISSION_ERROR";                        
                    }
                case FS_ERROR_FILE_TOO_BIG:
                    {
                        return FS_ERROR_FILE_TOO_BIG + ": FS_ERROR_FILE_TOO_BIG";                       
                    }
                case FS_ERROR_STORAGE_FULL:
                    {
                        return FS_ERROR_STORAGE_FULL + ": FS_ERROR_STORAGE_FULL";                       
                    }
                case FS_ERROR_JOURNAL_FULL:
                    {
                        return FS_ERROR_JOURNAL_FULL + ": FS_ERROR_JOURNAL_FULL";                       
                    }
                case FS_ERROR_UNSUPPORTED_CMD:
                    {
                        return FS_ERROR_UNSUPPORTED_CMD + ": FS_ERROR_UNSUPPORTED_CMD";                       
                    }
                case FS_ERROR_MEDIA_NOT_READY:
                    {
                        return FS_ERROR_MEDIA_NOT_READY + ": FS_ERROR_MEDIA_NOT_READY";
                    }
                case FS_ERROR_INVALID_MEDIA:
                    {
                        return FS_ERROR_INVALID_MEDIA + ": FS_ERROR_INVALID_MEDIA";
                    }
                case FS_ERROR_MEDIA_ERROR:
                    {
                        return FS_ERROR_MEDIA_ERROR + ": FS_ERROR_MEDIA_ERROR";
                    }
                case FS_ERROR_DATA_CORRUPTED:
                    {
                        return FS_ERROR_DATA_CORRUPTED + ": FS_ERROR_DATA_CORRUPTED";
                    }
                case FS_ERROR_WRITE_PROTECTED:
                    {
                        return FS_ERROR_WRITE_PROTECTED + ": FS_ERROR_WRITE_PROTECTED";                        
                    }
                case 0:
                    {
                        return "Okay";                        
                    }                    
                default:
                    {
                        return "Unkown Error";                       
                    }                   
            }
        }
        static StreamWriter com_log;
        static void Main(string[] args)
        {
            // Check if logs folder
            if (!Directory.Exists(logs_root))
            {
                Console.Error.WriteLine("Logs directory `{0}' does not exist!", logs_root);
                return;
            }
            // Delete logs
            System.IO.DirectoryInfo downloadedMessageInfo = new DirectoryInfo(logs_root);
            foreach (FileInfo file in downloadedMessageInfo.GetFiles())
            {
                file.Delete();
            }


            com_log = new StreamWriter(logs_root + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "-all.txt");
            // Start server
            string name = "[listener]";
            try
            {	
				 Console.WriteLine("loadiine server version 1.1");
                TcpListener listener = new TcpListener(IPAddress.Any, 7332);
                listener.Start();
                Console.WriteLine(name + " Listening on 7332");

                int index = 0;
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("connected");
                    Thread thread = new Thread(Handle);
                    thread.Name = "[" + index.ToString() + "]";
                    thread.Start(client);
                    index++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(name + " " + e.Message);
            }
            Console.WriteLine(name + " Exit");
        }

        static void Log(StreamWriter log, String str)
        {
            log.WriteLine(str);
            log.Flush();
            com_log.WriteLine(str);
            com_log.Flush();
            Console.WriteLine(str);
        }

        static void Handle(object client_obj)
        {
            string name = Thread.CurrentThread.Name;
            StreamWriter log = null;

            try
            {
                TcpClient client = (TcpClient)client_obj;
                using (NetworkStream stream = client.GetStream())
                {
                    EndianBinaryReader reader = new EndianBinaryReader(stream);
                    EndianBinaryWriter writer = new EndianBinaryWriter(stream);


                    // Log connection
                    //Console.WriteLine(name + " Accepted connection from client " + client.Client.RemoteEndPoint.ToString());

                    // Create log file for current thread
                    log = new StreamWriter(logs_root + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "-" + name + ".txt");
                    Log(log,name + " Accepted connection from client " + client.Client.RemoteEndPoint.ToString());

                    writer.Write(BYTE_SPECIAL);

                    while (true)
                    {
                        byte cmd_byte = reader.ReadByte();
                        switch (cmd_byte)
                        {
                            case BYTE_OPEN_FILE:
                            case BYTE_OPEN_FILE_ASYNC:
                                {
                                    int len_str = reader.ReadInt32();
                                    string str = reader.ReadString(Encoding.ASCII, len_str - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_OPEN_FILE)
                                        Log(log, name + " FSOpenFile(\"" + str + "\")");
                                    else
                                        Log(log, name + " FSOpenFileAsync(\"" + str + "\")");

                                    break;
                                }
                            case BYTE_READ_FILE:
                            case BYTE_READ_FILE_ASYNC:
                                {
                                    int size = reader.ReadInt32();
                                    int count = reader.ReadInt32();
                                    int fd = reader.ReadInt32();

                                    if (cmd_byte == BYTE_READ_FILE)
                                        Log(log, name + " FSReadFile(size=" + size.ToString() + ", count=" + count.ToString() + ", fd=" + fd.ToString() + ")");
                                    else
                                        Log(log, name + " FSReadFileAsync(size=" + size.ToString() + ", count=" + count.ToString() + ", fd=" + fd.ToString() + ")");

                                    break;
                                }
                            case BYTE_CLOSE_FILE:
                            case BYTE_CLOSE_FILE_ASYNC:
                                {
                                    int fd = reader.ReadInt32();

                                    if (cmd_byte == BYTE_CLOSE_FILE)
                                        Log(log, name + " FSCloseFile(" + fd.ToString() + ")");
                                    else
                                        Log(log, name + " FSCloseFileAsync(" + fd.ToString() + ")");

                                    break;
                                }
                            case BYTE_SETPOS:
                                {
                                    int fd = reader.ReadInt32();
                                    int pos = reader.ReadInt32();

                                    Log(log, name + " FSSetPos(fd=" + fd.ToString() + ", pos=" + pos.ToString() + ")");

                                    break;
                                }
                            case BYTE_STATFILE:
                                {
                                    int fd = reader.ReadInt32();
                                    Log(log, name + " FSGetStatFile(" + fd.ToString() + ")");

                                    break;
                                }
                            case BYTE_OPEN_DIR:
                            case BYTE_OPEN_DIR_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_OPEN_DIR)
                                        Log(log, name + " FSOpenDir(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSOpenDirAsync(\"" + path + "\")");

                                    break;
                                }
                            case BYTE_READ_DIR:
                            case BYTE_READ_DIR_ASYNC:
                                {
                                    int fd = reader.ReadInt32();

                                    if (cmd_byte == BYTE_READ_DIR)
                                        Log(log, name + " FSReadDir(fd=" + fd.ToString() + ")");
                                    else
                                        Log(log, name + " FSReadDirAsync(fd=" + fd.ToString() + ")");

                                    break;
                                }
                            case BYTE_CLOSE_DIR:
                            case BYTE_CLOSE_DIR_ASYNC:
                                {
                                    int fd = reader.ReadInt32();

                                    if (cmd_byte == BYTE_CLOSE_DIR)
                                        Log(log, name + " FSCloseDir(" + fd.ToString() + ")");
                                    else
                                        Log(log, name + " FSCloseDirAsync(" + fd.ToString() + ")");

                                    break;
                                }
                            case BYTE_CHANGE_DIR:
                            case BYTE_CHANGE_DIR_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_CHANGE_DIR)
                                        Log(log, name + " FSChangeDir(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSChangeDirAsync(\"" + path + "\")");

                                    break;
                                }
                            case BYTE_MAKE_DIR:
                            case BYTE_MAKE_DIR_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_CHANGE_DIR)
                                        Log(log, name + " FSMakeDir(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSMakeDirAsync(\"" + path + "\")");

                                    break;
                                }
                            case BYTE_RENAME:
                            case BYTE_RENAME_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_CHANGE_DIR)
                                        Log(log, name + " FSRename(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSRenameAsync(\"" + path + "\")");

                                    break;
                                }
                            case BYTE_REMOVE:
                            case BYTE_REMOVE_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_CHANGE_DIR)
                                        Log(log, name + " FSRemove(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSRemoveAsync(\"" + path + "\")");

                                    break;
                                }
                            case BYTE_GET_CWD:
                                {
                                    Log(log, name + " FSGetCwd()");

                                    break;
                                }
                            case BYTE_STAT:
                            case BYTE_STAT_ASYNC:
                                {
                                    int len_path = reader.ReadInt32();
                                    string path = reader.ReadString(Encoding.ASCII, len_path - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    if (cmd_byte == BYTE_STAT)
                                        Log(log, name + " FSGetStat(\"" + path + "\")");
                                    else
                                        Log(log, name + " FSGetStatAsync(\"" + path + "\")");
                                    break;
                                }
                            case BYTE_EOF:
                                {
                                    int fd = reader.ReadInt32();
                                    Log(log, name + " FSGetEof(" + fd.ToString() + ")");
                                    break;
                                }
                            case BYTE_GETPOS:
                                {
                                    int fd = reader.ReadInt32();
                                    Log(log, name + " FSGetPos(" + fd.ToString() + ")");
                                    break;
                                }
                            case BYTE_MOUNT_SD:
                                {
                                    Log(log, name + " Trying to mount SD card");
                                    break;
                                }
                            case BYTE_MOUNT_SD_OK:
                                {
                                    Log(log, name + " SD card mounted !");
                                    break;
                                }
                            case BYTE_MOUNT_SD_BAD:
                                {
                                    Log(log, name + " Can't mount SD card");
                                    break;
                                }
                            case BYTE_PING:
                                {
                                    int val1 = reader.ReadInt32();
                                    int val2 = reader.ReadInt32();

                                    Log(log, name + " PING RECEIVED with values : " + val1.ToString() + " - " + val2.ToString());
                                    break;
                                }
                            case BYTE_LOG_STR:
                                {
                                    int len_str = reader.ReadInt32();
                                    string str = reader.ReadString(Encoding.ASCII, len_str - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();

                                    Log(log, name + " LogString =>(\"" + str + "\")");
                                    break;
                                }
                            case BYTE_ERROR_CODE:
                                {
                                    int len_str = reader.ReadInt32();                                   
                                    string str = reader.ReadString(Encoding.ASCII, len_str - 1);
                                    if (reader.ReadByte() != 0) throw new InvalidDataException();
                                    int code = str[0] - ERROR_OFFSET;
                                    Log(log, name + " Error =>(\"" + getError(code)+  "\")");
                                    break;
                                }
                            default:
                                throw new InvalidDataException();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (log != null)
                    Log(log, name + " " + e.Message);
                else
                    Console.WriteLine(name + " " + e.Message);
            }
            finally
            {
                if (log != null)
                    log.Close();
            }
            Console.WriteLine(name + " Exit");
        }
    }
}
