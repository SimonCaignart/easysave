using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasySave
{
    class Server
    {

        // Listener socket
        Socket listener;
        // Client Socket
        Socket clientSocket;

        Save save;
        StateLog stateLog;
        public string saveName;
        public string destPath;
        public string srcPath;
        public string saveType;


        List<Thread> m_threads_list;

        public List<Thread> threads_list { get => m_threads_list; set => m_threads_list = value; }

        public Server()
        {
            save = new Save();
            stateLog = new StateLog();

            m_threads_list = new List<Thread>();

            // Establish the local endpoint  
            // for the socket. Dns.GetHostName 
            // returns the name of the host  
            // running the application. 
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation TCP/IP Socket using  
            // Socket Class Costructor 
            listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Using Bind() method we associate a 
            // network address to the Server Socket 
            // All client that will connect to this  
            // Server Socket must know this network 
            // Address 
            listener.Bind(localEndPoint);

            // Using Listen() method we create  
            // the Client list that will want 
            // to connect to Server 
            listener.Listen(10);

            Thread Server = new Thread(new ThreadStart(ExecuteServer));
            Server.IsBackground = true;
            Server.Start();
        }

        public void ExecuteServer()
        {
            try
            {
                while (true)
                {
                    // Suspend while waiting for 
                    // incoming connection Using  
                    // Accept() method the server  
                    // will accept connection of client 
                    clientSocket = listener.Accept();

                    // Data buffer 
                    byte[] bytes = new Byte[1024];
                    string data = null;
                    string[] mode;

                    while (true)
                    {

                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, numByte);

                        // An array of string, the first string is the mode (runSave,UpdateSaves),
                        // the second is the value that can be passed, the name of the save for exemple.
                        mode = data.Split(',');

                        // Send a response to the client according to the mode
                        switch (mode[0])
                        {
                            case "UpdateSaves":
                                updateSaves();
                                break;
                            case "RunSave":
                                runSave(mode[1]);
                                break;
                            case "RunAllSaves":
                                runAllSaves();
                                break;
                            case "PauseSave":
                                pauseSave(mode[1]);
                                break;
                            case "PlaySave":
                                playSave(mode[1]);
                                break;
                            case "StopSave":
                                stopSave(mode[1]);
                                break;
                        }
                        // Reset the data string
                        data = null;
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        public void updateSaves()
        {
            // Send a message to Client using Send() method

            string[] progresses = stateLog.GetAllProgresses();
            string[] saves = save.savejson.ListAllSaves();

            string savesprogress;
           
            if (saves.Length==0)
            {
                savesprogress = "";
            }
            else if (progresses.Length != 0 && saves.Length != 0 && progresses.Length == saves.Length)
            {
                var results = saves.Zip(progresses, (saves, progresses) => new { saves, progresses }).Where(x => x.saves != "").Select(x => x.saves + " - " + x.progresses);
                savesprogress = String.Join(",", results);
            }
            else
            {
                savesprogress = String.Join(",", saves);
            }
            
            byte[] message = Encoding.ASCII.GetBytes(savesprogress);

            clientSocket.Send(message);
        }

        private void runSave(string saveNameSocket)
        {
            string[] ListSave = save.savejson.ListAllSaves();

            int final = -1;

            for (int i = 0; i < ListSave.Length; i++)
            {
                if (saveNameSocket == ListSave[i])
                {
                    final = i;
                }
            }

            if (final != -1)
            {
                string[] SaveInformation = save.savejson.SaveInformation(final);

                saveName = SaveInformation[0];
                saveType = SaveInformation[1];
                srcPath = SaveInformation[2];
                destPath = SaveInformation[3];
                save.state.InsertBoolJson(1, saveName);

                Thread thread = new Thread(new ThreadStart(ThreadLoop));
                thread.Start();
                thread.Name = saveName;
                threads_list.Add(thread);
                Thread.Sleep(60);
            }
        }

        private void runAllSaves()
        {
            string[] ListSave = save.savejson.ListAllSaves();

            for (int i = 0; i < ListSave.Length; i++)
            {
                string[] SaveInformation = save.savejson.SaveInformation(i);

                saveName = SaveInformation[0];
                saveType = SaveInformation[1];
                srcPath = SaveInformation[2];
                destPath = SaveInformation[3];
                save.state.InsertBoolJson(1, saveName);

                Thread thread = new Thread(new ThreadStart(ThreadLoop));
                thread.Start();
                thread.Name = saveName;
                threads_list.Add(thread);
                Thread.Sleep(60);
            }
        }

        private void pauseSave(string saveName)
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                if (threads_list[i].Name == saveName.Split('(')[0].Trim())
                {
                    save.state.InsertBoolJson(2, threads_list[i].Name);
                }
            }
        }

        private void playSave(string saveName)
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                if (threads_list[i].Name == saveName.Split('(')[0].Trim())
                {
                    save.state.InsertBoolJson(1, threads_list[i].Name);
                }
            }
        }

        private void stopSave(string saveName)
        {
            for (int i = 0; i < threads_list.Count; i++)
            {
                if (threads_list[i].Name == saveName.Split('(')[0].Trim())
                {
                    save.state.InsertBoolJson(3, threads_list[i].Name);
                }
            }
        }

        public void ThreadLoop()
        {
            if (save.saveType == "differential")
            {
                save.DifferentialSave(srcPath, destPath);
            }
            else
            {
                save.CompleteSave(saveName, srcPath, destPath);
            }
        }
    }
}