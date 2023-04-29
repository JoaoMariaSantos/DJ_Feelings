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
        public float minDistanceForGeneration;
        public float maxDistance;
        public List<Diamond> diamonds;
        void Start()
        {
        }
        void Update()
        {
            int? indexDiamondToReset = CheckDiamondNeedsReset();
            if (indexDiamondToReset != null)
            {
                GenerateDiamond((int)indexDiamondToReset);
            }
        }

        private int? CheckDiamondNeedsReset()
        {
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (diamonds[i].GetCollectionStatus()){
                    Debug.Log(diamonds[i].gameObject.name + "wasCollected");
                     return i;
                }
                if (Vector2.Distance(playerMovement.GetPosFlat(), diamonds[i].GetPosFlat()) > maxDistance)
                {
                    Debug.Log(Vector2.Distance(playerMovement.GetPosFlat(), diamonds[i].GetPosFlat()));
                    return i;
                }
            }
            return null;
        }

        private void GenerateDiamond(int index)
        {
            float distanceFlat = Random.Range(minDistanceForGeneration, maxDistance);
            float angle = Random.Range(0, Mathf.PI * 2);
            Vector2 playerPos = playerMovement.GetPosFlat();
            Vector3 newPos = new Vector3(playerPos.x + Mathf.Cos(angle) * distanceFlat, 200, playerPos.y + Mathf.Sin(angle) * distanceFlat);

            if (CloseToOtherArtifacts(newPos, index))
            {
                GenerateDiamond(index);
                Debug.Log("Diamond too close to another");
                return;
            }

            diamonds[index].Reset(newPos);
        }

        private bool CloseToOtherArtifacts(Vector2 pos, int index)
        {
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (i == index) continue;
                else if (Vector2.Distance(diamonds[i].GetPosFlat(), pos) < minDistanceForGeneration) return true;
            }

            return false;
        }
    }
}