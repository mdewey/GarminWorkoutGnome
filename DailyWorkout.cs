public class DailyWorkout
{
  public string Title { get; set; } = "Unknown title";

  public List<string>? Workouts { get; set; } = new List<string>();

  public string? WarmUp { get; set; }
  public string? CoolDown { get; set; }
  public string? Betweens { get; set; }

  internal List<NikeWorkOut> GetWorkOut()
  {
    var workOuts = new List<String>();
    if (WarmUp != null)
    {
      workOuts.Add(WarmUp);
    }
    if (Workouts != null)
    {
      workOuts.AddRange(Workouts);
    }
    if (CoolDown != null)
    {
      workOuts.Add(CoolDown);
    }
    return workOuts.Select(WorkoutService.CreateNikeWorkOutFromString).ToList();
  }
}