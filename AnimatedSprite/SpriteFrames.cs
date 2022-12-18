using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpriteFrames), menuName = nameof(SpriteFrames))]
public class SpriteFrames : ScriptableObject
{
	[SerializeField] protected string animationName = "default";
	[SerializeField] protected List<Sprite> frames = new List<Sprite>();
}
