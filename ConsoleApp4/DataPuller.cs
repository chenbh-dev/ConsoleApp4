using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public class DataPuller
    {
        CancellationTokenSource tk;
        public DataPuller()
        {
        }
        public void Start(List<Func<RawBusMessage,bool>> actions)
        {
            
            tk = new CancellationTokenSource();
            Task.Factory.StartNew(() =>Pull(actions, tk.Token), TaskCreationOptions.LongRunning).Wait();
        }
        public void Stop()
        {
            if(tk != null)
            tk.Cancel();
        }
        public void Pull(List<Func<RawBusMessage, bool>> actions, CancellationToken token)
        {
            int counter = 0;
            while (!token.IsCancellationRequested)
            {
                var list = new List<byte>();
                for (int i = 0; i < 200; i++)
                {
                    list.Add(Convert.ToByte(i + 1));
                }

                foreach (var action in actions)
                {
                   action?.Invoke(new RawBusMessage(counter++, DateTime.Now, list));
                }         
                Thread.Sleep(50);
            }
        }
    }
}
