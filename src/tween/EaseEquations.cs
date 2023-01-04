using System;
using UnityEngine;
using static Com.Surbon.UnityPackage.Tweens.Tween;

namespace Com.Surbon.UnityPackage.Tweens
{
	public class EaseEquations
	{
		public static Func<float, float, float, float> GetEquation(TransitionType pTransitionType, EaseType pEaseType)
		{
			return equations[(int)pTransitionType, (int)pEaseType];
		}

		private static Func<float, float, float, float>[,] equations = new Func<float, float, float, float>[TRANSITION_MAX, EASE_MAX]
		{
			{ LinearIn, LinearIn, LinearIn, LinearIn },
			{ SineIn, SineOut, SineInOut, SineOutIn },
			{ QuintIn, QuintOut, QuintInOut, QuintOutIn },
			{ QuartIn, QuartOut, QuartInOut, QuartOutIn },
			{ QuadIn, QuadOut, QuadInOut, QuadOutIn },
			{ ExpoIn, ExpoOut, ExpoInOut, ExpoOutIn },
			{ ElasticIn, ElasticOut, ElasticInOut, ElasticOutIn },
			{ CubicIn, CubicOut, CubicInOut, CubicOutIn },
			{ CircIn, CircOut, CircInOut, CircOutIn },
			{ BounceIn, BounceOut, BounceInOut, BounceOutIn },
			{ BackIn, BackOut, BackInOut, BackOutIn }
		};

		#region LINEAR

		private static float LinearIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * pTime;
		}

		#endregion // LINEAR

		#region SINE

