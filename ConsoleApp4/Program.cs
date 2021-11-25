using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp4
{
 
    public class MethodsHolder
    {
        public async Task Publish(DecodedMessage message)
        {
            Console.WriteLine(message.ToString());
        }
        public async Task Publish2(DecodedMessage message)
        {
            Console.WriteLine(message.ToString());
        }
        public IEnumerable<DecodedMessage> Decode(IEnumerable<RawBusMessage> reading)
        {
            var list = new List<DecodedMessage>();
           return  InnerDecode(ref list);
           
        }
        private  IEnumerable<DecodedMessage> InnerDecode(ref List<DecodedMessage> decodedMessages)
        {
            for (int i = 0; i < 1000; i++)
            {
                decodedMessages.Add(new DecodedMessage()
                {
                    Label = "asd",
                    ReadingTime = DateTime.Now,
                    Source = "blabla",
                    Unit = "asdasdasdasd",
                    Value = i,
                    Counter = i + 2
                });                             
            }
            return decodedMessages;
        }
        public IEnumerable<RawBusMessage> BuildMessage(RawBusMessage rawMEssage)
        {
            var firstHalf = rawMEssage.Data.Skip(rawMEssage.Data.Length / 2).Take(rawMEssage.Data.Length / 2);
            var secondhalf = rawMEssage.Data.Take(rawMEssage.Data.Length / 2);
            var list = new List<RawBusMessage>();
            list.Add(new RawBusMessage(rawMEssage.Counter, rawMEssage.ReadingTime, firstHalf));
            list.Add(new RawBusMessage(rawMEssage.Counter, rawMEssage.ReadingTime, secondhalf));
            return list;
        }
    }

    public class DecodedMessage
    {
        public string Unit { get; set; }
        public double Value { get; set; }
        public string Source { get; set; }
        public string Label { get; set; }
        public DateTime ReadingTime { get; set; }
        public double Counter { get; set; }


        public override string ToString()
        {
            return Unit + Value + Source + Label + ReadingTime.ToString() + Counter;
        }
    }
    public class RawBusMessage
    {
        public int Counter { get; set; }
        public byte[] Data { get; set; }
        public DateTime ReadingTime { get; set; }

        public RawBusMessage(int counter ,DateTime date , IEnumerable<byte> data)
        {
            Counter = counter;
            ReadingTime = date;
            Data = data.ToArray();
        }
    }
    public class RoutedMessage
    {
        public RoutedMessage(int routeKey, DecodedMessage message)
        {
            RouteKey = routeKey;
            Message = message;
        }

        public int RouteKey { get; set; }
        public DecodedMessage Message { get; set; }
    }

    
    
   
    class Program
    {
        static void Main(string[] args)
        {
            StationManager di = new StationManager();
                di.Start();
        }

 
    }
}
