using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;

namespace SharpExec
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class ServiceControlHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private ServiceControlHandle()
            : base(true)
        {
        }
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            return NativeMethods.CloseServiceHandle(this.handle);
        }
    }
}

