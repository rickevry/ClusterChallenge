using System.Linq;
using System.Reflection;
using DAM2.Core.Shared.Interface;
using Google.Protobuf.Reflection;

namespace Worker1
{
    public class DescriptorProvider : IDescriptorProvider
    {
        public FileDescriptor[] GetDescriptors()
        {
			var descriptors = typeof(DescriptorProvider).Assembly.GetTypes()
				.Where(t => t.Name.EndsWith("Reflection"))
				.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static))
				.Where(pi => "Descriptor".Equals(pi.Name))
				.Select(pi =>
				{
					object? descriptor = pi.GetValue(null, null);
					if (descriptor is FileDescriptor fileDescriptor)
					{
						return fileDescriptor;
					}

					return default(FileDescriptor);
				})
				.Where(fd => fd != default(FileDescriptor))
				.ToArray();

			return descriptors;
        }
    }
}
