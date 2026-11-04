namespace TspStarter;

public static class StudentAlgorithms
{
    // TASK 1: construct a nearest-neighbour tour from the requested starting city.
    // Return each city exactly once. Do not append the start city again at the end.
    // Use distances[from, to] and choose a consistent rule for ties.
    public static int[] NearestNeighbour(double[,] distances, int startCity)
    {
        throw new NotImplementedException("Implement nearest neighbour here.");
    }

    // TASK 2: improve the supplied route using 2-opt.
    // Copy initialRoute before editing; preserve the start city and do not worsen cost.
    // The loaded EUC_2D matrices are symmetric. Explain your move-selection/stopping policy.
    public static int[] TwoOpt(double[,] distances, int[] initialRoute)
    {
        throw new NotImplementedException("Implement 2-opt here.");
    }
}
