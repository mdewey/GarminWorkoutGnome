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
      {"1k", "100000"},
      {"5k", "500000"},
      {"15secs", "15000"},
      {"30secs", "30000"},
      {"45secs" , "45000"},
      {"60secs" , "60000"},
      {"75secs", "75000"},
      {"90secs", "90000"},
      {"1mins" , "60000"},
      {"1:15mins", "75000"},
      {"1.5mins", "90000"},
      {"2mins" , "120000"},
      {"2.5mins","150000"},
      {"3mins" , "180000"},
      {"4mins" , "240000"},
      {"5mins" , "300000"},
      {"6mins" , "360000"},
      {"7mins" , "420000"},
      {"8mins" , "480000"},
      {"10mins", "600000"},
      {"11mins", "660000"},
      {"12mins", "720000"},
      {"15mins", "900000"},
      {"20mins", "1200000"},
      {"25mins", "1500000"},
      {"30mins", "1800000"},
      {"31mins", "1860000"},
      {"33mins", "1980000"},
      {"35mins", "2100000"},
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
    if (parts.Count() != 4 || s.Contains("[") || s.Contains("]"))
    {
      throw new Exception("Invalid workout line, " + s);
    }
    var intensity = Intensity.Recovery;
    var durationType = parts[2].Trim() == "time" ? WktStepDuration.Time : WktStepDuration.Distance;

    if (parts[0] == "recover")
    {
      intensity = Intensity.Recovery;
    }
    else if (parts[0] == "run" || parts[0] == "active")
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
    try
    {
      return new NikeWorkOut
      {
        Duration = uint.Parse(parts[3]),
        DurationType = durationType,
        Description = parts[1],
        Intensity = intensity
      };
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      Console.WriteLine(s);

      throw e;
    }

  }

  static public void CreateNikeWorkOut(string title, Sport sport, List<NikeWorkOut> workOuts)
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
    workoutMesg.SetSport(sport);
    workoutMesg.SetSubSport(SubSport.Invalid);
    workoutMesg.SetNumValidSteps((ushort)workoutSteps.Count);

    CreateWorkout(workoutMesg, workoutSteps);
  }

  static public WorkoutStepMesg? CreateWorkoutStep(int messageIndex,
          String? name = null,
          String? notes = null,
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
    Console.WriteLine(workoutMesg.GetSport());

    // Create the output stream, this can be any type of stream, including a file or memory stream. Must have read/write access
    // create directory if it doesn't exist
    var dir = $"workouts/{workoutMesg.GetSport()}";
    Console.WriteLine(dir);

    if (!Directory.Exists(dir))
    {
      Directory.CreateDirectory(dir);
    }
    FileStream fitDest = new FileStream($"workouts/{workoutMesg.GetSport()}/{workoutMesg.GetWktNameAsString().Replace(' ', '_')}.fit", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

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