		private static float SineIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * (1f - Mathf.Cos(pTime * (Mathf.PI / 2f)));
		}

		private static float SineOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * Mathf.Sin(pTime * (Mathf.PI / 2f));
		}

		private static float SineInOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial - pDelta / 2f * (Mathf.Cos(Mathf.PI * pTime) - 1f);
		}

		private static float SineOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				SineOut(pTime * 2f, pInitial, pDelta / 2f) :
				SineIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // SINE

		#region QUINT

		private static float QuintIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * Mathf.Pow(pTime, 5f);
		}

		private static float QuintOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * (Mathf.Pow(pTime - 1f, 5f) + 1f);
		}

		private static float QuintInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (pTime < 1f ? Mathf.Pow(pTime, 5f) : (Mathf.Pow(pTime - 2f, 5f) + 2f));
		}

		private static float QuintOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				QuintOut(pTime * 2f, pInitial, pDelta / 2f) :
				QuintIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // QUINT

		#region QUART

		private static float QuartIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * Mathf.Pow(pTime, 4f);
		}

		private static float QuartOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial - pDelta * (Mathf.Pow(pTime - 1f, 4f) - 1f);
		}

		private static float QuartInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (pTime < 1f ? Mathf.Pow(pTime, 4f) : -(Mathf.Pow(pTime - 2f, 4f) - 2f));
		}

		private static float QuartOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				QuartOut(pTime * 2f, pInitial, pDelta / 2f) :
				QuartIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // QUART

		#region QUAD

		private static float QuadIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * pTime * pTime;
		}

		private static float QuadOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial - pDelta * pTime * (pTime - 2f);
		}

		private static float QuadInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (pTime < 1f ? (pTime * pTime) : -((pTime - 1f) * (pTime - 3f) - 1f));
		}

		private static float QuadOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				QuadOut(pTime * 2f, pInitial, pDelta / 2f) :
				QuadIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // QUAD

		#region EXPO

		private static float ExpoIn(float pTime, float pInitial, float pDelta)
		{
			return pTime == 0 ? pInitial : pInitial + pDelta * (Mathf.Pow(2f, 10f * (pTime - 1)) - 0.001f);
		}

		private static float ExpoOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial + (pTime == 1f ? pDelta : pDelta * 1.001f * (-Mathf.Pow(2f, -10f * pTime) + 1f));
		}

		private static float ExpoInOut(float pTime, float pInitial, float pDelta)
		{
			if (pTime == 0f)
				return pInitial;
			else if (pTime == 1f)
				return pInitial + pDelta;

			pTime *= 2f;

			return pInitial + pDelta / 2f * (
				pTime < 1f ?
				Mathf.Pow(2f, 10f * (pTime - 1f)) - 0.0005f :
				1.0005f * (-Mathf.Pow(2f, -10f * (pTime - 1f)) + 2f)
			);
		}

		private static float ExpoOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				ExpoOut(pTime * 2f, pInitial, pDelta / 2f) :
				ExpoIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // EXPO

		#region ELASTIC

		private static float ElasticIn(float pTime, float pInitial, float pDelta)
		{
			if (pTime == 0f)
				return pInitial;
			else if (pTime == 1f)
				return pInitial + pDelta;

			--pTime;
			return pInitial - (pDelta * Mathf.Pow(2f, 10f * pTime) * Mathf.Sin((pTime - 0.075f) * (2f * Mathf.PI) / 0.3f));
		}

		private static float ElasticOut(float pTime, float pInitial, float pDelta)
		{
			if (pTime == 0f)
				return pInitial;
			else if (pTime == 1f)
				return pInitial + pDelta;

			return pInitial + pDelta * (1f + (Mathf.Pow(2f, -10f * pTime) * Mathf.Sin((pTime - 0.075f) * (2f * Mathf.PI) / 0.3f)));
		}

		private static float ElasticInOut(float pTime, float pInitial, float pDelta)
		{
			if (pTime == 0f)
				return pInitial;
			else if (pTime == 1f)
				return pInitial + pDelta;

			pTime = pTime * 2f - 1f;

			if (pTime < 0f)
			{
				return pInitial -
					0.5f * (pDelta * Mathf.Pow(2, 10f * pTime)) * Mathf.Sin((pTime - 0.1125f) * (2f * Mathf.PI) / 0.45f);
			}
			else
			{
				return pInitial + pDelta +
					0.5f * (pDelta * Mathf.Pow(2, -10f * pTime)) * Mathf.Sin((pTime - 0.1125f) * (2f * Mathf.PI) / 0.45f);
			}
		}

		private static float ElasticOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				ElasticOut(pTime * 2f, pInitial, pDelta / 2f) :
				ElasticIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // ELASTIC

		#region CUBIC

		private static float CubicIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * Mathf.Pow(pTime, 3f);
		}

		private static float CubicOut(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * (Mathf.Pow(pTime - 1f, 3f) + 1f);
		}

		private static float CubicInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (pTime < 1f ? Mathf.Pow(pTime, 3f) : Mathf.Pow(pTime - 2f, 3f) + 2f);
		}

		private static float CubicOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				CubicOut(pTime * 2f, pInitial, pDelta / 2f) :
				CubicIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // CUBIC

		#region CIRC

		private static float CircIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial - pDelta * (Mathf.Sqrt(1f - pTime * pTime) - 1f);
		}

		private static float CircOut(float pTime, float pInitial, float pDelta)
		{
			--pTime;
			return pInitial + pDelta * Mathf.Sqrt(1f - pTime * pTime);
		}

		private static float CircInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (
				pTime < 1f ?
				-(Mathf.Sqrt(1f - pTime * pTime) - 1f) :
				Mathf.Sqrt(1f - Mathf.Pow(pTime - 2f, 2f)) + 1f
			);
		}

		private static float CircOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				CircOut(pTime * 2f, pInitial, pDelta / 2f) :
				CircIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion CIRC

		#region BOUNCE

		private static float BounceIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta - BounceOut(1f - pTime, 0f, pDelta);
		}

		private static float BounceOut(float pTime, float pInitial, float pDelta)
		{
			if (pTime < (1 / 2.75f))
			{
				return pInitial + pDelta * (7.5625f * pTime * pTime);
			}
			else if (pTime < (2 / 2.75f))
			{
				pTime -= 1.5f / 2.75f;
				return pInitial + pDelta * (7.5625f * pTime * pTime + 0.75f);
			}
			else if (pTime < (2.5 / 2.75))
			{
				pTime -= 2.25f / 2.75f;
				return pInitial + pDelta * (7.5625f * pTime * pTime + 0.9375f);
			}
			else
			{
				pTime -= 2.625f / 2.75f;
				return pInitial + pDelta * (7.5625f * pTime * pTime + 0.984375f);
			}
		}

		private static float BounceInOut(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				BounceIn(pTime * 2f, pInitial, pDelta / 2f) :
				BounceOut(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		private static float BounceOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				BounceOut(pTime * 2f, pInitial, pDelta / 2f) :
				BounceIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // BOUNCE

		#region BACK

		private static float BackIn(float pTime, float pInitial, float pDelta)
		{
			return pInitial + pDelta * pTime * pTime * (2.70158f * pTime - 1.70158f);
		}

		private static float BackOut(float pTime, float pInitial, float pDelta)
		{
			--pTime;
			return pInitial + pDelta * (pTime * pTime * (2.70158f * pTime + 1.70158f) + 1f);
		}

		private static float BackInOut(float pTime, float pInitial, float pDelta)
		{
			pTime *= 2f;
			return pInitial + pDelta / 2f * (
				pTime < 1f ?
				pTime * pTime * (3.5949095f * pTime - 2.5949095f) :
				(Mathf.Pow(pTime - 2f, 2f) * (3.5949095f * (pTime - 2f) + 2.5949095f) + 2f)
			);
		}

		private static float BackOutIn(float pTime, float pInitial, float pDelta)
		{
			return pTime < 0.5f ?
				BackOut(pTime * 2f, pInitial, pDelta / 2f) :
				BackIn(pTime * 2f - 1f, pInitial + pDelta / 2f, pDelta / 2f);
		}

		#endregion // BACK
	}
}
