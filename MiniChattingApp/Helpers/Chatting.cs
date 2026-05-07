using Microsoft.EntityFrameworkCore;
using MiniChattingApp.DataBaseRelated.Data;
using MiniChattingApp.DataBaseRelated.DataAccess.Concrete.EntityFramework;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Concrete;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniChattingApp.Helpers
{
    public enum UserDbStatus
    {
        UserNotInDb = -1,
        UserInDbNotVerified,
        UserIsVerified,
        UserInvalidCredentials
    }
    public static class Chatting
    {
        static TcpListener _listener = null!;
        public static MiniChattingDBContext? DBContext { get; set; }
        public static EFUserDal? UserDal { get; set; }
        public static EFMessageDal? MessageDal { get; set; }
        public static UserService? UserService { get; set; }
        public static List<FileMessage>? FileMessages { get; set; }
        public static EFFileMessageDal? FileMessageDal { get; set; }
        public static List<User>? Users { get; set; }
        //consider creating static list messages, if added use everywhere 
        private const string SaveFolder = "ReceivedFiles";


        static List<TcpClient> _tcpClients = [];



        public static async Task Initiate()
        {

            try
            {
                Users = await UserDal!.GetAllAsync();

            }
            catch (Exception ex)
            {
                ex.Message.ShowErrorMessage();
            }
            IPAddress iPAddress = IPAddress.Any;
            int port = 44000;
            var ep = new IPEndPoint(iPAddress, port);
            _listener = new TcpListener(ep);

            _listener.Start();
            Console.WriteLine($"Listening over: {_listener.LocalEndpoint}");

            await Task.Run(AcceptClients);
        }

        private static void AcceptClients()
        {
            while (true)
            {
                var client = _listener.AcceptTcpClient();
                Task.Run(() => ClientHandler(client));
            }
        }

        private static async Task ClientHandler(TcpClient tcpClient)
        {
            try
            {
                var stream = tcpClient.GetStream();
                var br = new BinaryReader(stream);
                string email = br.ReadString();
                string username = br.ReadString();
                Console.Write($"Client {username} connected ");
                Console.WriteLine(email);

                var clientIpAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString();
                var clientPort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Port.ToString();

                var userStatus = await UserToDb(email, username, clientIpAddress, clientPort, tcpClient);
                if (userStatus == UserDbStatus.UserNotInDb)
                {
                    $"New user \"{username}\" detected, and was added to DB".ShowInformativeTextServer();
                }
                else if (userStatus == UserDbStatus.UserInDbNotVerified)
                {
                    $"User \"{username}\" is in DB, but not verified, sending verification email".ShowInformativeTextServer();
                }
                else if (userStatus == UserDbStatus.UserInvalidCredentials)
                {
                    $"User \"{username}\" tried to log in with invalid credentials".ShowErrorMessage();
                }
                else
                {
                    $"Verified user \"{username}\" logged in".ShowInformativeTextServer();
                }
                var user = await UserDal!.GetAsync(u => u.Email == email);
                await UserDal!.UpdateUserStatusAsync(user!.Id, true);
                _tcpClients.Add(tcpClient);

                var bw = new BinaryWriter(stream);
                string json = null!;
                //bw.Write(json);

                while (true)
                {
                    string msg = br.ReadString();
                    //Console.Write($"User \"{username}\": ", Console.ForegroundColor = ConsoleColor.Yellow);
                    if (Helper.IsValidJson(msg))
                    {
                        var jsonFile = JsonDocument.Parse(msg);

                        if (jsonFile.RootElement.ValueKind == JsonValueKind.Array &&
                            jsonFile.RootElement.GetArrayLength() != 0)
                        {
                            var type = jsonFile.RootElement[0].GetProperty("Type").GetString();
                            if (type == "chat")
                            {
                                var chat = JsonConvert.DeserializeObject<Chat>(msg);
                                var senderEmail = chat!.SenderEmail;
                                var content = chat.Content;
                                var receiverEmail = chat.ReceiverEmail;
                                var sentTime = chat.SendingTime;
                                await StartChat(senderEmail!, content!, receiverEmail!, sentTime);

                            }
                            else if (type == "message")
                            {
                                var messages = JsonConvert.DeserializeObject<List<Message>>(msg)!;
                                await MessageToDB(messages);
                            }
                        }
                        else if (jsonFile.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            var type = jsonFile.RootElement.GetProperty("Type").GetString();
                            if (type == "chat")
                            {
                                var chat = JsonConvert.DeserializeObject<Chat>(msg);
                                var senderEmail = chat!.SenderEmail;
                                var content = chat.Content;
                                var receiverEmail = chat.ReceiverEmail;
                                var sentTime = chat.SendingTime;
                                await StartChat(senderEmail!, content!, receiverEmail!, sentTime);

                            }
                            else if (type == "message")
                            {
                                var messages = JsonConvert.DeserializeObject<List<Message>>(msg)!;
                                await MessageToDB(messages);
                            }
                            else if (type == "user")
                            {
                                var jsonUser = JsonConvert.DeserializeObject<User>(msg)!;
                                await UserDal.UpdateUserVerificationStatusAsync(jsonUser.Id);
                            }
                        }
                    }
                    else if (msg == "_users")
                    {
                        Users = await UserDal.GetAllAsync();
                        json = JsonConvert.SerializeObject(Users);
                        bw.Write(json);
                    }
                    else if (msg == "_chatHistory")
                    {
                        var messages = await MessageDal!.GetAllAsync();
                        json = JsonConvert.SerializeObject(messages);
                        bw.Write(json);
                    }
                    else if (msg == "--file")
                    {
                        try
                        {
                            var address = br.ReadString().Split("\n");
                            var senderEmail = address[0];
                            var receiverId = int.Parse(address[1]);
                            await StartFile(tcpClient, email, receiverId);
                        }
                        catch (Exception ex)
                        {

                            ex.Message.ShowErrorMessage();
                        }
                    }
                    else if (msg == "_allFiles")
                    {
                        FileMessages = await FileMessageDal!.GetAllAsync();
                        json = JsonConvert.SerializeObject(FileMessages);
                        bw.Write(json);
                    }
                    else if (msg.StartsWith("_fileID:"))
                    {
                        var fileId = int.Parse(msg.Split(":")[1]);
                        var file = await FileMessageDal!.GetAsync(u => u.Id == fileId);
                        await SendFile(tcpClient, file!.Path!);
                    }
                    else if (msg == "--voice")
                    {
                        var fileInfo = br.ReadString().Split("\n");
                        var fileName = fileInfo[0] + "_" + fileInfo[1];
                        var senderEmail = fileInfo[1];
                        var receiverId = int.Parse(fileInfo[2]);
                        var fileSize = br.ReadInt64();

                        if (fileSize == 0) break;

                        var bytesFile = br.ReadBytes((int)fileSize);
                        await SaveFileAsync(fileName, senderEmail, receiverId, bytesFile);
                    }
                    //Console.ResetColor();
                    //Console.WriteLine(msg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                User? user = null;
                var clientIpAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString();
                var clientPort = ((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Port.ToString();
                try
                {
                    user = await UserDal!.GetAsync(u => u.IpAddress == clientIpAddress && u.Port == clientPort);
                }
                catch (Exception ex1)
                {
                    ex1.Message.ShowErrorMessage();
                }
                $"User \"{user!.Username}\" is disconnected, updating status...".ShowInformativeTextServer();
                if (await UserDal!.UpdateUserStatusAsync(user.Id, false))
                    $"\"{user.Username}\" status was updated".ShowInformativeTextServer();

            }
        }

        private static async Task SaveFileAsync(string fileName, string myEmail, int receiverId, byte[] bytesFile)
        {
            var senderUser = await UserDal!.GetAsync(u => u.Email == myEmail);
            var receiverUser = await UserDal.GetAsync(u => u.Id == receiverId);

            var savePath = $"Records\\{fileName}.wav";
            Directory.CreateDirectory("Records");

            await File.WriteAllBytesAsync(savePath, bytesFile);

            var fileMessage = new FileMessage
            {
                Path = savePath,
                ReceiverId = receiverId,
                SenderId = senderUser.Id
            };
            await FileMessageDal!.AddAsync(fileMessage);

        }

        private static async Task SendFile(TcpClient tcpClient, string filePath)
        {

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine("File not found");
                return;
            }

            string fileName = Path.GetFileName(filePath);
            long fileSize = new FileInfo(filePath).Length;

            var stream = tcpClient.GetStream();
            var bw = new BinaryWriter(stream);
            bw.Write("--FileStartReady");

            await Task.Delay(50);

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);// name to bytes
            byte[] fileNameLengthBytes = BitConverter.GetBytes(fileNameBytes.Length);//name.Length to bytes

            await stream.WriteAsync(fileNameLengthBytes);
            await stream.WriteAsync(fileNameBytes);


            byte[] fileSizeBytes = BitConverter.GetBytes(fileSize);
            await stream.WriteAsync(fileSizeBytes);

            byte[] buffer = new byte[8192];
            long totalSent = 0;

            await using var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 8192,
                useAsync: true
                );

            int read;

            while ((read = await fileStream.ReadAsync(buffer)) > 0)
            {
                await stream.WriteAsync(buffer.AsMemory(0, read));
                totalSent += read;
            }

            Console.WriteLine("File sent successfully");
        }

        private static async Task StartFile(TcpClient tcpClient, string myEmail, int receiverId)
        {
            var senderUser = await UserDal!.GetAsync(u => u.Email == myEmail);
            var receiverUser = await UserDal.GetAsync(u => u.Id == receiverId);

            Directory.CreateDirectory(SaveFolder);
            var stream = tcpClient.GetStream();

            //1. Read filename len

            byte[] fileNameLengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(fileNameLengthBuffer, 0, 4);
            int length = BitConverter.ToInt32(fileNameLengthBuffer, 0);
            int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

            //2. Read filename
            byte[] fileNameBuffer = new byte[fileNameLength];
            await stream.ReadExactlyAsync(fileNameBuffer, 0, fileNameLength);

            string fileName = Encoding.UTF8.GetString(fileNameBuffer);

            fileName = Path.GetFileName(fileName);

            //3. Read file size - long 8 byte

            byte[] fileSizeBuffer = new byte[8];
            await stream.ReadExactlyAsync(fileSizeBuffer, 0, 8);

            long fileSize = BitConverter.ToInt64(fileSizeBuffer, 0);

            Console.WriteLine($"Receiving file: {fileName}");
            Console.WriteLine($"File size: {fileSize} bytes");

            string savePath = Path.Combine(SaveFolder, fileName);

            //4. Read file bytes and save
            byte[] buffer = new byte[8192];
            long totalReceived = 0;

            await using var fileStream = new FileStream(
                savePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 8192,
                useAsync: true
                );
            while (totalReceived < fileSize)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, fileSize - totalReceived);

                int received = await stream.ReadAsync(buffer.AsMemory(0, bytesToRead));

                if (received == 0)
                {
                    throw new IOException("Client disconnected before file transfer completion");
                }

                await fileStream.WriteAsync(buffer.AsMemory(0, received));

                totalReceived += received;

                double progress = totalReceived * 100 / fileSize;

                Console.WriteLine($"Progress : {progress:F2}%");
            }
            Console.WriteLine($"File saved:{savePath}");

            var fileMessage = new FileMessage
            {
                Path = savePath,
                ReceiverId = receiverId,
                SenderId = senderUser.Id
            };
            await FileMessageDal!.AddAsync(fileMessage);
        }

        public static async Task MessageToDB(List<Message> messages)
        {
            foreach (var entity in messages)
            {
                await Program.dbContext.Messages
                    .Where(m => m.Id == entity.Id)
                    .ExecuteUpdateAsync(e =>
                    e.SetProperty(m => m.IsRead, entity.IsRead));
            }
        }

        private static async Task StartChat(string senderEmail, string content, string receiverEmail, DateTime sentTime)
        {
            var receiverUser = await UserDal!.GetAsync(u => u.Email == receiverEmail);
            var senderUser = await UserDal!.GetAsync(u => u.Email == senderEmail);

            var receiverIp = receiverUser!.IpAddress;
            var receiverPort = int.Parse(receiverUser!.Port!);
            var ep = new IPEndPoint(IPAddress.Parse(receiverIp!), receiverPort).ToString();
            var receiverClient = _tcpClients.FirstOrDefault(c => c.Client.RemoteEndPoint!.ToString() == ep);

            var message = new Message
            {
                Content = content,
                SenderId = senderUser!.Id,
                ReceiverId = receiverUser.Id,
                IsRead = false,
                SentTime = sentTime
            };
            await MessageDal!.AddAsync(message);
            if (receiverClient != null)
            {
                var stream = receiverClient!.GetStream();
                var bw = new BinaryWriter(stream);

                var result = $":I1Message from {senderUser!.Username}: {content}";
                bw.Write(result);

            }
        }

        private static async Task<UserDbStatus> UserToDb(string email, string username, string ipAddress, string port, TcpClient client)
        {
            var existing = Users!.FirstOrDefault(u => u.Email == email);
            if (existing == null)
            {
                try
                {
                    await UserDal!.AddAsync(new User
                    {
                        Email = email,
                        Username = username,
                        IpAddress = ipAddress,
                        Port = port,
                        IsOnline = true,
                        IsVerified = false
                    }
                    );
                }
                catch (Exception ex)
                {
                    ex.Message.ShowErrorMessage();
                }
                return UserDbStatus.UserNotInDb;
            }
            else if (existing!.IsVerified != true && existing.Username == username)
            {
                var clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
                var clientPort = ((IPEndPoint)client.Client.RemoteEndPoint!).Port.ToString();

                existing.IpAddress = clientIpAddress;
                existing.Port = clientPort;
                await UserDal!.UpdateAsync(existing);
                return UserDbStatus.UserInDbNotVerified;
            }
            else if (existing.Username != username)
            {
                var stream = client.GetStream();
                var bw = new BinaryWriter(stream);
                bw.Write(":E1User with same email and different username already exists");
                client.Dispose();
                return UserDbStatus.UserInvalidCredentials;
            }
            else
            {
                var clientIpAddress = ((IPEndPoint)client.Client.RemoteEndPoint!).Address.ToString();
                var clientPort = ((IPEndPoint)client.Client.RemoteEndPoint!).Port.ToString();

                existing.IpAddress = clientIpAddress;
                existing.Port = clientPort;
                await UserDal!.UpdateAsync(existing);
                return UserDbStatus.UserIsVerified;

            }
        }
    }
}
