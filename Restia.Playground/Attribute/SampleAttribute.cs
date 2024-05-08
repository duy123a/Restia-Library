namespace Restia.Playground.Attribute;
[System.AttributeUsage(AttributeTargets.Class | System.AttributeTargets.Struct, AllowMultiple = false)]
public class SampleAttribute : System.Attribute
{
	public string Name;
	public double Version;
	public SampleAttribute(string name, double version)
	{
		Name = name;
		Version = version;
	}
}
