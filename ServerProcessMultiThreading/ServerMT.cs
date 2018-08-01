using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerProcessMultiThreading
{
    class ServerMT
    {
        static void Main(string[] args)
        {
            List<Task> lstTasks = new List<Task>();
            lstTasks.Add(Task.Factory.StartNew(() => DoWork(1000)));
            lstTasks.Add(Task.Factory.StartNew(() => DoWork(3000)));
            lstTasks.Add(Task.Factory.StartNew(() => DoWork(4000)));

            Task.WaitAll(lstTasks.ToArray());
        }

        private static void DoWork(int sleepTime)
        {
            string queuePath = ".\\Private$\\josTst";

            Console.WriteLine("Thread {0} Runing...", Thread.CurrentThread.ManagedThreadId);

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
                        Thread.Sleep(sleepTime);

                        //simulate error in the process
                        //throw new Exception("Error Test");

                        tr.Commit();

                        Console.WriteLine("Read OK({0}): {1}, {2}, {3}", Thread.CurrentThread.ManagedThreadId, person.Id, person.Name, person.Birth);
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
