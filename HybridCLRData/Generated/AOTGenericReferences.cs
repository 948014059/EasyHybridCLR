public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//System.Action`1<System.Object>
	//System.Action`1<System.Int64>
	//System.Action`2<System.Int64,System.Object>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.IEnumerator`1<System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
	//System.Collections.Generic.Queue`1<System.Object>
	// }}

	public void RefMethods()
	{
		// System.Object UnityEngine.AssetBundle::LoadAsset<System.Object>(System.String)
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object)
	}
}