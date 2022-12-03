using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Rider.Route.Data
{
	internal class ClimbChallengeCalculator
	{
		private const decimal ClimbRatioLimit = 0.03m;
		private const decimal LowHeightRatio = 0.1m;

		private IReadOnlyList<RoutePoint> Points { get; }
		private IReadOnlyList<ClimbChallenge> Challenges { get; set; }= new List<ClimbChallenge>();
		public ClimbChallengeCalculator(IReadOnlyList<RoutePoint> points) 
		{
			Points = points;
		}
		public ObservableCollection<ClimbChallenge> Calculate()
		{
			CreateChallenges();
			while (ConnectChallenges())
			{

			}

			DeleteSmallChalenges();
			RemoveFlatFoothill();

			return new ObservableCollection<ClimbChallenge>(Challenges);
		}
		void RemoveFlatFoothill()
		{
			List<ClimbChallenge> challenges = new List<ClimbChallenge>();
			for (int i = 0; i < Challenges.Count; i++)
			{
				 RemoveFlatFoothill(Challenges[i], challenges);
			}
			Challenges = challenges;
		}
		public void RemoveFlatFoothill(ClimbChallenge chalenge, List<ClimbChallenge> result)
		{
			decimal lowHeight = chalenge.EndPoint.Elevation * LowHeightRatio + chalenge.StartPoint.Elevation;
			for (ushort i = chalenge.Start; i <= chalenge.End; i++)
			{
				if (Points[i].Elevation > lowHeight)
				{
					if(chalenge.End - i > 2)
					{
						result.Add( new ClimbChallenge(Points, i, chalenge.End));
						return;
					}
				}
			}
			result.Add(chalenge);
		}

		void DeleteSmallChalenges()
		{
			List<ClimbChallenge> challenges = new List<ClimbChallenge>();
			for (int i = 0; i < Challenges.Count; i++)
			{
				if (!IsSmall(Challenges[i])) challenges.Add(Challenges[i]);
			}
			Challenges = challenges;
		}
		public bool IsSmall(ClimbChallenge challenge)
		{
			decimal Lenght = challenge.EndPoint.Distance - challenge.StartPoint.Distance;
			decimal Height = challenge.EndPoint.Elevation - challenge.StartPoint.Elevation;
			decimal Grade = Height / (1000 * Lenght);

			if (Lenght > 2 && Height > 60m) return false;
			if (Lenght > 1 && Grade > .045m) return false;
			if (Lenght > 0.5m && Grade > .07m) return false;
			if (Lenght > 0.2m && Grade > .09m) return false;
			if (Lenght > 0.1m && Grade > .11m) return false;
			if (Lenght > 0.02m && Grade > .15m) return false;
			return true;
		}

		void CreateChallenges()
		{
			List<ClimbChallenge> challenges = new List<ClimbChallenge>();

			int start = -1;

			for (int i = 1; i < Points.Count; i++)
			{
				RoutePoint p0 = Points[i - 1];
				RoutePoint p1 = Points[i];

				decimal dist = p1.Distance - p0.Distance;
				decimal elevation = p1.Elevation - p0.Elevation;
				decimal ratio = elevation / dist;

				if (start < 0)
				{
					if (ratio > ClimbRatioLimit)
					{
						start = i - 1;
						continue;
					}
				}
				else
				{
					if (elevation <= 0)
					{
						challenges.Add(new ClimbChallenge(Points, (ushort)start, (ushort)(i - 1)));
						start = -1;
					}
				}
			}

			Challenges = challenges;
		}

		bool ConnectChallenges()
		{
			if (Challenges.Count < 2) return false;

			List<ClimbChallenge> connected = new List<ClimbChallenge>();

			ClimbChallenge actual = Challenges[0];

			ConnectChallenge(actual, 1, connected);
			int oldCount = Challenges.Count;

			Challenges = connected;

			return oldCount != Challenges.Count;
		}
		void ConnectChallenge(ClimbChallenge actual, int index, List<ClimbChallenge> results)
		{
			if (index >= Challenges.Count)
			{
				results.Add(actual);
				return;
			}

			if (index < Challenges.Count)
			{
				for (int i = index; i < Challenges.Count; i++)
				{
					if (IsNearby(actual, Challenges[i]))
					{
						if (IsAscending(actual, Challenges[i]))
						{
							ConnectChallenge(actual.CreateUnion(Challenges[i]), i + 1, results);
							return;
						}
					}
				}
			}
			results.Add(actual);

			ConnectChallenge(Challenges[index], index + 1, results);

		}

		public bool IsNearby(ClimbChallenge actual, ClimbChallenge next)
		{
			const decimal NearbyDistance = 2000; //m

			return (next.StartPoint.Distance - actual.EndPoint.Distance) < NearbyDistance;
		}
		public bool IsAscending(ClimbChallenge actual, ClimbChallenge next)
		{
			return (next.EndPoint.Elevation - actual.EndPoint.Elevation) > 0;
		}




	}
}
