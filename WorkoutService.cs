using Dynastream.Fit;

public class WorkoutService
{

  static public string PopulateDistancesInWorkOut(string s)
  {
    var lookUpTable = new Dictionary<string, string>{
      {"1miles", "160935"},
      {"2miles", "321870"},
      {"3miles", "482805"},
      {"4miles", "643740"},
      {"1K", "100000"},
      {"5k", "50000"},
      {"15secs", "15000"},
      {"30secs", "30000"},
      {"45secs" , "45000"},
      {"1mins" , "60000"},
      {"2mins" , "120000"},
      {"3mins" , "180000"},
      {"4mins" , "240000"},
      {"5mins" , "300000"},
      {"6mins" , "360000"},
      {"7mins" , "420000"},
    };
    foreach (var item in lookUpTable)
    {
      s = s.Replace($"[{item.Key}]", item.Value);
    }
    return s;

  }

  static public NikeWorkOut CreateNikeWorkOutFromString(string s)
  {
    var parts = s.Split(',');
    var intensity = Intensity.Recovery;
    var durationType = parts[2].Trim() == "time" ? WktStepDuration.Time : WktStepDuration.Distance;

    if (parts[0] == "recover")
    {
      intensity = Intensity.Recovery;
    }
    else if (parts[0] == "run")
    {
      intensity = Intensity.Active;
    }
    else if (parts[0] == "warmup")
    {
      intensity = Intensity.Warmup;
    }
    else if (parts[0] == "cooldown")
    {
      intensity = Intensity.Cooldown;
    }
    else if (parts[0] == "rest")
    {
      intensity = Intensity.Rest;
    }
    return new NikeWorkOut
    {
      Duration = uint.Parse(parts[3]),
      DurationType = durationType,
      Description = parts[1],
      Intensity = intensity
    };
  }

  static public void CreateNikeWorkOut(string title, List<NikeWorkOut> workOuts)
  {
    var workoutSteps = new List<WorkoutStepMesg>();

    foreach (var workOut in workOuts)
    {
      workoutSteps.Add(CreateWorkoutStep(messageIndex: workoutSteps.Count,
                                         name: workOut.Description,
                                         notes: workOut.Description,
                                         intensity: workOut.Intensity,
                                         durationType: workOut.DurationType,
                                         durationValue: workOut.Duration
                        ));
    }




    var workoutMesg = new WorkoutMesg();
    workoutMesg.SetWktName(title);
    workoutMesg.SetSport(Sport.Running);
    workoutMesg.SetSubSport(SubSport.Invalid);
    workoutMesg.SetNumValidSteps((ushort)workoutSteps.Count);

    CreateWorkout(workoutMesg, workoutSteps);
  }

  static public void CreateRun800RepeatsWorkout()
  {
    var workoutSteps = new List<WorkoutStepMesg>();

    workoutSteps.Add(CreateWorkoutStep(messageIndex: workoutSteps.Count,
                                        durationType: WktStepDuration.Distance,
                                        durationValue: 400000, // centimeters
                                        targetType: WktStepTarget.HeartRate,
                                        targetValue: 1,
                                        intensity: Intensity.Warmup));

    workoutSteps.Add(CreateWorkoutStep(messageIndex: workoutSteps.Count,
                                        durationType: WktStepDuration.Distance,
                                        durationValue: 80000, // centimeters
                                        targetType: WktStepTarget.HeartRate,
                                        targetValue: 4));

    workoutSteps.Add(CreateWorkoutStep(messageIndex: workoutSteps.Count,
                                        durationType: WktStepDuration.Distance,
                                        durationValue: 20000, // centimeters
                                        targetType: WktStepTarget.HeartRate,
                                        targetValue: 2,
                                        intensity: Intensity.Rest));

    workoutSteps.Add(CreateWorkoutStepRepeat(messageIndex: workoutSteps.Count, repeatFrom: 1, repetitions: 5));

    workoutSteps.Add(CreateWorkoutStep(messageIndex: workoutSteps.Count,
                                        durationType: WktStepDuration.Distance,
                                        durationValue: 100000, // centimeters
                                        targetType: WktStepTarget.HeartRate,
                                        targetValue: 2,
                                        intensity: Intensity.Cooldown));

    var workoutMesg = new WorkoutMesg();
    workoutMesg.SetWktName("Running 800m Repeats");
    workoutMesg.SetSport(Sport.Running);
    workoutMesg.SetSubSport(SubSport.Invalid);
    workoutMesg.SetNumValidSteps((ushort)workoutSteps.Count);

    CreateWorkout(workoutMesg, workoutSteps);
  }

