using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerReporter : MonoBehaviour {

    [SerializeField] MonoBehaviour ReportTo;


    private void OnTriggerEnter(Collider other) {
        ReportTo.SendMessage("OnTriggerEnterReported", other, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerStay(Collider other) {
        ReportTo.SendMessage("OnTriggerStayReported", other, SendMessageOptions.DontRequireReceiver);
    }

    private void OnTriggerExit(Collider other) {
        ReportTo.SendMessage("OnTriggerExitReported", other, SendMessageOptions.DontRequireReceiver);
    }
}
