using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Surbon.UnityPackage.Animation
{
	[Icon("Assets/AnimatedSprite/animated_sprite_icon.png"), AddComponentMenu("Rendering/AnimatedSprite")]
	[RequireComponent(typeof(SpriteRenderer))]
	public class AnimatedSprite : MonoBehaviour
	{
		/// <summary>
		/// Says if the <see cref="AnimatedSprite"/> is currently playing an animation.
		/// </summary>
		public bool Playing => enabled;

		/// <summary>
		/// Emitted when the animation is finished.
		/// </summary>
		public event Action Finished;

		[Header("Sprites")]
		[SerializeField] protected SpriteRenderer spriteRenderer;
		[SerializeField] protected List<AnimationFrames> animations = new List<AnimationFrames>();
		[Space]
		[Header("Animation")]
		[SerializeField] protected int speedScale = 30;

		protected string animationName;
		protected AnimationFrames frames;
		protected int currentFrame = 0;
		protected float timeElapsed = 0f;

		protected void Awake()
		{
			if (spriteRenderer == null)
				spriteRenderer = GetComponent<SpriteRenderer>();

			if (animations.Count == 0)
				animations[0] = ScriptableObject.CreateInstance<AnimationFrames>();

			frames = animations[0];
			animationName = animations[0].animationName;
		}

		protected void Update()
		{
			timeElapsed += Time.deltaTime;
			currentFrame = (int)(timeElapsed * speedScale);

			if (currentFrame >= frames.GetFrameCount())
			{
				if (frames.looping)
				{
					currentFrame %= frames.GetFrameCount();
				}
				else
				{
					Finished?.Invoke();
					Stop();
					return;
				}
			}

			spriteRenderer.sprite = frames[currentFrame];
		}

		/// <summary>
		/// Adds the given <see cref="AnimationFrames"/> into the list of animations.
		/// </summary>
		public void AddAnimation(AnimationFrames pAnimation)
		{
			animations.Add(pAnimation);
		}

		/// <summary>
		/// Adds the given <see cref="Sprite"/> as a frame in the selected animation.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		/// <param name="pIndex">The index of the given frame.</param>
		public void AddFrame(string pAnimation, Sprite pFrame, int pIndex = 0)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					animation.AddFrame(pFrame, pIndex);
					return;
				}
			}
		}

		/// <summary>
		/// Adds the given <see cref="Sprite"/> as a frame in the selected animation.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		/// <param name="pIndex">The index of the given frame.</param>
		public void AddFrame(int pAnimation, Sprite pFrame, int pIndex = 0)
		{
			animations[pAnimation].AddFrame(pFrame, pIndex);
		}

		/// <summary>
		/// Removes all the frames of the given animation.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public void Clear(string pAnimation)
		{
			for (int i = animations.Count - 1; i >= 0; i--)
			{
				if (animations[i].animationName == pAnimation)
				{
					Clear(i);
					return;
				}
			}
		}

		/// <summary>
		/// Removes all the frames of the given animation.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public void Clear(int pAnimation)
		{
			if (animations[pAnimation] == frames)
			{
				Stop();
				Reset();
			}

			animations[pAnimation].Clear();
		}

		/// <summary>
		/// Removes all animations. A "default" animation will be created.
		/// </summary>
		public void ClearAll()
		{
			for (int i = animations.Count - 1; i >= 0; i--)
			{
				animations[i].Clear();
				animations.RemoveAt(i);
			}

			animations.Add(ScriptableObject.CreateInstance<AnimationFrames>());
		}

		/// <summary>
		/// Returns the number of animations contained in the <see cref="AnimatedSprite"/>.
		/// </summary>
		public int GetAnimationCount() => animations.Count;

		/// <summary>
		/// Returns the current animation's name.
		/// </summary>
		public string GetCurrentAnimationName() => frames != null ? frames.animationName : "";

		/// <summary>
		/// Says if the selected animation is looping.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public bool GetLoop(string pAnimation)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
					return animation.looping;
			}

			return false;
		}

		/// <summary>
		/// Says if the selected animation is looping.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public bool GetLoop(int pAnimation)
		{
			return animations[pAnimation].looping;
		}

		/// <summary>
		/// Returns a <see cref="List{T}"/> containing all the <see cref="AnimationFrames"/>' names in
		/// the same order they are stored in the <see cref="AnimatedSprite"/>.
		/// </summary>
		public List<string> GetNames()
		{
			List<string> lNames = new List<string>();

			for (int i = 0; i < animations.Count; i++)
			{
				lNames.Add(animations[i].animationName);
			}

			return lNames;
		}

		/// <summary>
		/// Returns the animation speed in frame per seconds.
		/// </summary>
		public int GetSpeed() => speedScale;

		/// <summary>
		/// Returns the frame of the selected animation at the given index.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index</param>
		public Sprite GetFrame(string pAnimationName, int pIndex)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimationName)
					return animation[pIndex];
			}

			return null;
		}

		/// <summary>
		/// Returns the frame of the selected animation at the given index.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index</param>
		public Sprite GetFrame(int pAnimation, int pIndex)
		{
			return animations[pAnimation][pIndex];
		}

		/// <summary>
		/// Returns how many frames are in given animation.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public int GetFrameCount(string pAnimation)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
					return animation.GetFrameCount();
			}

			return -1;
		}

		/// <summary>
		/// Returns how many frames are in given animation.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public int GetFrameCount(int pAnimation)
		{
			return animations[pAnimation].GetFrameCount();
		}

		/// <summary>
		/// Says if the given animation exists in the <see cref="AnimatedSprite"/>.
		/// </summary>
		public bool HasAnimation(string pAnimation)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Starts or resume the current animation.
		/// </summary>
		public void Play() { enabled = true; }

		/// <summary>
		/// Removes the given animation.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		/// <param name="pAnimation"></param>
		public void RemoveAnimation(string pAnimation)
		{
			for (int i = animations.Count - 1; i >= 0; i--)
			{
				if (animations[i].animationName == pAnimation)
				{
					animations.RemoveAt(i);
					return;
				}
			}
		}

		/// <summary>
		/// Removes the given animation.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public void RemoveAnimation(int pAnimation)
		{
			animations.RemoveAt(pAnimation);
		}

		/// <summary>
		/// Removes the frame of the selected animation at the given index.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index.</param>
		public void RemoveFrame(string pAnimation, int pIndex)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					animation.RemoveFrame(pIndex);
					return;
				}
			}
		}

		/// <summary>
		/// Removes the frame of the selected animation at the given index.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index.</param>
		public void RemoveFrame(int pAnimation, int pIndex)
		{
			animations[pAnimation].RemoveFrame(pIndex);
		}

		/// <summary>
		/// Renames the selected animation with the given name.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public void RenameAnimation(string pAnimation, string pName)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					animation.animationName = pName;
					return;
				}
			}
		}

		/// <summary>
		/// Renames the selected animation with the given name.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public void RenameAnimation(int pAnimation, string pName)
		{
			animations[pAnimation].animationName = pName;
		}

		/// <summary>
		/// Resets the animation.
		/// </summary>
		public void Reset()
		{
			currentFrame = 0;
			timeElapsed = 0f;
		}

		/// <summary>
		/// Sets the current animation to the given animation.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public void SetCurrentAnimation(string pAnimation)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					frames = animation;
					return;
				}
			}
		}

		/// <summary>
		/// Sets the current animation to the given animation.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public void SetCurrentAnimation(int pAnimation)
		{
			frames = animations[pAnimation];
		}

		/// <summary>
		/// Makes the selected animation loop (or not).
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		public void SetLoop(string pAnimation, bool pLooping)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					animation.looping = pLooping;
					return;
				}
			}
		}

		/// <summary>
		/// Makes the selected animation loop (or not).
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		public void SetLoop(int pAnimation, bool pLooping)
		{
			animations[pAnimation].looping = pLooping;
		}

		/// <summary>
		/// Sets the animation speed to the given speed (it must be greater than 0).
		/// </summary>
		public void SetSpeed(int pSpeed)
		{
			if (pSpeed < 1)
			{
				Debug.LogError($"Argument out of range: the animation speed must be greater than 0 (current: {pSpeed}).");
				return;
			}

			speedScale = pSpeed;
		}

		/// <summary>
		/// Sets the frame of the selected animation at the given index.
		/// (You should use the overload with the index instead of this one as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index.</param>
		public void SetFrame(string pAnimation, Sprite pFrame, int pIndex)
		{
			foreach (AnimationFrames animation in animations)
			{
				if (animation.animationName == pAnimation)
				{
					animation[pIndex] = pFrame;
					return;
				}
			}
		}

		/// <summary>
		/// Sets the frame of the selected animation at the given index.
		/// (You should use this overload instead of the one with the string as much as you can)
		/// </summary>
		/// <param name="pIndex">The frame's index.</param>
		public void SetFrame(int pAnimation, Sprite pFrame, int pIndex)
		{
			animations[pAnimation][pIndex] = pFrame;
		}

		/// <summary>
		/// Stops the animation. It can be resumed by calling <see cref="Play"/>.
		/// </summary>
		public void Stop() { enabled = false; }
	}
}
