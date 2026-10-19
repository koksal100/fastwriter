namespace fastwriter.Services;

/// <summary>
/// Test oturumunun süre kuralları. İstemciden gelen süreye güvenilmez.
/// </summary>
public static class TestSessionRules
{
    /// <summary>Oturumun açıldıktan sonra en fazla kullanılabileceği süre.</summary>
    public static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(10);

    /// <summary>Skor kaydı için gereken en kısa yazma süresi.</summary>
    public const int MinimumDurationSeconds = 3;

    /// <summary>180 saniyeye ulaşan denemelerde ağ gecikmesi için tanınan tolerans.</summary>
    public const double FullExamToleranceSeconds = 2;

    /// <summary>Tam sınav bitiminden sonra ağ gecikmesi için tanınan ek süre.</summary>
    public const int MaximumGraceSeconds = 15;
}
