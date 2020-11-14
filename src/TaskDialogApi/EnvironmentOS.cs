using System;

namespace TaskDialogApi
{
    internal static class EnvironmentOS
    {
        public static bool IsWindowsXPOrLater => Environment.OSVersion.Platform == PlatformID.Win32NT &&
                                                 Environment.OSVersion.Version >= new Version(5, 1, 2600);
    }
}
