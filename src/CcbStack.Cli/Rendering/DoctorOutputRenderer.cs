using System.Text;
using CcbStack.Core.Checks;

namespace CcbStack.Cli.Rendering;

/// <summary>Renders a <see cref="DoctorReport"/> as human-readable terminal text.</summary>
internal static class DoctorOutputRenderer
{
    public static string ToText(DoctorReport report)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Doctor Report").AppendLine();

        foreach (var check in report.Checks)
        {
            var icon = check.Status switch
            {
                DoctorCheckStatus.Passed => "✔",
                DoctorCheckStatus.Warning => "⚠",
                DoctorCheckStatus.Failed => "✘",
                _ => "?",
            };

            var line = $"{icon} {check.Name}";
            if (check.Status != DoctorCheckStatus.Passed && check.Detail is not null)
            {
                line += $" — {check.Detail}";
            }

            builder.AppendLine(line);
        }

        return builder.ToString().TrimEnd();
    }
}
