using System.Diagnostics;
using TspStarter;

// EXPERIMENT SETTINGS: put downloaded .tsp files in Data and change this filename.
string datasetFile = "Data/berlin52.tsp";
int startCity = 0;                // Array index 0 corresponds to city 1 in the file.
double? knownOptimalCost = null;  // Optional: published optimum for this EXACT dataset.
bool runNearestNeighbour = false; // Enable after completing task 1.
bool runTwoOpt = false;           // Enable after completing task 2; also requires task 1.

// Data files are copied beside the executable by the project settings.
// This works both in an IDE and when running from a different working directory.
string datasetPath = Path.Combine(AppContext.BaseDirectory, datasetFile);
double[,] distances = Tsp.LoadDistances(datasetPath);
int cityCount = distances.GetLength(0); // Comes from the file, not a separate setting.

if (startCity < 0 || startCity >= cityCount) throw new ArgumentException("Starting city is out of range.");
if (runTwoOpt && !runNearestNeighbour) throw new ArgumentException("Enable nearest neighbour before 2-opt.");
if (knownOptimalCost is double reference && (!double.IsFinite(reference) || reference < 0))
    throw new ArgumentException("A known optimum must be finite and non-negative.");

Console.WriteLine($"Dataset: {datasetFile} | Cities: {cityCount} | Start city (file ID): {startCity + 1}");
Console.WriteLine("TSPLIB EUC_2D: symmetric distances rounded to integers\n");
Stopwatch timer = new Stopwatch();
double? optimalCost = knownOptimalCost;

// SUPPLIED BASELINE
if (cityCount <= 60)
{
    // Warm up before timing to reduce first-use/JIT effects. Discard its count.
    BruteForce.Solve(distances, startCity, out _);
    timer.Restart();
    int[] exactRoute = BruteForce.Solve(distances, startCity, out long toursEvaluated);
    timer.Stop();

    // Validation, final cost calculation and output are outside the timed section.
    Tsp.ValidateRoute(distances, exactRoute, startCity);
    double exactCost = Tsp.RouteCost(distances, exactRoute);
    if (knownOptimalCost.HasValue && knownOptimalCost.Value != exactCost)
        throw new InvalidOperationException("The supplied optimum does not match exhaustive search. Check the dataset and reference.");
    optimalCost = exactCost;
    PrintResult("Brute force", exactRoute, optimalCost.Value, timer.Elapsed.TotalMilliseconds);
    Console.WriteLine($"Unique tours evaluated: {toursEvaluated}\n");
}
else
{
    Console.WriteLine("Brute force skipped above 10 cities.\n");
    if (!optimalCost.HasValue) Console.WriteLine("No published optimum supplied; optimality gaps will be unavailable.\n");
}
if (!runNearestNeighbour) Console.WriteLine("Nearest neighbour and 2-opt are disabled until implemented.\n");

// TASK 1: this block becomes usable as soon as nearest neighbour is implemented.
if (runNearestNeighbour)
{
    StudentAlgorithms.NearestNeighbour(distances, startCity); // Untimed warm-up.
    timer.Restart();
    int[] nearestRoute = StudentAlgorithms.NearestNeighbour(distances, startCity);
    timer.Stop();
    double nearestMilliseconds = timer.Elapsed.TotalMilliseconds;

    Tsp.ValidateRoute(distances, nearestRoute, startCity);
    double nearestCost = Tsp.RouteCost(distances, nearestRoute);
    PrintResult("Nearest neighbour", nearestRoute, nearestCost, nearestMilliseconds);
    PrintGap(nearestCost, optimalCost);

    // TASK 2: the route produced above becomes the input to the improver.
    if (runTwoOpt)
    {
        StudentAlgorithms.TwoOpt(distances, (int[])nearestRoute.Clone()); // Untimed warm-up.
        int[] refinementInput = (int[])nearestRoute.Clone();
        timer.Restart();
        int[] improvedRoute = StudentAlgorithms.TwoOpt(distances, refinementInput);
        timer.Stop();
        double refinementMilliseconds = timer.Elapsed.TotalMilliseconds;

        Tsp.ValidateRoute(distances, improvedRoute, startCity);
        double improvedCost = Tsp.RouteCost(distances, improvedRoute);
        if (improvedCost > nearestCost + 1e-9 * Math.Max(1, nearestCost))
            throw new InvalidOperationException("2-opt returned a worse route.");

        PrintResult("2-opt (refinement only)", improvedRoute, improvedCost, refinementMilliseconds);
        PrintGap(improvedCost, optimalCost);
        Console.WriteLine($"Cost reduction from nearest neighbour: {nearestCost - improvedCost:F3}");
        Console.WriteLine($"Construction + refinement time: {nearestMilliseconds + refinementMilliseconds:F6} ms");
    }
}

// Small output helpers keep formatting consistent without hiding the experiment.
static void PrintResult(string name, int[] route, double cost, double milliseconds)
{
    Console.WriteLine(name);
    // Display original TSPLIB city IDs; internal arrays remain zero-based.
    Console.WriteLine($"Route (file IDs): {string.Join(" -> ", route.Select(city => city + 1))} -> {route[0] + 1}");
    Console.WriteLine($"Total cost: {cost:F3}");
    Console.WriteLine($"Elapsed time: {milliseconds:F6} ms");
}

static void PrintGap(double cost, double? optimum)
{
    if (optimum is double reference && cost < reference)
        throw new InvalidOperationException("Route cost is below the supplied optimum. Check your reference.");
    if (optimum is double value && value > 0)
        Console.WriteLine($"Optimality gap: {100 * (cost - value) / value:F2}%\n");
    else Console.WriteLine("Optimality gap unavailable (unknown or zero optimum).\n");
}
