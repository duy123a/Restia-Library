using Restia.Playground.Attribute;

namespace Restia.Playground.Class;
[Sample("Restia", 1.0)]
public class SampleClass
{
	public string Description;
	public SampleClass(string description = "This is a sample description")
	{
		Description = description;
	}
}
