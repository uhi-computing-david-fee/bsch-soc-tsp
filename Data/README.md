
# TSP data files

  
Keep this `Data` folder beside `Program.cs` and `TspStarter.csproj`.

  
## Choose a dataset

  
Change the filename near the top of `Program.cs`, then run the project:

  
```csharp

string  datasetFile = "Data/eil51-subset-8.tsp";

```
  

The program reads the city count from the file. File IDs start at 1; code indices start at 0, so `startCity = 0` means file city 1.

  
## Small files

  
Use these files with the supplied brute-force solver. It evaluates each symmetric tour once, so the counts below are `(cities - 1)! / 2`.

  
| File | Cities | Optimal cost | Tours evaluated |
|---|---:|---:|---:|
| `example-four.tsp` | 4 | 40 | 3 |
| `eil51-subset-4.tsp` | 4 | 102 | 3 |
| `eil51-subset-6.tsp` | 6 | 113 | 60 |
| `eil51-subset-8.tsp` | 8 | 138 | 2,520 |
| `eil51-subset-10.tsp` | 10 | 159 | 181,440 |

  
`example-four.tsp` is a four-city square for checking routes by hand. The four `eil51-subset` files preserve the first 4, 6, 8 or 10 cities from eil51. They are teaching subsets, not official TSPLIB instances, and are nested rather than independent datasets.

  
## Larger benchmark files

  
Brute force is limited to 10 cities. Use these files for nearest neighbour and 2-opt experiments.

  
| File | Cities | Published optimal cost |
|---|---:|---:|
| `eil51.tsp` | 51 | 426 |
| `berlin52.tsp` | 52 | 7,542 |
| `eil76.tsp` | 76 | 538 |
| `kroA100.tsp` | 100 | 21,282 |
  

To display an optimality gap, set `knownOptimalCost` in `Program.cs` to the reference for the selected file. The data files do not contain this value. The starter uses symmetric TSPLIB `EUC_2D` distances rounded to integers; use those same distances when comparing costs.

  
The benchmark files are from [TSPLIB](https://comopt.ifi.uni-heidelberg.de/software/TSPLIB95/). The subset optimum values were checked by exhaustive search and Held–Karp dynamic programming.
