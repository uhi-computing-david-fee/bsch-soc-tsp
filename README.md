# TSP Experiment (C#)


A console starter for investigating the Travelling Salesperson Problem (TSP) in the Science of Computing & Digital Media unit. The starter supplies dataset loading, distance calculation, route validation and a brute-force baseline. Students implement nearest neighbour and attempt 2-opt; the focus is on the experimental evidence and analysis.


## Run the project
 

Requires the .NET 8 SDK. Place the separately supplied `Data` folder beside `Program.cs` and `TspStarter.csproj`, then run:

  
```bash

dotnet  run  -c  Release

```
  

See `Data/README.md` for the dataset list, file descriptions, known optima and setup notes. The starter loads the selected file from the setting at the top of `Program.cs`:

  
```csharp

string  datasetFile = "Data/example-four.tsp";

```
  

The city count is read from the file. Rebuild after adding or changing dataset files.

  
## Supported data and routes

  
This starter supports symmetric TSPLIB coordinate files with `TYPE: TSP` and `EDGE_WEIGHT_TYPE: EUC_2D`. Distances are rounded to the nearest integer, with `.5` rounded upwards. Other distance types and asymmetric instances are not supported.
  

File city IDs start at 1; array indices start at 0. A route lists each city once, beginning with the selected start city. Do not repeat the start at the end: `Tsp.RouteCost` adds the return leg.

  
## What is supplied

  
| File | Purpose |
|---|---|
| `Program.cs` | Dataset and run settings, timing and console output |
| `Tsp.cs` | Dataset reader, distance matrix, route cost and validation |
| `BruteForce.cs` | Exact symmetric search for instances up to 10 cities |
| `StudentAlgorithms.cs` | Student implementation points |

  
For symmetric instances with at least three cities, brute force generates `(n-1)!` permutations and evaluates `(n-1)! / 2` distinct tours. Above 10 cities, it is skipped. Use small files for exact comparisons and larger benchmarks for heuristic experiments.

  
## Where to add your algorithms

  
Open `StudentAlgorithms.cs` and implement these methods:

  
```csharp

public  static  int[] NearestNeighbour(double[,] distances, int  startCity)

public static int[] TwoOpt(double[,] distances, int[] initialRoute)

```
  

Return a route containing every city once, with the start first. `TwoOpt` should work on a copy of its input and return a route no more expensive than the original. Enable the methods near the top of `Program.cs` by setting `runNearestNeighbour` and `runTwoOpt` to `true`; 2-opt requires nearest neighbour to be enabled.

  
The program runs nearest neighbour from the one `startCity` you select. To investigate starting-city effects, add a loop to run it from every city for at least one dataset. Choose and document a consistent tie rule.

  
2-opt is challenging. Make and test a serious attempt. If you do not complete it, report what you tried, the tests and results, and what you learned. Do not present invalid routes or unverified costs as successful results.

  

## Measurements

  
Each enabled method receives one untimed warm-up and one timed run. Validation and final cost calculation are outside the timed section. The program prints route cost, elapsed time and an optimality gap when a verified optimum is supplied. Set `knownOptimalCost` in `Program.cs` for the exact selected dataset; otherwise leave it `null`.

  
The program does not write CSV files. Repeat runs and record results separately, including the dataset, start city, route cost and timing. For 2-opt, distinguish refinement time from the combined nearest-neighbour-plus-refinement time.