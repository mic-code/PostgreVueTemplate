using System.ComponentModel.DataAnnotations;

namespace Models;

public class TestEntity
{
    [Key]
    public string Key { get; set; }
    public string Value { get; set; }
}
