using UnityEngine;
using System.Collections;

public class PriorityElement {
	public float priority;
	public PathElement stuff;
	
	public bool Beats(PriorityElement p) {
		return (this.priority < p.priority);
	}
	
	public PriorityElement(float p, PathElement s) {
		priority = p;
		stuff = s;
	}
}
