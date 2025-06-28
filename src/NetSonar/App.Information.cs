using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia.Platform;
using NetSonar.Avalonia.Extensions;
using Updatum;
using ZLinq;

namespace NetSonar.Avalonia;

public partial class App
{
    #region Utilities

    public static string Software => EntryApplication.AssemblyProduct!;
    public static string VersionArch => $"{EntryApplication.AssemblyVersionString} {RuntimeInformation.ProcessArchitecture}";
    public static string SoftwareWithVersion => $"{Software} v{EntryApplication.AssemblyVersionString}";
    public static string SoftwareWithVersionArch => $"{Software} v{VersionArch}";
    public static string SoftwareWithVersionRuntime => $"{SoftwareWithVersion} ({RuntimeInformation.RuntimeIdentifier})";

    [field: AllowNull, MaybeNull]
    public static string Authors
    {
        get
        {
            field ??= Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyMetadataAttribute>()
                .AsValueEnumerable()
                .FirstOrDefault(attribute => attribute.Key == "Authors")?.Value ?? string.Empty;
            return field;
        }
    }

    public const string License = $"GNU Affero General Public License v3.0 ({LicenseShort})";
    public const string LicenseShort = "AGPLv3";
    public static string LicenseUrl => $"{EntryApplication.AssemblyRepositoryUrl}/blob/master/LICENSE";
    public const string Country = "Portugal";
    public const string CountryShort = "PT";

    public const string DonateGitHubUrl = "https://github.com/sponsors/sn4k3";
    public const string DonatePayPalUrl = "https://www.paypal.com/donate/?hosted_button_id=3F9DKDNPWEYR6";

    public static string ApplicationPath => AppContext.BaseDirectory;
    public static string AppExecutable => Environment.ProcessPath!;


    [field: AllowNull, MaybeNull]
    public static string LicenseText
    {
        get
        {
            if (field is null)
            {
                Uri textFileUri = new($"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Assets/LICENSE");
                using StreamReader streamReader = new(AssetLoader.Open(textFileUri));
                field = streamReader.ReadToEnd();
            }

            return field;
        }
    }

    [field: AllowNull, MaybeNull]
    public static string TermsOfUseText
    {
        get
        {
            if (field is null)
            {
                Uri textFileUri = new($"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Assets/TermsOfUse.md");
                using StreamReader streamReader = new(AssetLoader.Open(textFileUri));
                field = streamReader.ReadToEnd();
            }

            return field;
        }
    }


    /// <summary>
    /// Gets born date and time
    /// </summary>
    public static DateTime Born => DateTime.SpecifyKind(new(2025, 7, 1, 17, 00, 00), DateTimeKind.Utc);

    /// <summary>
    /// Gets years
    /// </summary>
    public static int YearsOld => Born.Age();

    /// <summary>
    /// Return full age in a readable string
    /// </summary>
    public static string AgeStr
    {
        get
        {
            var sb = new StringBuilder($"{YearsOld} years");
            var born = Born;
            var now = DateTime.UtcNow;

            var months = 12 + now.Month - born.Month + (now.Day >= born.Day ? 0 : -1);
            if (months >= 12) months -= 12;
            if (months > 0)
            {
                sb.Append($", {months} month");
                if (months > 1) sb.Append("(s)");
            }

            var days = 31 + now.Day - born.Day;
            if (days >= 31) days -= 31;
            if (days > 0)
            {
                sb.Append($", {days} day");
                if (days > 1) sb.Append("(s)");
            }

            var hours = 12 + now.Hour - born.Hour;
            if (hours >= 12) hours -= 12;
            if (hours > 0)
            {
                sb.Append($", {hours} hour");
                if (hours > 1) sb.Append("(s)");
            }

            var minutes = 60 + now.Minute - born.Minute;
            if (minutes >= 60) minutes -= 60;
            if (minutes > 0)
            {
                sb.Append($", {minutes} minutes");
                if (minutes > 1) sb.Append("(s)");
            }

            var seconds = 60 + now.Second - born.Second;
            if (seconds >= 60) seconds -= 60;
            if (seconds > 0)
            {
                sb.Append($", {seconds} seconds");
                if (seconds > 1) sb.Append("(s)");
            }


            return sb.ToString();
        }
    }

