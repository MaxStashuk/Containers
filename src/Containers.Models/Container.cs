namespace Containers.Models;

public class Container
{
    
    public int Id { get; set; }
    public int ContainerTypeId { get; set; }
    public bool isHazardious { get; set; }
    public string Name { get; set; }
    
}