using Microsoft.EntityFrameworkCore;
using MiniChattingApp.DataBaseRelated.Data;
using MiniChattingApp.DataBaseRelated.DataAccess.Concrete.EntityFramework;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using MiniChattingApp.DataBaseRelated.Service.Concrete;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        public static List<User>? Users { get; set; }

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
                var json = JsonConvert.SerializeObject(Users);
                bw.Write(json);

                while (true)
                {
                    string msg = br.ReadString();
                    //Console.Write($"User \"{username}\": ", Console.ForegroundColor = ConsoleColor.Yellow);
                    if (Helper.IsValidJson(msg))
                    {
                        var chat = JsonConvert.DeserializeObject<Chat>(msg);
                        var senderEmail = chat!.SenderEmail;
                        var content = chat.Content;
                        var receiverEmail = chat.ReceiverEmail;

                        await StartChat(senderEmail!, content!, receiverEmail!);
                    }
                    else    if (msg == "_users")
                    {
                        bw = new BinaryWriter(stream);
                        json = JsonConvert.SerializeObject(Users);
                        bw.Write(json);
                    }
                    else if (msg == "_chatHistory")
                    {
                        var messages = MessageDal!.GetAllAsync();
                        json = JsonConvert.SerializeObject(messages);
                        bw.Write(json);
                    }
                    Console.ResetColor();
                    Console.WriteLine(msg);
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

        private static async Task StartChat(string senderEmail, string content, string receiverEmail)
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
                IsRead = false
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
                return UserDbStatus.UserIsVerified;

            }
        }
    }
}
