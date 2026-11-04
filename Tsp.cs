using System.Globalization;

namespace TspStarter;

public static class Tsp
{
    // Read the common TSPLIB EUC_2D format: headers followed by city ID, X, Y.
    // File IDs 1..n map to array indices 0..n-1; the row order need not be sorted.
    public static double[,] LoadDistances(string path)
    {
        string[] lines = File.ReadAllLines(path);
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        int coordinateStart = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;
            if (line.Equals("NODE_COORD_SECTION", StringComparison.OrdinalIgnoreCase))
            {
                coordinateStart = i + 1;
                break;
            }
            if (line.EndsWith("_SECTION", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Expected NODE_COORD_SECTION. This starter supports EUC_2D coordinate files only.");

            // Accept both 'DIMENSION: 8' and 'DIMENSION 8' header styles.
            string[] parts = line.Contains(':') ? line.Split(':', 2)
                : line.Split((char[]?)null, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2) headers[parts[0].Trim()] = parts[1].Trim();
        }

        if (!headers.TryGetValue("TYPE", out string? type) || !type.Equals("TSP", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Use a symmetric TSPLIB file with TYPE: TSP.");
        if (!headers.TryGetValue("EDGE_WEIGHT_TYPE", out string? weightType) || !weightType.Equals("EUC_2D", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Use EDGE_WEIGHT_TYPE: EUC_2D. Other TSPLIB distance types need different calculations and are not supported.");
        if (!headers.TryGetValue("DIMENSION", out string? dimension) || !int.TryParse(dimension, out int cityCount) || cityCount < 2)
            throw new InvalidDataException("DIMENSION must specify at least two cities.");
        if (headers.ContainsKey("EDGE_DATA_FORMAT"))
            throw new InvalidDataException("Restricted-edge instances are not supported; use a complete TSP instance.");
        if (coordinateStart < 0) throw new InvalidDataException("Missing NODE_COORD_SECTION.");

        double[] x = new double[cityCount];
        double[] y = new double[cityCount];
        bool[] loaded = new bool[cityCount];
        int loadedCount = 0;

        for (int i = coordinateStart; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;
            if (line.Equals("EOF", StringComparison.OrdinalIgnoreCase)) break;
            // Display coordinates do not change the distances. Reject other extra sections.
            if (line.Equals("DISPLAY_DATA_SECTION", StringComparison.OrdinalIgnoreCase))
            {
                for (; i < lines.Length; i++)
                    if (lines[i].Trim().EndsWith("_SECTION", StringComparison.OrdinalIgnoreCase)
                        && !lines[i].Trim().Equals("DISPLAY_DATA_SECTION", StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Extra problem sections are not supported.");
                break;
            }

            string[] parts = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3 || !int.TryParse(parts[0], out int id) || id < 1 || id > cityCount)
                throw new InvalidDataException($"Invalid coordinate row: {line}");
            if (loaded[id - 1]) throw new InvalidDataException($"City {id} is repeated.");
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double cityX)
                || !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double cityY)
                || !double.IsFinite(cityX) || !double.IsFinite(cityY))
                throw new InvalidDataException($"Invalid coordinates for city {id}.");

            x[id - 1] = cityX;
            y[id - 1] = cityY;
            loaded[id - 1] = true;
            loadedCount++;
        }
        if (loadedCount != cityCount) throw new InvalidDataException("Coordinate count does not match DIMENSION.");

        double[,] distances = new double[cityCount, cityCount];
        for (int i = 0; i < cityCount; i++)
        {
            for (int j = i + 1; j < cityCount; j++)
            {
                double dx = x[i] - x[j];
                double dy = y[i] - y[j];
                // TSPLIB EUC_2D rounds to the nearest integer, with .5 rounded UP.
                // Raw Euclidean distance or Math.Round's default midpoint rule would
                // produce incorrect comparisons against published benchmark optima.
                double distance = Math.Floor(Math.Sqrt(dx * dx + dy * dy) + 0.5);
                if (!double.IsFinite(distance)) throw new InvalidDataException("Coordinates are too large.");
                distances[i, j] = distance;
                distances[j, i] = distance;
            }
        }
        return distances;
    }

    // A route contains each city ONCE, e.g. [0, 2, 1, 3].
    // Its final journey back to the starting city is added here.
    // Call ValidateRoute before using an unfamiliar/student-generated route.
    public static double RouteCost(double[,] distances, int[] route)
    {
        double cost = 0;
        for (int i = 0; i < route.Length - 1; i++)
            cost += distances[route[i], route[i + 1]];

        cost += distances[route[route.Length - 1], route[0]];
        return cost;
    }

    public static void ValidateRoute(double[,] distances, int[] route, int startCity)
    {
        int cityCount = distances.GetLength(0);
        if (cityCount < 2 || distances.GetLength(1) != cityCount)
            throw new ArgumentException("Distances must be a square matrix with at least two cities.");
        if (startCity < 0 || startCity >= cityCount)
            throw new ArgumentException("Starting city is out of range.");
        if (route.Length != cityCount)
            throw new ArgumentException("The route must contain every city exactly once.");
        if (route[0] != startCity)
            throw new ArgumentException("The route must begin at the requested starting city.");

        bool[] visited = new bool[cityCount];
        foreach (int city in route)
        {
            if (city < 0 || city >= cityCount || visited[city])
                throw new ArgumentException("The route contains an invalid or repeated city.");
            visited[city] = true;
        }
    }
}
