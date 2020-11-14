using System;
using System.Runtime.InteropServices;

namespace TaskDialogApi
{
    public class TaskDialog
    {
        public void ShowDialog(IntPtr owner)
        {
            var config = new TaskDialogConfig
            {
                pszWindowTitle = "Task dialog sample",
                pszMainInstruction = "This is an example task dialog.",
                pszContent = "(Content goes here)",
                pszExpandedInformation = "(Expanded information goes here)",
                pszFooterText = "(Footer text goes here)",
                hwndParent = owner,
                dwCommonButtons = TaskDialogCommonButtonFlags.OkButton | TaskDialogCommonButtonFlags.CancelButton,
                dwFlags = TaskDialogFlags.NoDefaultRadioButton,
                cbSize = (uint)Marshal.SizeOf(typeof(TaskDialogConfig))
            };

            using (new ComCtlv6ActivationContext(enable: true))
            {
                TaskDialogIndirect(ref config, out _, out _, out _);
            }
        }

        [DllImport("comctl32.dll", PreserveSig = false)]
        private static extern void TaskDialogIndirect([In] ref TaskDialogConfig pTaskConfig, out int pnButton,
            out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TaskDialogConfig
    {
        public uint cbSize;
        public IntPtr hwndParent;
        public IntPtr hInstance;
        public TaskDialogFlags dwFlags;
        public TaskDialogCommonButtonFlags dwCommonButtons;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszWindowTitle;
        public IntPtr hMainIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszMainInstruction;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszContent;
        public uint cButtons;
        public IntPtr pButtons;
        public int nDefaultButton;
        public uint cRadioButtons;
        public IntPtr pRadioButtons;
        public int nDefaultRadioButton;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszVerificationText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedInformation;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedControlText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszCollapsedControlText;
        public IntPtr hFooterIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszFooterText;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TaskDialogCallback pfCallback;
        public IntPtr lpCallbackData;
        public uint cxWidth;
    }

    [Flags]
    internal enum TaskDialogFlags
    {
        EnableHyperLinks = 0x0001,
        UseHIconMain = 0x0002,
        UseHIconFooter = 0x0004,
        AllowDialogCancellation = 0x0008,
        UseCommandLinks = 0x0010,
        UseCommandLinksNoIcon = 0x0020,
        ExpandFooterArea = 0x0040,
        ExpandedByDefault = 0x0080,
        VerificationFlagChecked = 0x0100,
        ShowProgressBar = 0x0200,
        ShowMarqueeProgressBar = 0x0400,
        CallbackTimer = 0x0800,
        PositionRelativeToWindow = 0x1000,
        RtlLayout = 0x2000,
        NoDefaultRadioButton = 0x4000,
        CanBeMinimized = 0x8000
    }

    [Flags]
    internal enum TaskDialogCommonButtonFlags
    {
        OkButton = 0x0001, // selected control return value IDOK
        YesButton = 0x0002, // selected control return value IDYES
        NoButton = 0x0004, // selected control return value IDNO
        CancelButton = 0x0008, // selected control return value IDCANCEL
        RetryButton = 0x0010, // selected control return value IDRETRY
        CloseButton = 0x0020  // selected control return value IDCLOSE
    }

    public delegate uint TaskDialogCallback(IntPtr hwnd, uint uNotification, IntPtr wParam, IntPtr lParam, IntPtr dwRefData);
}
