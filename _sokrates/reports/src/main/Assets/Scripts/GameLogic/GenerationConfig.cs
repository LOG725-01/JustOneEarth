using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GenerationConfig
{
    public float forestProbability;
    public float mountainProbability;
    public float lakeProbability;
    public float plainProbability;
    public float desertProbability;

    public bool preventLargeDeserts;
    public float maxDesertClusterRatio;

    public bool forceLakeNearMountains;
    public bool forceForestNearLakes;

    // Charger la configuration depuis le fichier JSON dans Ressources
    public static GenerationConfig LoadFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("generation_config");
        if (jsonFile == null)
        {
            Debug.LogError("Fichier JSON de configuration non trouv� !");
            return null;
        }

        return JsonUtility.FromJson<GenerationConfig>(jsonFile.text);
    }
}
