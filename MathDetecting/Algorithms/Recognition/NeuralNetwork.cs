using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace MathDetecting.Algorithms.Recognition
{
    // Remove static if it will be needed or create aditional classes
    internal static class NeuralNetwork
    {

        // адрес и порт сервера, к которому будем подключаться
        const int port = 8005; // порт сервера
        const string address = "127.0.0.1"; // адрес сервера
        public static char Recognize(Bitmap image)
        {
            //char symbol = '0';
            byte[,] image_array = new byte[50,50];
            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                    image_array[i,j] = (byte)((Segmentation.Segmentation.GetBrightness((image.GetPixel(i,j))) > 120) ? 1: 0);

            return GetSymbol(image_array.ToString());
        }

        private static char GetSymbol(string message)
        {
            char answer = Convert.ToChar(null);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
               
                // подключаемся к удаленному хосту
                socket.Connect(ipPoint);
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket.Send(data);
 
                // получаем ответ
                data = new byte[10]; // буфер для ответа
                StringBuilder builder = new StringBuilder();
                int bytes = 0; // количество полученных байт
 
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (socket.Available > 0);
                answer = Convert.ToChar(builder.ToString());
                
                socket.Send(Encoding.Unicode.GetBytes("\0"));
                // закрываем сокет
                
            }
            catch(Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            return answer;
        }
    }
}