using Newtonsoft.Json;
using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Timers;

namespace ClientePush
{
    class Client
    {
        private static int _personId = 0;

        static void Main(string[] args)
        {
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 500;
            aTimer.Enabled = true;
            aTimer.Start();

            Console.ReadKey();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _personId++;

            var person = new Person()
            {
                Birth = DateTime.Now,
                Id = _personId,
                Name = "Pedro"
            };

            WriteMessage(person);
        }

        private static void WriteMessage(Person person)
        {
            string queuePath = ".\\Private$\\josTst";

            //si no existe la cola, la creo
            if (!MessageQueue.Exists(queuePath))
            {
                //se crea del tipo transaccional
                MessageQueue.Create(queuePath, true);
            }

            //escribe un mensaje en la cola
            using (MessageQueue myQueue = new MessageQueue(queuePath))
            {
                //serialize to json
                string json = JsonConvert.SerializeObject(person);

                //la escritura es transaccional
                using (MessageQueueTransaction tr = new MessageQueueTransaction())
                using (Message message = new Message())
                {
                    message.BodyStream = new MemoryStream(Encoding.ASCII.GetBytes(json));

                    tr.Begin();
                    myQueue.Send(message, tr);
                    tr.Commit();
                    Console.WriteLine("Push OK - PersonId {0}", _personId);
                }
            }
        }
    }
}
