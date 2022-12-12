using UnityEngine;

public class TestStatus : StatusEffect
{
  public TestStatus() : base()
  {
    _duration = 3;
  }

  public override void applyStatus()
  {
    Debug.Log("TestStatus applied!");
  }
}