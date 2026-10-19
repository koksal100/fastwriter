namespace fastwriter.Services;

/// <summary>
/// Sınav metniyle ilgili sabitler ve metni yeterli uzunluğa getiren yardımcılar.
/// Hem istemciye metin gönderirken hem de skoru sunucuda doğrularken aynı kurallar kullanılır.
/// </summary>
public static class ExamContent
{
    public const int DurationSeconds = 180;

    public const int TargetWordCount = 90;

    private const int MinimumLength = 2200;

    public static string Build(string? content)
    {
        content = (content ?? string.Empty).Trim();

        if (content.Length == 0)
        {
            return string.Empty;
        }

        if (content.Length >= MinimumLength)
        {
            return content;
        }

        var builder = new System.Text.StringBuilder(content);

        while (builder.Length < MinimumLength)
        {
            builder.Append(' ').Append(content);
        }

        return builder.ToString();
    }
}
