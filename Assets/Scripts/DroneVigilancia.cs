using UnityEngine;
using System.Collections;


enum modo{PATROL,ALERT,DETECTA};

public class DroneVigilancia : MonoBehaviour {
	
  private Transform target;
  
  //Shortcut to transform
  private Transform myTransform;
  
  private Vector3 targetLine;
  
  public float pulseTime;
  
  //Check enemy around
  public bool enemyAround;
  private Collider[] scannedColliders;
  private int enemyMask = 1 << 11;

  private modo status = modo.PATROL;
  
  //Field of View
  private float detectionDistance = 14.0f;
  private float detectionRadius = 10.0f;
  private float alertRadius = 45.0f;

  public Transform[] patrolTarget;
  
  public GameObject detectAlert;
  private GameObject existingAlert;

  private CDrone cdroneScript;

  void Awake () {

    cdroneScript = this.GetComponent<CDrone>();
    myTransform = this.transform;
    enemyAround = false;
    scannedColliders = new Collider[0];

  }

	// Use this for initialization
	void Start () {
		

		pulseTime = 1.0f;
	
	}

	// Overlap sphere pulse
	void Update () {

  if(cdroneScript == null)
      Debug.LogError("My script is empty go back");

  if(cdroneScript.isStunned())
      return;

    //Debug.DrawRay(myTransform.position,myTransform.forward * 10);
  if(status == modo.PATROL)
      Patrulha();
    else if(status == modo.ALERT)
            Alerta();
         else if(status == modo.DETECTA)
                 Detecta();

  }

  void Patrulha(){

    pulseTime -= Time.deltaTime;


    if(pulseTime < 0){
     scannedColliders = Physics.OverlapSphere(myTransform.position,16,enemyMask);//
       if(scannedColliders.Length > 0)
         enemyAround = true;
       else enemyAround = false;
      pulseTime = 1.0f;
    }

    if(enemyAround){

      // Fase de teste
      if(scannedColliders.Length > 1){
        float tempdistance = Vector3.Distance(myTransform.position,scannedColliders[0].transform.position);
        for(int i = 0; i < scannedColliders.Length; i ++ ){
          target = scannedColliders[i].transform;
          float tempdistanceindex = Vector3.Distance(myTransform.position,target.position);
          if(tempdistanceindex < tempdistance){
            tempdistance = tempdistanceindex;
            break;
          }else target = scannedColliders[0].transform;
        }

      }else target = scannedColliders[0].transform;

      targetLine = target.position - myTransform.position;


       if(Vector3.Distance(myTransform.position,target.position) < detectionDistance  && Vector3.Angle(targetLine,myTransform.forward) < detectionRadius){
          pulseTime = 5.0f;
          status = modo.DETECTA;
          existingAlert = GameObject.Instantiate(detectAlert,new Vector3(myTransform.position.x,myTransform.position.y + 15,myTransform.position.z), Quaternion.identity) as GameObject;
       }else if(Vector3.Distance(myTransform.position,target.position) < detectionDistance && Vector3.Angle(targetLine,myTransform.forward) < alertRadius){
          Debug.Log("Vigilancia: ALERT");
          status = modo.ALERT;
       }

    }else{
      target = null;
    }

    Debug.DrawRay(myTransform.position,myTransform.forward * 2);

  }


void Alerta () {

  targetLine = target.position - myTransform.position;

  Debug.DrawRay(myTransform.position,myTransform.forward * 2,Color.yellow); // Smelled ya !

  if(Vector3.Distance(myTransform.position,target.position) < detectionDistance){
    if(Vector3.Angle(targetLine,myTransform.forward) < detectionRadius ){
        pulseTime = 5.0f;
        Debug.LogWarning("Vigilancia: found!");
        status = modo.DETECTA;
        existingAlert = GameObject.Instantiate(detectAlert,new Vector3(myTransform.position.x,myTransform.position.y + 15,myTransform.position.z), Quaternion.identity) as GameObject;
    }
  }else status = modo.PATROL;

}

  void Detecta () {

    Debug.DrawRay(myTransform.position,myTransform.forward * 2,Color.red); // Found ya !
    pulseTime -= Time.deltaTime;

    if(pulseTime < 0){
    status = modo.PATROL;
    GameObject.Destroy(existingAlert);
    }
  }
		
	


/*/
	void OnDrawGizmosSelected () {
		
		Vector3 pos = transform.position;
		
		Gizmos.DrawWireSphere(pos,16);	
	
	}
//*/
}
