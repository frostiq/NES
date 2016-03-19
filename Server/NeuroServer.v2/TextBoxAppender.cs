using System.Windows.Controls;
using NLog;
using NLog.Targets;

namespace NeuroServer
{
    [Target("TextBoxAppender")]
    public sealed class TextBoxAppender : TargetWithLayout
    {
        public static TextBox TextBox;

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = this.Layout.Render(logEvent);

            TextBox.Text += logMessage + '\n';
        }
    }
}
