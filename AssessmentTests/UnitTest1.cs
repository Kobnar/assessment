using Assessment;

namespace AssessmentTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestIntegration()
    {
        Thing something = new Thing();
        Assert.That(something.IsReal);
    }
}