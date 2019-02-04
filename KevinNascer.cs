using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KevinNascer : StateMachineBehaviour {
    bool pular = false;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!GameManager.instance.infinito){
            if(GameManager.instance.editor)
            {
                GameManager.instance.FinalizarFase();
            }
            else
                GameManager.instance.KevinNascer();
        }
        else
        {
            if (GameManager.instance.infiniteBoard.morrer){;
                pular = true;
                GameManager.instance.FinalizarFase();
            }
                
            else{
                GameManager.instance.infiniteBoard.morrer = true;
                GameManager.instance.KevinNascer();
            }
        }
            
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!GameManager.instance.infinito || !pular)
            GameManager.instance.KevinViver ();
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
