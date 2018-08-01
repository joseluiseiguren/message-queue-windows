using Newtonsoft.Json;
using System;
using System.Messaging;
using System.Text;

namespace ServerProcess
{
    class Server
    {
        static void Main(string[] args)
        {
            string queuePath = ".\\Private$\\josTst";

            while (true)
            {
                using (MessageQueue myQueue = new MessageQueue(queuePath))
                using (MessageQueueTransaction tr = new MessageQueueTransaction())
                {
                    try
                    {
                        tr.Begin();

                        //get message from the queue
                        var message = myQueue.Receive(tr);

                        //decode message
                        UTF8Encoding encoding = new UTF8Encoding();
                        byte[] buffer = new byte[message.BodyStream.Length];
                        message.BodyStream.Position = 0;
                        message.BodyStream.Read(buffer, 0, (int)message.BodyStream.Length);

                        //deserialize message
                        Person person = JsonConvert.DeserializeObject<Person>(Encoding.Default.GetString(buffer));

                        //simulate hard process
                        System.Threading.Thread.Sleep(7000);

                        //simulate error in the process
                        //throw new Exception("Error Test");

                        tr.Commit();

                        Console.WriteLine("Read OK: {0}, {1}, {2}", person.Id, person.Name, person.Birth);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR :-(");
                    }
                }
            }            
            
        }
    }
}
