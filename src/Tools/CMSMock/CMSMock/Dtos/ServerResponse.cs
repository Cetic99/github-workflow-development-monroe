namespace CMSMock.Dtos;

public class ServerResponse
{
    public string CommandName { get; set; } = string.Empty;
    public CommandParam[] Params { get; set; } = [];
}

public class CommandParam
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public CommandParam(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