  static public WorkoutStepMesg CreateWorkoutStep(int messageIndex,
          String name = null,
          String notes = null,
          Intensity intensity = Intensity.Active,
          WktStepDuration durationType = WktStepDuration.Open,
          uint? durationValue = null,
          WktStepTarget targetType = WktStepTarget.Open,
          uint targetValue = 0,
          uint? customTargetValueLow = null,
          uint? customTargetValueHigh = null)
  {
    if (durationType == WktStepDuration.Invalid)
    {
      return null;
    }

    var workoutStepMesg = new WorkoutStepMesg();
    workoutStepMesg.SetMessageIndex((ushort)messageIndex);

    if (name != null)
    {
      workoutStepMesg.SetWktStepName(name);
    }

    if (notes != null)
    {
      workoutStepMesg.SetNotes(notes);
    }

    workoutStepMesg.SetIntensity(intensity);
    workoutStepMesg.SetDurationType(durationType);

    if (durationValue.HasValue)
    {
      workoutStepMesg.SetDurationValue(durationValue);
    }

    if (targetType != WktStepTarget.Invalid && customTargetValueLow.HasValue && customTargetValueHigh.HasValue)
    {
      workoutStepMesg.SetTargetType(targetType);
      workoutStepMesg.SetTargetValue(0);
      workoutStepMesg.SetCustomTargetValueLow(customTargetValueLow);
      workoutStepMesg.SetCustomTargetValueHigh(customTargetValueHigh);
    }
    else if (targetType != WktStepTarget.Invalid)
    {
      workoutStepMesg.SetTargetType(targetType);
      workoutStepMesg.SetTargetValue(targetValue);
      workoutStepMesg.SetCustomTargetValueLow(0);
      workoutStepMesg.SetCustomTargetValueHigh(0);
    }

    return workoutStepMesg;
  }


  private static WorkoutStepMesg CreateWorkoutStepRepeat(int messageIndex, uint repeatFrom, uint repetitions)
  {
    var workoutStepMesg = new WorkoutStepMesg();
    workoutStepMesg.SetMessageIndex((ushort)messageIndex);

    workoutStepMesg.SetDurationType(WktStepDuration.RepeatUntilStepsCmplt);
    workoutStepMesg.SetDurationValue(repeatFrom);

    workoutStepMesg.SetTargetType(WktStepTarget.Open);
    workoutStepMesg.SetTargetValue(repetitions);

    return workoutStepMesg;
  }

  static void CreateWorkout(WorkoutMesg workoutMesg, List<WorkoutStepMesg> workoutSteps)
  {
    // The combination of file type, manufacturer id, product id, and serial number should be unique.
    // When available, a non-random serial number should be used.
    Dynastream.Fit.File fileType = Dynastream.Fit.File.Workout;
    ushort manufacturerId = Manufacturer.Development;
    ushort productId = 0;
    Random random = new Random();
    uint serialNumber = (uint)random.Next();

    // Every FIT file MUST contain a File ID message
    var fileIdMesg = new FileIdMesg();
    fileIdMesg.SetType(fileType);
    fileIdMesg.SetManufacturer(manufacturerId);
    fileIdMesg.SetProduct(productId);
    fileIdMesg.SetTimeCreated(new Dynastream.Fit.DateTime(System.DateTime.UtcNow));
    fileIdMesg.SetSerialNumber(serialNumber);

    // Create the output stream, this can be any type of stream, including a file or memory stream. Must have read/write access
    FileStream fitDest = new FileStream($"workouts/{workoutMesg.GetWktNameAsString().Replace(' ', '_')}.fit", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

    // Create a FIT Encode object
    Encode encoder = new Encode(ProtocolVersion.V10);

    // Write the FIT header to the output stream
    encoder.Open(fitDest);

    // Write the messages to the file, in the proper sequence
    encoder.Write(fileIdMesg);
    encoder.Write(workoutMesg);

    foreach (WorkoutStepMesg workoutStep in workoutSteps)
    {
      encoder.Write(workoutStep);
    }

    // Update the data size in the header and calculate the CRC
    encoder.Close();

    // Close the output stream
    fitDest.Close();

    Console.WriteLine($"Encoded FIT file {fitDest.Name}");
  }



}