using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceRaycasterTest : MonoBehaviour
{
	[SerializeField] private Transform cutter = default;
	[SerializeField] private Camera cam = default;
	[SerializeField] private LayerMask layerMask = default;
	[SerializeField] private float rayCastDistance = 10f;

	[Header("Debug:")][SerializeField] private bool enableDebug;

	private RaycastHit? startHit;
	private RaycastHit? endHit;
	private RaycastHit? currentHit;

	private void Awake()
	{
		if (!cam)
		{
			cam = Camera.main;
		}
	}

	private void Update()
	{
		ProcessRayCastHits();
		TrySliceObject();
	
	}

	private void ProcessRayCastHits()
	{

		

			var rayOrigin = cutter.position;
			var dir = -cutter.up;
			RaycastHit hit;
			Ray ray=new Ray(rayOrigin, dir);
	     	Debug.DrawRay(rayOrigin, dir,Color.white,rayCastDistance);
			var isHit = Physics.Raycast(ray,out hit, rayCastDistance,layerMask);
			if (isHit)
			{
				currentHit = hit;

				if (!startHit.HasValue)
				{
					startHit = hit;
			
				}
			}
			else
			{
				if (!endHit.HasValue)
				{
					endHit = currentHit;
				}

				currentHit = null;
			}
		
	}

	

	private void TrySliceObject()
	{
		if (!startHit.HasValue)
		{
			return;
		}

		var startHitPoint = startHit.Value.point;
		var endHitPoint = startHit.Value.point+ -cutter.up;
		var objectToSlice = startHit.Value.collider.GetComponent<Slicable>();

		startHit = null;
		endHit = null;

		if (!objectToSlice)
		{
			return;
		}

		var forward = cam.transform.forward;
		var sliceVector = startHitPoint - endHitPoint;
		var inNormal = Vector3.Cross(sliceVector, forward);
		var inPont = Vector3.Lerp(startHitPoint, endHitPoint, 0.5f);

		var matrix = objectToSlice.transform.worldToLocalMatrix;
		var plane = new Plane(matrix.MultiplyVector(inNormal),
							  matrix.MultiplyPoint(startHitPoint));

		var isSliced = SliceTool.Slice(objectToSlice, plane);
		if (isSliced)
		{
			Destroy(objectToSlice.gameObject);
		}
	}
}
