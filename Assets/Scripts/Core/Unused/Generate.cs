using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Generate
{
    public static MapController.Map RandomMap(int width, int height, int difficulty, float loadFactor)
    {
        MapController.Map map = new MapController.Map();
        map.difficulty = difficulty;
        map.size = new IntVector(width, height);

        map.rooms = new MapController.Room[width][];
        for (int i = 0; i < width; i++)
            map.rooms[i] = new MapController.Room[height];

        int roomCount = 1; //generation starting room

        IntVector genStart = new IntVector(Random.Range(0, width), Random.Range(0, height));
        map.rooms[genStart.x][genStart.y] = RandomRoom(difficulty);

        Queue<IntVector> queue = new Queue<IntVector>();
        queue.Enqueue(genStart);
        // Debug.Log("Start generation at: (" + map.start.x + ", " + map.start.y + ")");

        while (queue.Count > 0)
        {
            if (1.0f * roomCount >= loadFactor * width * height)
                break;
            IntVector currentRoom = queue.Dequeue();
            int x = currentRoom.x;
            int y = currentRoom.y;
            
            int availableDirections = 0;
            for (int i = -1; i <= 1; i++)
                for (int j = i * i - 1; j <= 1; j += 2)
                    if (!OutOfBounds(new IntVector(x + i, y + j), map) && !map.rooms[x + i][y + j].active)
                        availableDirections++;
            float[] thresholds = new float[availableDirections];
            for (int i = 0; i < availableDirections; i++)
                thresholds[i] = 1 - 0.25f * i;

            // Randomly distribute thresholds among available directions
            for (int i = 0; i < availableDirections; i++)
            {
                float temp = thresholds[i];
                int randomIndex = Random.Range(i, availableDirections);
                thresholds[i] = thresholds[randomIndex];
                thresholds[randomIndex] = temp;
            }
            int thresholdIndex = 0;

            // Loops through four directions (i, j)
            for (int i = -1; i <= 1; i++)
            {
                for (int j = i*i - 1; j <= 1; j+=2)
                {
                    if (OutOfBounds(new IntVector(x + i, y + j), map) || map.rooms[x + i][y + j].active)
                        continue;

                    // string dir = (i == 0) ? ((j == -1) ? "up" : "down") : ((i == -1) ? "left" : "right");
                    // Debug.Log(dir + " " + thresholds[thresholdIndex]);
                    if (Random.value > thresholds[thresholdIndex++])
                        continue;
                    
                    // Debug.Log("got through");

                    bool intersect = false;

                    if (i == 0 && j == -1)
                        map.rooms[x][y].up = true;
                    else if (i == 0 && j == 1)
                        map.rooms[x][y].down = true;
                    else if (i == -1 && j == 0)
                        map.rooms[x][y].left = true;
                    else if (i == 1 && j == 0)
                        map.rooms[x][y].right = true;

                    int distance = WeightedDistance((j == 0) ? width : height);

                    // Check for edge at (x + i * distance, y + j * distance); if edge, shrink distance until inside the grid
                    for (int k = distance; k > 0; k--)
                        if (!OutOfBounds(new IntVector(x + i * k, y + j * k), map))
                        {
                            distance = k;
                            break;
                        }
                    // Debug.Log("distance: " + distance);
                    // Activate Corridors
                    for (int k = 1; k < distance; k++)
                    {
                        if (map.rooms[x + i * k][y + j * k].active)
                            intersect = true;
                        else
                        {
                            map.rooms[x + i * k][y + j * k] = RandomRoom(difficulty);
                            roomCount++;
                        }

                        if (i == 0)
                        {
                            map.rooms[x + i * k][y + j * k].up = true;
                            map.rooms[x + i * k][y + j * k].down = true;
                        }
                        else
                        {
                            map.rooms[x + i * k][y + j * k].left = true;
                            map.rooms[x + i * k][y + j * k].right = true;
                        }

                    }

                    // Activate endpoint and add to queue
                    if (map.rooms[x + i * distance][y + j * distance].active)
                        intersect = true;
                    else
                    {
                        map.rooms[x + i * distance][y + j * distance] = RandomRoom(difficulty);
                        roomCount++;
                    }

                    if (i == 0 && j == -1)
                        map.rooms[x + i * distance][y + j * distance].down = true;
                    else if (i == 0 && j == 1)
                        map.rooms[x + i * distance][y + j * distance].up = true;
                    else if (i == -1 && j == 0)
                        map.rooms[x + i * distance][y + j * distance].right = true;
                    else if (i == 1 && j == 0)
                        map.rooms[x + i * distance][y + j * distance].left = true;

                    if (!intersect)
                    {
                        IntVector endpoint = new IntVector(x + i * distance, y + j * distance);
                        // Debug.Log("Enqueuing: " + endpoint.ToString());
                        queue.Enqueue(endpoint);
                    }
                }
            }
        } // End while loop

        // Find start point
        queue.Clear();
        queue.Enqueue(genStart);
        List<IntVector> visited = new List<IntVector>();

        IntVector latest = null;
        while (queue.Count > 0)
        {
            IntVector latestRoom = queue.Dequeue();
            if (visited.Contains(latestRoom))
                continue;
            visited.Add(latestRoom);
            latest = latestRoom;

            int x = latestRoom.x;
            int y = latestRoom.y;

            if (map.rooms[x][y].up)
                queue.Enqueue(new IntVector(x, y - 1));
            if (map.rooms[x][y].down)
                queue.Enqueue(new IntVector(x, y + 1));
            if (map.rooms[x][y].left)
                queue.Enqueue(new IntVector(x - 1, y));
            if (map.rooms[x][y].right)
                queue.Enqueue(new IntVector(x + 1, y));
        }
        map.start = latest;
        // Debug.Log("Starts at " + map.start.ToString());

        // Find endpoint
        queue.Clear();
        visited.Clear();
        queue.Enqueue(map.start);

        while (queue.Count > 0)
        {
            IntVector latestRoom = queue.Dequeue();
            if (visited.Contains(latestRoom))
                continue;
            visited.Add(latestRoom);
            latest = latestRoom;

            int x = latestRoom.x;
            int y = latestRoom.y;

            if (map.rooms[x][y].up)
                queue.Enqueue(new IntVector(x, y - 1));
            if (map.rooms[x][y].down)
                queue.Enqueue(new IntVector(x, y + 1));
            if (map.rooms[x][y].left)
                queue.Enqueue(new IntVector(x - 1, y));
            if (map.rooms[x][y].right)
                queue.Enqueue(new IntVector(x + 1, y));
        }
        map.end = latest;
        // Debug.Log("Ends at " + map.end.ToString());

        // Debug.Log("Room count: " + roomCount);
        /* map preview
        string mapgen = "";
        for (int j = 0; j < height; j++)
        {
            string str1 = "";
            string str2 = "";
            for (int i = 0; i < width; i++)
            {
                string strP = "O";
                if (map.start.Equals(new IntVector(i, j)))
                    strP = "S";
                else if (map.end.Equals(new IntVector(i, j)))
                    strP = "E";
                str1 = str1 + (map.rooms[i][j].active ? strP : " ") + (map.rooms[i][j].right ? "--" : "  ");
                str2 = str2 + (map.rooms[i][j].down ? "|" : " ") + "  ";
            }
            mapgen = mapgen + "\n" + str1 + "\n" + str2;
        }
        Debug.Log(mapgen);
        */

        return map;
    }

    public static MapController.Room RandomRoom(int numWaves)
    {
        MapController.Room room = new MapController.Room();
        room.active = true;
        room.waves = new List<Wave>();
        for (int i = 0; i < numWaves; i++)
        {
            room.waves.Add(RandomWave());
        }
        return room;
    }

    public static Wave RandomWave()
    {
        return null; // TODO
    }
    
    private static bool OutOfBounds(IntVector roomIndex, MapController.Map map)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        return (x < 0 || y < 0 || x >= map.size.x || y >= map.size.y);
    }

    private static int WeightedDistance(int size)
    {
        int n = 1;
        while (size > n + 1)
        {
            size -= ++n;
        }
        int tri = n * (n + 1) / 2;
        float rand = Random.value;
        for (int i = 1; i <= n; i++)
        {
            rand -= (1.0f / tri * i);
            if (rand <= 0)
                return i;
        }
        return n;
    }
}
