using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class TemperatureRegulationScript : MonoBehaviour
{
   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMNeedyModule Needy;
   public Animator animator;
   private const string SwitchUp = "SwitchUpBool";
   private const string NeedyActive = "NeedyActive";

   bool isUp;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;
   bool bombSolved = false;
   float currentPos = 0.0f;

   int position = 0;

   public KMSelectable handle; // Reference to the handle object
   private float switchTimer = 25.0f;

   public Material topColor; // Color of the bar in the top half
   public Material bottomColor; // Color of the bar in the bottom half
   public GameObject redBarControl;
   public GameObject blueBarControl;

   private bool isNeedyActive = false;
   private bool isNeedyDeactivated = false;

   void Awake()
   {
      //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
      redBarControl.SetActive(false);
      blueBarControl.SetActive(false);
      ModuleId = ModuleIdCounter++;
      Needy.OnNeedyActivation += delegate
      {
         if (isNeedyDeactivated)
         {
            Needy.HandlePass();
         }
         else
         {
            OnNeedyActivate();
            StartCoroutine(timer());
         }
      };      
      Needy.OnTimerExpired += OnNeedyTimerExpired;
      handle.OnInteract += delegate () { HandleInteract(); return false; };
   }

   void OnNeedyActivate()
   {
      isNeedyActive = true;
      int number = Rnd.Range(0,2);
      if (number == 0){
         isUp = false;
      }
      else{
         isUp = true;
      }
      animator.SetBool(NeedyActive, isNeedyActive);
      
   }

   private IEnumerator timer(){
      while(true){
      if (isNeedyDeactivated){
         yield return null;
      }
      
      else{
         if (isUp == true)
            {
            switchTimer += 0.1f;
            }
         else
            {
            switchTimer -= 0.1f;
            }
         if (switchTimer >= 25.0f){
            redBarControl.SetActive(true);
            blueBarControl.SetActive(false);
            /*if(isUp == true){
               redBarControl.transform.localScale = new Vector3(0f,0f, currentPos += 0.01f);
            }
            if(isUp == false){
               redBarControl.transform.localScale = new Vector3(0f, 0f, currentPos -= 0.01f);
            }*/
         }
         else{
               redBarControl.SetActive(false);
               blueBarControl.SetActive(true);
               blueBarControl.transform.localScale = new Vector3(0f, 0f, 0.01f);
            }
         if (switchTimer < 0.0f || switchTimer > 49.9f)
            {
            GetComponent<KMNeedyModule>().HandleStrike();
            GetComponent<KMNeedyModule>().HandlePass();
            isNeedyActive = false;
            isNeedyDeactivated = true;
            }
         }
         yield return new WaitForSeconds(0.1f);
      }
   }

   void OnDestroy()
   {
      //Shit you need to do when the bomb ends
      isNeedyActive = false;
   }

   void Start()
   {
      // Initialize your module here
      Debug.LogFormat("[Temperature Regulation #{0}] Needy initiated.", ModuleId);
   }

   void Update(){
      animator.SetBool(SwitchUp, isUp);
      if (isNeedyActive)
      {
         //This keeps the timer number set to the selected number
         if (Needy.GetNeedyTimeRemaining().ToString() != switchTimer.ToString())
         {
            Needy.SetNeedyTimeRemaining(Mathf.Floor(switchTimer));
         }
      }
   }

   void HandleInteract()
   {
      if (isUp == true){
         isUp = false;
      }
      else{
         isUp = true;
      }
   }

   public void OnNeedyTimerExpired()
   {
      // Handle what happens when the needy timer expires
      // For example, consider the module as failed
      isNeedyActive = false;
      isNeedyDeactivated = true;
      GetComponent<KMNeedyModule>().HandleStrike();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand(string Command)
   {
      yield return null;
   }

   void TwitchHandleForcedSolve()
   {
      //Void so that autosolvers go to it first instead of potentially striking due to running out of time.
      StartCoroutine(HandleAutosolver());
   }

   IEnumerator HandleAutosolver()
   {
      yield return null;
   }
}
