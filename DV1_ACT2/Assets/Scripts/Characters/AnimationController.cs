using UnityEngine;

//Class to control the player and enemies animations 
public class AnimationController
{
    private Animator charAnimator;

    public AnimationController(GameObject character)
    {
        charAnimator = character.GetComponent<Animator>();
    }


    public void Animate(string animation)
    {
        charAnimator.Play(animation);
    }

    public bool AnimationOnCourse(string animation)
    {
        return charAnimator.GetCurrentAnimatorStateInfo(0).IsName(animation);
    }

}
