using GpxTools.Gpx;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Rider.Route.Data
{
	internal class ClimbChallenge : BindableBase
	{
		
		public IReadOnlyList<IPoint> Points { get; }
		private int MaxEnd { get; }

		int _Start;
		public int Start
		{
			get => _Start;
			set
			{
				if(value < 0)
				{
					SetProperty(ref _Start, 0);
				}
				else if (value >= _End)
				{
					SetProperty(ref _Start, _End - 1);
				}
				else
				{
					SetProperty(ref _Start, value);
				}
			}				
		}
		int _End;
		public int End
		{
			get => _End;
			set
			{
				if (value <= _Start) 
				{
					SetProperty(ref _End, (ushort)(_Start + 1));
				}
				else if(value > MaxEnd) 
				{
					SetProperty(ref _End, MaxEnd);
				}
				else
				{
					SetProperty(ref _End, value);
				}
			}
		}
		public IPoint StartPoint => Points[Start];
		public IPoint EndPoint => Points[End];
		public double Size => EndPoint.Distance - StartPoint.Distance;

		public ClimbChallenge(IReadOnlyList<IPoint> points, int start, int end)
		{
			if (points==null || points.Count < 2) throw new ArgumentNullException(nameof(points));
			if (start >= points.Count || start >= end) throw new ArgumentNullException(nameof(start));
			if (end >= points.Count) throw new ArgumentNullException(nameof(end));

			Points = points;
			MaxEnd = points.Count - 1;
			_Start= start;
			_End= end;
		}
		public void MoveStartToMinElevation()
		{
			double minElevation = Points[Start].Elevation;
			for(ushort i = (ushort)(Start+1); i <= End; i++) 
			{
				if(minElevation > Points[i].Elevation)
				{
					minElevation = Points[i].Elevation;
					Start = i;
				}
			}
		}
		public ClimbChallenge CreateUnion(ClimbChallenge next)
		{
			return new ClimbChallenge(Points, Start, next.End);
		}

	}
}
