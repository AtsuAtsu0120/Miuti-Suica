using Foundation;
using MessagePipe;
using VContainer;

namespace Core.LifetimeScope
{
    public class GameLifetimeScope : VContainer.Unity.LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // MessagePipeの初期化
            var options = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<UnionInfo>(options);
        }
    }
}