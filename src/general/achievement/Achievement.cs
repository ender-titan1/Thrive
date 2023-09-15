using Godot;
using Newtonsoft.Json;

/// <summary>
///   Definition of an achievement
/// </summary>
public class Achievement : IRegistryType
{
#pragma warning disable 169,649 // Used through reflection
    private string? untranslatedName;
    private string? untranslatedDescription;
#pragma warning restore 169,649

    [JsonProperty]
    [TranslateFrom(nameof(untranslatedName))]
    public string Name { get; private set; } = null!;

    [JsonProperty]
    [TranslateFrom(nameof(untranslatedDescription))]
    public string Description { get; private set; } = null!;

    [JsonProperty]
    public int RequiredProgressPoints { get; private set; }

    [JsonProperty]
    public bool ProgressPersistsBetweenGames { get; private set; }

    [JsonProperty]
    public string IconPath { get; private set; } = string.Empty;

    [JsonIgnore]
    public Texture? LoadedIcon { get; private set; }

    [JsonIgnore]
    public string InternalName { get; set; } = null!;

    public void Check(string name)
    {
        if (string.IsNullOrEmpty(Name))
            throw new InvalidRegistryDataException(name, GetType().Name, "Name is not set");

        if (string.IsNullOrEmpty(Description))
            throw new InvalidRegistryDataException(name, GetType().Name, "Description is not set");

        if (string.IsNullOrEmpty(IconPath))
            throw new InvalidRegistryDataException(name, GetType().Name, "IconPath is missing");

        if (RequiredProgressPoints <= 0)
        {
            throw new InvalidRegistryDataException(name, GetType().Name,
                "RequiredProgressPoints should be positive and non-zero");
        }

        TranslationHelper.CopyTranslateTemplatesToTranslateSource(this);
    }

    public void Resolve()
    {
        if (!string.IsNullOrEmpty(IconPath))
            LoadedIcon = GD.Load<Texture>(IconPath);
    }

    public void ApplyTranslations()
    {
        TranslationHelper.ApplyTranslations(this);
    }
}
