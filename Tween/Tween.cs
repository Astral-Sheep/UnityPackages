using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using SObject = System.Object;

namespace Com.Surbon.UnityPackage.Tweens
{
	/// <summary>
	/// A Godot-like <see cref="Tween"/> component.
	/// </summary>
	[Icon("Assets/Tween/tween_icon.png"), AddComponentMenu("Miscellaneous/Tween")]
	public class Tween : MonoBehaviour
	{
		internal const int TRANSITION_MAX = 11;
		internal const int EASE_MAX = 4;

		#region TWEENERS

		protected abstract class Tweener
		{
			protected Tween tween;

			protected float elapsedTime = 0f;
			protected float duration = 0f;
			protected float delay = 0f;

			public Tweener(Tween pTween, float pDuration, float pDelay)
			{
				tween = pTween;
				duration = pDuration;
				delay = pDelay;
			}

			public abstract void Run(float pDelta);

			public void ResetTime() { elapsedTime = 0f; }

			public virtual void Kill() { tween = null; }

			public bool Finished() => elapsedTime >= delay + duration;
		}

		protected class CallbackTweener : Tweener
		{
			protected Action callback;

			public CallbackTweener(Action pCallback, Tween pTween, float pDelay) : base(pTween, pDelay, 0f)
			{
				callback = pCallback;
			}

			public override void Run(float pDelta)
			{
				elapsedTime += pDelta;

				if (elapsedTime >= duration)
				{
					callback.Invoke();
					tween.RemoveTweener(this);
				}
			}
		}

		protected abstract class InterpolationTweener : Tweener
		{
			protected dynamic from;
			protected dynamic to;
			protected TransitionType transitionType = TransitionType.Linear;
			protected EaseType easeType = EaseType.EaseIn;

			protected Func<float, float, float, float> Interpolate;
			protected Action RunInternal;

			public InterpolationTweener(Tween pTween, float pDuration, float pDelay) : base(pTween, pDuration, pDelay) { }

			protected virtual bool Init<T>(IEquatable<T> pFrom, IEquatable<T> pTo, TransitionType pTransitionType, EaseType pEaseType)
			{
				from = pFrom;
				to = pTo;

				RunInternal = from switch
				{
					bool => RunBool,
					int => RunNonFloat,
					float => RunFloat,
					double => RunNonFloat,
					Vector2 => RunVector2,
					Vector2Int => RunVector2I,
					Vector3 => RunVector3,
					Vector3Int => RunVector3I,
					Vector4 => RunVector4,
					Quaternion => RunQuaternion,
					Color => RunColor,
					_ => null
				};

				if (RunInternal == null)
				{
					Debug.LogError($"The type {from.GetType()} cannot be interpolated.");
					return false;
				}

				transitionType = pTransitionType;
				easeType = pEaseType;

				Interpolate = EaseEquations.GetEquation(transitionType, easeType);

				return true;
			}

			protected abstract void RunBool();
			protected abstract void RunFloat();
			protected abstract void RunNonFloat();
			protected abstract void RunVector2();
			protected abstract void RunVector2I();
			protected abstract void RunVector3();
			protected abstract void RunVector3I();
			protected abstract void RunVector4();
			protected abstract void RunQuaternion();
			protected abstract void RunColor();
		}

		protected class MethodTweener : InterpolationTweener
		{
			protected Action<dynamic> method;

			public MethodTweener(Tween pTween, float pDuration, float pDelay) : base(pTween, pDuration, pDelay) { }

			public bool Init<T>(Action<dynamic> pMethod, IEquatable<T> pFrom, IEquatable<T> pTo, TransitionType pTransitionType, EaseType pEaseType)
			{
				method = pMethod;
				return base.Init(pFrom, pTo, pTransitionType, pEaseType);
			}

			public override void Run(float pDelta)
			{
				elapsedTime += pDelta;

				if (elapsedTime >= delay)
				{
					RunInternal();

					if (elapsedTime >= delay + duration)
					{
						tween.RemoveTweener(this);
					}
				}
			}

			#region INTERNAL_RUNNERS

			protected override void RunBool()
			{
				method.Invoke(Interpolate(
					Mathf.Clamp01((elapsedTime - delay) / duration),
					(float)from,
					(float)(to - from)
				) >= 0.5f);
			}

			protected override void RunFloat()
			{
				method.Invoke(Interpolate(Mathf.Clamp01((elapsedTime - delay) / duration), from, to - from));
			}

			protected override void RunNonFloat()
			{
				method.Invoke(Interpolate(
					Mathf.Clamp01((elapsedTime - delay) / duration),
					(float)from,
					(float)(to - from)
				));
			}

