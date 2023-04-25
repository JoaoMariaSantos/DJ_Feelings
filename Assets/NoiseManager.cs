using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Noise
{
    public class NoiseManager : MonoBehaviour
    {
        public Transform playerPos;
        [Range(-1,1)]
        public float arousalValue;
        [Range(-1,1)]
        public float valenceValue;
        float arousalValueEdited;
        float valenceValueEdited;
        public float mainValuesNoiseInc;
        public float startingTime;
        private float timeTracker;

        private float seed;
        private float secondSeed;
        void Awake()
        {
            seed = Random.Range(0, 10000);
            secondSeed = Mathf.PerlinNoise(seed, seed) + 10000 * 20000;
        }

        // Update is called once per frame
        void Update()
        {
            timeTracker += Time.deltaTime;

            float noiseOffsetX = playerPos.position.x * mainValuesNoiseInc;
            float noiseOffsetZ = playerPos.position.z * mainValuesNoiseInc;

            arousalValue = -1 + Mathf.PerlinNoise(seed + noiseOffsetX, noiseOffsetZ) * 2;
            valenceValue = -1 + Mathf.PerlinNoise(seed + noiseOffsetX + secondSeed, noiseOffsetZ + 1000) * 2;

            if (timeTracker < startingTime)
            {
                float multiplier = timeTracker / startingTime;
                arousalValue *= multiplier;
                valenceValue *= multiplier;
            }

            arousalValueEdited = 1 / (1 + Mathf.Pow(234, -arousalValue)) * 2 - 1; //through sigmoid graph for more extreme results
            valenceValueEdited = 1 / (1 + Mathf.Pow(234, -valenceValue)) * 2 - 1;

            arousalValueEdited = arousalValue;
            valenceValueEdited = valenceValue;
        }

        public float GetArousalRaw()
        {
            return arousalValue;
        }
        public float GetArousalEdited()
        {
            return arousalValueEdited;
        }
        public float GetValenceRaw()
        {
            return valenceValue;
        }
        public float GetValenceEdited()
        {
            return valenceValueEdited;
        }
    }
}
