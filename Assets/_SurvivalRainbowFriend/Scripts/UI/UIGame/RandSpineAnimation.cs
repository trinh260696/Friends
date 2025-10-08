using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandSpineAnimation : MonoBehaviour
{
	#region Inspector
	[SpineAnimation]
	public string startingAnimation;
	[SpineAnimation]
	public string BoxSkinAnimation;
	#endregion

	SkeletonGraphic skeletonAnimation;

	public Spine.AnimationState spineAnimationState;
	public Spine.Skeleton skeleton;

    private void Awake()
    {
    }
    void Start()
	{
		skeletonAnimation = GetComponent<SkeletonGraphic>();
		spineAnimationState = skeletonAnimation.AnimationState;
		skeleton = skeletonAnimation.Skeleton;
		NotificationCenter.DefaultCenter().AddObserver(this, "BoxSkinSelect");
		NotificationCenter.DefaultCenter().AddObserver(this, "RanSpinCoroutine");
		StartCoroutine("RandSpineAnim");
	}

	IEnumerator RandSpineAnim()
	{
		while (true)
		{
			string[] randAnim = { "dance", "dance2", "hiphop_home" };
			startingAnimation = randAnim[Random.Range(0, randAnim.Length)];
			if (skeleton.Skin.Name == "skin11" && startingAnimation=="hiphop_home")
				spineAnimationState.SetAnimation(0, "dance", true);
			else
			   spineAnimationState.SetAnimation(0, startingAnimation, true);
			yield return new WaitForSeconds(5f);
		}
	}

	public void BoxSkinSelect()
	{
		StopCoroutine("RandSpineAnim");
		BoxSkinAnimation = UISkin.spineNameBox;
        spineAnimationState.SetAnimation(0, BoxSkinAnimation, true);
	}
	public void RanSpinCoroutine()
	{
		StopCoroutine("RandSpineAnim");
		StartCoroutine("RandSpineAnim");
    }

}
