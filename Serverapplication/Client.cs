using System;
using System.Net.Sockets;
using System.Text;

namespace Serverapplication
{
    public enum Networkstatus
    {
        RIGHT,
        ERROR,

    }

    class Client
    {
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream myStream;
        private byte[] readBuff;


        public void Start()
        {

            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;
            myStream = Socket.GetStream();
            Array.Resize(ref readBuff, Socket.ReceiveBufferSize);
            myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnLogin, null);
            SendData("Client started");


        }

        void OnLogin(IAsyncResult result)
        {
            try
            {
                int readBytes = myStream.EndRead(result);
                if (Socket == null)
                {
                    return;
                }

                if (readBytes <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = null;
                Array.Resize(ref newBytes, readBytes);
                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readBytes);

                //HandleData

                string ss = Encoding.ASCII.GetString(newBytes);
                Console.WriteLine(ss);
                string[] verif = (Encoding.ASCII.GetString(newBytes)).Split(';');
                if (verif[0][0] == 'r')
                {
                    if (DAO.inserir(verif[1], verif[2]) == Networkstatus.RIGHT)
                    {
                        SendData(Networkstatus.RIGHT.ToString() +";" + Index);
                        myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
                        
                    }
                    else
                    {
                        CloseConnection();
                    }

                }
                else if (verif[0][0] == 'l')
                {

                    if (DAO.login(verif[1], verif[2]) == Networkstatus.RIGHT)
                    {
                        SendData(Networkstatus.RIGHT.ToString()+ ";"+ Index);
                        myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
                    }
                    else
                    {
                        CloseConnection();
                    }
                }
                else
                {

                    CloseConnection();
                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);


            }

        }

        void SendData(string message)
        {
            byte[] byts = Encoding.ASCII.GetBytes(message);
            myStream.WriteAsync(byts, 0, byts.Length);
        }

        void CloseConnection()
        {
            SendData(Networkstatus.ERROR.ToString());
            Socket.Close();
            Socket = null;
            Console.WriteLine("Player disconnected :" + IP);


        }

        void OnReceiveData(IAsyncResult result)// iasyn sao os bytes,
        {
            try
            {
                int readBytes = myStream.EndRead(result);
                if (Socket == null)
                {
                    return;
                }

                if (readBytes <= 0)
                {
                    CloseConnection();
                    return;
                }

                byte[] newBytes = null;
                Array.Resize(ref newBytes, readBytes);
                Buffer.BlockCopy(readBuff, 0, newBytes, 0, readBytes);

                //HandleData

                string s = Encoding.ASCII.GetString(newBytes);

               


                if (s.Contains("ranking"))
                {
                    SendData("ranking;" + DAO.pontos());
                    
                }
                else
                {

                    for (int i = 0; i < Network.Clients.Length; i++)
                    {
                        if (Index != i)
                        {
                            if (Network.Clients[i].Socket != null)
                            {

                                Console.WriteLine(s);

                                Network.Clients[i].SendData(s);
                            }
                        }
                    }
                }

                if (Socket == null)
                {
                    return;
                }

                myStream.BeginRead(readBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception ex)
            {
                CloseConnection();
                return;
            }
        }
    }
}
