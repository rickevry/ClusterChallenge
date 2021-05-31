using Google.Protobuf.Reflection;

namespace DAM2.Core.Shared.Interface
{
    public interface IDescriptorProvider
    {
        FileDescriptor[] GetDescriptors();
    }
}
