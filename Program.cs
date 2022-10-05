// See https://aka.ms/new-console-template for more information

Console.WriteLine("Ready, set, go!");

// read from workouts.md line by line
// if line starts with ## then create a new workout  
// var path = "nike-5k.md";
// var path = "src/running/nike-5k.md";
var path = "src/climbing/hangboard.quick.md";
var weeklyWorkouts = new List<DailyWorkout>();
DailyWorkout? workout = null;
Dynastream.Fit.Sport sport = Dynastream.Fit.Sport.All;
foreach (string line in System.IO.File.ReadLines(path))
{
  if (line == null || line == "") continue;
  if (line.StartsWith("##"))
  {
    if (workout != null)
    {
      weeklyWorkouts.Add(workout);
    }
    workout = new DailyWorkout();
    workout.Title = line.Replace("##", "").Trim();
  }
  else if (line.StartsWith(">"))
  {
    if (line.Contains("RockClimbing"))
    {
      sport = Dynastream.Fit.Sport.RockClimbing;
    }
    else if (line.Contains("Running"))
    {
      sport = Dynastream.Fit.Sport.Running;
    }
  }
  else if (line.StartsWith("warmup,"))
  {
    if (workout != null)
    {
      workout.WarmUp = WorkoutService.PopulateDistancesInWorkOut(line);
    }
  }
  else if (line.StartsWith("cooldown"))
  {
    if (workout != null)
    {
      workout.CoolDown = WorkoutService.PopulateDistancesInWorkOut(line);
    }
  }
  else if (line.StartsWith("run") || line.StartsWith("active"))
  {
    if (workout != null)
    {
      if (workout.Workouts == null)
      {
        workout.Workouts = new List<string>();
      }
      workout.Workouts.Add(WorkoutService.PopulateDistancesInWorkOut(line).Trim());
    }
  }
  else if (line.StartsWith('x'))
  {
    if (workout != null)
    {
      if (workout.Workouts == null)
      {
        workout.Workouts = new List<string>();
      }
      // split on the | 
      var splits = line.Split('|');
      var repeats = Int32.Parse(splits[0].Replace("x", "").Trim());
      var workoutLine = splits.Skip(1);
      for (var i = 0; i < repeats; i++)
      {
        foreach (var w in workoutLine)
        {
          workout.Workouts.Add(
            WorkoutService.PopulateDistancesInWorkOut(
              w
              .Replace("[i]", (i + 1).ToString())
              .Replace("[total]", repeats.ToString()))
            .Trim());
        }
      }
    }
  }
  else if (line.StartsWith("between:"))
  {
    if (workout != null)
    {
      if (workout.Workouts == null)
      {
        workout.Workouts = new List<string>();
      }
      var between = line.Replace("between:", "").Trim();
      var newList = new List<string>();
      var counter = 1;
      foreach (var w in workout.Workouts)
      {
        newList.Add(w);
        newList.Add(
          WorkoutService.PopulateDistancesInWorkOut(
            between
            .Replace("[i]", counter.ToString())
            .Replace("[total]", workout.Workouts.Count.ToString())));
        counter++;
      }
      workout.Workouts = newList;
    }
  }
}
if (workout != null)
{
  weeklyWorkouts.Add(workout);
}
// print out the weekly workouts
foreach (DailyWorkout wo in weeklyWorkouts)
{
  Console.WriteLine("----------");
  System.Console.WriteLine(wo.Title);
  System.Console.WriteLine(wo.WarmUp);
  if (wo.Workouts != null)
  {
    foreach (var w in wo.Workouts)
    {
      System.Console.WriteLine(w);
    }
  }
  System.Console.WriteLine(wo.CoolDown);
  WorkoutService.CreateNikeWorkOut(wo.Title, sport, wo.GetWorkOut());
}


