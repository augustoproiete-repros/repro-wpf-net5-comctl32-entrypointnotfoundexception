using System;
using System.IO;
using System.Runtime.InteropServices;

namespace TaskDialogApi
{
    internal sealed class ComCtlv6ActivationContext : IDisposable 
    {
        private IntPtr _cookie;
        private static ActCtx _enableThemingActivationContext;
        private static ActivationContextSafeHandle _activationContext;
        private static bool _contextCreationSucceeded;
        private static readonly object _contextCreationLock = new object();

        public ComCtlv6ActivationContext(bool enable)
        {
            if (!enable || !EnvironmentOS.IsWindowsXPOrLater) return;

            if (!EnsureActivateContextCreated()) return;

            if (!ActivateActCtx(_activationContext, out _cookie))
            {
                // Be sure cookie always zero if activation failed
                _cookie = IntPtr.Zero;
            }
        }

        ~ComCtlv6ActivationContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ReSharper disable once UnusedParameter.Local
        private void Dispose(bool disposing)
        {
            if (_cookie == IntPtr.Zero) return;

            if (DeactivateActCtx(0, _cookie))
            {
                // deactivation succeeded...
                _cookie = IntPtr.Zero;
            }
        }

        private static bool EnsureActivateContextCreated()
        {
            lock (_contextCreationLock)
            {
                if (!_contextCreationSucceeded)
                {
                    // Pull manifest from the .NET Framework install
                    // directory

                    var assemblyLoc = typeof(object).Assembly.Location;

                    string manifestLoc = null;
                    var installDir = string.Empty;
                    if (!string.IsNullOrWhiteSpace(assemblyLoc))
                    {
                        installDir = Path.GetDirectoryName(assemblyLoc) ?? string.Empty;
                        const string manifestName = "XPThemes.manifest";
                        manifestLoc = Path.Combine(installDir, manifestName);
                    }

                    if (!string.IsNullOrWhiteSpace(manifestLoc) && !string.IsNullOrWhiteSpace(installDir))
                    {
                        _enableThemingActivationContext = new ActCtx
                        {
                            cbSize = Marshal.SizeOf(typeof(ActCtx)),
                            lpSource = manifestLoc,
                            lpAssemblyDirectory = installDir,
                            dwFlags = ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID,
                        };

                        // Set the lpAssemblyDirectory to the install
                        // directory to prevent Win32 Side by Side from
                        // looking for comctl32 in the application
                        // directory, which could cause a bogus dll to be
                        // placed there and open a security hole.

                        // Note this will fail gracefully if file specified
                        // by manifestLoc doesn't exist.
                        _activationContext = CreateActCtx(ref _enableThemingActivationContext);
                        _contextCreationSucceeded = !_activationContext.IsInvalid;
                    }
                }

                // If we return false, we'll try again on the next call into
                // EnsureActivateContextCreated(), which is fine.
                return _contextCreationSucceeded;
            }
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern ActivationContextSafeHandle CreateActCtx(ref ActCtx actCtx);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ActivateActCtx(ActivationContextSafeHandle hActCtx, out IntPtr lpCookie);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeactivateActCtx(uint dwFlags, IntPtr lpCookie);

        // ReSharper disable InconsistentNaming
        private struct ActCtx
        {
#pragma warning disable 649
            // ReSharper disable NotAccessedField.Local
            // ReSharper disable UnusedMember.Global
            public int cbSize;
            public uint dwFlags;
            public string lpSource;
            public ushort wProcessorArchitecture;
            public ushort wLangId;
            public string lpAssemblyDirectory;
            public string lpResourceName;
            public string lpApplicationName;
            // ReSharper restore UnusedMember.Global
            // ReSharper restore NotAccessedField.Local
#pragma warning restore 649
        }
        // ReSharper restore InconsistentNaming

        // ReSharper disable once InconsistentNaming
        private const int ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID = 0x004;
    }
}
