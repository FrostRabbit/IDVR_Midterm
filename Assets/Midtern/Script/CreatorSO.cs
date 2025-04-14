using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creator", menuName = "Midterm/Creator")]
public class CreatorSO : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;

    public int health;
    public int atk = 1;

    public float size = 1;
    public float explosion_size = 1;
    public ParticleSystem deathEffect = null;
    public GameObject prefab = null;

    public Color color;
}
