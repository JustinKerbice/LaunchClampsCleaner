// LCC: Launch Clamps Cleaner, or how to send those damned annoying and buggy launch clamps back to where they belong for good
// they'll never block you with a foolish "can't get back to space center while moving" or such message.
// Also, launching from a Kerbtown building, they'll no longer be there if you've not clean them manually, and one by one !

// Justin Kerbice   09/09/2014
// License: Whatever license 1.1 (see included file for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// TODO: add FASA launchclamps support

namespace LaunchClampsCleaner
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class LaunchClampsCleaner : MonoBehaviour
	{
		private Vessel current_vessel;
		private List<Part> LaunchClamps_list;
		private bool lc_found = false;
		private float lastUpdate = 0.0f;
		private float lastFixedUpdate = 0.0f;
		private float logInterval = 5.0f;

		public LaunchClampsCleaner ()
		{
			Debug.Log ("LCC constructor");
		}

		void Awake ()
		{
			Debug.Log ("LCC Awake");

			current_vessel = FlightGlobals.ActiveVessel;

			if (current_vessel != null) {
				if (current_vessel.packed == true) {
					Debug.Log ("Awake: vessel packed");
				}
			} else {
				Debug.LogWarning ("awake: vessel is null!");
			}

			LaunchClamps_list = new List<Part>();

			Debug.Log ("LCC Awake end");
		}


		void Start ()
		{
			Debug.Log ("LCC start");

			current_vessel = FlightGlobals.ActiveVessel;

			if (current_vessel != null) {
				foreach (Part p in current_vessel.Parts) {
					if (p != null) {
						Debug.Log ("LCC start: part p=" + p.ToString ());

						if (p.name == "launchClamp1") {
							Debug.Log ("LCC st: LC found");
							lc_found = true;
							LaunchClamps_list.Add (p);
						}

					} else {
						Debug.Log ("Get a null part !");
					}
				}

				if (lc_found == false) {
					Debug.Log ("No launchclamp found with this vessel, see you next time !");
				}
			} else {
				Debug.LogWarning ("Start: vessel is null!");
			}

			Debug.Log ("LCC start end");
		}


		void Update ()
		{
			if ((Time.time - lastUpdate) > logInterval)
			{
				lastUpdate = Time.time;
				Debug.Log("LCC upd");
			}
		}


		//HERE: put the logic
		void FixedUpdate ()
		{
			if ((Time.time - lastFixedUpdate) > logInterval)
			{
				lastFixedUpdate = Time.time;
				Debug.Log("LCC fu");

				if (LaunchClamps_list.Count > 0) {
					foreach (Part launchclamp_to_remove in LaunchClamps_list) {
						if (launchclamp_to_remove != null) {
							Debug.Log ("Fu: LC " + launchclamp_to_remove.name + " desactivated");

							Debug.Log ("Fu, LC: started=" + launchclamp_to_remove.started.ToString () + "\n" +
							"packed=" + launchclamp_to_remove.packed.ToString () + "\n" +
							"connected=" + launchclamp_to_remove.isConnected.ToString () + "\n" +
							"attached=" + launchclamp_to_remove.isAttached.ToString () + "\n" +
							"state=" + launchclamp_to_remove.State.ToString ());

							// on pad: started=F,pack=T,conn=T,att=T,st=IDLE
							// ~600m packed "for orbit": Start=T,pack=T,conn=F,att=F,stat=ACTIVE
							//(+ part is desactivated)

							// > 2500m = launchclamp part are null (removed from memory ?)
							// get back < 2500m: these obejcts are gone, need ~to get them back as in the beginning
							if (launchclamp_to_remove.started == true &&
							    launchclamp_to_remove.packed == true &&
							    launchclamp_to_remove.isConnected == false &&
							    launchclamp_to_remove.isAttached == false &&
							    launchclamp_to_remove.State == PartStates.ACTIVE) {

								Debug.Log ("Fu: LC: can be removed now, destroy it");
								launchclamp_to_remove.explode ();
								LaunchClamps_list.Remove (launchclamp_to_remove);

								// destroying launchclamps may be bad for business, is there any way to recover them smoothly ? (or roughly at least)
							}

							//deactivate ();	// doesn't do the job
						} else {
							Debug.Log ("Launchclamp is null");
							// launchclamp has been destroyed by KSP, part is useless now, drop it
							LaunchClamps_list.Remove (launchclamp_to_remove);
						}
					}
				}
			}
		}


		void OnDestroy ()
		{
			Debug.Log ("LCC destroy");

			LaunchClamps_list.Clear ();

			Debug.Log ("LCC destroy end");
		}
	}
}