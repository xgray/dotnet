
namespace Bench
{
  public abstract class SerializeTest : BenchTest
  {

    public override void RunOnce()
    {
      if (this.test == "complex")
      {
        ComplexTest();
      }
      else
      {
        SimpleTest();
      }
    }

    public abstract void SimpleTest();
    public abstract void ComplexTest();
  }
}
