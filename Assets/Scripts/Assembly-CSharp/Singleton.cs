using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public static bool isApplicationQuitting;

	public static T Instance
	{
		get
		{
			if (isApplicationQuitting)
			{
				return null;
			}
			if (instance == null)
			{
				instance = Object.FindObjectOfType<T>();
				if (instance == null && Application.isPlaying)
				{
					instance = CreateInstance();
				}
			}
			return instance;
		}
	}

	public static bool Exist
	{
		get
		{
			return instance != null;
		}
	}

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += delegate
		{
			isApplicationQuitting = true;
		};
	}

	protected virtual void Awake()
	{
		isApplicationQuitting = false;
		if (instance != null && instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		instance = this as T;
		OnAwake();
		if (base.transform.parent == null)
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		instance = null;
	}

	protected virtual void OnAwake()
	{
	}

	public static bool TryGetInstance(out T result)
	{
		if (isApplicationQuitting)
		{
			result = null;
			return false;
		}
		result = Instance;
		return result != null;
	}

	private static T CreateInstance()
	{
		GameObject obj = new GameObject(typeof(T).Name);
		Object.DontDestroyOnLoad(obj);
		return obj.AddComponent<T>();
	}
}
