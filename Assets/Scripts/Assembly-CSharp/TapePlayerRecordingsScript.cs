using UnityEngine;

public class TapePlayerRecordingsScript : MonoBehaviour
{
	public Sprite closedSprite;

	public SpriteRenderer sprite;

	private int audVal;

	public AudioClip[] recordings = new AudioClip[5];

	public BaldiScript baldi;

	private AudioSource audioDevice;

	private void Start()
	{
		audioDevice = GetComponent<AudioSource>();
	}

	public void Play()
	{
		sprite.sprite = closedSprite;
		audVal = Mathf.RoundToInt(Random.Range(0f, 4f));
		audioDevice.PlayOneShot(recordings[audVal]);
		if (baldi.isActiveAndEnabled)
		{
			baldi.Hear(base.transform.position, 4f);
		}
	}
}
