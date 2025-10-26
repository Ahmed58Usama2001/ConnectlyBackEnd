
namespace Connectly.Core.Entities;

public class Group(string name)
{
    [Key]
    public string Name { get; set; } = name;

   public ICollection<Connection> connections { get; set; } = [];
}
