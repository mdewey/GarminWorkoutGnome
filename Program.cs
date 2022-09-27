// See https://aka.ms/new-console-template for more information

Console.WriteLine("Ready, set, go!");

var fifteenSeconds = 15000;
var thirtySeconds = 30000;
var fortyFiveSeconds = 45000;
var oneMinute = 60000;
var twoMinutes = 120000;
var fiveMinutes = 300000;
var sevenMinutes = 420000;
var tenMinutes = 600000;

var day1 = new string[] { $"recover, Recover Pace, {fiveMinutes}" };

var day1Workouts = day1.Select(s =>
{
  return WorkoutService.CreateNikeWorkOutFromString(s);
}).ToList();

WorkoutService.CreateNikeWorkOut("N.5k.1.1.recovery", day1Workouts);


var day2 = new List<string> {
  $"warmup, Warm up the legs, {fiveMinutes}",
};

var repeats = 8;
for (int i = 1; i <= repeats; i++)
{
  day2.Add($"run, Run 5k pace - {i}/{repeats}, {oneMinute}");
  day2.Add($"recover, Recover - {i}/{repeats}, {oneMinute}");
}
day2.Add($"cooldown, Cool down the legs, {fiveMinutes}");

var day2Workouts = day2.Select(s =>
{
  return WorkoutService.CreateNikeWorkOutFromString(s);
}).ToList();

WorkoutService.CreateNikeWorkOut("N.5k.1.2.speed", day1Workouts);

var day3 = new string[] { $"recover, Recover Pace, {sevenMinutes}" };
var day3Workouts = day3.Select(s =>
{
  return WorkoutService.CreateNikeWorkOutFromString(s);
}).ToList();
WorkoutService.CreateNikeWorkOut("N.5k.1.3.recovery", day3Workouts);

var day4 = new List<string> {
  $"warmup, Warm up the legs, {fiveMinutes}"
  };
var day4Intervals = new List<string> {
  $"run, Run 5k pace, {oneMinute}",
  $"run, Run 10k pace, {twoMinutes}",
  $"run, Run 5k pace, {oneMinute}",
  $"run, Mile Pace, {fortyFiveSeconds}",
  $"run, Mile Pace, {fortyFiveSeconds}",
  $"run, 10k Pace, {twoMinutes}",
  $"run, 5k Pace, {oneMinute}",
  $"run, Mile Pace, {fortyFiveSeconds}",
  $"run, Best Pace, {thirtySeconds}",
  $"run, Best Pace, {fifteenSeconds}",
  };

var counter = 1;
foreach (var interval in day4Intervals)
{
  day4.Add(interval);
  day4.Add(
    $"recover, Recover {counter}/{day4Intervals.Count}, {oneMinute}"
  );
  counter++;
}

day4.Add($"cooldown, Good Job - cool down, {fiveMinutes}");

var day4Workouts = day4.Select(s =>
{
  return WorkoutService.CreateNikeWorkOutFromString(s);
}).ToList();
WorkoutService.CreateNikeWorkOut("N.5k.1.4.speed", day4Workouts);

var day5 = new List<string> {
  $"warmup, Warm up the legs, {fiveMinutes}",
  $"run, 1 mile, {tenMinutes}",
  };

var day5Workouts = day5.Select(s =>
{
  return WorkoutService.CreateNikeWorkOutFromString(s);
}).ToList();
WorkoutService.CreateNikeWorkOut("N.5k.1.5.longrun", day5Workouts);