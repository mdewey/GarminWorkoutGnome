using Dynastream.Fit;

public class NikeWorkOut
{
  public int Repeats { get; set; } = 1;

  public uint? Time { get; set; }

  public string Description { get; set; } = "";

  public Intensity Intensity { get; set; } = Intensity.Active;
}