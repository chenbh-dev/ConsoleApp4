using ConsoleApp4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    public class StationManager
    {
        readonly List<ProcessingPipeline> _pipes;
        readonly DataPuller dp;

        public StationManager()
        {
           
            dp = new DataPuller();
            _pipes = new List<ProcessingPipeline>();
            for (int i = 0; i < 10; i++)
            {
                _pipes.Add(new ProcessingPipeline());
            }
        }
        public void Stop()
        {
            dp.Stop();
        }
        public void Start()
        {
            var list = new List<Func<RawBusMessage, bool>>();
            foreach (var pipe in _pipes)
            {
                pipe.StartPipelineAsync(new System.Threading.CancellationToken());
                list.Add(pipe.PushTOBufferMethod);
            }
            dp.Start(list);
        }
    }
}
