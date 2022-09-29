using Dynastream.Fit;

public class NikeWorkOut
{
  public int Repeats { get; set; } = 1;

  public uint? Duration { get; set; }

  public WktStepDuration DurationType { get; set; } = WktStepDuration.Time;

  public string Description { get; set; } = "";

  public Intensity Intensity { get; set; } = Intensity.Active;
}

