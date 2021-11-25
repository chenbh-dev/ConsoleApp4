using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp4
{
    public class ProcessingPipeline
    {

        MethodsHolder methodsHolder = new MethodsHolder();
        TransformBlock<RawBusMessage, IEnumerable<RawBusMessage>> writeRawMessageBlock;
        TransformManyBlock<IEnumerable<RawBusMessage>, DecodedMessage> decoderBlock;
        BroadcastBlock<DecodedMessage> broadcast;
        ActionBlock<DecodedMessage> realTimeFeedBlock, realTimeFeedBlock2;

        public Func<RawBusMessage, bool> PushTOBufferMethod => writeRawMessageBlock.Post;
        public async Task StartPipelineAsync(CancellationToken token)
        {
 
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var largeBufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 60000 };
            var realTimeBufferOptions = new ExecutionDataflowBlockOptions() { BoundedCapacity = 6000 };

            writeRawMessageBlock = new TransformBlock<RawBusMessage, IEnumerable<RawBusMessage>>((RawBusMessage msg) =>
            methodsHolder.BuildMessage(msg), largeBufferOptions);


            decoderBlock = new TransformManyBlock<IEnumerable<RawBusMessage>, DecodedMessage>(
               (IEnumerable<RawBusMessage> msg) => methodsHolder.Decode(msg), largeBufferOptions);

            broadcast = new BroadcastBlock<DecodedMessage>(msg => msg);

            realTimeFeedBlock = new ActionBlock<DecodedMessage>(
               (DecodedMessage msg) => methodsHolder.Publish(msg), realTimeBufferOptions);

            realTimeFeedBlock2 = new ActionBlock<DecodedMessage>(
               (DecodedMessage msg) => methodsHolder.Publish2(msg), realTimeBufferOptions);

            // link the blocks to together
            writeRawMessageBlock.LinkTo(decoderBlock, linkOptions);
            decoderBlock.LinkTo(broadcast, linkOptions);
            broadcast.LinkTo(realTimeFeedBlock, linkOptions);
            broadcast.LinkTo(realTimeFeedBlock2, linkOptions);


            await Task.WhenAll(realTimeFeedBlock.Completion);
        }
    }
}
