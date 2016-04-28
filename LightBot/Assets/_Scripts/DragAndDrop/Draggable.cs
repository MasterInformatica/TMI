using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public int MAX = 10;
	public bool clone = true;
	private GameObject myclone = null;
	public GameObject toClone = null;
	public Transform parentToReturnTo = null;
	private GameObject placeholder = null;
	public Transform placeholderParent = null;
	public Transform placeholderReturnParent = null;


	public void OnBeginDrag(PointerEventData eventData){
		Debug.Log ("OnBeginDrag");
		placeholder = new GameObject();

		LayoutElement le = placeholder.AddComponent<LayoutElement>();
		le.preferredWidth = this.GetComponent<LayoutElement>().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;

		if (clone) {
			myclone = Instantiate (toClone);
			parentToReturnTo = null;
			myclone.transform.SetParent (this.transform.parent.parent.parent.parent);
			myclone.GetComponent<CanvasGroup>().blocksRaycasts = false;
			myclone.GetComponent<LayoutElement>().ignoreLayout = true;
			myclone.GetComponent<RectTransform> ().sizeDelta = GetComponent<RectTransform> ().sizeDelta;
			myclone.transform.position = this.transform.position;
			placeholderReturnParent = this.transform.parent.parent.parent.parent;
		} else {
			parentToReturnTo = this.transform.parent;
			placeholderReturnParent = parentToReturnTo;
			this.transform.SetParent (this.transform.parent.parent.parent);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
			GetComponent<LayoutElement>().ignoreLayout = true;
		}
		placeholderParent = placeholderReturnParent;
		placeholder.transform.SetParent( placeholderParent );
		placeholder.transform.SetSiblingIndex( this.transform.GetSiblingIndex() );

	}
	int count;
	public void OnDrag(PointerEventData eventData){
		Vector3 globalMousePos;
		RectTransformUtility.ScreenPointToWorldPointInRectangle (eventData.pointerEnter.transform as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos);
		if (clone) {
			myclone.transform.position = globalMousePos;
			myclone.GetComponent<RectTransform> ().localScale = new Vector3(1.0f,1.0f,1.0f);
			//myclone.GetComponent<RectTransform> ().sizeDelta = new Vector2(30.0f,30.0f);
		} else {
			this.transform.position = globalMousePos;
			GetComponent<RectTransform> ().localScale = new Vector3(1.0f,1.0f,1.0f);

		}


		if (placeholder.transform.parent != placeholderParent) {
			placeholder.transform.SetParent (placeholderParent);
			if (placeholderParent != null) 
				placeholder.transform.SetSiblingIndex( placeholderParent.transform.GetSiblingIndex() );
		}
		int newSiblingIndex = placeholderParent.childCount;
		
		for(int i=0; i < placeholderParent.childCount; i++) {
			if(clone){
				if(myclone.transform.position.y > placeholderParent.GetChild(i).position.y) {
					if(myclone.transform.position.x < placeholderParent.GetChild(i).position.x) {
						
						newSiblingIndex = i;
						
						if(placeholder.transform.GetSiblingIndex() < newSiblingIndex)
							newSiblingIndex--;
						
						break;
					}
				}
			}else{
				if(this.transform.position.y > placeholderParent.GetChild(i).position.y) {
					if(this.transform.position.x < placeholderParent.GetChild(i).position.x) {
						
						newSiblingIndex = i;
						
						if(placeholder.transform.GetSiblingIndex() < newSiblingIndex)
							newSiblingIndex--;
						
						break;
					}
				}
			}
		}
		
		placeholder.transform.SetSiblingIndex(newSiblingIndex);
	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");
		if (clone){
			if(parentToReturnTo == null  || parentToReturnTo.name == "Trash" || parentToReturnTo.childCount >= MAX){
				Destroy (myclone);
			} else {
				myclone.GetComponent<Draggable>().clone = false;
				myclone.transform.SetParent (parentToReturnTo);
				myclone.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );
				myclone.GetComponent<CanvasGroup>().blocksRaycasts = true;
				myclone.GetComponent<RectTransform> ().localScale = new Vector3(1.0f,1.0f,1.0f);
				myclone.GetComponent<LayoutElement>().ignoreLayout = false;
			}
		} else {
			Debug.Log (parentToReturnTo.name);
			if(parentToReturnTo.name == "Trash" ){
				Destroy(gameObject);
			}else{
				this.transform.SetParent (parentToReturnTo);
				this.transform.SetSiblingIndex( placeholder.transform.GetSiblingIndex() );
				GetComponent<CanvasGroup>().blocksRaycasts = true;
				GetComponent<RectTransform> ().localScale = new Vector3(1.0f,1.0f,1.0f);
				GetComponent<LayoutElement>().ignoreLayout = false;
			}
		}
		Destroy(placeholder);		
	}


}