			protected override void RunVector2()
			{
				Vector2 lFrom = (Vector2)from;
				Vector2 lTo = (Vector2)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				method.Invoke(new Vector2(
					Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
					Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y)
				));
			}

			protected override void RunVector2I()
			{
				Vector2Int lFrom = (Vector2Int)from;
				Vector2Int lTo = (Vector2Int)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				method.Invoke(new Vector2Int(
					(int)Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
					(int)Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y)
				));
			}

			protected override void RunVector3()
			{
				Vector3 lFrom = (Vector3)from;
				Vector3 lTo = (Vector3)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				method.Invoke(new Vector3(
					Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
					Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
					Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z)
				));
			}

			protected override void RunVector3I()
			{
				Vector3Int lFrom = (Vector3Int)from;
				Vector3Int lTo = (Vector3Int)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				method.Invoke(new Vector3Int(
					(int)Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
					(int)Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
					(int)Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z)
				));
			}

			protected override void RunVector4()
			{
				Vector4 lFrom = (Vector4)from;
				Vector4 lTo = (Vector4)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				method.Invoke(new Vector4(
					Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
					Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
					Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z),
					Interpolate(lRatio, lFrom.w, lTo.w - lFrom.w)
				));
			}

			protected override void RunQuaternion()
			{
				method.Invoke(Quaternion.Slerp(
					(Quaternion)from,
					(Quaternion)to,
					Mathf.Clamp01((elapsedTime - delay) / duration))
				);
			}

			protected override void RunColor()
			{
				method.Invoke(Color.Lerp(
					(Color)from,
					(Color)to,
					Mathf.Clamp01((elapsedTime - delay) / duration))
				);
			}

			#endregion // INTERNAL_RUNNERS
		}

		protected class PropertyTweener : InterpolationTweener
		{
			protected SObject target = null;
			protected Type targetType = null;

			protected PropertyInfo property;

			public PropertyTweener(SObject pTarget, Tween pTween, float pDuration, float pDelay) : base(pTween, pDuration, pDelay)
			{
				target = pTarget;
				targetType = pTarget.GetType();
			}

			public bool Init<T>(string pProperty, IEquatable<T> pFrom, IEquatable<T> pTo, TransitionType pTransitionType, EaseType pEaseType)
			{
				// Test: the property exists
				if (targetType.GetProperty(pProperty) == null)
				{
					Debug.LogError($"{targetType.Name} does not contain property {pProperty}.");
					return false;
				}

				property = targetType.GetProperty(pProperty);

				// Test: the type of the property is the same as the type of the interpolation values
				if (property.PropertyType != pFrom.GetType())
				{
					Debug.LogError($"{targetType.Name}'s property {pProperty} is not a {pFrom.GetType()}.");
					return false;
				}

				return base.Init(pFrom, pTo, pTransitionType, pEaseType);
			}

			public override void Run(float pDelta)
			{
				elapsedTime += pDelta;

				if (elapsedTime >= delay)
				{
					RunInternal();

					if (elapsedTime >= delay + duration)
					{
						tween.RemoveTweener(this);
					}
				}
			}

			public override void Kill()
			{
				base.Kill();
				target = null;
			}

			#region INTERNAL_RUNNERS

			protected override void RunBool()
			{
				property.SetValue(
					target,
					Interpolate(
						Mathf.Clamp01((elapsedTime - delay) / duration),
						(float)from,
						(float)(to - from)
					) >= 0.5f
				);
			}

			protected override void RunFloat()
			{
				property.SetValue(
					target,
					Interpolate(
						Mathf.Clamp01((elapsedTime - delay) / duration),
						from,
						to - from
					)
				);
			}

			protected override void RunNonFloat()
			{
				property.SetValue(
					target,
					Interpolate(
						Mathf.Clamp01((elapsedTime - delay) / duration),
						(float)from,
						(float)(to - from)
					)
				);
			}

			protected override void RunVector2()
			{
				Vector2 lFrom = (Vector2)from;
				Vector2 lTo = (Vector2)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				property.SetValue(
					target,
					new Vector2(
						Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
						Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y)
					)
				);
			}

			protected override void RunVector2I()
			{
				Vector2Int lFrom = (Vector2Int)from;
				Vector2Int lTo = (Vector2Int)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				property.SetValue(
					target,
					new Vector2Int(
						(int)Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
						(int)Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y)
					)
				);
			}

			protected override void RunVector3()
			{
				Vector3 lFrom = (Vector3)from;
				Vector3 lTo = (Vector3)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				property.SetValue(
					target,
					new Vector3(
						Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
						Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
						Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z)
					)
				);
			}

			protected override void RunVector3I()
			{
				Vector3Int lFrom = (Vector3Int)from;
				Vector3Int lTo = (Vector3Int)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				property.SetValue(
					target,
					new Vector3Int(
						(int)Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
						(int)Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
						(int)Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z)
					)
				);
			}

			protected override void RunVector4()
			{
				Vector4 lFrom = (Vector4)from;
				Vector4 lTo = (Vector4)to;
				float lRatio = Mathf.Clamp01((elapsedTime - delay) / duration);

				property.SetValue(
					target,
					new Vector4(
						Interpolate(lRatio, lFrom.x, lTo.x - lFrom.x),
						Interpolate(lRatio, lFrom.y, lTo.y - lFrom.y),
						Interpolate(lRatio, lFrom.z, lTo.z - lFrom.z),
						Interpolate(lRatio, lFrom.w, lTo.w - lFrom.w)
					)
				);
			}

			protected override void RunQuaternion()
			{
				property.SetValue(
					target,
					Quaternion.Slerp((Quaternion)from, (Quaternion)to, Mathf.Clamp01((elapsedTime - delay) / duration))
				);
			}

			protected override void RunColor()
			{
				property.SetValue(
					target,
					Color.Lerp((Color)from, (Color)to, Mathf.Clamp01((elapsedTime - delay) / duration))
				);
			}

			#endregion // INTERNAL_RUNNERS
		}

		#endregion // TWEENERS

		#region ENUMS

		/// <summary>
		/// Enum used to get the curve of the interpolation.
		/// </summary>
		public enum TransitionType
		{
			/// <summary>
			/// The default transition.
			/// </summary>
			Linear = 0,
			/// <summary>
			/// A transition that uses the sinus curve.
			/// </summary>
			Sine = 1,
			/// <summary>
			/// A transition that uses the x to the power of 5 curve.
			/// </summary>
			Quint = 2,
			/// <summary>
			/// A transition that uses the x to the power of 4 curve.
			/// </summary>
			Quart = 3,
			/// <summary>
			/// A transition that uses the x squared curve.
			/// </summary>
			Quad = 4,
			/// <summary>
			/// A transition that uses the exponential curve.
			/// </summary>
			Expo = 5,
			/// <summary>
			/// A transition that uses an elastic curve.
			/// </summary>
			Elastic = 6,
			/// <summary>
			/// A transition that uses the x to the power of 3 curve.
			/// </summary>
			Cubic = 7,
			/// <summary>
			/// A transition that uses the a circular curve.
			/// </summary>
			Circ = 8,
			/// <summary>
			/// A transition that uses a bouncing curve.
			/// </summary>
			Bounce = 9,
			/// <summary>
			/// A transition that goes further than the max value and goes back to it.
			/// </summary>
			Back = 10,
		}

		/// <summary>
		/// Enum used to get the ease of the interpolation.
		/// </summary>
		public enum EaseType
		{
			/// <summary>
			/// The default ease. It starts slowly and speeds up.
			/// </summary>
			EaseIn = 0,
			/// <summary>
			/// The opposite of <see cref="EaseIn"/>. It starts quickly and slows down.
			/// </summary>
			EaseOut = 1,
			/// <summary>
			/// An ease that starts slowly, speeds up and slows down at the end.
			/// </summary>
			EaseInOut = 2,
			/// <summary>
			/// An ease that starts quickly, slows down in the middle and speeds up at the end.
			/// </summary>
			EaseOutIn = 3,
		}

		#endregion // ENUMS

		protected List<Tweener> tweeners = new List<Tweener>();
		protected float totalElapsedTime = 0f;
		protected float speedScale = 1f;

		protected virtual void Awake() { enabled = false; }

		protected virtual void Update() { RunTweeners(Time.deltaTime); }

		protected void OnDestroy()
		{
			if (tweeners.Count > 0)
			{
				for (int i = tweeners.Count - 1; i >= 0; i--)
				{
					tweeners[i].Kill();
					tweeners.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Runs the <see cref="Tween"/> by the given delta value in seconds. This is mostly useful for manual
		/// control when the <see cref="Tween"/> is paused. It can also be used to finish the <see cref="Tween"/>
		/// immediately, by setting delta longer than the whole duration of the <see cref="Tween"/> animation.
		/// </summary>
		/// <param name="pDelta">The delta time in seconds.</param>
		/// <returns>
		/// Returns true if the <see cref="Tween"/> still has <see cref="Tweener"/>s
		/// that haven't finished.
		/// </returns>
		public bool CustomStep(float pDelta)
		{
			RunTweeners(pDelta);

			for (int i = tweeners.Count - 1; i >= 0; i--)
			{
				if (!tweeners[i].Finished())
					return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the total time in seconds the <see cref="Tween"/> has been animating (not counting pauses).
		/// The time is affected by <see cref="SetSpeedScale(float)"/>, and <see cref="Stop"/> will reset it to 0.
		/// </summary>
		public float GetTotalTimeElapsed() => totalElapsedTime;

		/// <summary>
		/// Returns whether the <see cref="Tween"/> is currently running (it wasn't paused and it's not finished).
		/// </summary>
		public bool IsRunning() => enabled && tweeners.Count > 0;

		/// <summary>
		/// Aborts all tweening operations and destroys the <see cref="Tween"/>.
		/// </summary>
		public void Kill()
		{
			for (int i = tweeners.Count - 1; i >= 0; i--)
			{
				tweeners[i].Kill();
				tweeners.RemoveAt(i);
			}

			Destroy(this);
		}

		/// <summary>
		/// Pauses the tweening. The animation can be resumed by using <see cref="Play"/>
		/// </summary>
		public void Pause() { enabled = false; }

		/// <summary>
		/// Starts a <see cref="Tween"/>, or resumes a paused or stopped <see cref="Tween"/>.
		/// </summary>
		public void Play() { enabled = true; }

		/// <summary>
		/// Scales the speed of tweening. This affects all <see cref="Tweener"/>s and their delays.
		/// </summary>
		public Tween SetSpeedScale(float pSpeedScale)
		{
			if (pSpeedScale < 0)
			{
				Debug.LogError($"The speed scale must be greater than or equal to 0 (currently {pSpeedScale}).");
				return this;
			}

			speedScale = pSpeedScale;
			return this;
		}

		/// <summary>
		/// Stops the tweening and resets the <see cref="Tween"/> to its initial state.
		/// This will not remove any appended <see cref="Tweener"/>.
		/// </summary>
		public void Stop()
		{
			enabled = false;
			totalElapsedTime = 0f;

			foreach (Tweener tweener in tweeners)
			{
				tweener.ResetTime();
			}
		}

		#region INTERPOLATIONS

		public Tween InterpolateCallback(Action pCallback, float pDelay)
		{
			if (pDelay < 0)
			{
				Debug.LogError($"Tween delay must be greater than or equal to 0 (currently {pDelay}).");
				return this;
			}

			tweeners.Add(new CallbackTweener(pCallback, this, pDelay));
			return this;
		}

		public Tween InterpolateMethod<T>(Action<dynamic> pMethod, IEquatable<T> pFrom, IEquatable<T> pTo, float pDuration, TransitionType pTransitionType = TransitionType.Linear, EaseType pEaseType = EaseType.EaseIn, float pDelay = 0f)
		{
			if (pDuration < 0)
			{
				Debug.LogError($"Tween duration must be greater than or equal to 0 (currently {pDuration}).");
				return this;
			}

			if (pDelay < 0)
			{
				Debug.LogError($"Tween delay must be greater than or equal to 0 (currently {pDelay}).");
				return this;
			}

			MethodTweener lTweener = new MethodTweener(this, pDuration, pDelay);

			if (!lTweener.Init(pMethod, pFrom, pTo, pTransitionType, pEaseType))
				return this;

			tweeners.Add(lTweener);

			return this;
		}

		public Tween InterpolateProperty<T>(SObject pTarget, string pProperty, IEquatable<T> pFrom, IEquatable<T> pTo, float pDuration, TransitionType pTransitionType = TransitionType.Linear, EaseType pEaseType = EaseType.EaseIn, float pDelay = 0f)
		{
			if (pDuration < 0)
			{
				Debug.LogError($"Tween duration must be greater than or equal to 0 (currently {pDuration}).");
				return this;
			}

			if (pDelay < 0)
			{
				Debug.LogError($"Tween delay must be greater than or equal to 0 (currently {pDelay}).");
				return this;
			}

			PropertyTweener lTweener = new PropertyTweener(pTarget, this, pDuration, pDelay);

			if (!lTweener.Init(pProperty, pFrom, pTo, pTransitionType, pEaseType))
				return this;

			tweeners.Add(lTweener);

			return this;
		}

		#endregion // INTERPOLATIONS

		protected void RunTweeners(float pDelta)
		{
			for (int i = tweeners.Count - 1; i >= 0; i--)
			{
				tweeners[i].Run(pDelta * speedScale);
			}

			totalElapsedTime += pDelta * speedScale;

			if (tweeners.Count == 0)
				enabled = false;
		}

		protected void RemoveTweener(Tweener pTweener)
		{
			if (tweeners.Contains(pTweener))
				tweeners.Remove(pTweener);
		}
	}
}
