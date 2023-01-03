using System.Collections.Generic;
using UnityEngine;

namespace Com.Surbon.UnityPackage.Animation
{
	/// <summary>
	/// A class used to store the frames used by an <see cref="AnimatedSprite"/>.
	/// </summary>
	[CreateAssetMenu(fileName = nameof(AnimationFrames), menuName = nameof(AnimationFrames))]
	public class AnimationFrames : ScriptableObject
	{
		public Sprite this[int pIndex]
		{
			get => frames[pIndex];
			set
			{
				if (pIndex < 0 || pIndex >= frames.Count)
				{
					Debug.LogError($"Index out of range: invalid frame position ({pIndex}).");
					return;
				}

				frames[pIndex] = value;
			}
		}

		public bool looping = false;
		public string animationName = "default";
		[SerializeField] protected List<Sprite> frames = new List<Sprite>();

		public void AddFrame(Sprite pFrame, int pIndex)
		{
			if (pIndex < 0 || pIndex > frames.Count)
			{
				Debug.LogError($"Index out of range: invalid frame position ({pIndex}).");
				return;
			}

			frames.Insert(pIndex, pFrame);
		}

		public void Clear() { frames.Clear(); }

		public int GetFrameCount() => frames.Count;

		public void RemoveFrame(int pIndex)
		{
			if (pIndex < 0 || pIndex >= frames.Count)
			{
				Debug.LogError($"Index out of range: invalid frame position ({pIndex}).");
				return;
			}

			frames.RemoveAt(pIndex);
		}
	}
}
