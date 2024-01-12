using UnityEngine;
using System.Collections;


public static class FalloffGenerator {

    public enum FallOffType { None, Ocean, WestCoast, EastCoast, NorthCoast, SouthCoast, NWCoast, NECoast, SWCoast, SECoast }

	public static float[,] GenerateFalloffMap(int size, FallOffType falloffType, float intensity) {
		float[,] map = new float[size,size];
		if (falloffType == FallOffType.None)
		{
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
					map[i, j] = 1;
                }
            }
            return map;
        }

		if (falloffType == FallOffType.Ocean)
		{
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					map[i, j] = 1 - intensity;
                }
			}
			return map;
		}


        for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				float x = i / ((float)size-1);
				float y = j / ((float)size-1);

				if (falloffType == FallOffType.WestCoast)
				{
					x = 1-x;
					y = 0;
				}
				else if (falloffType == FallOffType.EastCoast)
				{
					//x = x;
                    y = 0;
                }
                if (falloffType == FallOffType.NorthCoast)
                {
                    y = 1-y;
					x = 0;
                }
                else if (falloffType == FallOffType.SouthCoast)
                {
                    //y = y;
					x = 0;
                }
				else if (falloffType == FallOffType.NWCoast)
				{
					x = 1-x;
					y = 1-y;
				}
                else if (falloffType == FallOffType.NECoast)
                {
                    //x = x;
                    y = 1 - y;
                }
                else if (falloffType == FallOffType.SWCoast)
                {
                    x =	1 - x;
                    //y = y;
                }
                else if (falloffType == FallOffType.SECoast)
                {
                    //x = x;
                    //y = y;
                }

                float valueX = SmoothStep(0, 1, x);
				float valueY = SmoothStep(0, 1, y);

                map [i, j] = Mathf.Max(valueX, valueY) * intensity;
                map[i, j] = 1 - map[i, j];

            }
		}
        return map;
	}
    //https://en.wikipedia.org/wiki/Smoothstep
    static float SmoothStep(float edge0, float edge1, float x)
    {
        // Scale, and clamp x to 0..1 range
        x = clamp((x-edge0) / (edge1-edge0));
        return x * x * (3.0f - 2.0f * x);
    }

    static float clamp(float x, float lowerlimit = 0.0f, float upperlimit = 1.0f)
    {
        if (x < lowerlimit) return lowerlimit;
        if (x > upperlimit) return upperlimit;
        return x;
    }

}