    /// <summary>
    /// Return full age in a readable string
    /// </summary>
    public static string AgeShortStr
    {
        get
        {
            var sb = new StringBuilder($"{YearsOld} years");
            var born = Born;
            var now = DateTime.UtcNow;

            var months = 12 + now.Month - born.Month + (now.Day >= born.Day ? 0 : -1);
            if (months >= 12) months -= 12;
            if (months > 0)
            {
                sb.Append($", {months} month");
                if (months > 1) sb.Append("(s)");
            }

            var days = 31 + now.Day - born.Day;
            if (days >= 31) days -= 31;
            if (days > 0)
            {
                sb.Append($", {days} day");
                if (days > 1) sb.Append("(s)");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// Checks if today is birthday
    /// </summary>
    public static bool IsBirthday
    {
        get
        {
            var born = Born;
            var now = DateTime.UtcNow;
            return born.Month == now.Month && born.Day == now.Day;
        }
    }

    /// <summary>
    /// Checks if today is birthday
    /// </summary>
    public static bool IsBirthdayWithin7Days => IsBirthdayWithOffset(7);

    /// <summary>
    /// Checks if today is birthday within some days range
    /// </summary>
    /// <param name="daysOffset">Number of positive days from birthday date which is still considered as birthday</param>
    /// <returns></returns>
    public static bool IsBirthdayWithOffset(byte daysOffset)
    {
        var born = Born;
        var now = DateTime.UtcNow;
        return born.Month == now.Month && (born.Day == now.Day || (now.Day >= born.Day && now.Day <= born.Day + daysOffset));
    }

    public static string BirthdayTitle => $"\ud83c\udf89\ud83c\udf82 Happy {YearsOld}th Birthday, {Software}! \ud83c\udf82\ud83c\udf89";
    public static string BirthdayMessage => string.Format(
        "Dear Resin Printing Enthusiasts,\r\n\r\nToday marks a special milestone as we celebrate the {1}th birthday of {0}, your trusted companion in the world of resin printing! \ud83e\udd73\ud83c\udf89 We're thrilled to have been part of your journey, ensuring smooth and flawless prints every step of the way.\r\n\r\nOver the past {1} years, {0} has been your go-to solution for checking and fixing files, ensuring that your prints are always top-notch. From detecting potential problems to offering solutions with a bunch of powerful tools, {0} has been by your side, making your printing experience more hassle-free and enjoyable.\r\n\r\nAs we commemorate this occasion, we want to express our deepest gratitude to all of you for your unwavering support and feedback. Your passion for resin printing fuels our commitment to innovation and excellence, driving us to continuously improve {0} to meet your evolving needs.\r\n\r\nOn this special day, we celebrate not just the software, but also the vibrant community that surrounds it. Your creativity, expertise, and camaraderie have been instrumental in shaping {0} into the powerful tool it is today.\r\n\r\nAs a token of appreciation, we invite you to consider making a donation to support the ongoing development and maintenance of {0}. Your contributions help ensure that {0} remains free and accessible to resin printing enthusiasts worldwide. You can easily make a donation by accessing the main menu, navigating to \"Help\", and selecting \"Donate\".\r\n\r\nAdditionally, don't forget to join our community forums to share your success stories with {0}! Whether it's showcasing your remarkable prints or sharing how {0} has helped you overcome challenges, we'd love to hear from you. You can find the community forums under the main menu, navigating to \"Help\", and selecting \"Community forums\".\r\n\r\nPlease note that this message will show only once a year on {0}'s birthday, so don't miss this opportunity to show your support and share your achievements!\r\n\r\nHappy {1}th Birthday, {0}! Here's to another year of innovation, creativity, and flawless prints! \ud83d\ude80\ud83c\udf88\r\n\r\nWarm regards,\r\n{0}",
        Software, YearsOld);

    #endregion

    public static CultureInfo OptimalCultureInfo
    {
        get
        {
            var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
            cultureInfo.NumberFormat.CurrencyGroupSeparator = CultureInfo.InvariantCulture.NumberFormat.CurrencyGroupSeparator;
            cultureInfo.NumberFormat.NumberDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            cultureInfo.NumberFormat.NumberGroupSeparator = CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator;
            cultureInfo.NumberFormat.PercentDecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.PercentDecimalSeparator;
            cultureInfo.NumberFormat.PercentGroupSeparator = CultureInfo.InvariantCulture.NumberFormat.PercentGroupSeparator;
            return cultureInfo;
        }
    }
}