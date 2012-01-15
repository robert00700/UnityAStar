using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathElement {
	public int x;
	public int y;
	public float sofar;
	public PathElement prev;
	
	public PathElement(int x1, int y1, float so, PathElement p) {
		x = x1;
		y = y1;
		sofar = so;
		prev = p;
	}
	
	public PathElement(int x1, int y1, PathElement p) {
		x = x1;
		y = y1;
		sofar = 0.0f;
		prev = p;
	}
}

public class Square {
	public int x, y;
	public Square(int xx, int yy) {
		x = xx;
		y = yy;
	}
	public override int GetHashCode() {
      return x * 1337 + y * 42;
   }
	public override bool Equals(System.Object obj) {
		Square s = (Square)obj;
		return (x == s.x) && (y == s.y);
	}
}

public class AStar {
	public static Vector3[] FindRoute(Vector3 s, Vector3 e) {
		PriorityQueue q = new PriorityQueue();
		Dictionary<Square, bool> dic = new Dictionary<Square, bool>();
		
		PathElement start = new PathElement(PathGrid.FromReal(s.x), PathGrid.FromReal(s.z), null);
		PathElement end = new PathElement(PathGrid.FromReal(e.x), PathGrid.FromReal(e.z), null);
		
		q.Enqueue(new PriorityElement(DistFromEnd(start, end), start));

		//Debug.Log("Calculating  for " + start.x + ", " + start.y + " :: " + end.x + ", " + end.y);
		
		int tick = 0 ;
		while(!q.IsEmpty() && tick < 100000) {
			tick ++;
			PathElement p = (PathElement)q.Dequeue().stuff;

			float dist = DistFromEnd(p, end);
			if(dist < 1.0f) {
				PathElement cur = p;
				
				int size = 1;
				while(cur != start) {
					//Debug.Log(cur.x + " " + cur.y);
					cur = cur.prev;
					size ++;
				}
				
				cur = p;
				Vector3[] route = new Vector3[size - 1];
				while(cur != start) {
					route[size - 2] = PathGrid.ToReal(cur.x, cur.y);
					cur = cur.prev;
					size --;
				}
				
				//SDebug.Log("START");
				return route;
			}
			else {
				AddOptions(q, p, end, dic);
			}
		}
		
		Debug.Log("Cannot find route!");
		Vector3[] route2 = new Vector3[1];
		route2[0] = Vector3.zero;
		return route2;
	}
	
	private static void AddOptions(PriorityQueue q, PathElement p, PathElement end, Dictionary<Square, bool> dic) {
		PathElement[] next = new PathElement[8];
		
		next[0] = GeneratePath(1, 0, 1.0f , p, dic);
		next[1] = GeneratePath(0, 1, 1.0f, p, dic);
		next[2] = GeneratePath(-1, 0, 1.0f, p, dic);
		next[3] = GeneratePath(0, -1, 1.0f, p, dic);
		
		next[4] = GeneratePath(1, 1, 1.4f, p, dic);
		next[5] = GeneratePath(-1, -1, 1.4f, p, dic);
		next[6] = GeneratePath(-1, 1, 1.4f, p, dic);
		next[7] = GeneratePath(1, -1, 1.4f, p, dic);

		for(int loop = 0; loop < 8; loop++) {
			if(next[loop] != null) q.Enqueue(new PriorityElement(next[loop].sofar +  Random.value * 0.5f  + DistFromEnd(next[loop], end), next[loop]));	
		}
	}
	
	private static PathElement GeneratePath(int dx, int dy, float dist, PathElement p, Dictionary<Square, bool> dic) {
		Square s = new Square(dx + p.x, dy + p.y);
		
		if(dic.ContainsKey(s)) {
			return null;
		}
		else {
			dic.Add(s, true);
			
			float difficulty = PathGrid.getGrid(p.x + dx, p.y + dy);
			if(Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1) {
				difficulty = 0.33f * (difficulty + PathGrid.getGrid(p.x + dx, p.y) + PathGrid.getGrid(p.x, p.y + dy));
			}
			
			return new PathElement(p.x + dx, p.y + dy, p.sofar + dist + difficulty, p);
		}
	}
	
	private static float DistFromEnd(PathElement p, PathElement end) {
		return Vector2.Distance(new Vector2(p.x, p.y), new Vector2(end.x, end.y));
	}
}
