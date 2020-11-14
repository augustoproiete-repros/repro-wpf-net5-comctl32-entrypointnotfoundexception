using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace TaskDialogApi
{
#pragma warning disable SYSLIB0003 // Type or member is obsolete
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
#pragma warning restore SYSLIB0003 // Type or member is obsolete
    internal class ActivationContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ActivationContextSafeHandle()
            : base(true)
        {
        }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        protected override bool ReleaseHandle()
        {
            ReleaseActCtx(handle);
            return true;
        }

#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [DllImport("kernel32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        private static extern void ReleaseActCtx(IntPtr hActCtx);
    }
}
