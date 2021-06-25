using UnityEngine;

[System.Serializable]
public class Mapping{
    public MappingUnit[] mappedSong;
    public float noteSpeed;
    public float fixedDelay;
    public float offset;
  
    [System.Serializable]
    public class MappingUnit{
        public float strumTime;
        public float endContinuous;
        public string activatorXPosition;
        public string activatorYPosition;
        
        public float xPosition;
        public float yPosition;
        public GameObject noteInstantiated;
        public GameObject longBarInstantiated;
    }

    public static class ActivatorPositions{
       public const float leftX = -1.7f;
       public const float middleLeftX = -0.6f;
       public const float middleRightX = 0.6f;
       public const float rightX = 1.7f;
       public const float redY = -4f;
       public const float blueY = -2.6f;
       public const float creatorY = -2.6f;
    }
}