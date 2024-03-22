using Core.Components;

namespace Foundation
{
    public class UnionInfo
    {
        public UnionInfo(FallingObject self, FallingObject other)
        {
            Other = other;
            Self = self;
        }
        public FallingObject Self { get; }
        public FallingObject Other { get; }
    }
}