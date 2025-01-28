namespace Configuration.API;

public class User
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string? MiddleName { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var other = (User)obj;

        return string.Equals(FirstName, other.FirstName, StringComparison.Ordinal) &&
               string.Equals(LastName, other.LastName, StringComparison.Ordinal) &&
               string.Equals(MiddleName, other.MiddleName, StringComparison.Ordinal);
    }
}