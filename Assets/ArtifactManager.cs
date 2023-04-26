using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Noise;
using Player;

namespace Artifact
{
    public class ArtifactManager : MonoBehaviour
    {
        public GameObject diamond;
        public NoiseManager noiseManager;
        public PlayerMovement playerMovement;
        public float minDistanceForGeneration = 10;
        public float maxDistance = 20;
        public List<Diamond> diamonds;
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            int? diamondTooFar = CheckDiamondTooFar();
            if(diamondTooFar != null)
            {
                GenerateDiamond(diamondTooFar);
            }
        }

        private int? CheckDiamondTooFar()
        {
            for(int i = 0; i < diamonds.Count; i++)
            {
                if(Vector2.Distance(playerMovement.GetPosFlat(), diamonds[i].GetPosFlat()) > maxDistance) return i;
            }
            return null;
        }

        private void GenerateDiamond(int? index)
        {
            float distanceFlat = Random.Range(minDistanceForGeneration, maxDistance);
            float angle = Random.Range(0, Mathf.PI * 2);
        }
    }
}
