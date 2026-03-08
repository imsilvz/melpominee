using Melpominee.app.Models.Characters.VTMV5;
namespace Melpominee.app.Models.Web.VTMV5;

public class CharacterUpdateWrapper<ICharacterUpdate>
{
    public string? UpdateId { get; set; }
    public ICharacterUpdate? UpdateData { get; set; }
}

public class VampireCharacterResponse
{
    public bool Success { get; set; } = false;
    public string? Error { get; set; }
    public VampireV5Character? Character { get; set; }
}

public class VampireCharacterListResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<VampireV5Header>? CharacterList { get; set; }
}

public class VampireCharacterCreateResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int? CharacterId { get; set; }
}

public class VampireHeaderResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Header? Character { get; set; }
}

public class VampireAttributesResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Attributes? Attributes { get; set; }
}

public class VampireSkillsResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Skills? Skills { get; set; }
}

public class VampireStatResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5SecondaryStats? Stats { get; set; }
}

public class VampireDisciplinesResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Disciplines? Disciplines { get; set; }
}

public class VampirePowersResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<string>? Powers { get; set; }
}

public class VampireBeliefsResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Beliefs? Beliefs { get; set; }
}

public class VampireBackgroundMeritFlawResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5BackgroundMeritFlaw? Backgrounds { get; set; }
    public VampireV5BackgroundMeritFlaw? Merits { get; set; }
    public VampireV5BackgroundMeritFlaw? Flaws { get; set; }
}

public class VampireProfileResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public VampireV5Profile? Profile { get; set; }
}